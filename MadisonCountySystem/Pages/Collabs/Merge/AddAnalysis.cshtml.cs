using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs.Merge
{
    public class AddAnalysisModel : PageModel
    {
        public List<Analysis> AnalysisList { get; set; }
        public List<AnalysisCollab> AnalysisCollabs { get; set; }
        public List<int> AddedAnalyses { get; set; }
        public static String CollabID { get; set; }
        public String CollabName { get; set; }
        public AnalysisCollab AnalysisCollab { get; set; }

        public AddAnalysisModel()
        {
            AnalysisList = new List<Analysis>();
            AnalysisCollabs = new List<AnalysisCollab>();
            AddedAnalyses = new List<int>();
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

                SqlDataReader activeReader = DBClass.AnalysisReader();
                while (activeReader.Read())
                {
                    foreach (var analysisCollab in AnalysisCollabs)
                    {
                        if (analysisCollab.AnalysisID == Int32.Parse(activeReader["AnalysisID"].ToString()))
                        {
                            AddedAnalyses.Add(Int32.Parse(activeReader["AnalysisID"].ToString()));
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader analysisReader = DBClass.AnalysisReader();
                while (analysisReader.Read())
                {
                    AnalysisList.Add(new Analysis
                    {
                        AnalysisID = Int32.Parse(analysisReader["AnalysisID"].ToString()),
                        AnalysisName = analysisReader["AnalysisName"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();

                List<Analysis> itemsToRemove = new List<Analysis>();

                foreach (var item in AnalysisList)
                {
                    foreach (int id in AddedAnalyses)
                    {
                        if (item.AnalysisID == id)
                        {
                            itemsToRemove.Add(item);
                            break;
                        }
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    AnalysisList.Remove(item);
                }
            }
        }

        public IActionResult OnPostUpdateDB(int selectedAnalysis)
        {
            AnalysisCollab = new AnalysisCollab
            {
                AnalysisID = selectedAnalysis,
                CollabID = Int32.Parse(CollabID)
            };
            DBClass.InsertAnalysisCollab(AnalysisCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Collabs/AnalysisList");
        }
    }
}
