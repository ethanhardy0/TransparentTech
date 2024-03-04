using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs.Merge
{
    public class AddDatasetModel : PageModel
    {
        public List<Dataset> DatasetList { get; set; }
        public List<DatasetCollab> DatasetCollabs { get; set; }
        public List<int> AddedDatasets { get; set; }
        public static String CollabID { get; set; }
        public String CollabName { get; set; }
        public DatasetCollab DatasetCollab { get; set; }

        public AddDatasetModel()
        {
            DatasetList = new List<Dataset>();
            DatasetCollabs = new List<DatasetCollab>();
            AddedDatasets = new List<int>();
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

                SqlDataReader activeReader = DBClass.DatasetReader();
                while (activeReader.Read())
                {
                    foreach (var datasetCollab in DatasetCollabs)
                    {
                        if (datasetCollab.DatasetID == Int32.Parse(activeReader["DatasetID"].ToString()))
                        {
                            AddedDatasets.Add(Int32.Parse(activeReader["DatasetID"].ToString()));
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader datasetReader = DBClass.DatasetReader();
                while (datasetReader.Read())
                {
                    DatasetList.Add(new Dataset
                    {
                        DatasetID = Int32.Parse(datasetReader["DatasetID"].ToString()),
                        DatasetName = datasetReader["DatasetName"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
                List<Dataset> itemsToRemove = new List<Dataset>();

                foreach (var item in DatasetList)
                {
                    foreach (int id in AddedDatasets)
                    {
                        if (item.DatasetID == id)
                        {
                            itemsToRemove.Add(item);
                            break;
                        }
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    DatasetList.Remove(item);
                }
            }
        }

        public IActionResult OnPostUpdateDB(int selectedDataset)
        {
            DatasetCollab = new DatasetCollab
            {
                DatasetID = selectedDataset,
                CollabID = Int32.Parse(CollabID)
            };
            DBClass.InsertDatasetCollab(DatasetCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Collabs/DatasetList");
        }
    }
}
