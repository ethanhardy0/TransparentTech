using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Linq;
using System.Collections.Generic;


namespace MadisonCountySystem.Pages.Main
{
    public class KnowledgeLibModel : PageModel
    {
        public List<KnowledgeItem> KnowledgeItemList { get; set; }
        public List<KnowledgeItem> Dep1Knowledge { get; set; }
        public List<KnowledgeItem> Dep2Knowledge { get; set; }
        public List<KnowledgeItem> Dep3Knowledge { get; set; }
        public List<KnowledgeItem> Dep4Knowledge { get; set; }
        public List<KnowledgeItem> Dep5Knowledge { get; set; }

        [BindProperty]
        [Required] public String Search { get; set; }

        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public KnowledgeItem SelectedKI { get; set; }
        public List<Collab> ActiveCollabs { get; set; }
        public List<Department> ActiveDepts { get; set; }

        public static List<KnowledgeItem> SearchKI { get; set; }
        public List<DepartmentKnowledge> asscDepts { get; set; }

        public KnowledgeLibModel()
        {
            KnowledgeItemList = new List<KnowledgeItem>();
            asscDepts = new List<DepartmentKnowledge>();
        }

        public void OnGet(String actionType)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            HttpContext.Session.SetString("LibType", "Main");
            if(actionType != null)
            {
                String[] parts = actionType.Split(':');
                if (parts.Length == 2)
                {
                    ActionType = parts[0];
                    SelectedItemID = Int32.Parse(parts[1]);

                    SqlDataReader SelectedKIReader = DBClass.KnowledgeItemReader();
                    while (SelectedKIReader.Read())
                    {
                        if (parts[1] == SelectedKIReader["KnowledgeID"].ToString())
                        SelectedKI = new KnowledgeItem
                        {
                            KnowledgeTitle = SelectedKIReader["KnowledgeTitle"].ToString(),
                            KnowledgeSubject = SelectedKIReader["KnowledgeSubject"].ToString(),
                            KnowledgeCategory = SelectedKIReader["KnowledgeCategory"].ToString(),
                            KnowledgePostDate = SelectedKIReader["KnowledgePostDate"].ToString(),
                            OwnerName = SelectedKIReader["Username"].ToString(),
                            KnowledgeID = Int32.Parse(SelectedKIReader["KnowledgeID"].ToString())
                        };
                    }
                    DBClass.KnowledgeDBConnection.Close();
                    if(ActionType == "Collab")
                    {
                        ActiveCollabs = new List<Collab>();
                        ActiveCollabs = AddToCollab();
                    }
                }
            }

            SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
            while (KnowledgeItemReader.Read())
            {
                KnowledgeItemList.Add(new KnowledgeItem
                {
                    KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                    KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                    KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                    // add Owner later
                    KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                    OwnerName = KnowledgeItemReader["Username"].ToString(),
                    KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString())
                });
            }
            DBClass.KnowledgeDBConnection.Close();

            

            Dep1Knowledge = new List<KnowledgeItem>();
            Dep2Knowledge = new List<KnowledgeItem>();
            Dep3Knowledge = new List<KnowledgeItem>();
            Dep4Knowledge = new List<KnowledgeItem>();
            Dep5Knowledge = new List<KnowledgeItem>();

