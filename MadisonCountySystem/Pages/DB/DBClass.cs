using MadisonCountySystem.Pages.DataClasses;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace MadisonCountySystem.Pages.DB
    {
        public class DBClass
        {
            // Use this class to define methods that make connecting to
            // and retrieving data from the DB easier.

            // Connection Object at Data Field Level
            public static SqlConnection KnowledgeDBConnection = new SqlConnection();

            // Connection String - How to find and connect to DB
            private static readonly String? KnowledgeDBConnString = "Server=Localhost;Database=Lab3;Trusted_Connection=True";

            private static readonly String? AUTHConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";
            // Error Message
            public static String? QueryError { get; set; }
            //Connection Methods:

            //Basic User Reader
            public static SqlDataReader UserReader()
            {
                SqlCommand cmdUserRead = new SqlCommand();
                cmdUserRead.Connection = KnowledgeDBConnection;
                cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserRead.CommandText = "SELECT * FROM SysUser";
                cmdUserRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdUserRead.ExecuteReader();

                return tempReader;
            }
            public static SqlDataReader LoggedUserReader(int userID)
            {
                SqlCommand cmdUserRead = new SqlCommand();
                cmdUserRead.Connection = KnowledgeDBConnection;
                cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserRead.CommandText = "SELECT * FROM SysUser WHERE UserID = @UserID";
                cmdUserRead.Parameters.AddWithValue("@UserID", userID);
                cmdUserRead.Connection.Open();

                SqlDataReader tempReader = cmdUserRead.ExecuteReader();
                return tempReader;
            }

            public static SqlDataReader KnowledgeItemReader()
            {
                SqlCommand cmdKnowledgeRead = new SqlCommand();
                cmdKnowledgeRead.Connection = KnowledgeDBConnection;
                cmdKnowledgeRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdKnowledgeRead.CommandText = "SELECT * FROM KnowledgeItem LEFT JOIN SysUser ON KnowledgeItem.OwnerID = SysUser.UserID";
                cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader DatasetReader()
            {
                SqlCommand cmdDatasetRead = new SqlCommand();
                cmdDatasetRead.Connection = KnowledgeDBConnection;
                cmdDatasetRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdDatasetRead.CommandText = "SELECT * FROM Dataset LEFT JOIN SysUser ON Dataset.OwnerID=SysUser.UserID";
                cmdDatasetRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdDatasetRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader PlanReader()
            {
                SqlCommand cmdPlanRead = new SqlCommand();
                cmdPlanRead.Connection = KnowledgeDBConnection;
                cmdPlanRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdPlanRead.CommandText = "SELECT * FROM SysPlan";
                cmdPlanRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdPlanRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader CollabReader()
            {
                SqlCommand cmdCollabRead = new SqlCommand();
                cmdCollabRead.Connection = KnowledgeDBConnection;
                cmdCollabRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdCollabRead.CommandText = "SELECT * FROM Collaboration";
                cmdCollabRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdCollabRead.ExecuteReader();

                return tempReader;
            }


            public static SqlDataReader AnalysisReader()
            {
                String SqlQuery = "SELECT * FROM Analysis LEFT JOIN KnowledgeItem ON Analysis.KnowledgeID = KnowledgeItem.KnowledgeID ";
                SqlQuery += "LEFT JOIN SysUser ON Analysis.OwnerID = SysUser.UserID LEFT JOIN Dataset ON Analysis.DatasetID = Dataset.DatasetID;";
                SqlCommand cmdAnalysisRead = new SqlCommand();
                cmdAnalysisRead.Connection = KnowledgeDBConnection;
                cmdAnalysisRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdAnalysisRead.CommandText = SqlQuery;
                cmdAnalysisRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdAnalysisRead.ExecuteReader();

                return tempReader;
            }

            //SysLogin table call skipped

            public static SqlDataReader ExcelReader(int datasetID)
            {
                SqlCommand cmdExcelRead = new SqlCommand();
                cmdExcelRead.Connection = KnowledgeDBConnection;
                cmdExcelRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdExcelRead.CommandText = "SELECT * FROM Spreadsheet WHERE DatasetID = @DatasetID";
                cmdExcelRead.Parameters.AddWithValue("@DatasetID", datasetID);
                cmdExcelRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdExcelRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader ColumnDataReader(int fileID)
            {
                SqlCommand cmdColRead = new SqlCommand();
                cmdColRead.Connection = KnowledgeDBConnection;
                cmdColRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdColRead.CommandText = "SELECT * FROM ColumnData WHERE FileID = @FileID";
                cmdColRead.Parameters.AddWithValue("@FileID", fileID);
                cmdColRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdColRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader SheetDataReader(int fileID)
            {
                SqlCommand cmdSheetRead = new SqlCommand();
                cmdSheetRead.Connection = KnowledgeDBConnection;
                cmdSheetRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdSheetRead.CommandText = "SELECT * FROM SheetData WHERE FileID = @FileID";
                cmdSheetRead.Parameters.AddWithValue("@FileID", fileID);
                cmdSheetRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdSheetRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader PlanStepReader()
            {
                SqlCommand cmdPlanStepRead = new SqlCommand();
                cmdPlanStepRead.Connection = KnowledgeDBConnection;
                cmdPlanStepRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdPlanStepRead.CommandText = "SELECT * FROM PlanStep";
                cmdPlanStepRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdPlanStepRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader UserCollabReader()
            {
                SqlCommand cmdUserCollabRead = new SqlCommand();
                cmdUserCollabRead.Connection = KnowledgeDBConnection;
                cmdUserCollabRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserCollabRead.CommandText = "SELECT * FROM UserCollab";
                cmdUserCollabRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdUserCollabRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader KnowledgeCollabReader()
            {
                SqlCommand cmdKnowledgeCollabRead = new SqlCommand();
                cmdKnowledgeCollabRead.Connection = KnowledgeDBConnection;
                cmdKnowledgeCollabRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdKnowledgeCollabRead.CommandText = "SELECT * FROM KnowledgeCollab";
                cmdKnowledgeCollabRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdKnowledgeCollabRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader AnalysisCollabReader()
            {
                SqlCommand cmdAnalysisCollabRead = new SqlCommand();
                cmdAnalysisCollabRead.Connection = KnowledgeDBConnection;
                cmdAnalysisCollabRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdAnalysisCollabRead.CommandText = "SELECT * FROM AnalysisCollab";
                cmdAnalysisCollabRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdAnalysisCollabRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader DatasetCollabReader()
            {
                SqlCommand cmdDatasetCollabRead = new SqlCommand();
                cmdDatasetCollabRead.Connection = KnowledgeDBConnection;
                cmdDatasetCollabRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdDatasetCollabRead.CommandText = "SELECT * FROM DatasetCollab";
                cmdDatasetCollabRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdDatasetCollabRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader ChatReader()
            {
                SqlCommand cmdChatRead = new SqlCommand();
                cmdChatRead.Connection = KnowledgeDBConnection;
                cmdChatRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdChatRead.CommandText = "SELECT * FROM CollabChat LEFT JOIN SysUser ON CollabChat.PostedBy = SysUser.UserID;";
                cmdChatRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdChatRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader DatasetQueryReader(String sqlQuery)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = KnowledgeDBConnection;
                cmd.Connection.ConnectionString = KnowledgeDBConnString;
                cmd.CommandText = sqlQuery;
                cmd.Connection.Open();

                try
                {
                    SqlDataReader tempReader = cmd.ExecuteReader();
                    return tempReader;
                }
                catch (Exception ex)
                {
                    QueryError = ex.Message;
                    return null;
                }

            }

            public static SqlDataReader GeneralReader(String sqlQuery)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = KnowledgeDBConnection;
                cmd.Connection.ConnectionString = KnowledgeDBConnString;
                cmd.CommandText = sqlQuery;
                cmd.Connection.Open();
                SqlDataReader tempReader = cmd.ExecuteReader();

                return tempReader;

            }

            public static int InsertAnalysis(Analysis a)
            {
                String sqlQuery = "INSERT INTO Analysis (AnalysisName, AnalysisType, AnalysisCreatedDate, DatasetID, OwnerID, KnowledgeID) " +
                    "VALUES (@AnalysisName, @AnalysisType, @AnalysisCreatedDate, @DatasetID, @OwnerID, @KnowledgeID);" +
                    "SELECT CAST(scope_identity() AS int);"; // This line gets the newly generated AnalysisID";

                using (SqlCommand cmdAnalysisRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdAnalysisRead.Parameters.AddWithValue("@AnalysisName", a.AnalysisName);
                    cmdAnalysisRead.Parameters.AddWithValue("@AnalysisType", a.AnalysisType);
                    cmdAnalysisRead.Parameters.AddWithValue("@AnalysisCreatedDate", a.AnalysisCreatedDate);
                    cmdAnalysisRead.Parameters.AddWithValue("@DatasetID", a.DatasetID);
                    cmdAnalysisRead.Parameters.AddWithValue("@OwnerID", a.OwnerID);
                    cmdAnalysisRead.Parameters.AddWithValue("@KnowledgeID", a.KnowledgeID);

                    KnowledgeDBConnection.Open();
                    int newAnalysisID = (int)cmdAnalysisRead.ExecuteScalar(); // Executes the command and returns the new AnalysisID
                    KnowledgeDBConnection.Close(); // Don't forget to close the connection
                    return newAnalysisID;
                }
            }

            public static void InsertAnalysisCollab(AnalysisCollab a)
            {
                String sqlQuery = "INSERT INTO AnalysisCollab (CollabID, AnalysisID) VALUES (@CollabID, @AnalysisID);";

                using (SqlCommand cmdAnalysisCollabRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdAnalysisCollabRead.Parameters.AddWithValue("@CollabID", a.CollabID);
                    cmdAnalysisCollabRead.Parameters.AddWithValue("@AnalysisID", a.AnalysisID);

                    KnowledgeDBConnection.Open();
                    cmdAnalysisCollabRead.ExecuteNonQuery();
                }
            }

            public static void InsertPlan(SysPlan a)
            {
                String sqlQuery = "INSERT INTO SysPlan (PlanName, PlanContents, PlanCreatedDate, CollabID) VALUES (@PlanName, @PlanContents, @PlanCreatedDate, @CollabID);";

                using (SqlCommand cmdAnalysisRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdAnalysisRead.Parameters.AddWithValue("@PlanName", a.PlanName);
                    cmdAnalysisRead.Parameters.AddWithValue("@PlanContents", a.PlanContents);
                    cmdAnalysisRead.Parameters.AddWithValue("@PlanCreatedDate", a.PlanCreatedDate);
                    cmdAnalysisRead.Parameters.AddWithValue("@CollabID", a.CollabID);

                    KnowledgeDBConnection.Open();
                    cmdAnalysisRead.ExecuteNonQuery();
                }
            }
            public static int InsertDataset(Dataset a)
            {
                String sqlQuery = "INSERT INTO Dataset (DatasetName, DatasetType, DatasetContents, DatasetCreatedDate, OwnerID) " +
                    "VALUES (@DatasetName, @DatasetType, @DatasetContents, @DatasetCreatedDate, @OwnerID);" +
                    "SELECT CAST(scope_identity() AS int);"; // This line gets the newly generated DatasetID"

                using (SqlCommand cmdDatasetRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdDatasetRead.Parameters.AddWithValue("@DatasetName", a.DatasetName);
                    cmdDatasetRead.Parameters.AddWithValue("@DatasetType", a.DatasetType);
                    cmdDatasetRead.Parameters.AddWithValue("@DatasetContents", a.DatasetContents);
                    cmdDatasetRead.Parameters.AddWithValue("@DatasetCreatedDate", a.DatasetCreatedDate);
                    cmdDatasetRead.Parameters.AddWithValue("@OwnerID", a.OwnerID);

                    KnowledgeDBConnection.Open();
                    int newDatasetID = (int)cmdDatasetRead.ExecuteScalar(); // Executes the command and returns the new datasetID
                    KnowledgeDBConnection.Close(); // Don't forget to close the connection
                    return newDatasetID;
                }
            }
            public static int InsertKnowledgeItem(KnowledgeItem a)
            {
                String sqlQuery = "INSERT INTO KnowledgeItem (KnowledgeTitle, KnowledgeSubject, KnowledgeCategory, KnowledgeInformation, KnowledgePostDate, OwnerID, Strengths, Weaknesses, Opportunities, Threats) VALUES (@KnowledgeTitle," +
                    " @KnowledgeSubject, @KnowledgeCategory, @KnowledgeInformation, @KnowledgePostDate, @OwnerID, @Strengths, @Weaknesses, @Opportunities, @Threats);" +
                    "SELECT CAST(scope_identity() AS int);"; // This line gets the newly generated DatasetID"

                using (SqlCommand cmdKnowledgeRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdKnowledgeRead.Parameters.AddWithValue("@KnowledgeTitle", a.KnowledgeTitle);
                    cmdKnowledgeRead.Parameters.AddWithValue("@KnowledgeSubject", a.KnowledgeSubject);
                    cmdKnowledgeRead.Parameters.AddWithValue("@KnowledgeCategory", a.KnowledgeCategory);
                    cmdKnowledgeRead.Parameters.AddWithValue("@KnowledgeInformation", a.KnowledgeInformation);
                    cmdKnowledgeRead.Parameters.AddWithValue("@KnowledgePostDate", a.KnowledgePostDate);
                    cmdKnowledgeRead.Parameters.AddWithValue("@OwnerID", a.OwnerID);
                    cmdKnowledgeRead.Parameters.AddWithValue("@Strengths", a.Strengths);
                    cmdKnowledgeRead.Parameters.AddWithValue("@Weaknesses", a.Weaknesses);
                    cmdKnowledgeRead.Parameters.AddWithValue("@Opportunities", a.Opportunities);
                    cmdKnowledgeRead.Parameters.AddWithValue("@Threats", a.Threats);

                    KnowledgeDBConnection.Open();
                    int newKnowledgeID = (int)cmdKnowledgeRead.ExecuteScalar(); // Executes the command and returns the new AnalysisID
                    KnowledgeDBConnection.Close();
                    return newKnowledgeID;
                }
            }
            public static void InsertUser(SysUser a)
            {
                String sqlQuery = "INSERT INTO SysUser (Username, Email, FirstName, LastName, Phone) VALUES (@Username," +
                    " @Email, @FirstName, @LastName, @Phone);";

                using (SqlCommand cmdUserRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdUserRead.Parameters.AddWithValue("@Username", a.Username);
                    cmdUserRead.Parameters.AddWithValue("@Email", a.Email);
                    cmdUserRead.Parameters.AddWithValue("@FirstName", a.FirstName);
                    cmdUserRead.Parameters.AddWithValue("@LastName", a.LastName);
                    cmdUserRead.Parameters.AddWithValue("@Phone", a.Phone);

                    KnowledgeDBConnection.Open();
                    cmdUserRead.ExecuteNonQuery();
                }
            }

            public static int InsertCollab(Collab a)
            {
                String sqlQuery = "INSERT INTO Collaboration (CollabName, CollabNotes, CollabCreatedDate) " +
                    "VALUES (@CollabName, @CollabNotes, @CollabCreatedDate);" +
                    "SELECT CAST(scope_identity() AS int);";

                using (SqlCommand cmdCollabRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdCollabRead.Parameters.AddWithValue("@CollabName", a.CollabName);
                    cmdCollabRead.Parameters.AddWithValue("@CollabNotes", a.CollabNotes);
                    cmdCollabRead.Parameters.AddWithValue("@CollabCreatedDate", a.CollabCreatedDate);
                    KnowledgeDBConnection.Open();
                    int newCollabID = (int)cmdCollabRead.ExecuteScalar();
                    KnowledgeDBConnection.Close();
                    return newCollabID;
                }
            }

            public static void InsertUserCollab(UserCollab a)
            {
                String sqlQuery = "INSERT INTO UserCollab (UserRole, UserID, CollabID)" +
                    "VALUES (@UserRole, @UserID, @CollabID);";

                using (SqlCommand cmdInsertUserCollab = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdInsertUserCollab.Parameters.AddWithValue("@UserRole", a.UserRole);
                    cmdInsertUserCollab.Parameters.AddWithValue("@UserID", a.UserID);
                    cmdInsertUserCollab.Parameters.AddWithValue("@CollabID", a.CollabID);
                    KnowledgeDBConnection.Close();
                    KnowledgeDBConnection.Open();
                    cmdInsertUserCollab.ExecuteNonQuery();
                }
            }

            public static void InsertDatasetCollab(DatasetCollab a)
            {
                String sqlQuery = "INSERT INTO DatasetCollab (CollabID, DatasetID) VALUES (@CollabID, @DatasetID);";

                using (SqlCommand cmdDatasetCollabRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdDatasetCollabRead.Parameters.AddWithValue("@CollabID", a.CollabID);
                    cmdDatasetCollabRead.Parameters.AddWithValue("@DatasetID", a.DatasetID);

                    KnowledgeDBConnection.Open();
                    cmdDatasetCollabRead.ExecuteNonQuery();
                }
            }

            public static void InsertKnowledgeCollab(KnowledgeItemCollab a)
            {
                String sqlQuery = "INSERT INTO KnowledgeCollab (CollabID, KnowledgeID) VALUES (@CollabID, @KnowledgeID);";

                using (SqlCommand cmdKnowledgeCollabRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdKnowledgeCollabRead.Parameters.AddWithValue("@CollabID", a.CollabID);
                    cmdKnowledgeCollabRead.Parameters.AddWithValue("@KnowledgeID", a.KnowledgeID);

                    KnowledgeDBConnection.Open();
                    cmdKnowledgeCollabRead.ExecuteNonQuery();
                }
            }

            public static void InsertChat(CollabChat a)
            {
                String sqlQuery = "INSERT INTO CollabChat (ChatContents, PostedDate, PostedBy, CollabID) VALUES (@ChatContents, @PostedDate, @PostedBy, @CollabID);";

                using (SqlCommand cmdChatRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdChatRead.Parameters.AddWithValue("@ChatContents", a.ChatContents);
                    cmdChatRead.Parameters.AddWithValue("@PostedDate", a.PostedDate);
                    cmdChatRead.Parameters.AddWithValue("@PostedBy", a.UserID);
                    cmdChatRead.Parameters.AddWithValue("@CollabID", a.CollabID);

                    KnowledgeDBConnection.Open();
                    cmdChatRead.ExecuteNonQuery();
                }
            }

            public static void InsertPlanStep(PlanStep a)
            {
                String sqlQuery = "INSERT INTO PlanStep (PlanStepName, StepData, StepCreatedDate, DueDate, OwnerID, PlanID) VALUES (@PlanStepName, @StepData, @StepCreatedDate, @DueDate, @OwnerID, @PlanID);";

                using (SqlCommand cmdChatRead = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdChatRead.Parameters.AddWithValue("@PlanStepName", a.PlanStepName);
                    cmdChatRead.Parameters.AddWithValue("@StepData", a.StepData);
                    cmdChatRead.Parameters.AddWithValue("@StepCreatedDate", DateTime.Now.ToString());
                    cmdChatRead.Parameters.AddWithValue("@DueDate", a.DueDate);
                    cmdChatRead.Parameters.AddWithValue("@OwnerID", a.OwnerID);
                    cmdChatRead.Parameters.AddWithValue("@PlanID", a.PlanID);

                    KnowledgeDBConnection.Open();
                    cmdChatRead.ExecuteNonQuery();
                }
            }

            public static int SecureLogin(string Username, string Password)
            {
                string loginQuery = "SELECT COUNT(*) FROM Credentials where Username = @Username and Password = @Password";
                SqlCommand cmdLogin = new SqlCommand();
                cmdLogin.Connection = KnowledgeDBConnection;
                cmdLogin.Connection.ConnectionString = KnowledgeDBConnString;
                cmdLogin.CommandText = loginQuery;
                cmdLogin.Parameters.AddWithValue("@Username", Username);
                cmdLogin.Parameters.AddWithValue("@Password", Password);
                cmdLogin.Connection.Open();
                // ExecuteScalar() returns back data type Object
                // Use a typecast to convert this to an int.
                // Method returns first column of first row.
                int rowCount = (int)cmdLogin.ExecuteScalar();
                return rowCount;
            }

            public static SqlDataReader LoginUser(String Username)
            {
                SqlCommand cmdLoginUserRead = new SqlCommand();
                cmdLoginUserRead.Connection = KnowledgeDBConnection;
                cmdLoginUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdLoginUserRead.CommandText = "SELECT UserID FROM SysUser WHERE Username = @username";
                cmdLoginUserRead.Parameters.AddWithValue("@Username", Username);
                cmdLoginUserRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdLoginUserRead.ExecuteReader();

                return tempReader;
            }

            public static bool HashedParameterLogin(string Username, string Password)
            {
                SqlCommand cmdLogin = new SqlCommand();
                cmdLogin.Connection = KnowledgeDBConnection;
                cmdLogin.Connection.ConnectionString = AUTHConnString;
                cmdLogin.CommandType = System.Data.CommandType.StoredProcedure;
                cmdLogin.Parameters.AddWithValue("@SysUsername", Username);
                cmdLogin.CommandText = "sp_Lab3Login";
                cmdLogin.Connection.Open();
                // ExecuteScalar() returns back data type Object
                // Use a typecast to convert this to an int.
                // Method returns first column of first row.
                SqlDataReader hashReader = cmdLogin.ExecuteReader();
                if (hashReader.Read())
                {
                    string correctHash = hashReader["SysPassword"].ToString();
                    if (PasswordHash.ValidatePassword(Password, correctHash))
                    {
                        return true;
                    }
                }
                return false;
            }
            public static void CreateHashedUser(string Username, string Password, int UserID)
            {
                string loginQuery = "INSERT INTO HashedCredentials (SysUsername,SysPassword, UserID) values (@SysUsername, @SysPassword, @UserID)";
                SqlCommand cmdLogin = new SqlCommand();
                cmdLogin.Connection = KnowledgeDBConnection;
                cmdLogin.Connection.ConnectionString = AUTHConnString;
                cmdLogin.CommandText = loginQuery;
                cmdLogin.Parameters.AddWithValue("@SysUsername", Username);
                cmdLogin.Parameters.AddWithValue("@UserID", UserID);
                cmdLogin.Parameters.AddWithValue("@SysPassword",
                PasswordHash.HashPassword(Password));
                cmdLogin.Connection.Open();
                // ExecuteScalar() returns back data type Object
                // Use a typecast to convert this to an int.
                // Method returns first column of first row.
                cmdLogin.ExecuteNonQuery();
            }

            public static int InsertUserFull(SysUser a)
            {
                String sqlQuery = "INSERT INTO SysUser (Username, Email, FirstName, LastName, Phone, Street, City, State, Zip, UserType) VALUES (@Username," +
                    " @Email, @FirstName, @LastName, @Phone, @Street, @City, @State, @Zip, @UserType);" +
                    "SELECT CAST(scope_identity() AS int);";

                SqlCommand cmdUserRead = new SqlCommand();
                cmdUserRead.Connection = KnowledgeDBConnection;
                cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserRead.CommandText = sqlQuery;
                cmdUserRead.Parameters.AddWithValue("@Username", a.Username);
                cmdUserRead.Parameters.AddWithValue("@Email", a.Email);
                cmdUserRead.Parameters.AddWithValue("@FirstName", a.FirstName);
                cmdUserRead.Parameters.AddWithValue("@LastName", a.LastName);
                cmdUserRead.Parameters.AddWithValue("@Phone", a.Phone);
                cmdUserRead.Parameters.AddWithValue("@Street", a.Street);
                cmdUserRead.Parameters.AddWithValue("@City", a.City);
                cmdUserRead.Parameters.AddWithValue("@State", a.State);
                cmdUserRead.Parameters.AddWithValue("@Zip", a.Zip);
                cmdUserRead.Parameters.AddWithValue("@UserType", a.UserType);

                cmdUserRead.Connection.Open();
                int newUserID = (int)cmdUserRead.ExecuteScalar();
                KnowledgeDBConnection.Close(); // Don't forget to close the connection
                return newUserID;
            }

            public static void UploadDatasetCSV(DataTable dt, String tableName, String fileName)
            {
                tableName = Regex.Replace(tableName, @"\s", string.Empty);
                //Creates a table based on columns (all fields will be nvarchar)
                string sql = "Create Table " + tableName + "(";
                foreach (DataColumn column in dt.Columns)
                {
                    sql += "[" + column.ColumnName + "] " + "nvarchar(MAX)" + ",";
                }
                sql = sql.TrimEnd(new char[] { ',' }) + ")";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = KnowledgeDBConnection;
                cmd.Connection.ConnectionString = KnowledgeDBConnString;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                // Bulk Inserts the CSV file into the created table (Row terminator caused issues, try catch allows system to try two different Bulk inserts 
                // with different row terminators)
                try
                {
                    sql = "BULK INSERT " + tableName + " FROM '" + Directory.GetCurrentDirectory() + @"\wwwroot\csvupload\" + fileName
                        + "' WITH (FIRSTROW = 2, FIELDTERMINATOR = ',', ROWTERMINATOR = '\\n', FORMAT = 'CSV', TABLOCK)";
                    cmd.CommandText = sql;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    cmd.Connection.Close();
                    sql = "BULK INSERT " + tableName + " FROM '" + Directory.GetCurrentDirectory() + @"\wwwroot\csvupload\" + fileName
                        + "' WITH (FIRSTROW = 2, FIELDTERMINATOR = ',', ROWTERMINATOR = '0x0a', FORMAT = 'CSV', TABLOCK)";
                    cmd.CommandText = sql;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public static int InsertKeyReport(CollabReport c)
            {
                String sqlQuery = "INSERT INTO CollabReport (KeyID, KeyType, ReportCreatedDate, CollabID) VALUES (@KeyID," +
                    " @KeyType, @ReportCreatedDate, @CollabID);" +
                    "SELECT CAST(scope_identity() AS int);";

                SqlCommand cmdUserRead = new SqlCommand();
                cmdUserRead.Connection = KnowledgeDBConnection;
                cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserRead.CommandText = sqlQuery;
                cmdUserRead.Parameters.AddWithValue("@KeyID", c.KeyID);
                cmdUserRead.Parameters.AddWithValue("@KeyType", c.KeyType);
                cmdUserRead.Parameters.AddWithValue("@ReportCreatedDate", c.ReportCreatedDate);
                cmdUserRead.Parameters.AddWithValue("@CollabID", c.CollabID);

                cmdUserRead.Connection.Open();
                int newReportID = (int)cmdUserRead.ExecuteScalar();
                KnowledgeDBConnection.Close(); // Don't forget to close the connection
                return newReportID;
            }

            public static void InsertReportKI(CollabReport c)
            {
                String sqlQuery = "INSERT INTO CollabReport (KnowledgeID, CollabReportParent, ItemType) VALUES (@KnowledgeID, @ReportID, @ItemType);";

                SqlCommand cmdUserRead = new SqlCommand();
                cmdUserRead.Connection = KnowledgeDBConnection;
                cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserRead.CommandText = sqlQuery;
                cmdUserRead.Parameters.AddWithValue("@KnowledgeID", c.KnowledgeID);
                cmdUserRead.Parameters.AddWithValue("@ReportID", c.CollabReportParent);
                cmdUserRead.Parameters.AddWithValue("@ItemType", "Knowledge");
                cmdUserRead.Connection.Open();
                cmdUserRead.ExecuteNonQuery();
            }

            public static void InsertReportUser(CollabReport c)
            {
                String sqlQuery = "INSERT INTO CollabReport (UserID, CollabReportParent, ItemType) VALUES (@UserID, @ReportID, @ItemType);";

                SqlCommand cmdUserRead = new SqlCommand();
                cmdUserRead.Connection = KnowledgeDBConnection;
                cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdUserRead.CommandText = sqlQuery;
                cmdUserRead.Parameters.AddWithValue("@UserID", c.UserID);
                cmdUserRead.Parameters.AddWithValue("@ReportID", c.CollabReportParent);
                cmdUserRead.Parameters.AddWithValue("@ItemType", "User");
                cmdUserRead.Connection.Open();
                cmdUserRead.ExecuteNonQuery();
            }

            public static SqlDataReader KeyReportReader()
            {
                SqlCommand cmdReportRead = new SqlCommand();
                cmdReportRead.Connection = KnowledgeDBConnection;
                cmdReportRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdReportRead.CommandText = "SELECT * FROM CollabReport WHERE CollabReportParent IS NULL";
                cmdReportRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdReportRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader KeyReportReader(int collabID)
            {
                SqlCommand cmdReportRead = new SqlCommand();
                cmdReportRead.Connection = KnowledgeDBConnection;
                cmdReportRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdReportRead.CommandText = "SELECT * FROM CollabReport WHERE CollabReportParent IS NULL AND CollabID = @CollabID";
                cmdReportRead.Parameters.AddWithValue("@CollabID", collabID);
                cmdReportRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdReportRead.ExecuteReader();

                return tempReader;
            }

            public static SqlDataReader ReportItemReader(int reportID)
            {
                SqlCommand cmdReportRead = new SqlCommand();
                cmdReportRead.Connection = KnowledgeDBConnection;
                cmdReportRead.Connection.ConnectionString = KnowledgeDBConnString;
                cmdReportRead.CommandText = "SELECT * FROM CollabReport " +
                    "LEFT JOIN KnowledgeItem ON CollabReport.KnowledgeID = KnowledgeItem.KnowledgeID " +
                    "LEFT JOIN SysUser A ON CollabReport.UserID = A.UserID " +
                    "LEFT JOIN SysUser B ON KnowledgeItem.OwnerID = B.UserID " +
                    "WHERE CollabReportParent = @ReportID";
                cmdReportRead.Parameters.AddWithValue("@ReportID", reportID);
                cmdReportRead.Connection.Open(); // Open connection here, close in Model!

                SqlDataReader tempReader = cmdReportRead.ExecuteReader();

                return tempReader;
            }
        }
    }
