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
using System.IO;
using System.Text;
using System.Reflection.PortableExecutable;

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
        public List<Department> ActiveDepts { get; set; }

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

                GetActiveDepts();
            }

        }
        public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                GetActiveDepts();
                return Page();
            }

            foreach (var file in FormFiles)
            {
                if (file.FileName.EndsWith(".xlsx") || file.FileName.EndsWith(".xls")) // Check if the file is an Excel file
                {
                    using (var stream = file.OpenReadStream())
                    {
                        // Read Excel file and insert data into the database
                        ReadExcel(stream, file.FileName);
                    }
                }
                else if (file.FileName.EndsWith(".csv")) // Check if the file is a CSV file
                {
                    ProcessCsvFile(file);
                }
            }

            CreateAndInsertDataset(selectedDep);

            // Redirect based on current location
            return CurrentLocation == "Collab"
                ? InsertDatasetCollab()
                : RedirectToPage("/Main/DatasetLib");
        }

        // Reads Excel file to display on page then takes the data read and sends it to DB
        public void ReadExcel(Stream fileStream, string fileName)
        {
            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
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
                CreateTableAndInsertExcelData(DatasetName, fileData.Columns, fileData.Rows);
            }
        }


        private void CreateTableAndInsertExcelData(string tableName, List<string> headers, List<List<string>> rows)
        {
            // Create table script
            StringBuilder createTableScript = new StringBuilder($"CREATE TABLE [{tableName}] (");

            // Iterate over each header
            for (int i = 0; i < headers.Count; i++)
            {
                var columnName = headers[i].Replace(" ", "_"); // Replace spaces with underscores
                                                               // Append column name with TEXT data type
                try
                {
                    // Detect if column contains decimal values
                    Double.Parse(rows[0][i]);
                    createTableScript.Append($"[{columnName}] DECIMAL(18,2), ");
                }
                catch
                {
                    // Fall back if string value is detected
                    createTableScript.Append($"[{columnName}] NVARCHAR(100), ");
                }
            }

            // Remove trailing comma and space
            createTableScript.Remove(createTableScript.Length - 2, 2);
            createTableScript.Append(")");

            // Execute create table script
            using (var connection = new SqlConnection(connectionString))
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
                var columnName = header.Replace(" ", "_"); // Replace spaces with underscores
                insertQuery.Append($"[{columnName}], ");
            }

            // Remove trailing comma and space
            insertQuery.Remove(insertQuery.Length - 2, 2);
            insertQuery.Append(") VALUES (");

            // Add parameter placeholders
            foreach (var header in headers)
            {
                insertQuery.Append($"@param{header.Replace(" ", "_")}, ");
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
                        // Check if the value exists in rowData
                        string value = i < rowData.Count ? rowData[i] : null;

                        // Check if the value is numerical
                        if (double.TryParse(value, out double numericValue))
                        {
                            command.Parameters.AddWithValue($"@param{headers[i].Replace(" ", "_")}", numericValue);
                        }
                        else if (value != null && value != DBNull.Value.ToString())
                        {
                            // If not numerical and not DBNull, add as string
                            command.Parameters.AddWithValue($"@param{headers[i].Replace(" ", "_")}", value);
                        }
                        else
                        {
                            // If value is DBNull or null, add DBNull.Value
                            command.Parameters.AddWithValue($"@param{headers[i].Replace(" ", "_")}", DBNull.Value);
                        }
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

        public void CreateAndInsertDataset(int selectedDep)
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
            if (selectedDep != 0)
            {
                DBClass.InsertDepartmentDataset(new DepartmentDataset
                {
                    DepartmentID = selectedDep,
                    DatasetID = newDatasetID
                });
                DBClass.KnowledgeDBConnection.Close();
            }
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
            GetActiveDepts();
            return Page();
        }
        public IActionResult OnPostCancel()
        {
            ModelState.Clear();
            DatasetName = null;
            DatasetType = null;
            DatasetContents = null;
            GetActiveDepts();
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
        public void GetActiveDepts()
        {
            ActiveDepts = new List<Department>();

            if (HttpContext.Session.GetString("typeUser") != "Admin" && HttpContext.Session.GetString("typeUser") != "Super")
            {
                for (int i = 1; i < 6; i++)
                {
                    SqlDataReader depReader = DBClass.DepartmentReader();
                    while (depReader.Read())
                    {
                        if (Int32.Parse(depReader["DepartmentID"].ToString()) == i)
                        {
                            if (HttpContext.Session.GetInt32("dep" + i) == 1)
                            {
                                ActiveDepts.Add(new Department
                                {
                                    DepartmentID = i,
                                    DepartmentName = depReader["DepartmentName"].ToString()
                                });
                            }
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
            }
            else
            {
                SqlDataReader depReader = DBClass.DepartmentReader();
                while (depReader.Read())
                {
                    ActiveDepts.Add(new Department
                    {
                        DepartmentID = Int32.Parse(depReader["DepartmentID"].ToString()),
                        DepartmentName = depReader["DepartmentName"].ToString()
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
        }
    }
}
