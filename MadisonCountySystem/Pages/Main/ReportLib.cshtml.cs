using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.Main
{
    public class ReportLibModel : PageModel
    {
        public List<CollabReport> KeyReports { get; set; }
        public List<CollabReport> CollabReports { get; set; }
        public List<CollabReport> CollabReports2 { get; set; }
        public ReportLibModel()
        {
            KeyReports = new List<CollabReport>();
            CollabReports = new List<CollabReport>();
            CollabReports2 = new List<CollabReport>();
        }
        public void OnGet()
        {
            HttpContext.Session.SetString("LibType", "Main");
            SqlDataReader reportReader = DBClass.KeyReportReader();
            while (reportReader.Read())
            {
                KeyReports.Add(new CollabReport
                {
                    CollabReportID = Int32.Parse(reportReader["CollabReportID"].ToString()),
                    KeyID = Int32.Parse(reportReader["KeyID"].ToString()),
                    KeyType = reportReader["KeyType"].ToString(),
                    ReportCreatedDate = reportReader["ReportCreatedDate"].ToString(),
                    CollabID = Int32.Parse(reportReader["CollabID"].ToString())
                });
            }
            DBClass.KnowledgeDBConnection.Close();

            foreach (var report in KeyReports)
            {
                if (report.KeyType == "Plan")
                {
                    SqlDataReader planReader = DBClass.PlanReader();
                    while (planReader.Read())
                    {
                        if (Int32.Parse(planReader["PlanID"].ToString()) == report.KeyID)
                        {
                            CollabReports.Add(new CollabReport
                            {
                                CollabReportID = report.CollabReportID,
                                KeyID = report.KeyID,
                                KeyType = report.KeyType,
                                ReportCreatedDate = report.ReportCreatedDate,
                                CollabID = report.CollabID,
                                KeyName = planReader["PlanName"].ToString()
                            });
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
                else if (report.KeyType == "Knowledge Item")
                {
                    SqlDataReader KIReader = DBClass.KnowledgeItemReader();
                    while (KIReader.Read())
                    {
                        if (Int32.Parse(KIReader["KnowledgeID"].ToString()) == report.KeyID)
                        {
                            CollabReports.Add(new CollabReport
                            {
                                CollabReportID = report.CollabReportID,
                                KeyID = report.KeyID,
                                KeyType = report.KeyType,
                                ReportCreatedDate = report.ReportCreatedDate,
                                CollabID = report.CollabID,
                                KeyName = KIReader["KnowledgeTitle"].ToString()
                            });
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }
            foreach (var report in CollabReports)
            {
                SqlDataReader collabReader = DBClass.CollabReader();
                while (collabReader.Read())
                {
                    if (report.CollabID == Int32.Parse(collabReader["CollabID"].ToString()))
                    {
                        CollabReports2.Add(new CollabReport
                        {
                            CollabReportID = report.CollabReportID,
                            KeyID = report.KeyID,
                            KeyType = report.KeyType,
                            ReportCreatedDate = report.ReportCreatedDate,
                            CollabID = report.CollabID,
                            KeyName = report.KeyName,
                            CollabName = collabReader["CollabName"].ToString()
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
    }
}
