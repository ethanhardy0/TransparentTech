using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Numerics;

namespace MadisonCountySystem.Pages.Main
{
    public class UserHomeModel : PageModel
    {
        public SysUser LoggedUser { get; set; }
        public String? PhotoDir { get; set; }
        public List<Collab> Collabs { get; set; }
        public List<Department> Departments { get; set; }
        public IActionResult OnGet()
        {
            // Redirects user if not logged in
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
            else
            {
                // Set Library type to Main (No collab selected)
                HttpContext.Session.SetString("LibType", "Main");

                // Read the logged in user
                SqlDataReader userReader = DBClass.LoggedUserReader(Int32.Parse(HttpContext.Session.GetString("userID")));
                LoggedUser = new SysUser();

                // Put into data object
                if (userReader.Read())
                {
                    LoggedUser.Username = userReader["Username"].ToString();
                    LoggedUser.Email = userReader["Email"].ToString();
                    LoggedUser.FirstName = userReader["FirstName"].ToString();
                    LoggedUser.LastName = userReader["LastName"].ToString();
                    LoggedUser.Phone = userReader["Phone"].ToString();
                    LoggedUser.UserType = userReader["UserType"].ToString();
                }

                DBClass.KnowledgeDBConnection.Close();

                // Gets the profile picture directory
                PhotoDir = DBClass.UserPhotoReader(Int32.Parse(HttpContext.Session.GetString("userID")));

                DBClass.KnowledgeDBConnection.Close();

                Collabs = new List<Collab>();

                if (HttpContext.Session.GetString("typeUser") != "Admin")
                {
                    SqlDataReader read = DBClass.GeneralReader("SELECT Collaboration.CollabID, CollabName FROM UserCollab JOIN Collaboration ON Collaboration.CollabID " +
                        "= UserCollab.CollabID WHERE UserCollab.UserID = " + HttpContext.Session.GetString("userID") + ";");

                    while (read.Read())
                    {
                        Collabs.Add(new Collab
                        {
                            CollabName = read["CollabName"].ToString(),
                            CollabID = Int32.Parse(read["CollabID"].ToString())
                        });
                    }

                    DBClass.KnowledgeDBConnection.Close();
                }
                else
                {
                    SqlDataReader collabReader = DBClass.CollabReader();
                    while (collabReader.Read())
                    {
                        Collabs.Add(new Collab
                        {
                            CollabName = collabReader["CollabName"].ToString(),
                            CollabID = Int32.Parse(collabReader["CollabID"].ToString())
                        });
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }

                Departments = new List<Department>();
                if (HttpContext.Session.GetString("typeUser") == "Admin")
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
                    for (int i = 1; i <= 5; i++)
                    {
                        if (HttpContext.Session.GetInt32("dep" + i) == 1)
                        {
                            SqlDataReader depReader = DBClass.DepartmentReader();
                            while (depReader.Read())
                            {
                                if (Int32.Parse(depReader["DepartmentID"].ToString()) == i)
                                {
                                    Departments.Add(new Department
                                    {
                                        DepartmentID = i,
                                        DepartmentName = depReader["DepartmentName"].ToString()
                                    });
                                    break;
                                }
                            }
                            DBClass.KnowledgeDBConnection.Close();
                        }
                    }
                }

                return Page();
            }

        }
    }
}
