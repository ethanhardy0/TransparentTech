using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs
{
    public class KnowledgeListModel : PageModel
    {
        public static String CollabID { get; set; }
        public String CollabName { get; set; }
        public List<KnowledgeItem> KnowledgeList { get; set; }
        public List<KnowledgeItemCollab> KnowledgeCollabs { get; set; }
        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public KnowledgeItem SelectedKI { get; set; }

        public KnowledgeListModel()
        {
            KnowledgeList = new List<KnowledgeItem>();
            KnowledgeCollabs = new List<KnowledgeItemCollab>();
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
                HttpContext.Session.SetString("LibType", "Collab");
                CollabName = HttpContext.Session.GetString("collabName");
                CollabID = HttpContext.Session.GetString("collabID");
                if (actionType != null)
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
                    }
                }

                SqlDataReader knowledgeCollabReader = DBClass.KnowledgeCollabReader();
                while (knowledgeCollabReader.Read())
                {
                    if (CollabID == knowledgeCollabReader["CollabID"].ToString())
                    {
                        KnowledgeCollabs.Add(new KnowledgeItemCollab
                        {
                            KnowledgeID = Int32.Parse(knowledgeCollabReader["KnowledgeID"].ToString()),
                            CollabID = Int32.Parse(knowledgeCollabReader["CollabID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader knowledgeReader = DBClass.KnowledgeItemReader();
                while (knowledgeReader.Read())
                {
                    foreach (var knowledgeCollab in KnowledgeCollabs)
                    {
                        if (knowledgeCollab.KnowledgeID == Int32.Parse(knowledgeReader["KnowledgeID"].ToString()))
                        {
                            KnowledgeList.Add(new KnowledgeItem
                            {
                                KnowledgeID = Int32.Parse(knowledgeReader["KnowledgeID"].ToString()),
                                KnowledgeTitle = knowledgeReader["KnowledgeTitle"].ToString(),
                                KnowledgeSubject = knowledgeReader["KnowledgeSubject"].ToString(),
                                KnowledgeCategory = knowledgeReader["KnowledgeCategory"].ToString(),
                                KnowledgeInformation = knowledgeReader["KnowledgeInformation"].ToString(),
                                KnowledgePostDate = knowledgeReader["KnowledgePostDate"].ToString(),
                                OwnerID = Int32.Parse(knowledgeReader["OwnerID"].ToString()),
                                OwnerFirst = knowledgeReader["FirstName"].ToString(),
                                OwnerLast = knowledgeReader["LastName"].ToString(),
                                OwnerName = knowledgeReader["Username"].ToString(),
                            });
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
        public IActionResult OnPostLinkItem()
        {
            return RedirectToPage("/Collabs/Merge/AddKI");
        }

        public IActionResult OnPostCreateItem()
        {
            return RedirectToPage("/RecordCreate/CreateKnowledge");
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

        public IActionResult OnPostRemoveCollab()
        {
            DBClass.RemoveKnowledgeItemCollab(SelectedItemID, Int32.Parse(CollabID));
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }
    }
}