namespace MadisonCountySystem.Pages.DataClasses
{
    public class SheetData
    {
        public int SheetDataID { get; set; }
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public String? CellData { get; set; }
        public int FileID { get; set; }
    }
}
