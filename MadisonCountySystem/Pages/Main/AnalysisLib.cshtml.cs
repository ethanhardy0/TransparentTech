using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Main
{
    public class AnalysisLibModel : PageModel
    {
        public List<Analysis> AnalysisList { get; set; }
        public AnalysisLibModel()
        {
            AnalysisList = new List<Analysis>();
        }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else if (HttpContext.Session.GetString("typeUser") != "Admin" && HttpContext.Session.GetString("typeUser") != "Super")
            {
                HttpContext.Response.Redirect("/Main/Collaborations");
            }
            HttpContext.Session.SetString("LibType", "Main");
            SqlDataReader analysisReader = DBClass.AnalysisReader();
            while (analysisReader.Read())
            {
                AnalysisList.Add(new Analysis
                {
                    AnalysisID = Int32.Parse(analysisReader["AnalysisID"].ToString()),
                    AnalysisName = analysisReader["AnalysisName"].ToString(),
                    AnalysisType = analysisReader["AnalysisType"].ToString(),
                    AnalysisResult = analysisReader["AnalysisResult"].ToString(),
                    AnalysisCreatedDate = analysisReader["AnalysisCreatedDate"].ToString(),
                    DatasetID = Int32.Parse(analysisReader["DatasetID"].ToString()),
                    OwnerID = Int32.Parse(analysisReader["OwnerID"].ToString()),
                    KnowledgeID = Int32.Parse(analysisReader["KnowledgeID"].ToString())
                });
            }
            DBClass.KnowledgeDBConnection.Close();
        }

        public IActionResult OnPostAddCollab(int selectedAnalysis)
        {

            return RedirectToPage("/ButtonCollab/AnalysisButton", new { itemID = selectedAnalysis });
        }

        public IActionResult OnPostCreateAnalysis()
        {
            return RedirectToPage("/RecordCreate/CreateAnalysis");
        }
    }
}
