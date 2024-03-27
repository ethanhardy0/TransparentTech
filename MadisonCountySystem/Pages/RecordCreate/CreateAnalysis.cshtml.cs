using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateAnalysisModel : PageModel
    {
        public List<Dataset> DatasetList { get; set; }
        public List<KnowledgeItem> KnowledgeItemList { get; set; }
        public Analysis Analysis { get; set; }
        public AnalysisCollab AnalysisCollab { get; set; }
        public static String CurrentLocation { get; set; }

        [BindProperty]
        [Required]
        public String AnalysisName { get; set; }
        [BindProperty]
        [Required]
        public String AnalysisType { get; set; }
        //[BindProperty]
        //[Required]
        public String activeDataset { get; set; }

        public String activeKI { get; set; }
        [BindProperty] public String DataType { get; set; }
        [BindProperty] public int PredictYear { get; set; }
        [BindProperty] public double InflationRate { get; set; }

        public CreateAnalysisModel()
        {
            DatasetList = new List<Dataset>();
            KnowledgeItemList = new List<KnowledgeItem>();
        }

        public void OnGet(int existingDatasetID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CurrentLocation = HttpContext.Session.GetString("LibType");
                SqlDataReader DatasetReader = DBClass.DatasetReader();
                while (DatasetReader.Read())
                {
                    DatasetList.Add(new Dataset
                    {
                        DatasetID = Int32.Parse(DatasetReader["DatasetID"].ToString()),
                        DatasetName = DatasetReader["DatasetName"].ToString(),
                    });
                }
                DBClass.KnowledgeDBConnection.Close();

                if(existingDatasetID > 0)
                {
                    DatasetList = new List<Dataset>();
                    SqlDataReader dsReader2 = DBClass.DatasetReader();
                    while (dsReader2.Read())
                    {
                        if(Int32.Parse(dsReader2["DatasetID"].ToString()) == existingDatasetID)
                        {
                            DatasetList.Add(new Dataset
                            {
                                DatasetID = existingDatasetID,
                                DatasetName = dsReader2["DatasetName"].ToString()
                            });
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }

                SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
                while (KnowledgeItemReader.Read())
                {
                    KnowledgeItemList.Add(new KnowledgeItem
                    {
                        KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()),
                        KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostAddDB(int selectedDataset, int selectedKI)
        {

            if (!ModelState.IsValid)
            {
                SqlDataReader DatasetReader = DBClass.DatasetReader();
                while (DatasetReader.Read())
                {
                    DatasetList.Add(new Dataset
                    {
                        DatasetID = Int32.Parse(DatasetReader["DatasetID"].ToString()),
                        DatasetName = DatasetReader["DatasetName"].ToString(),
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
                SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
                while (KnowledgeItemReader.Read())
                {
                    KnowledgeItemList.Add(new KnowledgeItem
                    {
                        KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()),
                        KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
                return Page(); // Return to the page to display validation errors
            }

            Analysis = new Analysis
            {
                AnalysisName = AnalysisName,
                AnalysisType = AnalysisType,
                DatasetID = selectedDataset,
                OwnerID = Int32.Parse(HttpContext.Session.GetString("userID")),
                KnowledgeID = selectedKI,
                AnalysisCreatedDate = DateTime.Now.ToString()
            };

            int newAnalysisID = DBClass.InsertAnalysis(Analysis);
            DBClass.KnowledgeDBConnection.Close();

            if (CurrentLocation == "Collab")
            {
                AnalysisCollab = new AnalysisCollab
                {
                    AnalysisID = newAnalysisID,
                    CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"))
                };

                DBClass.InsertAnalysisCollab(AnalysisCollab);
                DBClass.KnowledgeDBConnection.Close();
                return RedirectToPage("/Collabs/AnalysisList");
            }
            else
            {
                return RedirectToPage("/Main/AnalysisLib");
            }

        }

        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                AnalysisName = "Fun Analysis";
                SqlDataReader DatasetReader = DBClass.DatasetReader();
                while (DatasetReader.Read())
                {
                    DatasetList.Add(new Dataset
                    {
                        DatasetID = Int32.Parse(DatasetReader["DatasetID"].ToString()),
                        DatasetName = DatasetReader["DatasetName"].ToString(),
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
                SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
                while (KnowledgeItemReader.Read())
                {
                    KnowledgeItemList.Add(new KnowledgeItem
                    {
                        KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()),
                        KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
            return Page();
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            AnalysisName = null;
            SqlDataReader DatasetReader = DBClass.DatasetReader();
            while (DatasetReader.Read())
            {
                DatasetList.Add(new Dataset
                {
                    DatasetID = Int32.Parse(DatasetReader["DatasetID"].ToString()),
                    DatasetName = DatasetReader["DatasetName"].ToString(),
                });
            }
            DBClass.KnowledgeDBConnection.Close();
            SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
            while (KnowledgeItemReader.Read())
            {
                KnowledgeItemList.Add(new KnowledgeItem
                {
                    KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()),
                    KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString()
                });
            }
            DBClass.KnowledgeDBConnection.Close();
            return Page();
        }
    }
}
