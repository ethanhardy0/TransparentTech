using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs.Merge
{
    public class AddUserModel : PageModel
    {
        public List<SysUser> UserList { get; set; }
        public List<UserCollab> UserCollabs { get; set; }
        public List<int> AddedUsers { get; set; }
        public static String CollabID { get; set; }
        public String CollabName { get; set; }
        public UserCollab UserCollab { get; set; }
        [BindProperty]
        [Required]
        public String UserRole { get; set; }

        public AddUserModel()
        {
            UserList = new List<SysUser>();
            UserCollabs = new List<UserCollab>();
            AddedUsers = new List<int>();
        }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else if(HttpContext.Session.GetString("typeUser") != "Admin")
            {
                HttpContext.Response.Redirect("/Main/Collaborations");
            }
            else
            {


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
                            CollabID = Int32.Parse(userCollabReader["CollabID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader activeReader = DBClass.UserReader();
                while (activeReader.Read())
                {
                    foreach (var userCollab in UserCollabs)
                    {
                        if (userCollab.UserID == Int32.Parse(activeReader["UserID"].ToString()))
                        {
                            AddedUsers.Add(Int32.Parse(activeReader["UserID"].ToString()));
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader userReader = DBClass.UserReader();
                while (userReader.Read())
                {
                    UserList.Add(new SysUser
                    {
                        UserID = Int32.Parse(userReader["UserID"].ToString()),
                        Username = userReader["Username"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();

                List<SysUser> itemsToRemove = new List<SysUser>();

                foreach (var item in UserList)
                {
                    foreach (int id in AddedUsers)
                    {
                        if (item.UserID == id)
                        {
                            itemsToRemove.Add(item);
                            break;
                        }
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    UserList.Remove(item);
                }
            }
        }

        public IActionResult OnPostUpdateDB(int selectedUser)
        {
            UserCollab = new UserCollab
            {
                UserID = selectedUser,
                CollabID = Int32.Parse(CollabID),
                UserRole = UserRole
            };
            DBClass.InsertUserCollab(UserCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Collabs/CollabUsers");
        }
    }
}
