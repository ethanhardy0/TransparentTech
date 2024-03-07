using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreatePlanStepModel : PageModel
    {
        public static int UserID { get; set; }
        public static int CollabID { get; set; }
        public String CollabName { get; set; }

        [BindProperty, Required]
        public String StepName { get; set; }

        [BindProperty, Required]
        public String StepData { get; set; }
        [BindProperty, Required]
        public DateTime DueDate { get; set; }
        public static int PlanID { get; set; }
        public PlanStep PlanStep { get; set; }
        public String PlanName { get; set; }
        public static int ExistingStepID { get; set; }
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
                ExistingStepID = ExistingID;
                PlanID = Int32.Parse(HttpContext.Session.GetString("planID"));
                UserID = Int32.Parse(HttpContext.Session.GetString("userID"));
                CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"));
                CollabName = HttpContext.Session.GetString("collabName");
                if (ExistingID != 0)
                {
                    CreateorUpdate = "Update";
                    SqlDataReader StepReader = DBClass.PlanStepReader();
                    while (StepReader.Read())
                    {
                        if (ExistingID == Int32.Parse(StepReader["PlanStepID"].ToString()))
                        {
                            StepName = StepReader["PlanStepName"].ToString();
                            StepData = StepReader["StepData"].ToString();
                            DueDate = DateTime.Parse(StepReader["DueDate"].ToString());
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }

                SqlDataReader planReader = DBClass.PlanReader();
                while (planReader.Read())
                {
                    if (PlanID == Int32.Parse(planReader["PlanID"].ToString()))
                    {
                        PlanName = planReader["PlanName"].ToString();
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            StepName = null;
            StepData = null;
            DueDate = default;
            CollabName = HttpContext.Session.GetString("collabName");
            SqlDataReader planReader = DBClass.PlanReader();
            while (planReader.Read())
            {
                if (PlanID == Int32.Parse(planReader["PlanID"].ToString()))
                {
                    PlanName = planReader["PlanName"].ToString();
                }
            }
            DBClass.KnowledgeDBConnection.Close();
            return Page();
        }

        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            StepName = "Wake Up";
            StepData = "Get out of bed, get dressed";
            DueDate = new DateTime(2024, 2, 24, 12, 0, 0);
            CollabName = HttpContext.Session.GetString("collabName");
            SqlDataReader planReader = DBClass.PlanReader();
            while (planReader.Read())
            {
                if (PlanID == Int32.Parse(planReader["PlanID"].ToString()))
                {
                    PlanName = planReader["PlanName"].ToString();
                }
            }
            DBClass.KnowledgeDBConnection.Close();
            return Page();
        }

        public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            PlanStep = new PlanStep
            {
                PlanStepName = StepName,
                StepData = StepData,
                DueDate = DueDate.ToString(),
                OwnerID = UserID,
                PlanID = PlanID,
                PlanStepID = ExistingStepID
            };
            if(ExistingStepID == 0)
            {
                DBClass.InsertPlanStep(PlanStep);
                DBClass.KnowledgeDBConnection.Close();
            }
            else
            {
                DBClass.UpdateExistingPlanStep(PlanStep);
                DBClass.KnowledgeDBConnection.Close();
            }
            return RedirectToPage("/Collabs/PlanStepTable", new { planID = PlanID });
        }
    }
}
