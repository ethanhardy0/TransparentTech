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
using ExcelDataReader;
using System.Text;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateDatasetModel : PageModel
    {
        public Dataset Dataset { get; set; }
        public String? Cols { get; set; }
        public List<SelectListItem> CSVFiles { get; set; }
        public List<IFormFile> FormFiles { get; set; }
        public FileInfo[] UploadedCSVs { get; set; }
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

        public Dictionary<string, ExcelTable> HeadersByFile { get; set; }

        private static readonly String? connectionString =
            "Server=Localhost;Database=Auxillary;Trusted_Connection=true";

        public CreateDatasetModel()
        {
            HeadersByFile = new Dictionary<string, ExcelTable>();
        }

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

        public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<String> filePaths = new List<String>();
            foreach (var file in FormFiles)
            {
                if (file.FileName.EndsWith(".xlsx") || file.FileName.EndsWith(".xls")) // Check if the file is an Excel file
                {
                    ReadExcel(file);
                }
                else if (file.FileName.EndsWith(".csv")) // Check if the file is a CSV file
                {
                    ProcessCsvFile(file);
                }
            }

            CreateAndInsertDataset();

            // Redirect based on current location
            if (CurrentLocation == "Collab")
            {
                return InsertDatasetCollab();
            }
            else
            {
                // Otherwise, redirect to DatasetLib
                return RedirectToPage("/Main/DatasetLib");
            }
        }

        // Reads Excel file to display on page then takes the data read and sends it to DB
        private void ReadExcel(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var fileData = new ExcelTable();
                    fileData.Columns = new List<string>();
                    fileData.Rows = new List<List<string>>();

                    // Read headers
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        fileData.Columns.Add(reader.GetValue(i)?.ToString());
                    }

                    // Read rows
                    while (reader.Read())
                    {
                        var row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i)?.ToString());
                        }
                        fileData.Rows.Add(row);
                    }

                    HeadersByFile[file.FileName] = fileData;

                    // Call CreateTableAndInsertData
                    CreateTableAndInsertExcelData(Path.GetFileNameWithoutExtension(file.FileName), fileData.Columns, fileData.Rows);
                }
            }
        }

        private void CreateTableAndInsertExcelData(string tableName, List<string> headers, List<List<string>> rows)
        {
            // Create table script
            StringBuilder createTableScript = new StringBuilder($"CREATE TABLE [{tableName}] (");

            // Iterate over each header
            foreach (var header in headers)
            {
                var columnName = header;
                // Append column name with TEXT data type
                createTableScript.Append($"[{columnName}] TEXT, ");
            }

            // Remove trailing comma and space
            createTableScript.Remove(createTableScript.Length - 2, 2);
            createTableScript.Append(")");

            // Execute create table script
            using (var connection = new SqlConnection(connectionString)) // Replace 'connectionString' with your actual connection string
            {
                connection.Open();
                using (var command = new SqlCommand(createTableScript.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Insert data into the table
            foreach (var row in rows)
            {
                InsertExcelData(tableName, headers, row);
            }
        }

        private void InsertExcelData(string tableName, List<string> headers, List<string> rowData)
        {
            // Construct insert query
            StringBuilder insertQuery = new StringBuilder($"INSERT INTO {tableName} (");
            foreach (var header in headers)
            {
                insertQuery.Append($"{header}, ");
            }

            // Remove trailing comma and space
            insertQuery.Remove(insertQuery.Length - 2, 2);
            insertQuery.Append(") VALUES (");

            // Add parameter placeholders
            foreach (var header in headers)
            {
                insertQuery.Append("@param" + header + ", ");
            }

            // Remove trailing comma and space
            insertQuery.Remove(insertQuery.Length - 2, 2);
            insertQuery.Append(")");

            // Execute insert query
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertQuery.ToString(), connection))
                {
                    // Add parameter values
                    for (int i = 0; i < headers.Count; i++)
                    {
                        command.Parameters.AddWithValue("@param" + headers[i], rowData[i]);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        private void ProcessCsvFile(IFormFile file)
        {
            if (file.Length > 0)
            {
                var filePath = Directory.GetCurrentDirectory() + @"/wwwroot/csvupload/" + file.FileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Process CSV files similarly
                using (var reader = new StreamReader(filePath))
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

        public void CreateAndInsertDataset()
        {
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
        }

        public IActionResult InsertDatasetCollab()
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
