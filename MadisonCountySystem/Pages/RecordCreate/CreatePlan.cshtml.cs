using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"));
                CollabName = HttpContext.Session.GetString("collabName");
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
                CollabID = CollabID
            };
            DBClass.InsertPlan(SysPlan);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Collabs/PlanList");
        }
    }
}
