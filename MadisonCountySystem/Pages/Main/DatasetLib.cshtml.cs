using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;


namespace MadisonCountySystem.Pages.Main
{
    public class DatasetModel : PageModel
    {
        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public Dataset SelectedDataset { get; set; }
        public List<Collab> ActiveCollabs { get; set; }
        public List<Dataset> DatasetList { get; set; }


        public DatasetModel()
        {
            DatasetList = new List<Dataset>();
        }

        public void OnGet(String actionType)
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
            if (actionType != null)
            {
                String[] parts = actionType.Split(':');
                if (parts.Length == 2)
                {
                    ActionType = parts[0];
                    SelectedItemID = Int32.Parse(parts[1]);

                    SqlDataReader SelectedDataserReader = DBClass.DatasetReader();
                    while (SelectedDataserReader.Read())
                    {
                        if (parts[1] == SelectedDataserReader["DatasetID"].ToString())
                            SelectedDataset = new Dataset
                            {
                                DatasetName = SelectedDataserReader["DatasetName"].ToString(),
                                DatasetContents = SelectedDataserReader["DatasetContents"].ToString(),
                                DatasetType = SelectedDataserReader["DatasetType"].ToString(),
                                // add Owner later
                                DatasetCreatedDate = SelectedDataserReader["DatasetCreatedDate"].ToString(),
                                OwnerName = SelectedDataserReader["Username"].ToString(),
                                DatasetID = Int32.Parse(SelectedDataserReader["DatasetID"].ToString())
                            };
                    }
                    DBClass.KnowledgeDBConnection.Close();
                    if (ActionType == "Collab")
                    {
                        ActiveCollabs = new List<Collab>();
                        ActiveCollabs = AddToCollab();
                    }
                }
            }

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

        //public IActionResult OnPostAddCollab(int selectedDataset)
        //{

        //    return RedirectToPage("/ButtonCollab/DatasetButton", new { itemID = selectedDataset });
        //}
        public IActionResult OnPostCreateDataset()
        {
            return RedirectToPage("/RecordCreate/CreateDataset");
        }

        public IActionResult OnPostClose()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteRecord()
        {
            DBClass.DeleteDataset(SelectedItemID);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }

        public IActionResult OnPostAddCollab(int selectedCollab)
        {
            DatasetCollab DatasetCollab = new DatasetCollab
            {
                CollabID = selectedCollab,
                DatasetID = SelectedItemID
            };
            DBClass.InsertDatasetCollab(DatasetCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }

        public List<Collab> AddToCollab()
        {
            int UserID = Int32.Parse(HttpContext.Session.GetString("userID"));
            List<UserCollab> UserCollabs = new List<UserCollab>();
            SqlDataReader UserCollabsReader = DBClass.UserCollabReader();
            while (UserCollabsReader.Read())
            {
                if (UserID == Int32.Parse(UserCollabsReader["UserID"].ToString()))
                {
                    UserCollabs.Add(new UserCollab
                    {
                        CollabID = Int32.Parse(UserCollabsReader["CollabID"].ToString())
                    });
                }
            }
            DBClass.KnowledgeDBConnection.Close();

            List<Collab> CollabList = new List<Collab>();
            SqlDataReader collabReader = DBClass.CollabReader();
            while (collabReader.Read())
            {
                foreach (UserCollab userCollab in UserCollabs)
                {
                    if (userCollab.CollabID == Int32.Parse(collabReader["CollabID"].ToString()))
                    {
                        CollabList.Add(new Collab
                        {
                            CollabID = Int32.Parse(collabReader["CollabID"].ToString()),
                            CollabName = collabReader["CollabName"].ToString()
                        });
                    }
                }

            }
            DBClass.KnowledgeDBConnection.Close();
            return CollabList;
        }
    }
}
