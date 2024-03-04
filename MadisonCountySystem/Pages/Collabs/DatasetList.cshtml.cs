using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs
{
    public class DatasetListModel : PageModel
    {
        public String CollabID { get; set; }
        public String CollabName { get; set; }
        public List<Dataset> DatasetList { get; set; }
        public List<DatasetCollab> DatasetCollabs { get; set; }

        public DatasetListModel()
        {
            DatasetList = new List<Dataset>();
            DatasetCollabs = new List<DatasetCollab>();
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

                SqlDataReader datasetCollabReader = DBClass.DatasetCollabReader();
                while (datasetCollabReader.Read())
                {
                    if (CollabID == datasetCollabReader["CollabID"].ToString())
                    {
                        DatasetCollabs.Add(new DatasetCollab
                        {
                            DatasetID = Int32.Parse(datasetCollabReader["DatasetID"].ToString()),
                            CollabID = Int32.Parse(datasetCollabReader["CollabID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader datasetReader = DBClass.DatasetReader();
                while (datasetReader.Read())
                {
                    foreach (var datasetCollab in DatasetCollabs)
                    {
                        if (datasetCollab.DatasetID == Int32.Parse(datasetReader["DatasetID"].ToString()))
                        {
                            DatasetList.Add(new Dataset
                            {
                                DatasetID = datasetCollab.DatasetID,
                                DatasetName = datasetReader["DatasetName"].ToString(),
                                DatasetType = datasetReader["DatasetType"].ToString(),
                                DatasetContents = datasetReader["DatasetContents"].ToString(),
                                DatasetCreatedDate = datasetReader["DatasetCreatedDate"].ToString(),
                                OwnerID = Int32.Parse(datasetReader["OwnerID"].ToString()),
                            });
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostLinkItem()
        {
            return RedirectToPage("/Collabs/Merge/AddDataset");
        }

        public IActionResult OnPostCreateItem()
        {
            return RedirectToPage("/RecordCreate/CreateDataset");
        }
    }
}
