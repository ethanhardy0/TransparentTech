using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages.Main
{
    public class KnowledgeLibModel : PageModel
    {
        public List<KnowledgeItem> KnowledgeItemList { get; set; }

        [BindProperty]
        [Required] public String Search { get; set; }



        public KnowledgeLibModel()
        {
            KnowledgeItemList = new List<KnowledgeItem>();
        }

        public void OnGet()
        {
            HttpContext.Session.SetString("LibType", "Main");
            SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
            while (KnowledgeItemReader.Read())
            {
                KnowledgeItemList.Add(new KnowledgeItem
                {
                    KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                    KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                    KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                    // add Owner later
                    KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                    OwnerName = KnowledgeItemReader["Username"].ToString(),
                    KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString())
                });
            }
            DBClass.KnowledgeDBConnection.Close();
        }
        public IActionResult OnPostAddCollab(int selectedKnowledgeItem)
        {

            return RedirectToPage("/ButtonCollab/KnowledgeButton", new { itemID = selectedKnowledgeItem, itemType = "Knowledge" });
        }
        public IActionResult OnPostCreateKnowledgeItem()
        {
            return RedirectToPage("/RecordCreate/CreateKnowledge");
        }

        public IActionResult OnPostNarrowSearch()
        {
            SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
            while (KnowledgeItemReader.Read())
            {
                if (KnowledgeItemReader["KnowledgeTitle"].ToString().ToLower().Contains(Search.ToLower()) || KnowledgeItemReader["Username"].ToString().ToLower().Contains(Search.ToLower()))
                {
                    KnowledgeItemList.Add(new KnowledgeItem
                    {
                        KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                        KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                        KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                        // add Owner later
                        KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                        OwnerName = KnowledgeItemReader["Username"].ToString()
                    });
                }
            }
            DBClass.KnowledgeDBConnection.Close();
            ModelState.Clear();
            Search = null;
            return Page();
        }
    }
}
