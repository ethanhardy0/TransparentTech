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
        public int KnowledgeTypeID { get; set; }
        public SWOT SWOT { get; set; }
        public PEST PEST { get; set; }

        public KnowledgeInfoModel()
        {
            KnowledgeItem = new KnowledgeItem();
            SWOT = new SWOT();
            PEST = new PEST();
        }

        public void OnGet(int kiID)
        {
            KnowledgeID = kiID;

            SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
            while (KnowledgeItemReader.Read())
            {
                if (KnowledgeID == Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()))
                {
                    KnowledgeTypeID = Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString());

                    KnowledgeItem.KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString();
                    KnowledgeItem.KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString();
                    KnowledgeItem.KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString();
                    KnowledgeItem.KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString();
                    KnowledgeItem.KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString();
                    KnowledgeItem.OwnerName = KnowledgeItemReader["Username"].ToString();
                    KnowledgeItem.OwnerFirst = KnowledgeItemReader["FirstName"].ToString();
                    KnowledgeItem.OwnerLast = KnowledgeItemReader["LastName"].ToString();

                    if (Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 2)
                    {
                        /*SWOT.KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString();
                        SWOT.KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString();
                        SWOT.KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString();
                        SWOT.KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString();
                        SWOT.KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString();
                        SWOT.OwnerName = KnowledgeItemReader["Username"].ToString();
                        SWOT.OwnerFirst = KnowledgeItemReader["FirstName"].ToString();
                        SWOT.OwnerLast = KnowledgeItemReader["LastName"].ToString();*/
                        SWOT.Strengths = KnowledgeItemReader["Strengths"].ToString();
                        SWOT.Weaknesses = KnowledgeItemReader["Weaknesses"].ToString();
                        SWOT.Opportunities = KnowledgeItemReader["Opportunities"].ToString();
                        SWOT.Threats = KnowledgeItemReader["Threats"].ToString();

                    } else if (Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 3)
                    {
                        /*PEST.KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString();
                        PEST.KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString();
                        PEST.KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString();
                        PEST.KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString();
                        PEST.KnowledgePostDate = KnowledgeItemReader["KnowledgePostDate"].ToString();
                        PEST.OwnerName = KnowledgeItemReader["Username"].ToString();
                        PEST.OwnerFirst = KnowledgeItemReader["FirstName"].ToString();
                        PEST.OwnerLast = KnowledgeItemReader["LastName"].ToString();*/
                        PEST.Political = KnowledgeItemReader["Political"].ToString();
                        PEST.Economic = KnowledgeItemReader["Economic"].ToString();
                        PEST.Social = KnowledgeItemReader["Social"].ToString();
                        PEST.Technological = KnowledgeItemReader["Technological"].ToString();
                    }

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