            for (int i = 1; i < 6; i++)
            {
                List<int> IDs = new List<int>();

                SqlDataReader depKIReader = DBClass.GeneralReader("SELECT KnowledgeID FROM DepartmentKnowledge WHERE DepartmentID = " + i + ";");

                while (depKIReader.Read()) 
                { 
                    IDs.Add(depKIReader.GetInt32(0));
                }

                DBClass.KnowledgeDBConnection.Close();

                foreach (var ID in IDs)
                {
                    SqlDataReader KIReader = DBClass.GeneralReader("SELECT KnowledgeTitle, KnowledgeSubject, KnowledgeCategory, KnowledgePostDate, KnowledgeID, Username" + 
                        " FROM KnowledgeItem JOIN SysUser ON KnowledgeItem.OwnerID = SysUser.UserID WHERE KnowledgeID = " + ID + ";");

                    if (KIReader.Read())
                    {
                        KnowledgeItem temp = new KnowledgeItem{
                            KnowledgeTitle = KIReader["KnowledgeTitle"].ToString(),
                            KnowledgeSubject = KIReader["KnowledgeSubject"].ToString(),
                            KnowledgeCategory = KIReader["KnowledgeCategory"].ToString(),
                            KnowledgePostDate = KIReader["KnowledgePostDate"].ToString(),
                            OwnerName = KIReader["Username"].ToString(),
                            KnowledgeID = Int32.Parse(KIReader["KnowledgeID"].ToString())
                        };

                        switch (i)
                        {
                            case 1: 
                                Dep1Knowledge.Add(temp);
                                break;
                            case 2:
                                Dep2Knowledge.Add(temp);
                                break;
                            case 3:
                                Dep3Knowledge.Add(temp);
                                break;
                            case 4:
                                Dep4Knowledge.Add(temp);
                                break;
                            case 5:
                                Dep5Knowledge.Add(temp);
                                break;
                            default:
                                break;
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }
            if(HttpContext.Session.GetString("typeUser") != "Admin" && HttpContext.Session.GetString("typeUser") != "Super")
            {
                DepartmentItems();
            }
            SqlDataReader depKI = DBClass.DepartmentKnowledgeReader();
            while (depKI.Read())
            {
                asscDepts.Add(new DepartmentKnowledge
                {
                    KnowledgeID = Int32.Parse(depKI["KnowledgeID"].ToString()),
                    DepartmentName = depKI["DepartmentName"].ToString()
                });
            }
            DBClass.KnowledgeDBConnection.Close();
            List<String> asscDept2 = new List<String>();
            foreach (var ki in KnowledgeItemList)
            {
                foreach (var dep in asscDepts)
                {
                    if (dep.KnowledgeID == ki.KnowledgeID)
                    {
                        asscDept2.Add(dep.DepartmentName);
                    }
                }
                ki.Departments = new List<String>();
                ki.Departments.AddRange(asscDept2);
                asscDept2.Clear();
            }

            SearchKI = new List<KnowledgeItem>();
            SearchKI.AddRange(KnowledgeItemList);
            GetActiveDepts();
        }

        public IActionResult OnPostCreateKnowledgeItem()
        {
            return RedirectToPage("/RecordCreate/CreateKnowledge");
        }

        public IActionResult OnPostNarrowSearch()
        {
            foreach (var item in SearchKI)
            {
                if (item.KnowledgeTitle.ToLower().Contains(Search.ToLower()) || item.OwnerName.ToLower().Contains(Search.ToLower()))
                {
                    KnowledgeItemList.Add(item);
                }
            }
            GetActiveDepts();
            return Page();
        }

        public IActionResult OnPostClose()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteRecord()
        {
            DBClass.DeleteKnowledgeItem(SelectedItemID);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }

        public IActionResult OnPostAddCollab(int selectedCollab)
        {
            KnowledgeItemCollab KnowledgeItemCollab = new KnowledgeItemCollab
            {
                CollabID = selectedCollab,
                KnowledgeID = SelectedItemID
            };
            DBClass.InsertKnowledgeCollab(KnowledgeItemCollab);
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
            KnowledgeItemList = new List<KnowledgeItem>();
            if (HttpContext.Session.GetInt32("dep1") == 1)
            {
                KnowledgeItemList.AddRange(Dep1Knowledge);
            }
            if (HttpContext.Session.GetInt32("dep2") == 1)
            {
                KnowledgeItemList.AddRange(Dep2Knowledge);
            }
            if (HttpContext.Session.GetInt32("dep3") == 1)
            {
                KnowledgeItemList.AddRange(Dep3Knowledge);
            }
            if (HttpContext.Session.GetInt32("dep4") == 1)
            {
                KnowledgeItemList.AddRange(Dep4Knowledge);
            }
            if (HttpContext.Session.GetInt32("dep5") == 1)
            {
                KnowledgeItemList.AddRange(Dep5Knowledge);
            }


            KnowledgeItemList = KnowledgeItemList.GroupBy(obj => obj.KnowledgeID).Select(group => group.First()).ToList();
        }

        public IActionResult OnPostFilterDep(String selectedDep)
        {
            foreach(var item in SearchKI)
            {
                foreach(var dep in item.Departments)
                {
                    if(dep == selectedDep)
                    {
                        KnowledgeItemList.Add(item);
                        break;
                    }
                }
            }
            GetActiveDepts();
            ModelState.Clear();
            return Page();
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
            String testQuery = "SELECT * FROM DepartmentKnowledge WHERE DepartmentID = ";
            testQuery += selectedDep;
            testQuery += " AND KnowledgeID = ";
            testQuery += SelectedItemID;
            if (DBClass.GeneralReader(testQuery).HasRows)
            {
                DBClass.KnowledgeDBConnection.Close();
                return RedirectToPage();
            }
            DBClass.KnowledgeDBConnection.Close();

            DBClass.InsertDepartmentKnowledge(new DepartmentKnowledge 
            { 
                DepartmentID = selectedDep, 
                KnowledgeID = SelectedItemID 
            });
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }
    }
}
