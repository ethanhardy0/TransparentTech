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
        public List<UserCollab> UserCollabs {  get; set; }
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

                SqlDataReader read = DBClass.GeneralReader("SELECT Collaboration.CollabID, CollabName, UserRole FROM UserCollab JOIN Collaboration ON Collaboration.CollabID " +
                    "= UserCollab.CollabID WHERE UserCollab.UserID = " + HttpContext.Session.GetString("userID") + ";");

                Collabs = new List<Collab>();
                UserCollabs = new List<UserCollab>();

                while (read.Read())
                {
                    Collabs.Add(new Collab
                    {
                        CollabName = read["CollabName"].ToString(),
                        CollabID = Int32.Parse(read["CollabID"].ToString())
                    });

                    UserCollabs.Add(new UserCollab
                    {
                        UserRole = read["UserRole"].ToString()
                    });
                }

                DBClass.KnowledgeDBConnection.Close();

                return Page();
            }

        }
    }
}
