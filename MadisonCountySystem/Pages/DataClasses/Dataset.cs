namespace MadisonCountySystem.Pages.DataClasses
{
    public class Dataset
    {
        public int DatasetID { get; set; }
        public String? DatasetName { get; set; }
        public String? DatasetType { get; set; }
        public String? DatasetContents { get; set; }
        public String? DatasetCreatedDate { get; set; }
        public String? DatasetStatus { get; set; }
        public int OwnerID { get; set; }
        public String? OwnerName { get; set; }
        public String? OwnerFirst { get; set; }
        public String? OwnerLast { get; set; }
        public List<String> Departments { get; set; }
    }
}
