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
        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public Collab SelectedCollab { get; set; }
        public String UserType { get; set; }

        public CollaborationsModel()
        {
            CollabList = new List<Collab>();
            UserCollabs = new List<UserCollab>();
        }

        public void OnGet(String actionType)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                UserType = HttpContext.Session.GetString("typeUser");

                if (actionType != null)
                {
                    String[] parts = actionType.Split(':');
                    if (parts.Length == 2)
                    {
                        ActionType = parts[0];
                        SelectedItemID = Int32.Parse(parts[1]);

                        SqlDataReader SelectedCollabReader = DBClass.CollabReader();
                        while (SelectedCollabReader.Read())
                        {
                            if (parts[1] == SelectedCollabReader["CollabID"].ToString())
                                SelectedCollab = new Collab
                                {
                                    CollabName = SelectedCollabReader["CollabName"].ToString(),
                                    CollabNotes = SelectedCollabReader["CollabNotes"].ToString(),
                                    CollabCreatedDate = SelectedCollabReader["CollabCreatedDate"].ToString(),
                                    CollabID = Int32.Parse(SelectedCollabReader["CollabID"].ToString())
                                };
                        }
                        DBClass.KnowledgeDBConnection.Close();
                    }
                }

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
                if (UserType == "Admin")
                {
                    foreach (var collab in CollabList)
                    {
                        UserCollabs.Add(new UserCollab
                        {
                            CollabID = collab.CollabID,
                            UserRole = "Administrator"
                        });
                    }
                }
                else
                {
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
        }
        public IActionResult OnPostNavCollab(int selectedCollab)
        {
            return RedirectToPage("/Collabs/Index", new { collabID = selectedCollab });
        }

        public IActionResult OnPostCreateCollab()
        {
            return RedirectToPage("/RecordCreate/CreateCollab");
        }

        public IActionResult OnPostClose()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteRecord()
        {
            DBClass.DeleteCollab(SelectedItemID);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }


    }
}
