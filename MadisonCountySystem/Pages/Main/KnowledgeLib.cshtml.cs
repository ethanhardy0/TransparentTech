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

        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public KnowledgeItem SelectedKI { get; set; }
        public List<Collab> ActiveCollabs { get; set; }

        public KnowledgeLibModel()
        {
            KnowledgeItemList = new List<KnowledgeItem>();
        }

        public void OnGet(String actionType)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            HttpContext.Session.SetString("LibType", "Main");
            if(actionType != null)
            {
                String[] parts = actionType.Split(':');
                if (parts.Length == 2)
                {
                    ActionType = parts[0];
                    SelectedItemID = Int32.Parse(parts[1]);

                    SqlDataReader SelectedKIReader = DBClass.KnowledgeItemReader();
                    while (SelectedKIReader.Read())
                    {
                        if (parts[1] == SelectedKIReader["KnowledgeID"].ToString())
                        SelectedKI = new KnowledgeItem
                        {
                            KnowledgeTitle = SelectedKIReader["KnowledgeTitle"].ToString(),
                            KnowledgeSubject = SelectedKIReader["KnowledgeSubject"].ToString(),
                            KnowledgeCategory = SelectedKIReader["KnowledgeCategory"].ToString(),
                            // add Owner later
                            KnowledgePostDate = SelectedKIReader["KnowledgePostDate"].ToString(),
                            OwnerName = SelectedKIReader["Username"].ToString(),
                            KnowledgeID = Int32.Parse(SelectedKIReader["KnowledgeID"].ToString())
                        };
                    }
                    DBClass.KnowledgeDBConnection.Close();
                    if(ActionType == "Collab")
                    {
                        ActiveCollabs = new List<Collab>();
                        ActiveCollabs = AddToCollab();
                    }
                }
            }

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
        //public IActionResult OnPostAddCollab(int selectedKnowledgeItem)
        //{

        //    return RedirectToPage("/ButtonCollab/KnowledgeButton", new { itemID = selectedKnowledgeItem});
        //}
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
                        OwnerName = KnowledgeItemReader["Username"].ToString(),
                        KnowledgeID = Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString())
                    });
                }
            }
            DBClass.KnowledgeDBConnection.Close();
            ModelState.Clear();
            Search = null;
            return Page();
        }

        public IActionResult OnPostClose()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteRecord()
        {
            DBClass.DeleteKnowledgeItem(SelectedItemID);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }

        public IActionResult OnPostAddCollab(int selectedCollab)
        {
            KnowledgeItemCollab KnowledgeItemCollab = new KnowledgeItemCollab
            {
                CollabID = selectedCollab,
                KnowledgeID = SelectedItemID
            };
            DBClass.InsertKnowledgeCollab(KnowledgeItemCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }

        public List<Collab> AddToCollab()
        {
            int UserID = Int32.Parse(HttpContext.Session.GetString("userID"));
            List<UserCollab> UserCollabs = new List<UserCollab>();
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

            List<Collab> CollabList = new List<Collab>(); 
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
            return CollabList;
        }
    }
}
