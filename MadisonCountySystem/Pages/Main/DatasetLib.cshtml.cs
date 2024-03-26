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
        public List<Dataset> Dep1Data { get; set; }
        public List<Dataset> Dep2Data { get; set; }
        public List<Dataset> Dep3Data { get; set; }
        public List<Dataset> Dep4Data { get; set; }
        public List<Dataset> Dep5Data { get; set; }
        public List<Department> ActiveDepts { get; set; }
        public List<DepartmentDataset> asscDepts { get; set; }


        public DatasetModel()
        {
            DatasetList = new List<Dataset>();
            asscDepts = new List<DepartmentDataset>();
        }

        public void OnGet(String actionType)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
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

            Dep1Data = new List<Dataset>();
            Dep2Data = new List<Dataset>();
            Dep3Data = new List<Dataset>();
            Dep4Data = new List<Dataset>();
            Dep5Data = new List<Dataset>();

            for (int i = 1; i < 6; i++)
            {
                List<int> IDs = new List<int>();

                SqlDataReader depDataReader = DBClass.GeneralReader("SELECT DatasetID FROM DepartmentDataset WHERE DepartmentID = " + i + ";");

                while (depDataReader.Read())
                {
                    IDs.Add(depDataReader.GetInt32(0));
                }

                DBClass.KnowledgeDBConnection.Close();

                foreach (var ID in IDs)
                {
                    SqlDataReader DataReader = DBClass.GeneralReader("SELECT * FROM Dataset WHERE DatasetID = " + ID + ";");

                    if (DataReader.Read())
                    {
                        Dataset temp = new Dataset
                        {
                            DatasetName = DataReader["DatasetName"].ToString(),
                            DatasetType = DataReader["DatasetType"].ToString(),
                            DatasetContents = DataReader["DatasetContents"].ToString(),
                            DatasetCreatedDate = DataReader["DatasetCreatedDate"].ToString(),
                            DatasetStatus = DataReader["DatasetStatus"].ToString(),
                            //Add join to general reader to get owner name
                            //OwnerName = KIReader["Username"].ToString(),
                            DatasetID = Int32.Parse(DataReader["DatasetID"].ToString())
                        };

                        switch (i)
                        {
                            case 1:
                                Dep1Data.Add(temp);
                                break;
                            case 2:
                                Dep2Data.Add(temp);
                                break;
                            case 3:
                                Dep3Data.Add(temp);
                                break;
                            case 4:
                                Dep4Data.Add(temp);
                                break;
                            case 5:
                                Dep5Data.Add(temp);
                                break;
                            default:
                                break;
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }
            if (HttpContext.Session.GetString("typeUser") != "Admin" && HttpContext.Session.GetString("typeUser") != "Super")
            {
                DepartmentItems();
            }
            SqlDataReader depKI = DBClass.DepartmentDatasetReader();
            while (depKI.Read())
            {
                asscDepts.Add(new DepartmentDataset
                {
                    DatasetID = Int32.Parse(depKI["DatasetID"].ToString()),
                    DepartmentName = depKI["DepartmentName"].ToString()
                });
            }
            DBClass.KnowledgeDBConnection.Close();

            List<String> asscDept2 = new List<String>();
            foreach (var data in DatasetList)
            {
                foreach (var dep in asscDepts)
                {
                    if (dep.DatasetID == data.DatasetID)
                    {
                        asscDept2.Add(dep.DepartmentName);
                    }
                }
                data.Departments = new List<String>();
                data.Departments.AddRange(asscDept2);
                asscDept2.Clear();
            }
            GetActiveDepts();
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

        public void DepartmentItems()
        {
            DatasetList = new List<Dataset>();
            if (HttpContext.Session.GetInt32("dep1") == 1)
            {
                DatasetList.AddRange(Dep1Data);
            }
            if (HttpContext.Session.GetInt32("dep2") == 1)
            {
                DatasetList.AddRange(Dep2Data);
            }
            if (HttpContext.Session.GetInt32("dep3") == 1)
            {
                DatasetList.AddRange(Dep3Data);
            }
            if (HttpContext.Session.GetInt32("dep4") == 1)
            {
                DatasetList.AddRange(Dep4Data);
            }
            if (HttpContext.Session.GetInt32("dep5") == 1)
            {
                DatasetList.AddRange(Dep5Data);
            }


            DatasetList = DatasetList.GroupBy(obj => obj.DatasetID).Select(group => group.First()).ToList();
        }

        public void GetActiveDepts()
        {
            ActiveDepts = new List<Department>();

            if (HttpContext.Session.GetString("typeUser") != "Admin" && HttpContext.Session.GetString("typeUser") != "Super")
            {
                for (int i = 1; i < 6; i++)
                {
                    SqlDataReader depReader = DBClass.DepartmentReader();
                    while (depReader.Read())
                    {
                        if (Int32.Parse(depReader["DepartmentID"].ToString()) == i)
                        {
                            if (HttpContext.Session.GetInt32("dep" + i) == 1)
                            {
                                ActiveDepts.Add(new Department
                                {
                                    DepartmentID = i,
                                    DepartmentName = depReader["DepartmentName"].ToString()
                                });
                            }
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }
            else
            {
                SqlDataReader depReader = DBClass.DepartmentReader();
                while (depReader.Read())
                {
                    ActiveDepts.Add(new Department
                    {
                        DepartmentID = Int32.Parse(depReader["DepartmentID"].ToString()),
                        DepartmentName = depReader["DepartmentName"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostAddDep(int selectedDep)
        {
            String testQuery = "SELECT * FROM DepartmentDataset WHERE DepartmentID = ";
            testQuery += selectedDep;
            testQuery += " AND DatasetID = ";
            testQuery += SelectedItemID;
            if (DBClass.GeneralReader(testQuery).HasRows)
            {
                DBClass.KnowledgeDBConnection.Close();
                return RedirectToPage();
            }
            DBClass.KnowledgeDBConnection.Close();

            DBClass.InsertDepartmentDataset(new DepartmentDataset
            {
                DepartmentID = selectedDep,
                DatasetID = SelectedItemID
            });
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }
    }
}
