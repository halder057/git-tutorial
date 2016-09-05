using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZhiPattChangeScanner.Models;
using ZhiPattChangeScanner.Utilities;
using ZhiPattChangeScanner.DataAccessLayer.DatabaseContexts;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using ZhiNotificationCommon.Utilities;
using ZhiPattChangeScanner.DataAccessLayer.DataAccess;

namespace ZhiPattChangeScanner
{
    public class Processor
    {
        public void Process(Logger logger)
        {
            try
            {
                logger.LogInfo("Change processing started.");

                var changesDetailsTemplates = new PattChangesDetailsTemplates();

                logger.LogInfo("Change scanning started.");

                var latestHistoryEntry = ScanHistoryDataAccess.GetLatestScanHistoryEntry(Utilities.Constants.PattApplicationId);

                logger.LogInfo("Change scanning completed.");

                logger.LogInfo("Getting latest changes and details started.");

                if (latestHistoryEntry != null)
                    changesDetailsTemplates = GetLatestChangesAndDetails(logger, latestHistoryEntry.LogID);
                else
                    changesDetailsTemplates = GetLatestChangesAndDetails(logger);

                if (changesDetailsTemplates == null || changesDetailsTemplates.Changes.Count < 1)
                {
                    changesDetailsTemplates = GetCompiledChangesDetailsTemplates(changesDetailsTemplates);
                    logger.LogInfo(string.Format("Getting latest changes and details successfully completed. A total of {0} new change(s) found.",
                               changesDetailsTemplates.Changes.Count));

                    logger.LogInfo("Storing latest changes and details started.");
                    SaveChangesAndDetails(logger, changesDetailsTemplates);
                    logger.LogInfo("Storing latest changes and details completed.");
                }
                else
                {
                    logger.LogInfo("No new changes found.");
                }

                logger.LogInfo("Change processing completed.");
            }
            catch (Exception ex)
            {
                log4net.LogicalThreadContext.Properties["ModuleID"] = (int)ZhiNotificationCommon.Utilities.Module.ChangeScannerApplication;
                logger.LogError("Error in change scanning.", ex);
            }
        }

