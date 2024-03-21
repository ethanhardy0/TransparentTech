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
        public List<Analysis> Dep1Analysis { get; set; }
        public List<Analysis> Dep2Analysis { get; set; }
        public List<Analysis> Dep3Analysis { get; set; }
        public List<Analysis> Dep4Analysis { get; set; }
        public List<Analysis> Dep5Analysis { get; set; }

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

            Dep1Analysis = new List<Analysis>();
            Dep2Analysis = new List<Analysis>();
            Dep3Analysis = new List<Analysis>();
            Dep4Analysis = new List<Analysis>();
            Dep5Analysis = new List<Analysis>();

            for (int i = 1; i < 6; i++)
            {
                List<int> IDs = new List<int>();

                SqlDataReader depAnalysiseader = DBClass.GeneralReader("SELECT AnalysisID FROM DepartmentAnalysis WHERE AnalysisID = " + i + ";");

                while (depAnalysiseader.Read())
                {
                    IDs.Add(depAnalysiseader.GetInt32(0));
                }

                DBClass.KnowledgeDBConnection.Close();

                foreach (var ID in IDs)
                {
                    SqlDataReader AnalysisReader = DBClass.GeneralReader("SELECT * FROM Analysis WHERE AnalysisID = " + ID + ";");

                    if (AnalysisReader.Read())
                    {
                        Analysis temp = new Analysis
                        {
                            AnalysisName = AnalysisReader["AnalysisName"].ToString(),
                            AnalysisType = AnalysisReader["AnalysisType"].ToString(),
                            AnalysisResult = AnalysisReader["AnalysisResult"].ToString(),
                            AnalysisCreatedDate = AnalysisReader["AnalysisCreatedDate"].ToString(),
                            AnalysisStatus = AnalysisReader["AnalysisStatus"].ToString(),
                            //Add join to general reader to get owner name
                            //OwnerName = KIReader["Username"].ToString(),
                            AnalysisID = Int32.Parse(AnalysisReader["AnalysisID"].ToString())
                        };

                        switch (i)
                        {
                            case 1:
                                Dep1Analysis.Add(temp);
                                break;
                            case 2:
                                Dep2Analysis.Add(temp);
                                break;
                            case 3:
                                Dep3Analysis.Add(temp);
                                break;
                            case 4:
                                Dep4Analysis.Add(temp);
                                break;
                            case 5:
                                Dep5Analysis.Add(temp);
                                break;
                            default:
                                break;
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }
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
