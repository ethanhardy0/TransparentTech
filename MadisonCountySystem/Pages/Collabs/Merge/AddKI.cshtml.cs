using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Collabs.Merge
{
    public class AddKIModel : PageModel
    {
        public List<KnowledgeItem> KnowledgeList { get; set; }
        public List<KnowledgeItemCollab> KnowledgeCollabs { get; set; }
        public List<int> AddedKnowledge { get; set; }
        public static String CollabID { get; set; }
        public String CollabName { get; set; }
        public KnowledgeItemCollab KnowledgeCollab { get; set; }

        public AddKIModel()
        {
            KnowledgeList = new List<KnowledgeItem>();
            KnowledgeCollabs = new List<KnowledgeItemCollab>();
            AddedKnowledge = new List<int>();
        }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {

                CollabName = HttpContext.Session.GetString("collabName");
                CollabID = HttpContext.Session.GetString("collabID");

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

                SqlDataReader activeReader = DBClass.KnowledgeItemReader();
                while (activeReader.Read())
                {
                    foreach (var knowledgeCollab in KnowledgeCollabs)
                    {
                        if (knowledgeCollab.KnowledgeID == Int32.Parse(activeReader["KnowledgeID"].ToString()))
                        {
                            AddedKnowledge.Add(Int32.Parse(activeReader["KnowledgeID"].ToString()));
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader knowledgeReader = DBClass.KnowledgeItemReader();
                while (knowledgeReader.Read())
                {
                    KnowledgeList.Add(new KnowledgeItem
                    {
                        KnowledgeID = Int32.Parse(knowledgeReader["KnowledgeID"].ToString()),
                        KnowledgeTitle = knowledgeReader["KnowledgeTitle"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();

                List<KnowledgeItem> itemsToRemove = new List<KnowledgeItem>();

                foreach (var item in KnowledgeList)
                {
                    foreach (int id in AddedKnowledge)
                    {
                        if (item.KnowledgeID == id)
                        {
                            itemsToRemove.Add(item);
                            break;
                        }
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    KnowledgeList.Remove(item);
                }
            }
        }

        public IActionResult OnPostUpdateDB(int selectedKnowledge)
        {
            KnowledgeCollab = new KnowledgeItemCollab
            {
                KnowledgeID = selectedKnowledge,
                CollabID = Int32.Parse(CollabID)
            };
            DBClass.InsertKnowledgeCollab(KnowledgeCollab);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Collabs/KnowledgeList");
        }
    }
}
