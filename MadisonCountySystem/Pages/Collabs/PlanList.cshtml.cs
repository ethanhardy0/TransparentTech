using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages.Collabs
{
    public class PlanListModel : PageModel
    {
        public String CollabID { get; set; }
        public String CollabName { get; set; }
        public List<SysPlan> PlansList { get; set; }
        public List<KnowledgeItem> KnowledgeList { get; set; }
        public List<SWOT> SWOTList { get; set; }
        public List<PEST> PESTList { get; set; }
        public List<KnowledgeItemCollab> KnowledgeCollabs { get; set; }
        public String ActionType { get; set; }
        public static int SelectedItemID { get; set; }
        public SysPlan SelectedPlan { get; set; }
        public PlanListModel()
        {
            PlansList = new List<SysPlan>();
            KnowledgeList = new List<KnowledgeItem>();
            SWOTList = new List<SWOT>();
            PESTList = new List<PEST>();
            KnowledgeCollabs = new List<KnowledgeItemCollab>();
        }

        public bool ShowModal { get; set; } = false;

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
                    if (parts.Length == 2)
                    {
                        ActionType = parts[0];
                        SelectedItemID = Int32.Parse(parts[1]);
                        SqlDataReader SelectedPlanReader = DBClass.PlanReader();
                        while (SelectedPlanReader.Read())
                        {
                            if (parts[1] == SelectedPlanReader["PlanID"].ToString())
                                SelectedPlan = new SysPlan
                                {
                                    PlanName = SelectedPlanReader["PlanName"].ToString(),
                                    PlanContents = SelectedPlanReader["PlanContents"].ToString(),
                                    PlanCreatedDate = SelectedPlanReader["PlanCreatedDate"].ToString(),
                                };
                        }
                        DBClass.KnowledgeDBConnection.Close();
                    }
                }
                HttpContext.Session.SetString("LibType", "Collab");
                CollabID = HttpContext.Session.GetString("collabID");
                CollabName = HttpContext.Session.GetString("collabName");
                SqlDataReader planReader = DBClass.PlanReader();
                while (planReader.Read())
                {
                    if (CollabID == planReader["CollabID"].ToString())
                    {
                        PlansList.Add(new SysPlan
                        {
                            PlanID = Int32.Parse(planReader["PlanID"].ToString()),
                            PlanName = planReader["PlanName"].ToString(),
                            PlanContents = planReader["PlanContents"].ToString(),
                            PlanCreatedDate = planReader["PlanCreatedDate"].ToString(),
                            CollabID = Int32.Parse(planReader["PlanID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }

        public IActionResult OnPostPrint(String selectedItemID)
        {
            if (selectedItemID == null)
            {
                return Page();
            }
            var parts = selectedItemID.Split('-');
            if (parts.Length != 2)
            {
                return Page();
            }
            else
            {
                var itemType = parts[0];
                var itemID = parts[1];

                if (itemType == "Plan")
                {
                    return RedirectToPage("/Collabs/PrintReport", new { ID = itemID, type = itemType });
                }
                else if (itemType == "Knowledge Item")
                {
                    return RedirectToPage("/Collabs/PrintReport", new { ID = itemID, type = itemType });
                }
                else
                {
                    return Page();
                }
            }
        }
        public IActionResult OnPostShowSteps(int selectedPlan)
        {
            return RedirectToPage("/Collabs/PlanStepTable", new { ActionType = "None:0:" + selectedPlan.ToString() });
        }

        public IActionResult OnPostAddPlan()
        {
            return RedirectToPage("/RecordCreate/CreatePlan");
        }
        public IActionResult OnPostClose()
        {
            return RedirectToPage("/Collabs/PlanList");
        }
        public void OnPostToggleModal()
        {
            ShowModal = !ShowModal;
            HttpContext.Session.SetString("LibType", "Collab");
            CollabID = HttpContext.Session.GetString("collabID");
            CollabName = HttpContext.Session.GetString("collabName");
            SqlDataReader planReader = DBClass.PlanReader();
            while (planReader.Read())
            {
                if (CollabID == planReader["CollabID"].ToString())
                {
                    PlansList.Add(new SysPlan
                    {
                        PlanID = Int32.Parse(planReader["PlanID"].ToString()),
                        PlanName = planReader["PlanName"].ToString(),
                        PlanContents = planReader["PlanContents"].ToString(),
                        PlanCreatedDate = planReader["PlanCreatedDate"].ToString(),
                        CollabID = Int32.Parse(planReader["PlanID"].ToString())
                    });
                }
            }
            DBClass.KnowledgeDBConnection.Close();
            DBClass.KnowledgeDBConnection.Close();
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

                        if (Int32.Parse(knowledgeReader["KnowledgeTypeID"].ToString()) == 1)
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
                        } else if (Int32.Parse(knowledgeReader["KnowledgeTypeID"].ToString()) == 2)
                        {
                            SWOTList.Add(new SWOT
                            {
                                KnowledgeID = Int32.Parse(knowledgeReader["KnowledgeID"].ToString()),
                                KnowledgeTitle = knowledgeReader["KnowledgeTitle"].ToString(),
                                KnowledgeSubject = knowledgeReader["KnowledgeSubject"].ToString(),
                                KnowledgeCategory = knowledgeReader["KnowledgeCategory"].ToString(),
                                KnowledgeInformation = knowledgeReader["KnowledgeInformation"].ToString(),
                                KnowledgePostDate = knowledgeReader["KnowledgePostDate"].ToString(),
                                KnowledgeTypeID = Int32.Parse(knowledgeReader["KnowledgeTypeID"].ToString()),
                                OwnerName = knowledgeReader["Username"].ToString(),
                                OwnerFirst = knowledgeReader["FirstName"].ToString(),
                                OwnerLast = knowledgeReader["LastName"].ToString(),
                                Strengths = knowledgeReader["Strengths"].ToString(),
                                Weaknesses = knowledgeReader["Weaknesses"].ToString(),
                                Opportunities = knowledgeReader["Opportunities"].ToString(),
                                Threats = knowledgeReader["Threats"].ToString()
                            });
                        } else if (Int32.Parse(knowledgeReader["KnowledgeTypeID"].ToString()) == 3)
                        {
                            PESTList.Add(new PEST
                            {
                                KnowledgeID = Int32.Parse(knowledgeReader["KnowledgeID"].ToString()),
                                KnowledgeTitle = knowledgeReader["KnowledgeTitle"].ToString(),
                                KnowledgeSubject = knowledgeReader["KnowledgeSubject"].ToString(),
                                KnowledgeCategory = knowledgeReader["KnowledgeCategory"].ToString(),
                                KnowledgeInformation = knowledgeReader["KnowledgeInformation"].ToString(),
                                KnowledgePostDate = knowledgeReader["KnowledgePostDate"].ToString(),
                                KnowledgeTypeID = Int32.Parse(knowledgeReader["KnowledgeTypeID"].ToString()),
                                OwnerName = knowledgeReader["Username"].ToString(),
                                OwnerFirst = knowledgeReader["FirstName"].ToString(),
                                OwnerLast = knowledgeReader["LastName"].ToString(),
                                Political = knowledgeReader["Political"].ToString(),
                                Economic = knowledgeReader["Economic"].ToString(),
                                Social = knowledgeReader["Social"].ToString(),
                                Technological = knowledgeReader["Technological"].ToString()
                            });
                        }
                    }

                }
            }
            DBClass.KnowledgeDBConnection.Close();
        }

        public IActionResult OnPostDeleteRecord()
        {
            DBClass.DeletePlan(SelectedItemID);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage();
        }
    }
}
