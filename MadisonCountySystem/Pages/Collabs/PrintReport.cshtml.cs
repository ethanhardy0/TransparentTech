using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
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
        public List<KnowledgeItem> KeyKI { get; set; }
        public List<KnowledgeItem> Research { get; set; }
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
            KeyKI = new List<KnowledgeItem>();
            PlanSteps = new List<PlanStep>();
            PlansList = new List<SysPlan>();
            KnowledgeCollabs = new List<KnowledgeItemCollab>();
            Research = new List<KnowledgeItem>();
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
                            KeyKI.Add(new KnowledgeItem
                            {
                                KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                                KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString(),
                                KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString(),
                                KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString(),
                                KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString(),
                                OwnerName = KnowledgeItemReader["Username"].ToString(),
                                OwnerFirst = KnowledgeItemReader["FirstName"].ToString(),
                                OwnerLast = KnowledgeItemReader["LastName"].ToString(),
                                //Strengths = KnowledgeItemReader["Strengths"].ToString(),
                                //Weaknesses = KnowledgeItemReader["Weaknesses"].ToString(),
                                //Opportunities = KnowledgeItemReader["Opportunities"].ToString(),
                                //Threats = KnowledgeItemReader["Threats"].ToString()
                            });

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
                SqlDataReader knowledgeReader = DBClass.KnowledgeItemReader();
                while (knowledgeReader.Read())
                {
                    foreach (var knowledgeCollab in KnowledgeCollabs)
                    {
                        if (knowledgeCollab.KnowledgeID == int.Parse(knowledgeReader["KnowledgeID"].ToString()) && knowledgeCollab.KnowledgeID != int.Parse(KnowledgeID))
                        {
                            Research.Add(new KnowledgeItem
                            {
                                KnowledgeID = int.Parse(knowledgeReader["KnowledgeID"].ToString()),
                                KnowledgeTitle = knowledgeReader["KnowledgeTitle"].ToString(),
                                KnowledgeSubject = knowledgeReader["KnowledgeSubject"].ToString(),
                                KnowledgeCategory = knowledgeReader["KnowledgeCategory"].ToString(),
                                KnowledgeInformation = knowledgeReader["KnowledgeInformation"].ToString(),
                                KnowledgePostDate = knowledgeReader["KnowledgePostDate"].ToString(),
                                OwnerID = int.Parse(knowledgeReader["OwnerID"].ToString()),
                                OwnerFirst = knowledgeReader["FirstName"].ToString(),
                                OwnerLast = knowledgeReader["LastName"].ToString(),
                                OwnerName = knowledgeReader["Username"].ToString(),
                                //Strengths = knowledgeReader["Strengths"].ToString(),
                                //Weaknesses = knowledgeReader["Weaknesses"].ToString(),
                                //Opportunities = knowledgeReader["Opportunities"].ToString(),
                                //Threats = knowledgeReader["Threats"].ToString()
                            });
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
