using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Numerics;
using System.Threading;

namespace MadisonCountySystem.Pages.Collabs
{
    public class PrintReportModel : PageModel
    {
        public static String ItemID { get; set; }
        public string PlanName { get; set; }
        public string KnowledgeID { get; set; }
        public String dataType { get; set; }
        public static String CollabID { get; set; }
        public string CollabName { get; set; }
        public List<KnowledgeItem> KeyStandard { get; set; }
        public List<SWOT> KeySWOT { get; set; }
        public List<PEST> KeyPEST { get; set; }
        public List<KnowledgeItem> ResearchStandard { get; set; }
        public List<SWOT> ResearchSWOT { get; set; }
        public List<PEST> ResearchPEST { get; set; }
        public List<KnowledgeItemCollab> KnowledgeCollabs { get; set; }
        public List<PlanStep> PlanSteps { get; set; }
        public List<SysPlan> PlansList { get; set; }
        public List<SysUser> UserList { get; set; }
        public List<UserCollab> UserCollabs { get; set; }
        public static String ReportType { get; set; }

        public static List<UserCollab> UserCollabsRef { get; set; }
        public static List<KnowledgeItemCollab> KnowledgeCollabsRef { get; set; }
        public PrintReportModel()
        {
            KeyStandard = new List<KnowledgeItem>();
            KeySWOT = new List<SWOT>();
            KeyPEST = new List<PEST>();
            PlanSteps = new List<PlanStep>();
            PlansList = new List<SysPlan>();
            KnowledgeCollabs = new List<KnowledgeItemCollab>();
            ResearchStandard = new List<KnowledgeItem>();
            ResearchSWOT = new List<SWOT>();
            ResearchPEST = new List<PEST>();
            UserList = new List<SysUser>();
            UserCollabs = new List<UserCollab>();
        }
        public void OnGet(string ID, string type)
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
                dataType = type;
                ItemID = ID;
                ReportType = type;
                KnowledgeID = "-1";

