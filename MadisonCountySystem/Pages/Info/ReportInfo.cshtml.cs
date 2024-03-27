using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Numerics;
using System.Threading;

namespace MadisonCountySystem.Pages.Info
{
    public class ReportInfoModel : PageModel
    {
        public CollabReport KeyItem { get; set; }
        public String PlanName { get; set; }
        public int KnowledgeID { get; set; }
        public string CollabName { get; set; }
        public KnowledgeItem KeyKI { get; set; }
        public List<KnowledgeItem> Research { get; set; }
        public List<PlanStep> PlanSteps { get; set; }
        public List<SysUser> UserList { get; set; }
        public ReportInfoModel()
        {
            PlanSteps = new List<PlanStep>();
            Research = new List<KnowledgeItem>();
            UserList = new List<SysUser>();
        }
        public void OnGet(int reportID)
        {
            KnowledgeID = -1;
            SqlDataReader reportReader = DBClass.KeyReportReader();
            while (reportReader.Read())
            {
                if (reportID == Int32.Parse(reportReader["CollabReportID"].ToString()))
                {
                    KeyItem = new CollabReport
                    {
                        CollabReportID = Int32.Parse(reportReader["CollabReportID"].ToString()),
                        KeyID = Int32.Parse(reportReader["KeyID"].ToString()),
                        KeyType = reportReader["KeyType"].ToString(),
                        ReportCreatedDate = reportReader["ReportCreatedDate"].ToString(),
                        CollabID = Int32.Parse(reportReader["CollabID"].ToString())
                    };
                }
            }
            DBClass.KnowledgeDBConnection.Close();

            SqlDataReader collabReader = DBClass.CollabReader();
            while (collabReader.Read())
            {
                if (KeyItem.CollabID == Int32.Parse(collabReader["CollabID"].ToString()))
                {
                    CollabName = collabReader["CollabName"].ToString();
                }
            }
            DBClass.KnowledgeDBConnection.Close();

            if (KeyItem.KeyType == "Plan")
            {
                SqlDataReader stepReader = DBClass.PlanStepReader();
                while (stepReader.Read())
                {
                    if (KeyItem.KeyID == Int32.Parse(stepReader["PlanID"].ToString()))
                    {
                        PlanSteps.Add(new PlanStep
                        {
                            PlanStepName = stepReader["PlanStepName"].ToString(),
                            StepData = stepReader["StepData"].ToString(),
                            StepCreatedDate = stepReader["StepCreatedDate"].ToString(),
                            DueDate = stepReader["DueDate"].ToString(),
                            PlanID = Int32.Parse(stepReader["PlanID"].ToString())
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
                SqlDataReader planReader = DBClass.PlanReader();
                while (planReader.Read())
                {
                    if (KeyItem.KeyID == Int32.Parse(planReader["PlanID"].ToString()))
                    {
                        PlanName = planReader["PlanName"].ToString();
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
            else if (KeyItem.KeyType == "Knowledge Item")
            {
                KnowledgeID = KeyItem.KeyID;
                SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
                while (KnowledgeItemReader.Read())
                {
                    if (KeyItem.KeyID == Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()))
                    {
                        KeyKI = new KnowledgeItem
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
                        };
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }

            SqlDataReader reportItemReader = DBClass.ReportItemReader(KeyItem.CollabReportID);
            while (reportItemReader.Read())
            {
                if (reportItemReader["ItemType"].ToString() == "Knowledge")
                {
                    if (KnowledgeID != Int32.Parse(reportItemReader["KnowledgeID"].ToString()))
                    {
                        Research.Add(new KnowledgeItem
                        {
                            KnowledgeTitle = reportItemReader["KnowledgeTitle"].ToString(),
                            KnowledgeSubject = reportItemReader["KnowledgeSubject"].ToString(),
                            KnowledgeCategory = reportItemReader["KnowledgeCategory"].ToString(),
                            KnowledgeInformation = reportItemReader["KnowledgeInformation"].ToString(),
                            KnowledgePostDate = reportItemReader["KnowledgePostDate"].ToString(),
                            OwnerID = Int32.Parse(reportItemReader["OwnerID"].ToString()),
                            OwnerFirst = reportItemReader["FirstName"].ToString(),
                            OwnerLast = reportItemReader["LastName"].ToString(),
                            OwnerName = reportItemReader["Username"].ToString(),
                            //Strengths = reportItemReader["Strengths"].ToString(),
                            //Weaknesses = reportItemReader["Weaknesses"].ToString(),
                            //Opportunities = reportItemReader["Opportunities"].ToString(),
                            //Threats = reportItemReader["Threats"].ToString()
                        });
                    }
                }
                else if (reportItemReader["ItemType"].ToString() == "User")
                {
                    UserList.Add(new SysUser
                    {
                        Username = reportItemReader["Username"].ToString(),
                        FirstName = reportItemReader["FirstName"].ToString(),
                        LastName = reportItemReader["LastName"].ToString(),
                        Email = reportItemReader["Email"].ToString(),
                        Phone = reportItemReader["Phone"].ToString(),
                    });
                }
            }
            DBClass.KnowledgeDBConnection.Close();
        }
    }
}
