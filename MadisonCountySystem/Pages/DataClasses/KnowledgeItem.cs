namespace MadisonCountySystem.Pages.DataClasses
{
    public class KnowledgeItem
    {
        public int KnowledgeID { get; set; }
        public String? KnowledgeTitle { get; set; }
        public String? KnowledgeSubject { get; set; }
        public String? KnowledgeCategory { get; set; }
        public String? KnowledgeInformation { get; set; }
        public String? KnowledgePostDate { get; set; }
        public int OwnerID { get; set; }
        public String? OwnerName { get; set; }
        public String? OwnerFirst { get; set; }
        public String? OwnerLast { get; set; }
        public String? Strengths { get; set; }
        public String? Weaknesses { get; set; }
        public String? Opportunities { get; set; }
        public String? Threats { get; set; }
    }
}
