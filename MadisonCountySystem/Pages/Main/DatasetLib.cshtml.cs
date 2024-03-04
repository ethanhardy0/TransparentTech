using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;


namespace MadisonCountySystem.Pages.Main
{
    public class DatasetModel : PageModel
    {
        public List<Dataset> DatasetList { get; set; }


        public DatasetModel()
        {
            DatasetList = new List<Dataset>();
        }

        public void OnGet()
        {
            HttpContext.Session.SetString("LibType", "Main");
            SqlDataReader DatasetReader = DBClass.DatasetReader();
            while (DatasetReader.Read())
            {
                DatasetList.Add(new Dataset
                {
                    DatasetID = Int32.Parse(DatasetReader["DatasetID"].ToString()),
                    DatasetName = DatasetReader["DatasetName"].ToString(),
                    DatasetType = DatasetReader["DatasetType"].ToString(),
                    DatasetCreatedDate = DatasetReader["DatasetCreatedDate"].ToString(),
                });
            }
            DBClass.KnowledgeDBConnection.Close();
        }

        public IActionResult OnPostAddCollab(int selectedDataset)
        {

            return RedirectToPage("/ButtonCollab/DatasetButton", new { itemID = selectedDataset });
        }
        public IActionResult OnPostCreateDataset()
        {
            return RedirectToPage("/RecordCreate/CreateDataset");
        }

    }
}
