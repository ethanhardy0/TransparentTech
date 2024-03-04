namespace MadisonCountySystem.Pages.DataClasses
{
    public class Analysis
    {
        public int AnalysisID { get; set; }
        public String? AnalysisName { get; set; }
        public String? AnalysisType { get; set; }
        public String? AnalysisResult { get; set; }
        public String? AnalysisCreatedDate { get; set; }
        public int DatasetID { get; set; }
        public int OwnerID { get; set; }
        public int KnowledgeID { get; set; }
        public String? OwnerName { get; set; }
        public String? OwnerFirst { get; set; }
        public String? OwnerLast { get; set; }
        public String? DatasetName { get; set; }
        public String? DatasetType { get; set; }
        public String? DatasetContents { get; set; }
        public String? DatasetCreatedDate { get; set; }
        public String? KnowledgeTitle { get; set; }
        public String? KnowledgeSubject { get; set; }
        public String? KnowledgeCategory { get; set; }
        public String? KnowledgeInformation { get; set; }
        public String? KnowledgePostDate { get; set; }
    }
}
