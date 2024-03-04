using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using MadisonCountySystem.Pages.Main;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace MadisonCountySystem.Pages.ButtonCollab
{
    public class AnalysisButtonModel : PageModel
    {
        public static int ItemID { get; set; }
        public String ItemName { get; set; }
        public static int UserID { get; set; }
        public AnalysisCollab AnalysisCollab { get; set; }

        public List<Collab> CollabList { get; set; }
        public List<UserCollab> UserCollabs { get; set; }
        public AnalysisButtonModel()
        {
            CollabList = new List<Collab>();
            UserCollabs = new List<UserCollab>();
        }
        public void OnGet(int itemID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                ItemID = itemID;
                UserID = Int32.Parse(HttpContext.Session.GetString("userID"));
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

                SqlDataReader AnalysisReader = DBClass.AnalysisReader();
                while (AnalysisReader.Read())
                {
                    if (itemID == Int32.Parse(AnalysisReader["AnalysisID"].ToString()))
                    {
                        ItemName = AnalysisReader["AnalysisName"].ToString();
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostUpdateDB(int selectedCollab)
        {
            AnalysisCollab = new AnalysisCollab
            {
                CollabID = selectedCollab,
                AnalysisID = ItemID
            };
            DBClass.InsertAnalysisCollab(AnalysisCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Main/AnalysisLib");
        }
    }
}