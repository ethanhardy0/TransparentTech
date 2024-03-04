using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages.Info
{
    public class KnowledgeInfoModel : PageModel
    {
        public static int KnowledgeID { get; set; }
        public KnowledgeItem KnowledgeItem { get; set; }

        public KnowledgeInfoModel()
        {
            KnowledgeItem = new KnowledgeItem();
        }

        public void OnGet(int kiID)
        {
            KnowledgeID = kiID;

            SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
            while (KnowledgeItemReader.Read())
            {
                if (KnowledgeID == Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()))
                {
                    KnowledgeItem.KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString();
                    KnowledgeItem.KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString();
                    KnowledgeItem.KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString();
                    KnowledgeItem.KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString();
                    KnowledgeItem.KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString();
                    KnowledgeItem.OwnerName = KnowledgeItemReader["Username"].ToString();
                    KnowledgeItem.OwnerFirst = KnowledgeItemReader["FirstName"].ToString();
                    KnowledgeItem.OwnerLast = KnowledgeItemReader["LastName"].ToString();
                    KnowledgeItem.Strengths = KnowledgeItemReader["Strengths"].ToString();
                    KnowledgeItem.Weaknesses = KnowledgeItemReader["Weaknesses"].ToString();
                    KnowledgeItem.Opportunities = KnowledgeItemReader["Opportunities"].ToString();
                    KnowledgeItem.Threats = KnowledgeItemReader["Threats"].ToString();

                }
            }
            DBClass.KnowledgeDBConnection.Close();
            //might want to get owner information
            //might want to get analysis information
            //might want to get dataset information through analysis
            //might want to get spreadsheet information through dataset (for now, might change to relating to KI directly)
            //might want to get collab information
        }
    }
}
