using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiPattChangeScanner.Utilities
{
    public class Constants
    {
        public const string PattChangeDetailsRetrievalStoredProcedure = "[dbo].[usp_GetPattChangesDetailsTemplates]";
        public const string ChangeDetailsStoringStoredProcedure = "[dbo].[usp_SavePattChangesAndDetails]";
        public const string TherapAreaDrugPatternInTemplateMessage = " for <%TA%>/<%Drug%>";
        public const string TherapAreaPatternInTemplateMessage = "<%TA%>/";
        public const string TherapAreaFieldName = "TA";
        public const string DrugFieldName = "Drug";
        public const int PattApplicationId = 1;
        public const int ConnectionTimeoutInSeconds = 1800;
    }
}
