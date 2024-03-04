using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;


namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateKnowledgeModel : PageModel
    {
        public KnowledgeItem KnowledgeItem { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeTitle { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeSubject { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeCategory { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeInformation { get; set; }
        public String KnowledgePostDate { get; set; }
        public static String CurrentLocation { get; set; }
        public KnowledgeItemCollab KnowledgeCollab { get; set; }
        [BindProperty]
        public bool IsSwotItem { get; set; } // This property is bound to the checkbox
        [BindProperty]
        public String Strengths { get; set; }
        [BindProperty]
        public String Weakness { get; set; }
        [BindProperty]
        public String Opportunities { get; set; }
        [BindProperty]
        public String Threats { get; set; }


        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CurrentLocation = HttpContext.Session.GetString("LibType");
            }

        }

        public IActionResult OnPost()
        {
            ModelState.Clear();
            Strengths = null;
            Weakness = null;
            Opportunities = null;
            Threats = null;
            return Page();
        }

        public IActionResult OnPostAddDB()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }


            KnowledgeItem = new KnowledgeItem()
            {
                KnowledgeTitle = KnowledgeTitle,
                KnowledgeSubject = KnowledgeSubject,
                KnowledgeCategory = KnowledgeCategory,
                KnowledgeInformation = KnowledgeInformation,
                KnowledgePostDate = DateTime.Now.ToString(),
                OwnerID = Int32.Parse(HttpContext.Session.GetString("userID")),
                Strengths = Strengths,
                Opportunities = Opportunities,
                Threats = Threats,
                Weaknesses = Weakness
            };
            int newKnowledgeID = DBClass.InsertKnowledgeItem(KnowledgeItem);
            DBClass.KnowledgeDBConnection.Close();

            if (CurrentLocation == "Collab")
            {
                KnowledgeCollab = new KnowledgeItemCollab
                {
                    KnowledgeID = newKnowledgeID,
                    CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"))
                };
                DBClass.InsertKnowledgeCollab(KnowledgeCollab);
                DBClass.KnowledgeDBConnection.Close();
                return RedirectToPage("/Collabs/KnowledgeList");
            }
            else
            {
                return RedirectToPage("/Main/KnowledgeLib");
            }
        }
        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();

                if (IsSwotItem)
                {
                    Strengths = "Learning is good";
                    Weakness = "Learning is tough";
                    Opportunities = "Learning is fun sometimes";
                    Threats = "You should probably go to class to learn";
                }
                KnowledgeTitle = "How to learn";
                KnowledgeSubject = "Wisdom";
                KnowledgeCategory = "Book";
                KnowledgeInformation = "300 Pages";
            }
            return Page();
        }
        public IActionResult OnPostCancel()
        {
            ModelState.Clear();
            KnowledgeTitle = null;
            KnowledgeSubject = null;
            KnowledgeCategory = null;
            KnowledgeInformation = null;
            Strengths = null;
            Weakness = null;
            Opportunities = null;
            Threats = null;
            return Page();
        }
    }
}
