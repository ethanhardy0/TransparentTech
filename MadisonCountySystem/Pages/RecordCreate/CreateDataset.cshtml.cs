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
        public string ErrorMessage = "";
        public String CreateorUpdate { get; set; }
        public static String PriorName { get; set; }
        public static int ExistingDatasetID { get; set; }

        private static readonly String? connectionString =
            "Server=Localhost;Database=Auxillary;Trusted_Connection=true";

        public CreateDatasetModel()
        {
            HeadersByFile = new Dictionary<string, ExcelTable>();
        }

        public void OnGet(int ExistingID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CurrentLocation = HttpContext.Session.GetString("LibType");
                CreateorUpdate = "Create";
                PriorName = null;
                ExistingDatasetID = 0;
                if (ExistingID > 0)
                {
                    CreateorUpdate = "Update";
                    ExistingDatasetID = ExistingID;
                    SqlDataReader newDatasetReader = DBClass.DatasetReader();
                    while (newDatasetReader.Read())
                    {
                        if (Int32.Parse(newDatasetReader["DatasetID"].ToString()) == ExistingID)
                        {
                            DatasetName = newDatasetReader["DatasetName"].ToString();
                            DatasetType = newDatasetReader["DatasetType"].ToString();
                            DatasetContents = newDatasetReader["DatasetContents"].ToString();
                            PriorName = newDatasetReader["DatasetName"].ToString();
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }

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


        public IActionResult OnPostAddDB(int selectedDep)
        {
            if (!ModelState.IsValid)
            {
                GetActiveDepts();
                return Page();
            }

            DatasetName = DatasetName.Replace(" ", "_").Replace("-", "_").Replace(",", "_").Replace("/", "_").Replace("\\", "_");
            if (PriorName != null)
            {
                String sqlQuery = "UPDATE Dataset SET DatasetName = '";
                sqlQuery += DatasetName;
                sqlQuery += "', DatasetContents = '";
                sqlQuery += DatasetContents;
                sqlQuery += "', DatasetType = '";
                sqlQuery += DatasetType;
                sqlQuery += "' WHERE DatasetID = ";
                sqlQuery += ExistingDatasetID;
                DBClass.GeneralReader(sqlQuery);
                DBClass.KnowledgeDBConnection.Close();

                String sqlQueryAux = "EXEC sp_rename '";
                sqlQueryAux += PriorName;
                sqlQueryAux += "', '";
                sqlQueryAux += DatasetName;
                sqlQueryAux += "';";
                DBClass.AuxGeneralReader(sqlQueryAux);
                DBClass.KnowledgeDBConnection.Close();
                if (HttpContext.Session.GetString("LibType") == "Collab")
                {
                    return RedirectToPage("/Collabs/DatasetList");
                }
                else
                {
                    return RedirectToPage("/Main/DatasetLib");
                }

            }
            else
            {
                foreach (var file in FormFiles)
                {
                    try
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
                    catch (Exception ex)
                    {
                        // Store the error message 
                        TempData["ErrorMessage"] = $"Error processing file {file.FileName}: {ex.Message}";
                        GetActiveDepts();
                        return RedirectToPage("/Main/DatasetLib"); // Return the page to display the error message
                    }
                }

                try
                {
                    CreateAndInsertDataset(selectedDep);

                    // Redirect based on current location
                    return CurrentLocation == "Collab"
                        ? InsertDatasetCollab()
                        : RedirectToPage("/Main/DatasetLib");
                }
                catch (Exception ex)
                {
                    // Store the error message
                    TempData["ErrorMessage"] = $"Error creating or inserting dataset: {ex.Message}";
                    GetActiveDepts();
                    return RedirectToPage("/Main/DatasetLib"); // Return the page to display the error message
                }
            }


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
                    var columnName = CleanColumnName(reader.GetValue(i)?.ToString());
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

        // Creates the excel based off the excel spreadsheet
        private void CreateTableAndInsertExcelData(string tableName, List<string> headers, List<List<string>> rows)
        {
            // Create table script
            StringBuilder createTableScript = new StringBuilder($"CREATE TABLE [{tableName}] (");

            // Iterate over each header
            for (int i = 0; i < headers.Count; i++)
            {
                var columnName = headers[i].Trim()
                    .Replace(" ", "_")      // Replace spaces with underscores
                    .Replace("/", "_")      // Replace / with underscores
                    .Replace("%", "pct");   // Replace % with pct
                columnName = Regex.Replace(columnName, @"_+", "_"); //multiple underscores into sinle
                columnName = columnName.TrimEnd('_'); // trims the end of the column

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

        // Inserts the data from excel based off of the columns
        private void InsertExcelData(string tableName, List<string> headers, List<string> rowData)
        {
            // Create the parameterized query string for inserting data
            StringBuilder insertQuery = new StringBuilder($"INSERT INTO [{tableName}] (");

            // Append column names
            foreach (var header in headers)
            {
                insertQuery.Append($"[{CleanColumnName(header)}], ");
            }

            // Remove trailing comma and space
            insertQuery.Remove(insertQuery.Length - 2, 2);
            insertQuery.Append(") VALUES (");

            // Append parameter placeholders
            for (int i = 0; i < headers.Count; i++)
            {
                insertQuery.Append($"@param{i}, ");
            }

            // Remove trailing comma and space
            insertQuery.Remove(insertQuery.Length - 2, 2);
            insertQuery.Append(")");

            // Execute the parameterized query
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertQuery.ToString(), connection))
                {
                    // Add parameters for each column value
                    for (int i = 0; i < headers.Count; i++)
                    {
                        // Assuming all values are strings for simplicity
                        var paramValue = (object)rowData[i] ?? DBNull.Value;
                        command.Parameters.AddWithValue($"@param{i}", paramValue);
                    }

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }


        private string CleanColumnName(string columnName)
        {
            // Remove special characters and trim leading/trailing spaces
            if (!string.IsNullOrEmpty(columnName))
            {
                // Remove non-alphanumeric characters and replace spaces with underscores
                columnName = Regex.Replace(columnName, @"[^\w\s]", "").Replace(" ", "_");
                columnName = columnName.Trim();
            }
            return columnName;
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
                CreateorUpdate = "Create";
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
                OnGet(0);
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
