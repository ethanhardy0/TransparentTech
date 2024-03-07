using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Formats.Asn1;
using CsvHelper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using OfficeOpenXml;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateDatasetModel : PageModel
    {
        public Dataset Dataset { get; set; }
        public String? Cols { get; set; }
        public List<SelectListItem> CSVFiles { get; set; }
        public List<IFormFile> FormFiles { get; set; }
        public FileInfo[] UploadedCSVs { get; set; }
        //[BindProperty]
        //public String? SelectedFileName { get; set; }
        [BindProperty]
        [Required] public String DatasetName { get; set; }
        [BindProperty]
        [Required] public String DatasetType { get; set; }
        [BindProperty]
        [Required] public String DatasetContents { get; set; }
        public String DatasetCreatedDate { get; set; }
        public static String CurrentLocation { get; set; }
        public DatasetCollab DatasetCollab { get; set; }
        public int newDatasetID { get; set; }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CurrentLocation = HttpContext.Session.GetString("LibType");

                String fileDir = Directory.GetCurrentDirectory() + @"\wwwroot\csvupload\";
                DirectoryInfo fileInfo = new DirectoryInfo(fileDir);
                UploadedCSVs = fileInfo.GetFiles();
                CSVFiles = new List<SelectListItem>();
                foreach (FileInfo file in UploadedCSVs)
                {
                    CSVFiles.Add(
                        new SelectListItem(file.Name, file.Name));
                }
            }

        }

        /*public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            List<String> filePaths = new List<String>();
            foreach (var file in FormFiles)
            {
                if (file.Length > 0)
                {
                    var filePath = Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                using (var reader = new StreamReader(Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName))
                {
                    using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        using (var cdr = new CsvDataReader(csv))
                        {
                            var dt = new DataTable();
                            dt.Load(cdr);
                            DBClass.UploadDatasetCSV(dt, DatasetName, file.FileName);
                            DBClass.KnowledgeDBConnection.Close();
                        }
                    }
                }


            }

            Dataset = new Dataset()
            {
                DatasetName = Regex.Replace(DatasetName, @"\s", string.Empty),
                DatasetType = DatasetType,
                DatasetContents = DatasetContents,
                DatasetCreatedDate = DateTime.Now.ToString(),
                OwnerID = Int32.Parse(HttpContext.Session.GetString("userID"))
            };

            newDatasetID = DBClass.InsertDataset(Dataset);
            DBClass.KnowledgeDBConnection.Close();

            if (CurrentLocation == "Collab")
            {
                DatasetCollab = new DatasetCollab
                {
                    DatasetID = newDatasetID,
                    CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"))
                };
                DBClass.InsertDatasetCollab(DatasetCollab);
                DBClass.KnowledgeDBConnection.Close();
                return RedirectToPage("/Collabs/DatasetList");
            }
            else
            {
                return RedirectToPage("/Main/DatasetLib");
            }
        }*/

        public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<String> filePaths = new List<String>();
            foreach (var file in FormFiles)
            {
                if (file.FileName.EndsWith(".xlsx")) // Check if the file is an Excel file
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet
                            var rowCount = worksheet.Dimension.Rows;
                            var colCount = worksheet.Dimension.Columns;

                            // Read data from Excel and process as needed
                            DataTable dt = new DataTable();
                            for (int row = 1; row <= rowCount; row++)
                            {
                                var newRow = dt.Rows.Add();
                                for (int col = 1; col <= colCount; col++)
                                {
                                    newRow[col - 1] = worksheet.Cells[row, col].Value?.ToString();
                                }
                            }

                            // Upload data to database or perform other operations
                            DBClass.UploadDatasetCSV(dt, DatasetName, file.FileName);
                            DBClass.KnowledgeDBConnection.Close();
                        }
                    }
                }
                else if (file.FileName.EndsWith(".csv")) // Check if the file is a CSV file
                {

                    if (file.Length > 0)
                    {
                        var filePath = Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName;
                        filePaths.Add(filePath);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }

                    // Process CSV files similarly
                    using (var reader = new StreamReader(Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName))
                    {
                        using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            using (var cdr = new CsvDataReader(csv))
                            {
                                var dt = new DataTable();
                                dt.Load(cdr);
                                DBClass.UploadDatasetCSV(dt, DatasetName, file.FileName);
                                DBClass.KnowledgeDBConnection.Close();
                            }
                        }
                    }
                }
            }

            // Create new Dataset
            Dataset = new Dataset()
            {
                DatasetName = Regex.Replace(DatasetName, @"\s", string.Empty),
                DatasetType = DatasetType,
                DatasetContents = DatasetContents,
                DatasetCreatedDate = DateTime.Now.ToString(),
                OwnerID = Int32.Parse(HttpContext.Session.GetString("userID"))
            };

            // Insert Dataset into database
            newDatasetID = DBClass.InsertDataset(Dataset);
            DBClass.KnowledgeDBConnection.Close();

            // Redirect based on current location
            if (CurrentLocation == "Collab")
            {
                // Insert into DatasetCollab table if the current location is Collab
                DatasetCollab = new DatasetCollab
                {
                    DatasetID = newDatasetID,
                    CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"))
                };
                DBClass.InsertDatasetCollab(DatasetCollab);
                DBClass.KnowledgeDBConnection.Close();
                return RedirectToPage("/Collabs/DatasetList");
            }
            else
            {
                // Otherwise, redirect to DatasetLib
                return RedirectToPage("/Main/DatasetLib");
            }
        }

        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                DatasetName = "Capital Gains Data";
                DatasetType = "WORD";
                DatasetContents = "You paid no Capital Gains Tax";
                foreach (var file in FormFiles)
                {
                    FormFiles.Add(file);
                }
            }
            return Page();
        }
        public IActionResult OnPostCancel()
        {
            ModelState.Clear();
            DatasetName = null;
            DatasetType = null;
            DatasetContents = null;
            return Page();

        }

        public void OnPostText()
        {
            List<String> filePaths = new List<String>();
            foreach (var file in FormFiles)
            {
                if (file.Length > 0)
                {
                    var filePath = Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                using (var reader = new StreamReader(Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName))
                {
                    using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        using (var cdr = new CsvDataReader(csv))
                        {
                            var dt = new DataTable();
                            dt.Load(cdr);
                            foreach (DataColumn row in dt.Columns)
                            {
                                Cols += row.ColumnName;
                            }
                        }
                    }
                }
                OnGet();
            }
        }
    }
}
