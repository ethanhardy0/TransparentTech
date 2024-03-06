using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Threading;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreatePlanModel : PageModel
    {
        [BindProperty]
        [Required] public String? PlanName { get; set; }
        [BindProperty]
        [Required]
        public String? Description { get; set; }
        public static int CollabID { get; set; }
        public SysPlan SysPlan { get; set; }
        public String CollabName { get; set; }
        public static int ExistingPlanID { get; set; }
        public String CreateorUpdate { get; set; }

        public void OnGet(int ExistingID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CreateorUpdate = "Create";
                ExistingPlanID = ExistingID;
                CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"));
                CollabName = HttpContext.Session.GetString("collabName");
                if (ExistingID != 0)
                {
                    CreateorUpdate = "Update";
                    SqlDataReader PlanReader = DBClass.PlanReader();
                    while (PlanReader.Read())
                    {
                        if (ExistingID == Int32.Parse(PlanReader["PlanID"].ToString()))
                        {
                            PlanName = PlanReader["PlanName"].ToString();
                            Description = PlanReader["PlanContents"].ToString();
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }

        }

        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            PlanName = "OurFirstPlan";
            Description = "This is a description for our plan.";
            CollabName = HttpContext.Session.GetString("collabName");
            return Page();
        }
        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            PlanName = null;
            Description = null;
            CollabName = HttpContext.Session.GetString("collabName");
            return Page();
        }

        public IActionResult OnPostCreatePlan()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SysPlan = new SysPlan
            {
                PlanName = PlanName,
                PlanContents = Description,
                PlanCreatedDate = DateTime.Now.ToString(),
                CollabID = CollabID,
                PlanID = ExistingPlanID
            };
            if (ExistingPlanID == 0)
            {
                DBClass.InsertPlan(SysPlan);
                DBClass.KnowledgeDBConnection.Close();
            }
            else
            {
                DBClass.UpdateExistingPlan(SysPlan);
                DBClass.KnowledgeDBConnection.Close();
            }
            return RedirectToPage("/Collabs/PlanList");
        }
    }
}
