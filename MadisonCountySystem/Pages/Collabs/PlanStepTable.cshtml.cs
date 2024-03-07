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
        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public PlanStep SelectedStep { get; set; }

        public PlanStepTableModel()
        {
            PlanSteps = new List<PlanStep>();
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
                if (actionType != null)
                {
                    String[] parts = actionType.Split(':');
                    if (parts.Length == 3)
                    {
                        ActionType = parts[0];
                        SelectedItemID = Int32.Parse(parts[1]);
                        PlanID = Int32.Parse(parts[2]);
                        if (parts[1] != "0")
                        {
                            SqlDataReader SelectedStepReader = DBClass.PlanStepReader();
                            while (SelectedStepReader.Read())
                            {
                                if (parts[1] == SelectedStepReader["PlanStepID"].ToString())
                                    SelectedStep = new PlanStep
                                    {
                                        PlanStepName = SelectedStepReader["PlanStepName"].ToString(),
                                        StepData = SelectedStepReader["StepData"].ToString(),
                                        DueDate = SelectedStepReader["DueDate"].ToString(),
                                    };
                            }
                            DBClass.KnowledgeDBConnection.Close();
                        }
                    }
                    else { PlanID = 0; }
                }
                HttpContext.Session.SetString("LibType", "Collab");
                CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"));
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
                            PlanID = Int32.Parse(stepReader["PlanID"].ToString()),
                            PlanStepID = Int32.Parse(stepReader["PlanStepID"].ToString())
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

        public IActionResult OnPostClose()
        {
            return RedirectToPage("/Collabs/PlanStepTable", new {actionType = "None:0:" + PlanID.ToString()});
        }

        public IActionResult OnPostDeleteRecord()
        {
            DBClass.DeletePlanStep(SelectedItemID);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Collabs/PlanStepTable", new { actionType = "None:0:" + PlanID.ToString() });
        }
    }
}
