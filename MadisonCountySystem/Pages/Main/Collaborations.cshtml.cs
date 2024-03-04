using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Main
{
    public class CollaborationsModel : PageModel
    {
        public List<Collab> CollabList { get; set; }
        public List<UserCollab> UserCollabs { get; set; }
        public String SelectedCollabName { get; set; }
        public int UserID { get; set; }

        public CollaborationsModel()
        {
            CollabList = new List<Collab>();
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
                UserID = Int32.Parse(HttpContext.Session.GetString("userID"));
                HttpContext.Session.SetString("LibType", "Main");
                SqlDataReader collabReader = DBClass.CollabReader();
                while (collabReader.Read())
                {
                    CollabList.Add(new Collab
                    {
                        CollabID = Int32.Parse(collabReader["CollabID"].ToString()),
                        CollabName = collabReader["CollabName"].ToString(),
                        CollabCreatedDate = collabReader["CollabCreatedDate"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
                SqlDataReader UserCollabsReader = DBClass.UserCollabReader();
                while (UserCollabsReader.Read())
                {
                    if (UserID == Int32.Parse(UserCollabsReader["UserID"].ToString()))
                    {
                        UserCollabs.Add(new UserCollab
                        {
                            UserCollabID = Int32.Parse(UserCollabsReader["UserCollabID"].ToString()),
                            UserRole = UserCollabsReader["UserRole"].ToString(),
                            UserID = Int32.Parse(UserCollabsReader["UserID"].ToString()),
                            CollabID = Int32.Parse(UserCollabsReader["CollabID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
        public IActionResult OnPostNavCollab(int selectedCollab)
        {
            return RedirectToPage("/Collabs/Index", new { collabID = selectedCollab });
        }

        public IActionResult OnPostCreateCollab()
        {
            return RedirectToPage("/RecordCreate/CreateCollab");
        }
    }
}
