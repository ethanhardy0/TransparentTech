//This Razor Page exists to populate the session with the current Collab, accessible from all other Collab RPs
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages.Collabs
{
    public class IndexModel : PageModel
    {
        public String? CollabID { get; set; }
        public static String CollabName { get; set; }
        public IActionResult OnGet(int collabID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
            else
            {
                CollabID = collabID.ToString();
                SqlDataReader collabReader = DBClass.CollabReader();
                while (collabReader.Read())
                {
                    if (CollabID == collabReader["CollabID"].ToString())
                    {
                        CollabName = collabReader["CollabName"].ToString();
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
                HttpContext.Session.SetString("collabID", CollabID);
                HttpContext.Session.SetString("collabName", CollabName);

                return RedirectToPage("/Collabs/PlanList");
            }
        }
    }
}
