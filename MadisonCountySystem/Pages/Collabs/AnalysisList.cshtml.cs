using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs
{
    public class AnalysisListModel : PageModel
    {
        public String CollabID { get; set; }
        public String CollabName { get; set; }
        public List<Analysis> AnalysisList { get; set; }
        public List<AnalysisCollab> AnalysisCollabs { get; set; }

        public AnalysisListModel()
        {
            AnalysisList = new List<Analysis>();
            AnalysisCollabs = new List<AnalysisCollab>();
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                HttpContext.Session.SetString("LibType", "Collab");
                CollabName = HttpContext.Session.GetString("collabName");
                CollabID = HttpContext.Session.GetString("collabID");

                SqlDataReader analysisCollabReader = DBClass.AnalysisCollabReader();
                while (analysisCollabReader.Read())
                {
                    if (CollabID == analysisCollabReader["CollabID"].ToString())
                    {
                        AnalysisCollabs.Add(new AnalysisCollab
                        {
                            AnalysisID = Int32.Parse(analysisCollabReader["AnalysisID"].ToString()),
                            CollabID = Int32.Parse(analysisCollabReader["CollabID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader analysisReader = DBClass.AnalysisReader();
                while (analysisReader.Read())
                {
                    foreach (var analysisCollab in AnalysisCollabs)
                    {
                        if (analysisCollab.AnalysisID == Int32.Parse(analysisReader["AnalysisID"].ToString()))
                        {
                            AnalysisList.Add(new Analysis
                            {
                                AnalysisID = analysisCollab.AnalysisID,
                                AnalysisName = analysisReader["AnalysisName"].ToString(),
                                AnalysisType = analysisReader["AnalysisType"].ToString(),
                                AnalysisResult = analysisReader["AnalysisResult"].ToString(),
                                AnalysisCreatedDate = analysisReader["AnalysisCreatedDate"].ToString(),
                                DatasetID = Int32.Parse(analysisReader["DatasetID"].ToString()),
                                OwnerID = Int32.Parse(analysisReader["OwnerID"].ToString()),
                                KnowledgeID = Int32.Parse(analysisReader["KnowledgeID"].ToString())
                            });
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostLinkItem()
        {
            return RedirectToPage("/Collabs/Merge/AddAnalysis");
        }

        public IActionResult OnPostCreateItem()
        {
            return RedirectToPage("/RecordCreate/CreateAnalysis");
        }
    }
}
