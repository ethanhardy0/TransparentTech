using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages.Main
{
    public class UserLibModel : PageModel
    {
        public List<SysUser> UserList { get; set; }
        public UserLibModel()
        {
            UserList = new List<SysUser>();
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
                HttpContext.Session.SetString("LibType", "Main");
                SqlDataReader UserReader = DBClass.UserReader();
                while (UserReader.Read())
                {
                    UserList.Add(new SysUser
                    {
                        Username = UserReader["Username"].ToString(),
                        Email = UserReader["Email"].ToString(),
                        FirstName = UserReader["FirstName"].ToString(),
                        LastName = UserReader["LastName"].ToString(),
                        Phone = UserReader["Phone"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
        public IActionResult OnPostCreateUser()
        {
            return RedirectToPage("/RecordCreate/SignupForm");
        }
    }
}
