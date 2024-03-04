namespace MadisonCountySystem.Pages.DataClasses
{
    public class CollabReport
    {
        public int CollabReportID { get; set; }
        public int KeyID { get; set; }
        public String? KeyType { get; set; }
        public String? ReportCreatedDate { get; set; }
        public int CollabID { get; set; }
        public int KnowledgeID { get; set; }
        public int UserID { get; set; }
        public int CollabReportParent { get; set; }
        public String? KeyName { get; set; }
        public String? ItemType { get; set; }
        public String? CollabName { get; set; }
    }
}
