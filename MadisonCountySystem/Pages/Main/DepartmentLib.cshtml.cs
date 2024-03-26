using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using MadisonCountySystem.Pages.DataClasses;
using System.ComponentModel.DataAnnotations;

namespace MadisonCountySystem.Pages.Main
{
    public class DepartmentLibModel : PageModel
    {
        [BindProperty,Required]
        public int activeDataset { get; set; }
        public List<Department> Departments { get; set; }
        public static int SelectedID { get; set; }
        public String SelectedName { get; set; }
        public List<SysUser> ExistingUsers { get; set; }
        public List<SysUser> OtherUsers { get; set; }
        public List<UserDepartment> UserDepartments { get; set; }
        public String ActionType { get; set; }

        public DepartmentLibModel() 
        {
            Departments = new List<Department>();
            
        }
        public void OnGet(String SelectedDepartmentID)
        {
            HttpContext.Session.SetString("LibType", "Main");
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else if (HttpContext.Session.GetString("typeUser") != "Admin")
            {
                HttpContext.Response.Redirect("/Main/Collaborations");
            }
            else
            {
                if (SelectedDepartmentID == null)
                {
                    SqlDataReader depReader = DBClass.DepartmentReader();
                    while (depReader.Read())
                    {
                        Departments.Add(new Department
                        {
                            DepartmentID = Int32.Parse(depReader["DepartmentID"].ToString()),
                            DepartmentName = depReader["DepartmentName"].ToString()
                        });
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
                else
                {
                    String[] parts = SelectedDepartmentID.Split(':');
                    if (parts.Length == 2)
                    {
                        ActionType = parts[0];
                        SelectedID = Int32.Parse(parts[1]);
                    }

                    SqlDataReader depReader = DBClass.DepartmentReader();
                    while (depReader.Read())
                    {
                        if (Int32.Parse(depReader["DepartmentID"].ToString()) == SelectedID)
                        {
                            SelectedName = depReader["DepartmentName"].ToString();
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                    UserDepartments = new List<UserDepartment>();
                    SqlDataReader userDepReader = DBClass.UserDepartmentReader();
                    while(userDepReader.Read())
                    {
                        if (Int32.Parse(userDepReader["DepartmentID"].ToString()) == SelectedID)
                        {
                            UserDepartments.Add(new UserDepartment
                            {
                                UserID = Int32.Parse(userDepReader["UserID"].ToString()),
                                DepartmentID = Int32.Parse(userDepReader["DepartmentID"].ToString())
                            });
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();

                    List<int> EUsers = new List<int>();
                    foreach(var user in UserDepartments)
                    {
                        EUsers.Add(user.UserID);
                    }

                    List<int> OUsers = new List<int>();
                    SqlDataReader userReader1 = DBClass.UserReader();
                    while (userReader1.Read())
                    {
                        if (userReader1["UserType"].ToString() != "Admin" && userReader1["Email"].ToString() != null)
                        {
                            OUsers.Add(Int32.Parse(userReader1["UserID"].ToString()));
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                    OUsers.RemoveAll(item => EUsers.Contains(item));

                    ExistingUsers = new List<SysUser>();
                    OtherUsers = new List<SysUser>();
                    SqlDataReader existUserReader = DBClass.UserReader();
                    while(existUserReader.Read())
                    {
                        foreach(int id in EUsers)
                        {
                            if(id == Int32.Parse(existUserReader["UserID"].ToString()))
                            {
                                ExistingUsers.Add(new SysUser
                                {
                                    UserID = id,
                                    Username = existUserReader["Username"].ToString(),
                                    FirstName = existUserReader["FirstName"].ToString(),
                                    LastName = existUserReader["LastName"].ToString()
                                });
                                break;
                            }
                        }
                        foreach(int id in OUsers)
                        {
                            if (id == Int32.Parse(existUserReader["UserID"].ToString()))
                            {
                                OtherUsers.Add(new SysUser
                                {
                                    UserID = id,
                                    Username = existUserReader["Username"].ToString(),
                                    FirstName = existUserReader["FirstName"].ToString(),
                                    LastName = existUserReader["LastName"].ToString()
                                });
                                break;
                            }
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();

                    
                }
            }
        }

        public IActionResult OnPostClose()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostAddUser()
        {
            DBClass.InsertUserDepartment(SelectedID, activeDataset);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveUser()
        {
            DBClass.DeleteUserDepartment(SelectedID, activeDataset);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }
    }
}
