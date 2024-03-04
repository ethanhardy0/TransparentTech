using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages.Info
{
    public class AnalysisInfoModel : PageModel
    {
        public static int AnalysisID { get; set; }
        public Analysis Analysis { get; set; }

        public AnalysisInfoModel()
        {
            Analysis = new Analysis();
        }

        public void OnGet(int analysisID)
        {
            AnalysisID = analysisID;

            SqlDataReader AnalysisReader = DBClass.AnalysisReader();
            while (AnalysisReader.Read())
            {
                if (AnalysisID == Int32.Parse(AnalysisReader["AnalysisID"].ToString()))
                {
                    Analysis.DatasetID = Int32.Parse(AnalysisReader["DatasetID"].ToString());
                    Analysis.KnowledgeID = Int32.Parse(AnalysisReader["KnowledgeID"].ToString());
                    Analysis.AnalysisName = AnalysisReader["AnalysisName"].ToString();
                    Analysis.AnalysisType = AnalysisReader["AnalysisType"].ToString();
                    Analysis.AnalysisResult = AnalysisReader["AnalysisResult"].ToString();
                    Analysis.AnalysisCreatedDate = AnalysisReader["AnalysisCreatedDate"].ToString();
                    Analysis.OwnerName = AnalysisReader["Username"].ToString();
                    Analysis.OwnerFirst = AnalysisReader["FirstName"].ToString();
                    Analysis.OwnerLast = AnalysisReader["LastName"].ToString();
                    Analysis.DatasetName = AnalysisReader["DatasetName"].ToString();
                    Analysis.KnowledgeTitle = AnalysisReader["KnowledgeTitle"].ToString();

                }
            }
            DBClass.KnowledgeDBConnection.Close();
            //might want to get owner information
            //might want to get analysis information
            //might want to get dataset information through analysis
            //might want to get spreadsheet information through dataset (for now, might change to relating to KI directly)
            //might want to get collab information
        }
    }
}
