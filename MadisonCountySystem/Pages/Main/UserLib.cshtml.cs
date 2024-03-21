using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages.Main
{
    public class UserLibModel : PageModel
    {
        public List<SysUser> UserList { get; set; }
        public bool Action { get; set; }
        public static int UserRemoveID { get; set; }
        public String UserRemoveName { get; set; }
		public UserLibModel()
        {
            UserList = new List<SysUser>();
        }
        public void OnGet(int userRemove)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else if(HttpContext.Session.GetString("typeUser") != "Admin" && HttpContext.Session.GetString("typeUser") != "Super")
            {
                HttpContext.Response.Redirect("/Main/Collaborations");
            }
            else
            {
                Action = false;
                if(userRemove != 0)
                {
                    UserRemoveID = userRemove;
					SqlDataReader deleteUser = DBClass.UserReader();
					while (deleteUser.Read())
					{
						if(UserRemoveID == Int32.Parse(deleteUser["UserID"].ToString()))
                        {
                            UserRemoveName = deleteUser["Username"].ToString();
                            Action = true;
						}
					}
					DBClass.KnowledgeDBConnection.Close();
				}
                HttpContext.Session.SetString("LibType", "Main");
                SqlDataReader UserReader = DBClass.UserReader();
                while (UserReader.Read())
                {
                    String Rolename;
                    if (UserReader["UserType"].ToString() == "Admin")
                    {
                        Rolename = "Administrator";
                    }
                    else if(UserReader["UserType"].ToString() == "Super")
                    {
                        Rolename = "Super User";
                    }
                    else
                    {
                        Rolename = "Standard User";
                    }
                    UserList.Add(new SysUser
                    {
                        Username = UserReader["Username"].ToString(),
                        UserID = Int32.Parse(UserReader["UserID"].ToString()),
						Email = UserReader["Email"].ToString(),
                        FirstName = UserReader["FirstName"].ToString(),
                        LastName = UserReader["LastName"].ToString(),
                        Phone = UserReader["Phone"].ToString(),
                        UserRole = Rolename
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
        public IActionResult OnPostCreateUser()
        {
            return RedirectToPage("/RecordCreate/SignupForm");
        }

		public IActionResult OnPostClose()
		{
			return RedirectToPage();
		}

		public IActionResult OnPostDeleteRecord()
		{
			DBClass.RemoveUser(UserRemoveID);
			DBClass.KnowledgeDBConnection.Close();
			return RedirectToPage();
		}
	}
}