                if (type == "Plan")
                {
                    SqlDataReader stepReader = DBClass.PlanStepReader();
                    while (stepReader.Read())
                    {
                        if (int.Parse(ItemID) == int.Parse(stepReader["PlanID"].ToString()))
                        {
                            PlanSteps.Add(new PlanStep
                            {
                                PlanStepName = stepReader["PlanStepName"].ToString(),
                                StepData = stepReader["StepData"].ToString(),
                                StepCreatedDate = stepReader["StepCreatedDate"].ToString(),
                                DueDate = stepReader["DueDate"].ToString(),
                                PlanID = int.Parse(stepReader["PlanID"].ToString())
                            });
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                    SqlDataReader planReader = DBClass.PlanReader();
                    while (planReader.Read())
                    {
                        if (ItemID == planReader["PlanID"].ToString())
                        {
                            PlanName = planReader["PlanName"].ToString();
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }

                else if (type == "Knowledge Item")
                {
                    KnowledgeID = ID;
                    SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
                    while (KnowledgeItemReader.Read())
                    {
                        if (ItemID == KnowledgeItemReader["KnowledgeID"].ToString())
                        {
                            if (Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 1)
                            {
                                KeyStandard.Add(new KnowledgeItem
                                {
                                    KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                                    KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                                    KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                                    KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString(),
                                    KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                                    KnowledgeTypeID = Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()),
                                    OwnerName = KnowledgeItemReader["Username"].ToString(),
                                    OwnerFirst = KnowledgeItemReader["FirstName"].ToString(),
                                    OwnerLast = KnowledgeItemReader["LastName"].ToString(),
                                });
                            } else if (Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 2)
                            {
                                KeySWOT.Add(new SWOT
                                {
                                    KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                                    KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                                    KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                                    KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString(),
                                    KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                                    KnowledgeTypeID = Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()),
                                    OwnerName = KnowledgeItemReader["Username"].ToString(),
                                    OwnerFirst = KnowledgeItemReader["FirstName"].ToString(),
                                    OwnerLast = KnowledgeItemReader["LastName"].ToString(),
                                    Strengths = KnowledgeItemReader["Strengths"].ToString(),
                                    Weaknesses = KnowledgeItemReader["Weaknesses"].ToString(),
                                    Opportunities = KnowledgeItemReader["Opportunities"].ToString(),
                                    Threats = KnowledgeItemReader["Threats"].ToString()
                                });
                            } else if (Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 3)
                            {
                                KeyPEST.Add(new PEST
                                {
                                    KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                                    KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                                    KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                                    KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString(),
                                    KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                                    KnowledgeTypeID = Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()),
                                    OwnerName = KnowledgeItemReader["Username"].ToString(),
                                    OwnerFirst = KnowledgeItemReader["FirstName"].ToString(),
                                    OwnerLast = KnowledgeItemReader["LastName"].ToString(),
                                    Political = KnowledgeItemReader["Political"].ToString(),
                                    Economic = KnowledgeItemReader["Economic"].ToString(),
                                    Social = KnowledgeItemReader["Social"].ToString(),
                                    Technological = KnowledgeItemReader["Technological"].ToString()
                                });
                            }
                             
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();

                }
                SqlDataReader knowledgeCollabReader = DBClass.KnowledgeCollabReader();
                while (knowledgeCollabReader.Read())
                {
                    if (CollabID == knowledgeCollabReader["CollabID"].ToString())
                    {
                        KnowledgeCollabs.Add(new KnowledgeItemCollab
                        {
                            KnowledgeID = int.Parse(knowledgeCollabReader["KnowledgeID"].ToString()),
                            CollabID = int.Parse(knowledgeCollabReader["CollabID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
                SqlDataReader KnowledgeReader = DBClass.KnowledgeItemReader();
                while (KnowledgeReader.Read())
                {
                    foreach (var knowledgeCollab in KnowledgeCollabs)
                    {
                        if (knowledgeCollab.KnowledgeID == int.Parse(KnowledgeReader["KnowledgeID"].ToString()) && knowledgeCollab.KnowledgeID != int.Parse(KnowledgeID))
                        {
                            if (Int32.Parse(KnowledgeReader["KnowledgeTypeID"].ToString()) == 1)
                            {
                                ResearchStandard.Add(new KnowledgeItem
                                {
                                    KnowledgeTitle = KnowledgeReader["KnowledgeTitle"].ToString(),
                                    KnowledgeSubject = KnowledgeReader["KnowledgeSubject"].ToString(),
                                    KnowledgeCategory = KnowledgeReader["KnowledgeCategory"].ToString(),
                                    KnowledgeInformation = KnowledgeReader["KnowledgeInformation"].ToString(),
                                    KnowledgePostDate = KnowledgeReader["KnowledgePostDate"].ToString(),
                                    KnowledgeTypeID = Int32.Parse(KnowledgeReader["KnowledgeTypeID"].ToString()),
                                    OwnerName = KnowledgeReader["Username"].ToString(),
                                    OwnerFirst = KnowledgeReader["FirstName"].ToString(),
                                    OwnerLast = KnowledgeReader["LastName"].ToString(),
                                });
                            }
                            else if (Int32.Parse(KnowledgeReader["KnowledgeTypeID"].ToString()) == 2)
                            {
                                ResearchSWOT.Add(new SWOT
                                {
                                    KnowledgeTitle = KnowledgeReader["KnowledgeTitle"].ToString(),
                                    KnowledgeSubject = KnowledgeReader["KnowledgeSubject"].ToString(),
                                    KnowledgeCategory = KnowledgeReader["KnowledgeCategory"].ToString(),
                                    KnowledgeInformation = KnowledgeReader["KnowledgeInformation"].ToString(),
                                    KnowledgePostDate = KnowledgeReader["KnowledgePostDate"].ToString(),
                                    KnowledgeTypeID = Int32.Parse(KnowledgeReader["KnowledgeTypeID"].ToString()),
                                    OwnerName = KnowledgeReader["Username"].ToString(),
                                    OwnerFirst = KnowledgeReader["FirstName"].ToString(),
                                    OwnerLast = KnowledgeReader["LastName"].ToString(),
                                    Strengths = KnowledgeReader["Strengths"].ToString(),
                                    Weaknesses = KnowledgeReader["Weaknesses"].ToString(),
                                    Opportunities = KnowledgeReader["Opportunities"].ToString(),
                                    Threats = KnowledgeReader["Threats"].ToString()
                                });

                            } else if (Int32.Parse(KnowledgeReader["KnowledgeTypeID"].ToString()) == 3) { 
                                ResearchPEST.Add(new PEST
                                {
                                    KnowledgeTitle = KnowledgeReader["KnowledgeTitle"].ToString(),
                                    KnowledgeSubject = KnowledgeReader["KnowledgeSubject"].ToString(),
                                    KnowledgeCategory = KnowledgeReader["KnowledgeCategory"].ToString(),
                                    KnowledgeInformation = KnowledgeReader["KnowledgeInformation"].ToString(),
                                    KnowledgePostDate = KnowledgeReader["KnowledgePostDate"].ToString(),
                                    KnowledgeTypeID = Int32.Parse(KnowledgeReader["KnowledgeTypeID"].ToString()),
                                    OwnerName = KnowledgeReader["Username"].ToString(),
                                    OwnerFirst = KnowledgeReader["FirstName"].ToString(),
                                    OwnerLast = KnowledgeReader["LastName"].ToString(),
                                    Political = KnowledgeReader["Political"].ToString(),
                                    Economic = KnowledgeReader["Economic"].ToString(),
                                    Social = KnowledgeReader["Social"].ToString(),
                                    Technological = KnowledgeReader["Technological"].ToString()
                                });
                            }
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
                SqlDataReader userCollabReader = DBClass.UserCollabReader();
                while (userCollabReader.Read())
                {
                    if (CollabID == userCollabReader["CollabID"].ToString())
                    {
                        UserCollabs.Add(new UserCollab
                        {
                            UserID = int.Parse(userCollabReader["UserID"].ToString()),
                            CollabID = int.Parse(userCollabReader["CollabID"].ToString()),
                            UserRole = userCollabReader["UserRole"].ToString()
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();

                SqlDataReader userReader = DBClass.UserReader();
                while (userReader.Read())
                {
                    foreach (var userCollab in UserCollabs)
                    {
                        if (userCollab.UserID == int.Parse(userReader["UserID"].ToString()))
                        {
                            UserList.Add(new SysUser
                            {
                                Username = userReader["Username"].ToString(),
                                FirstName = userReader["FirstName"].ToString(),
                                LastName = userReader["LastName"].ToString(),
                                Email = userReader["Email"].ToString(),
                                Phone = userReader["Phone"].ToString(),
                                UserRole = userCollab.UserRole
                            });
                        }
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
                UserCollabsRef = new List<UserCollab>();
                KnowledgeCollabsRef = new List<KnowledgeItemCollab>();
                UserCollabsRef.AddRange(UserCollabs);
                KnowledgeCollabsRef.AddRange(KnowledgeCollabs);
            }
        }
        public IActionResult OnPostCreateReport()
        {
            CollabReport KeyReport = new CollabReport
            {
                KeyID = Int32.Parse(ItemID),
                KeyType = ReportType,
                ReportCreatedDate = DateTime.Now.ToString(),
                CollabID = Int32.Parse(CollabID)
            };
            int newReportID = DBClass.InsertKeyReport(KeyReport);

            foreach (var userCollab in UserCollabsRef)
            {
                DBClass.InsertReportUser(new CollabReport
                {
                    UserID = userCollab.UserID,
                    CollabReportParent = newReportID,
                });
                DBClass.KnowledgeDBConnection.Close();
            }
            foreach (var KnowledgeCollab in KnowledgeCollabsRef)
            {
                DBClass.InsertReportKI(new CollabReport
                {
                    KnowledgeID = KnowledgeCollab.KnowledgeID,
                    CollabReportParent = newReportID,
                });
                DBClass.KnowledgeDBConnection.Close();
            }
            return RedirectToPage("/Collabs/PlanList");
        }
    }
}
