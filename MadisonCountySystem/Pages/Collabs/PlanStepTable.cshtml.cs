using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages.Collabs
{
    public class PlanStepTableModel : PageModel
    {
        public List<PlanStep> PlanSteps { get; set; }
        public int CollabID { get; set; }
        public static int PlanID { get; set; }

        public PlanStepTableModel()
        {
            PlanSteps = new List<PlanStep>();
        }
        public void OnGet(int planID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                HttpContext.Session.SetString("LibType", "Collab");
                CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"));
                PlanID = planID;
                HttpContext.Session.SetString("planID", PlanID.ToString());
                SqlDataReader stepReader = DBClass.PlanStepReader();
                while (stepReader.Read())
                {
                    if (PlanID == Int32.Parse(stepReader["PlanID"].ToString()))
                    {
                        PlanSteps.Add(new PlanStep
                        {
                            PlanStepName = stepReader["PlanStepName"].ToString(),
                            StepData = stepReader["StepData"].ToString(),
                            StepCreatedDate = stepReader["StepCreatedDate"].ToString(),
                            DueDate = stepReader["DueDate"].ToString(),
                            PlanID = Int32.Parse(stepReader["PlanID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
        public IActionResult OnPostAddStep()
        {
            return RedirectToPage("/RecordCreate/CreatePlanStep");
        }
    }
}
