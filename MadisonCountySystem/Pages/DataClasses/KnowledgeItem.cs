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
        public int KnowledgeTypeID { get; set; }
        public String? OwnerName { get; set; }
        public String? OwnerFirst { get; set; }
        public String? OwnerLast { get; set; }
        public List<String> Departments { get; set; }
    }
}
