using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs
{
    public class CollabUsersModel : PageModel
    {
        public String CollabID { get; set; }
        public String CollabName { get; set; }
        public List<SysUser> UserList { get; set; }
        public List<UserCollab> UserCollabs { get; set; }

        public CollabUsersModel()
        {
            UserList = new List<SysUser>();
            UserCollabs = new List<UserCollab>();
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

                SqlDataReader userCollabReader = DBClass.UserCollabReader();
                while (userCollabReader.Read())
                {
                    if (CollabID == userCollabReader["CollabID"].ToString())
                    {
                        UserCollabs.Add(new UserCollab
                        {
                            UserID = Int32.Parse(userCollabReader["UserID"].ToString()),
                            CollabID = Int32.Parse(userCollabReader["CollabID"].ToString()),
                            UserRole = userCollabReader["UserRole"].ToString()
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader userReader = DBClass.UserReader();
                while (userReader.Read())
                {
                    foreach (var userCollab in UserCollabs)
                    {
                        if (userCollab.UserID == Int32.Parse(userReader["UserID"].ToString()))
                        {
                            UserList.Add(new SysUser
                            {
                                Username = userReader["Username"].ToString(),
                                FirstName = userReader["FirstName"].ToString(),
                                LastName = userReader["LastName"].ToString(),
                                Email = userReader["Email"].ToString(),
                                Phone = userReader["Phone"].ToString(),
                                UserRole = userCollab.UserRole
                            });
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostLinkItem()
        {
            return RedirectToPage("/Collabs/Merge/AddUser");
        }
    }
}
