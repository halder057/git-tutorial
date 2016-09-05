using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ZhiNotification.Models;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotification.MembershipInfrastructure;
using ZhiNotificationCommon.DataAccess;
using ZhiNotificationCommon.Models;

namespace ZhiNotification.Services
{
    public class ViewLogService
    {
        public ViewLogModel GetModuleAndEventData(string username)
        {
            ViewLogModel data = new ViewLogModel();
            
            AccountService accService = new AccountService();
            User loggedIn = accService.GetUser(username);
            bool isSuperAdmin = Roles.IsUserInRole(RoleNames.SuperAdmin);
            
            if (isSuperAdmin)
            {
                data.Applications = GetApplications();
                data.Modules = GetModules(); // GetModules also fetchs corresponding Events
            }
            return data;
        }
        public List<Application> GetApplications()
        {
            return CommonDataAccess.GetApplications();
        }
        public List<Module> GetModules()
        {
            // Get Module and Events Assocs from database

            List<NotificationModuleNotificationEventAssoc> moduleEventAssocListFromDatabase = LogDataAccess.GetNotificationModuleEventAssoc();
            
            List<Module> moduleListForView = new List<Module>();
            Dictionary<int, Module> moduleDictionaryForView = new Dictionary<int, Module>();
            foreach(var assoc in moduleEventAssocListFromDatabase)
            {
                NotificationModule moduleInDB = assoc.NotificationModule;
                NotificationEvent eventInDB = assoc.NotificationEvent;

                if(moduleDictionaryForView.ContainsKey(moduleInDB.ID)) // module already in Dictionary
                {
                    Event eventObjectForModule = new Event();
                    eventObjectForModule.eventId = eventInDB.ID;
                    eventObjectForModule.eventName = eventInDB.EventName;

                    moduleDictionaryForView[moduleInDB.ID].moduleEvents.Add(eventObjectForModule);
                }
                else // New module... Add this module to dictionary
                {
                    Module module = new Module();
                    module.moduleId = moduleInDB.ID;
                    module.moduleName = moduleInDB.Modulename;
                    Event eventObjectForModule = new Event();
                    eventObjectForModule.eventId = eventInDB.ID;
                    eventObjectForModule.eventName = eventInDB.EventName;
                    module.moduleEvents = new List<Event>();
                    module.moduleEvents.Add(eventObjectForModule);

                    moduleDictionaryForView.Add(module.moduleId, module);
                }
            }
            moduleListForView = new List<Module>( moduleDictionaryForView.Values);

            return moduleListForView;
        }

        public ErrorModel GetErrors(int moduleId)
        {
            ErrorModel errorViewModel = new ErrorModel();
            if(moduleId > -1)
            {
                // Fetch notification errors from database
                errorViewModel.Errors = LogDataAccess.GetNotificationErrors(moduleId);
            }
            return errorViewModel;
        }
    }
}