        private PattChangesDetailsTemplates GetLatestChangesAndDetails(Logger logger, int? lastProcessedLogId = null)
        {
            try
            {
                using (var pattDataEntryContext = new PattDataEntryContext())
                {
                    var changesDetailsTemplates = new PattChangesDetailsTemplates();
                    pattDataEntryContext.Database.Initialize(force: false);

                    var cmd = pattDataEntryContext.Database.Connection.CreateCommand();
                    cmd.CommandText = Utilities.Constants.PattChangeDetailsRetrievalStoredProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Utilities.Constants.ConnectionTimeoutInSeconds;

                    if (lastProcessedLogId.HasValue)
                    {
                        SqlParameter lastProcessedLogIdParam = new SqlParameter("@LastProcessedLogId", lastProcessedLogId.Value);
                        cmd.Parameters.Add(lastProcessedLogIdParam);
                    }

                    SqlParameter applicationIdParam = new SqlParameter("@ApplicationId", Utilities.Constants.PattApplicationId);
                    cmd.Parameters.Add(applicationIdParam);

                    pattDataEntryContext.Database.Connection.Open();

                    var reader = cmd.ExecuteReader();
                    if (reader != null)
                    {
                        changesDetailsTemplates.Changes = ((IObjectContextAdapter)pattDataEntryContext)
                            .ObjectContext
                            .Translate<ChangeObject>(reader).ToList();

                        reader.NextResult();
                        changesDetailsTemplates.ChangeDetails = ((IObjectContextAdapter)pattDataEntryContext)
                                .ObjectContext
                                .Translate<ChangeDetailObject>(reader).ToList();

                        reader.NextResult();
                        changesDetailsTemplates.NotificationTemplateMessages = ((IObjectContextAdapter)pattDataEntryContext)
                                .ObjectContext
                                .Translate<NotificationTemplateMessageObject>(reader).ToList();
                    }
                    return changesDetailsTemplates;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while trying to obtain the latest changes. ", ex);
            }
            return null;
        }

        private PattChangesDetailsTemplates GetCompiledChangesDetailsTemplates(PattChangesDetailsTemplates changesDetailsTemplates)
        {
            if (changesDetailsTemplates.NotificationTemplateMessages.Count > 1)
            {
                for (var i = 0; i < changesDetailsTemplates.Changes.Count; i += 1)
                {
                    var currentChange = changesDetailsTemplates.Changes[i];
                    int logId = currentChange.LogId.Value;
                    var notificationTemplateMessagesForCurrentCategory = changesDetailsTemplates.NotificationTemplateMessages
                        .Where(x => x.NotificationCategoryID == currentChange.NotificationCategoryID).ToList();
                    string defaultTemplateMessage = notificationTemplateMessagesForCurrentCategory[0].Message;
                    string nullOldValueTemplateMessage = notificationTemplateMessagesForCurrentCategory[1].Message;

                    var drugForCurrentChange = changesDetailsTemplates.ChangeDetails
                            .FirstOrDefault(f => f.LogId == logId
                                && string.Equals(f.Name, Utilities.Constants.DrugFieldName, StringComparison.InvariantCultureIgnoreCase));

                    var therapAreaForCurrentChange = changesDetailsTemplates.ChangeDetails
                            .FirstOrDefault(f => f.LogId == logId
                                && string.Equals(f.Name, Utilities.Constants.TherapAreaFieldName, StringComparison.InvariantCultureIgnoreCase));

                    string compiledMessage = string.IsNullOrWhiteSpace(currentChange.OldValue) ? nullOldValueTemplateMessage : defaultTemplateMessage;

                    if ((therapAreaForCurrentChange == null || string.IsNullOrWhiteSpace(therapAreaForCurrentChange.Value))
                        && (drugForCurrentChange == null || string.IsNullOrWhiteSpace(drugForCurrentChange.Value)))
                    {
                        compiledMessage = compiledMessage.Replace(Utilities.Constants.TherapAreaDrugPatternInTemplateMessage, string.Empty);
                    }
                    else if (therapAreaForCurrentChange == null || string.IsNullOrWhiteSpace(therapAreaForCurrentChange.Value))
                    {
                        compiledMessage = compiledMessage.Replace(Utilities.Constants.TherapAreaPatternInTemplateMessage, string.Empty);
                    }

                    Regex rgx = new Regex("<%\\w+%>");
                    MatchCollection matches = rgx.Matches(compiledMessage);

                    foreach (var item in matches)
                    {
                        string field = item.ToString().Replace("<%", "").Replace("%>", "").Trim();

                        if (!ZhiNotificationCommon.Utilities.Constants.CommonNotificationTemplateFields.Contains(field))
                        {
                            var fieldDetailEntry = changesDetailsTemplates.ChangeDetails
                            .FirstOrDefault(f => f.LogId == logId && string.Equals(f.Name, field, StringComparison.InvariantCultureIgnoreCase));
                            if (fieldDetailEntry != null)
                            {
                                compiledMessage = compiledMessage.Replace(item.ToString(), fieldDetailEntry.Value);
                            }
                            else
                            {
                                compiledMessage = compiledMessage.Replace(item.ToString(), string.Empty);
                            }
                        }
                        else
                        {
                            compiledMessage = compiledMessage.Replace(item.ToString(),
                                (currentChange.GetType().GetProperty(field).GetValue(currentChange, null) ?? string.Empty).ToString());
                        }
                    }

                    changesDetailsTemplates.Changes[i].NotificationMessage = compiledMessage;
                }
            }

            return changesDetailsTemplates;
        }

        private void SaveChangesAndDetails(Logger logger, PattChangesDetailsTemplates changesDetailsTemplates)
        {
            try
            {
                DataTable changeTable = Helpers.GetDataTable("udt_Change", changesDetailsTemplates.Changes);

                DataTable changeDetailsTable = Helpers.GetDataTable("udt_ChangeDetail", changesDetailsTemplates.ChangeDetails);

                using (var notyContext = new ZhiNotificationCommon.DataAccess.DatabaseContexts.NotificationContext())
                {
                    notyContext.Database.Initialize(force: false);
                    var cmd = notyContext.Database.Connection.CreateCommand();
                    cmd.CommandText = Utilities.Constants.ChangeDetailsStoringStoredProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Utilities.Constants.ConnectionTimeoutInSeconds;

                    SqlParameter changesParam = new SqlParameter("@Changes", changeTable);
                    cmd.Parameters.Add(changesParam);

                    SqlParameter changeDetailsParam = new SqlParameter("@ChangeDetails", changeDetailsTable);
                    cmd.Parameters.Add(changeDetailsParam);

                    notyContext.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while trying to store the changes and details. ", ex);
            }
        }
    }
}
