﻿using MadisonCountySystem.Pages.DataClasses;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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

        private static readonly String? AuxConnString = "Server=Localhost;Database=Auxillary;Trusted_Connection=True";
        // Error Message
        public static String? QueryError { get; set; }
		//Connection Methods:

		// ------------------------------------------- Users ---------------------------------------------------------------------------------------------------------
		public static SqlDataReader UserReader()
		{
			SqlCommand cmdUserRead = new SqlCommand();
			cmdUserRead.Connection = KnowledgeDBConnection;
			cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
			cmdUserRead.CommandText = "GetUserReader";
			cmdUserRead.CommandType = CommandType.StoredProcedure;
			cmdUserRead.Connection.Open(); // Open connection here, close in Model!

			SqlDataReader tempReader = cmdUserRead.ExecuteReader();

			return tempReader;
		}

		public static void InsertUser(SysUser a)
		{
			string sqlQuery = "InsertUser";

			using (SqlCommand cmdUserInsert = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdUserInsert.CommandType = CommandType.StoredProcedure;

				cmdUserInsert.Parameters.AddWithValue("@Username", a.Username);
				cmdUserInsert.Parameters.AddWithValue("@Email", a.Email);
				cmdUserInsert.Parameters.AddWithValue("@FirstName", a.FirstName);
				cmdUserInsert.Parameters.AddWithValue("@LastName", a.LastName);
				cmdUserInsert.Parameters.AddWithValue("@Phone", a.Phone);

				KnowledgeDBConnection.Open();
				cmdUserInsert.ExecuteNonQuery();
			}
		}

		public static int InsertUserFull(SysUser a)
		{
			string sqlQuery = "InsertUserFull";

			using (SqlCommand cmdUserInsert = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdUserInsert.CommandType = CommandType.StoredProcedure;

				cmdUserInsert.Parameters.AddWithValue("@Username", a.Username);
				cmdUserInsert.Parameters.AddWithValue("@Email", a.Email);
				cmdUserInsert.Parameters.AddWithValue("@FirstName", a.FirstName);
				cmdUserInsert.Parameters.AddWithValue("@LastName", a.LastName);
				cmdUserInsert.Parameters.AddWithValue("@Phone", a.Phone);
				cmdUserInsert.Parameters.AddWithValue("@Street", a.Street);
				cmdUserInsert.Parameters.AddWithValue("@City", a.City);
				cmdUserInsert.Parameters.AddWithValue("@State", a.State);
				cmdUserInsert.Parameters.AddWithValue("@Zip", a.Zip);
				cmdUserInsert.Parameters.AddWithValue("@UserType", a.UserType);

				KnowledgeDBConnection.Open();
				int newUserID = (int)cmdUserInsert.ExecuteScalar();
				KnowledgeDBConnection.Close(); // Don't forget to close the connection
				return newUserID;
			}
		}

		public static void RemoveUser(int UserID)
		{
			string sqlQuery = "RemoveUser";

			using (SqlCommand cmdUserRemove = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdUserRemove.CommandType = CommandType.StoredProcedure;
				cmdUserRemove.Parameters.AddWithValue("@UserID", UserID);

				KnowledgeDBConnection.Open();
				cmdUserRemove.ExecuteNonQuery();
			}
		}

		public static void UpdateExistingUser(SysUser k)
		{
			string sqlQuery = "UpdateExistingUser";

			using (SqlCommand cmdUserUpdate = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdUserUpdate.CommandType = CommandType.StoredProcedure;

				cmdUserUpdate.Parameters.AddWithValue("@UserID", k.UserID);
				cmdUserUpdate.Parameters.AddWithValue("@Username", k.Username);
				cmdUserUpdate.Parameters.AddWithValue("@Email", k.Email);
				cmdUserUpdate.Parameters.AddWithValue("@FirstName", k.FirstName);
				cmdUserUpdate.Parameters.AddWithValue("@LastName", k.LastName);
				cmdUserUpdate.Parameters.AddWithValue("@Street", k.Street);
				cmdUserUpdate.Parameters.AddWithValue("@City", k.City);
				cmdUserUpdate.Parameters.AddWithValue("@State", k.State);
				cmdUserUpdate.Parameters.AddWithValue("@Zip", k.Zip);
				cmdUserUpdate.Parameters.AddWithValue("@Phone", k.Phone);
				cmdUserUpdate.Parameters.AddWithValue("@UserType", k.UserType);

				KnowledgeDBConnection.Open();
				cmdUserUpdate.ExecuteNonQuery();
			}
		}

		public static void UpdateHashedUsername(string Username, int UserID)
		{
			string loginQuery = "UpdateHashedUsername";

			using (SqlCommand cmdLogin = new SqlCommand(loginQuery, KnowledgeDBConnection))
			{
				cmdLogin.CommandType = CommandType.StoredProcedure;

				cmdLogin.Parameters.AddWithValue("@Username", Username);
				cmdLogin.Parameters.AddWithValue("@UserID", UserID);

				KnowledgeDBConnection.Open();
				cmdLogin.ExecuteNonQuery();
			}
		}

		public static void UpdateHashedPassword(string Password, int UserID)
		{
			string loginQuery = "UpdateHashedPassword";

			using (SqlCommand cmdLogin = new SqlCommand(loginQuery, KnowledgeDBConnection))
			{
				cmdLogin.CommandType = CommandType.StoredProcedure;

				cmdLogin.Parameters.AddWithValue("@UserID", UserID);
				cmdLogin.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(Password));

				KnowledgeDBConnection.Open();
				cmdLogin.ExecuteNonQuery();
			}
		}

		// ------------------------------------------- KIs -----------------------------------------------------------------------------------------------------------
		public static SqlDataReader KnowledgeItemReader()
		{
			SqlCommand cmdKnowledgeRead = new SqlCommand();
			cmdKnowledgeRead.Connection = KnowledgeDBConnection;
			cmdKnowledgeRead.Connection.ConnectionString = KnowledgeDBConnString;
			cmdKnowledgeRead.CommandText = "KnowledgeItemReader";
			cmdKnowledgeRead.CommandType = CommandType.StoredProcedure;
			cmdKnowledgeRead.Connection.Open(); // Open connection here, close in Model!

			SqlDataReader tempReader = cmdKnowledgeRead.ExecuteReader();

			return tempReader;
		}

		public static int InsertKnowledgeItem(KnowledgeItem a)
		{
			string sqlQuery = "InsertKnowledgeItem";

			using (SqlCommand cmdKnowledgeInsert = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdKnowledgeInsert.CommandType = CommandType.StoredProcedure;

				cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeTitle", a.KnowledgeTitle);
				cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeSubject", a.KnowledgeSubject);
				cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeCategory", a.KnowledgeCategory);
				cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeInformation", a.KnowledgeInformation);
				cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgePostDate", a.KnowledgePostDate);
				cmdKnowledgeInsert.Parameters.AddWithValue("@OwnerID", a.OwnerID);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeTypeID", a.KnowledgeTypeID);

                KnowledgeDBConnection.Open();
				int newKnowledgeID = (int)cmdKnowledgeInsert.ExecuteScalar(); // Executes the command and returns the new KnowledgeID
				KnowledgeDBConnection.Close();
				return newKnowledgeID;
			}
		}

        public static int InsertSWOT(SWOT a)
        {
            string sqlQuery = "InsertKnowledgeItem";

            using (SqlCommand cmdKnowledgeInsert = new SqlCommand(sqlQuery, KnowledgeDBConnection))
            {
                cmdKnowledgeInsert.CommandType = CommandType.StoredProcedure;

                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeTitle", a.KnowledgeTitle);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeSubject", a.KnowledgeSubject);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeCategory", a.KnowledgeCategory);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeInformation", a.KnowledgeInformation);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgePostDate", a.KnowledgePostDate);
                cmdKnowledgeInsert.Parameters.AddWithValue("@OwnerID", a.OwnerID);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeTypeID", a.KnowledgeTypeID);

                KnowledgeDBConnection.Open();
                int newKnowledgeID = (int)cmdKnowledgeInsert.ExecuteScalar(); // Executes the command and returns the new KnowledgeID
                KnowledgeDBConnection.Close();

                sqlQuery = "InsertSWOT";

                using (SqlCommand cmdSWOTInsert =new SqlCommand(sqlQuery,KnowledgeDBConnection))
                {
                    cmdSWOTInsert.CommandType = CommandType.StoredProcedure;
                    cmdSWOTInsert.Parameters.AddWithValue("@Strengths", a.Strengths);
                    cmdSWOTInsert.Parameters.AddWithValue("@Weaknesses", a.Weaknesses);
                    cmdSWOTInsert.Parameters.AddWithValue("@Opportunities", a.Opportunities);
                    cmdSWOTInsert.Parameters.AddWithValue("@Threats", a.Threats);
                    cmdSWOTInsert.Parameters.AddWithValue("@KnowledgeID", newKnowledgeID);
                    KnowledgeDBConnection.Open();
                    cmdSWOTInsert.ExecuteScalar();
                    KnowledgeDBConnection.Close();
                }
                return newKnowledgeID;
            }
        }

        public static int InsertPEST(PEST a)
        {
            string sqlQuery = "InsertKnowledgeItem";

            using (SqlCommand cmdKnowledgeInsert = new SqlCommand(sqlQuery, KnowledgeDBConnection))
            {
                cmdKnowledgeInsert.CommandType = CommandType.StoredProcedure;

                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeTitle", a.KnowledgeTitle);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeSubject", a.KnowledgeSubject);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeCategory", a.KnowledgeCategory);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeInformation", a.KnowledgeInformation);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgePostDate", a.KnowledgePostDate);
                cmdKnowledgeInsert.Parameters.AddWithValue("@OwnerID", a.OwnerID);
                cmdKnowledgeInsert.Parameters.AddWithValue("@KnowledgeTypeID", a.KnowledgeTypeID);

                KnowledgeDBConnection.Open();
                int newKnowledgeID = (int)cmdKnowledgeInsert.ExecuteScalar(); // Executes the command and returns the new KnowledgeID
                KnowledgeDBConnection.Close();

                sqlQuery = "InsertPEST";

                using (SqlCommand cmdPESTInsert = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdPESTInsert.CommandType = CommandType.StoredProcedure;
                    cmdPESTInsert.Parameters.AddWithValue("@Political", a.Political);
                    cmdPESTInsert.Parameters.AddWithValue("@Economic", a.Economic);
                    cmdPESTInsert.Parameters.AddWithValue("@Social", a.Social);
                    cmdPESTInsert.Parameters.AddWithValue("@Technological", a.Technological);
                    cmdPESTInsert.Parameters.AddWithValue("@KnowledgeID", newKnowledgeID);
                    KnowledgeDBConnection.Open();
                    cmdPESTInsert.ExecuteScalar();
                    KnowledgeDBConnection.Close();
                }
                return newKnowledgeID;
            }
        }

        public static void DeleteKnowledgeItem(int KnowledgeID)
		{
			string sqlQuery = "DeleteKnowledgeItem";

			using (SqlCommand cmdDeleteKnowledgeItem = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdDeleteKnowledgeItem.CommandType = CommandType.StoredProcedure;

				cmdDeleteKnowledgeItem.Parameters.AddWithValue("@KnowledgeID", KnowledgeID);

				KnowledgeDBConnection.Open();
				cmdDeleteKnowledgeItem.ExecuteNonQuery();
				KnowledgeDBConnection.Close(); // Don't forget to close the connection
			}
		}

		public static void RemoveKnowledgeItemCollab(int KnowledgeID, int CollabID)
		{
			string sqlQuery = "RemoveKnowledgeItemCollab";

			using (SqlCommand cmdRemoveCollab = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdRemoveCollab.CommandType = CommandType.StoredProcedure;

				cmdRemoveCollab.Parameters.AddWithValue("@KnowledgeID", KnowledgeID);
				cmdRemoveCollab.Parameters.AddWithValue("@CollabID", CollabID);

				KnowledgeDBConnection.Open();
				cmdRemoveCollab.ExecuteNonQuery();
			}
		}

		public static void UpdateExistingKI(KnowledgeItem k)
		{
			string sqlQuery = "UpdateExistingKI";

			using (SqlCommand cmdUpdateKI = new SqlCommand(sqlQuery, KnowledgeDBConnection))
			{
				cmdUpdateKI.CommandType = CommandType.StoredProcedure;

				cmdUpdateKI.Parameters.AddWithValue("@KnowledgeID", k.KnowledgeID);
				cmdUpdateKI.Parameters.AddWithValue("@KnowledgeTitle", k.KnowledgeTitle);
				cmdUpdateKI.Parameters.AddWithValue("@KnowledgeSubject", k.KnowledgeSubject);
				cmdUpdateKI.Parameters.AddWithValue("@KnowledgeCategory", k.KnowledgeCategory);
				cmdUpdateKI.Parameters.AddWithValue("@KnowledgeInformation", k.KnowledgeInformation);

                KnowledgeDBConnection.Open();
				cmdUpdateKI.ExecuteNonQuery();
			}
		}

        public static void UpdateExistingKI(PEST a)
        {
            string sqlQuery = "UpdateExistingKI";

            using (SqlCommand cmdKnowledge = new SqlCommand(sqlQuery, KnowledgeDBConnection))
            {
                cmdKnowledge.CommandType = CommandType.StoredProcedure;

                cmdKnowledge.Parameters.AddWithValue("@KnowledgeID", a.KnowledgeID);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeTitle", a.KnowledgeTitle);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeSubject", a.KnowledgeSubject);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeCategory", a.KnowledgeCategory);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeInformation", a.KnowledgeInformation);

                KnowledgeDBConnection.Open();
                cmdKnowledge.ExecuteScalar();
                KnowledgeDBConnection.Close();

                sqlQuery = "UpdatePEST";

                using (SqlCommand cmdPEST = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdPEST.CommandType = CommandType.StoredProcedure;
                    cmdPEST.Parameters.AddWithValue("@Political", a.Political);
                    cmdPEST.Parameters.AddWithValue("@Economic", a.Economic);
                    cmdPEST.Parameters.AddWithValue("@Social", a.Social);
                    cmdPEST.Parameters.AddWithValue("@Technological", a.Technological);
                    cmdPEST.Parameters.AddWithValue("@KnowledgeID", a.KnowledgeID);
                    KnowledgeDBConnection.Open();
                    cmdPEST.ExecuteScalar();
                }
            }
        }

        public static void UpdateExistingKI(SWOT a)
        {
            string sqlQuery = "UpdateExistingKI";

            using (SqlCommand cmdKnowledge = new SqlCommand(sqlQuery, KnowledgeDBConnection))
            {
                cmdKnowledge.CommandType = CommandType.StoredProcedure;

                cmdKnowledge.Parameters.AddWithValue("@KnowledgeID", a.KnowledgeID);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeTitle", a.KnowledgeTitle);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeSubject", a.KnowledgeSubject);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeCategory", a.KnowledgeCategory);
                cmdKnowledge.Parameters.AddWithValue("@KnowledgeInformation", a.KnowledgeInformation);

                KnowledgeDBConnection.Open();
                cmdKnowledge.ExecuteScalar();
                KnowledgeDBConnection.Close();

                sqlQuery = "UpdateSWOT";

                using (SqlCommand cmdSWOT = new SqlCommand(sqlQuery, KnowledgeDBConnection))
                {
                    cmdSWOT.CommandType = CommandType.StoredProcedure;
                    cmdSWOT.Parameters.AddWithValue("@Strengths", a.Strengths);
                    cmdSWOT.Parameters.AddWithValue("@Weaknesses", a.Weaknesses);
                    cmdSWOT.Parameters.AddWithValue("@Opportunities", a.Opportunities);
                    cmdSWOT.Parameters.AddWithValue("@Threats", a.Threats);
                    cmdSWOT.Parameters.AddWithValue("@KnowledgeID", a.KnowledgeID);
                    KnowledgeDBConnection.Open();
                    cmdSWOT.ExecuteScalar();
                }
            }
        }


        // ------------------------------------------- Datasets ------------------------------------------------------------------------------------------------------
        public static SqlDataReader DatasetReader()
        {
            SqlCommand cmdDatasetRead = new SqlCommand();
            cmdDatasetRead.Connection = KnowledgeDBConnection;
            cmdDatasetRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdDatasetRead.CommandText = "SELECT * FROM Dataset LEFT JOIN SysUser ON Dataset.OwnerID=SysUser.UserID WHERE DatasetStatus != 'Deleted' OR DatasetStatus IS NULL;";
            cmdDatasetRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdDatasetRead.ExecuteReader();

            return tempReader;
        }

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
                cmdDatasetRead.Connection.ConnectionString = KnowledgeDBConnString;
                KnowledgeDBConnection.Open();
                int newDatasetID = (int)cmdDatasetRead.ExecuteScalar(); // Executes the command and returns the new datasetID
                KnowledgeDBConnection.Close(); // Don't forget to close the connection
                return newDatasetID;
            }
        }

        public static void UploadDatasetCSV(DataTable dt, String tableName, String fileName)
        {
            tableName = Regex.Replace(tableName, @"\s", string.Empty);
            //Creates a table based on columns (all fields will be nvarchar)
            string sql = "Create Table " + tableName + "(";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                try
                {
                    try
                    {
                        Int32.Parse(dt.Rows[0][i].ToString());
                        sql += "[" + dt.Columns[i].ColumnName + "] " + "INT" + ",";
                    } catch 
                    {
                        Double.Parse(dt.Rows[0][i].ToString());
                        sql += "[" + dt.Columns[i].ColumnName + "] " + "DECIMAL(18,2)" + ",";
                    }
                }
                catch
                {
                    sql += "[" + dt.Columns[i].ColumnName + "] " + "nvarchar(MAX)" + ",";
                }
            }
            sql = sql.TrimEnd(new char[] { ',' }) + ")";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = KnowledgeDBConnection;
            cmd.Connection.ConnectionString = AuxConnString;
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

        public static void DeleteDataset(int datasetID)
        {
            SqlDataReader read = GeneralReader("SELECT DatasetName FROM Dataset WHERE DatasetID = " + datasetID + ";");

            read.Read();

            String tableName = read[0].ToString();

            KnowledgeDBConnection.Close();

            String query2 = "DROP TABLE " + tableName + ";";
            SqlCommand cmdTableRead = new SqlCommand();
            cmdTableRead.Connection = KnowledgeDBConnection;
            cmdTableRead.Connection.ConnectionString = AuxConnString;
            cmdTableRead.CommandText = query2;
            cmdTableRead.Connection.Open();
            cmdTableRead.ExecuteNonQuery();
            cmdTableRead.Connection.Close();

            cmdTableRead = new SqlCommand();
            cmdTableRead.Connection = KnowledgeDBConnection;
            cmdTableRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdTableRead.CommandText = "UPDATE Dataset SET DatasetStatus = 'Deleted' WHERE DatasetID = " + datasetID;
            cmdTableRead.Connection.Open();
            cmdTableRead.ExecuteNonQuery();
            cmdTableRead.Connection.Close();
        }

        // ------------------------------------------- Plans ---------------------------------------------------------------------------------------------------------
        public static SqlDataReader PlanReader()
        {
            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = KnowledgeDBConnection;
            cmdPlanRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdPlanRead.CommandText = "SELECT * FROM SysPlan WHERE PlanStatus != 'Deleted' OR PlanStatus IS NULL;";
            cmdPlanRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPlanRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader PlanStepReader()
        {
            SqlCommand cmdPlanStepRead = new SqlCommand();
            cmdPlanStepRead.Connection = KnowledgeDBConnection;
            cmdPlanStepRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdPlanStepRead.CommandText = "SELECT * FROM PlanStep WHERE StepStatus != 'Deleted' OR StepStatus IS NULL;";
            cmdPlanStepRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPlanStepRead.ExecuteReader();

            return tempReader;
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

        public static void DeletePlan(int PlanID)
        {
            String sqlQuery = "UPDATE SysPlan SET PlanStatus = 'Deleted' WHERE PlanID = @PlanID";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@PlanID", PlanID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void DeletePlanStep(int PlanStepID)
        {
            String sqlQuery = "UPDATE PlanStep SET StepStatus = 'Deleted' WHERE PlanStepID = @PlanStepID";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@PlanStepID", PlanStepID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void UpdateExistingPlan(SysPlan k)
        {
            String sqlQuery = "UPDATE SysPlan SET PlanName = @PlanName, PlanContents = @PlanContents WHERE PlanID = @PlanID;";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@PlanID", k.PlanID);
            cmdUserRead.Parameters.AddWithValue("@PlanName", k.PlanName); // AddWithValue for KnowledgeTitle
            cmdUserRead.Parameters.AddWithValue("@PlanContents", k.PlanContents); // AddWithValue for KnowledgeSubject

            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void UpdateExistingPlanStep(PlanStep k)
        {
            String sqlQuery = "UPDATE PlanStep SET PlanStepName = @PlanStepName, StepData = @StepData, DueDate = @DueDate WHERE PlanStepID = @PlanStepID;";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@PlanStepID", k.PlanStepID);
            cmdUserRead.Parameters.AddWithValue("@PlanStepName", k.PlanStepName);
            cmdUserRead.Parameters.AddWithValue("@StepData", k.StepData);
            cmdUserRead.Parameters.AddWithValue("@DueDate", k.DueDate);

            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }


        // ------------------------------------------- Collabs -------------------------------------------------------------------------------------------------------
        public static SqlDataReader CollabReader()
        {
            SqlCommand cmdCollabRead = new SqlCommand();
            cmdCollabRead.Connection = KnowledgeDBConnection;
            cmdCollabRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdCollabRead.CommandText = "SELECT * FROM Collaboration WHERE CollabStatus != 'Deleted' OR CollabStatus IS NULL;";
            cmdCollabRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdCollabRead.ExecuteReader();

            return tempReader;
        }

        public static void DeleteCollab(int CollabID)
        {
            String sqlQuery = "UPDATE Collaboration SET CollabStatus = 'Deleted' WHERE CollabID = @CollabID";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@CollabID", CollabID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void UpdateExistingCollab(Collab k)
        {
            String sqlQuery = "UPDATE Collaboration SET CollabName = @CollabName, CollabNotes = @CollabNotes WHERE CollabID = @CollabID;";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@CollabID", k.CollabID);
            cmdUserRead.Parameters.AddWithValue("@CollabName", k.CollabName); // AddWithValue for KnowledgeTitle
            cmdUserRead.Parameters.AddWithValue("@CollabNotes", k.CollabNotes); // AddWithValue for KnowledgeSubject

            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
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
                "LEFT JOIN SWOT ON CollabReport.KnowledgeID = SWOT.KnowledgeID " +
                "LEFT JOIN PEST ON CollabReport.KnowledgeID = PEST.KnowledgeID " +
                "WHERE CollabReportParent = @ReportID;";
            cmdReportRead.Parameters.AddWithValue("@ReportID", reportID);
            cmdReportRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdReportRead.ExecuteReader();

            return tempReader;
        }

// ------------------------------------------- Logins --------------------------------------------------------------------------------------------------------
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
            cmdLoginUserRead.CommandText = "SELECT * FROM SysUser WHERE Username = @username";
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

        // ------------------------------------------- Analysis ------------------------------------------------------------------------------------------------------
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

        // ------------------------------------------ User Photo --------------------------------------------------------------------------------------------------
        public static String UserPhotoReader(int userID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = KnowledgeDBConnection;
            cmd.Connection.ConnectionString = KnowledgeDBConnString;
            cmd.CommandText = "SELECT Directory FROM UserPhoto WHERE UserID = " + userID + ";";
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();

            if (tempReader.HasRows)
            {
                tempReader.Read();
                return tempReader["Directory"].ToString();
            }
            else
            {
                return "~/images/blankuser.jpg";
            }
        }

        public static void InsertUserPhoto(int userID, String imgDir)
        {
            SqlDataReader temp = GeneralReader("SELECT * FROM UserPhoto WHERE UserID = " + userID + ";");
            // Removes current profile picture if it exists
            if (temp.HasRows)
            {
                KnowledgeDBConnection.Close();
                SqlCommand clearPhoto = new SqlCommand();
                clearPhoto.Connection = KnowledgeDBConnection;
                clearPhoto.Connection.ConnectionString = KnowledgeDBConnString;
                clearPhoto.CommandText = "DELETE FROM UserPhoto WHERE UserID = " + userID + ";";
                clearPhoto.Connection.Open();
                clearPhoto.ExecuteNonQuery();
                KnowledgeDBConnection.Close();
            } else
            {
                KnowledgeDBConnection.Close();
            }

            // Inserts the new photo
            String sqlQuery = "INSERT INTO UserPhoto (UserID, Directory) VALUES (@UserID, @Directory);";

            SqlCommand cmdUserPhoto= new SqlCommand();
            cmdUserPhoto.Connection = KnowledgeDBConnection;
            cmdUserPhoto.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserPhoto.CommandText = sqlQuery;
            cmdUserPhoto.Parameters.AddWithValue("@UserID", userID);
            cmdUserPhoto.Parameters.AddWithValue("@Directory", imgDir);
            cmdUserPhoto.Connection.Open();
            cmdUserPhoto.ExecuteNonQuery();
        }

        // ------------------------------------------ General Query --------------------------------------------------------------------------------------------------
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

        public static SqlDataReader AuxGeneralReader(String sqlQuery)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = KnowledgeDBConnection;
            cmd.Connection.ConnectionString = AuxConnString;
            cmd.CommandText = sqlQuery;
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();

            return tempReader;

        }

        // ------------------------------------------ Department --------------------------------------------------------------------------------------------------
        public static SqlDataReader DepartmentReader()
        {
            String SqlQuery = "SELECT * FROM Department;";
            SqlCommand cmdAnalysisRead = new SqlCommand();
            cmdAnalysisRead.Connection = KnowledgeDBConnection;
            cmdAnalysisRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdAnalysisRead.CommandText = SqlQuery;
            cmdAnalysisRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAnalysisRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader UserDepartmentReader()
        {
            String SqlQuery = "SELECT * FROM UserDepartment LEFT JOIN SysUser ON UserDepartment.UserID = SysUser.UserID;";
            SqlCommand cmdAnalysisRead = new SqlCommand();
            cmdAnalysisRead.Connection = KnowledgeDBConnection;
            cmdAnalysisRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdAnalysisRead.CommandText = SqlQuery;
            cmdAnalysisRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAnalysisRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader DepartmentKnowledgeReader()
        {
            String SqlQuery = "SELECT * FROM DepartmentKnowledge LEFT JOIN Department ON DepartmentKnowledge.DepartmentID = Department.DepartmentID;";
            SqlCommand cmdAnalysisRead = new SqlCommand();
            cmdAnalysisRead.Connection = KnowledgeDBConnection;
            cmdAnalysisRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdAnalysisRead.CommandText = SqlQuery;
            cmdAnalysisRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAnalysisRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader DepartmentAnalysisReader()
        {
            String SqlQuery = "SELECT * FROM DepartmentAnalysis LEFT JOIN Department ON DepartmentAnalysis.DepartmentID = Department.DepartmentID;";
            SqlCommand cmdAnalysisRead = new SqlCommand();
            cmdAnalysisRead.Connection = KnowledgeDBConnection;
            cmdAnalysisRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdAnalysisRead.CommandText = SqlQuery;
            cmdAnalysisRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAnalysisRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader DepartmentDatasetReader()
        {
            String SqlQuery = "SELECT * FROM DepartmentDataset LEFT JOIN Department ON DepartmentDataset.DepartmentID = Department.DepartmentID;";
            SqlCommand cmdAnalysisRead = new SqlCommand();
            cmdAnalysisRead.Connection = KnowledgeDBConnection;
            cmdAnalysisRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdAnalysisRead.CommandText = SqlQuery;
            cmdAnalysisRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAnalysisRead.ExecuteReader();

            return tempReader;
        }

        public static void InsertUserDepartment(int DepartmentID, int UserID)
        {
            String sqlQuery = "INSERT INTO UserDepartment (UserID, DepartmentID) VALUES (@UserID, @DepartmentID);";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@UserID", UserID);
            cmdUserRead.Parameters.AddWithValue("@DepartmentID", DepartmentID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void DeleteUserDepartment(int DepartmentID, int UserID)
        {
            String sqlQuery = "DELETE FROM UserDepartment WHERE UserID = @UserID AND DepartmentID = @DepartmentID;";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@UserID", UserID);
            cmdUserRead.Parameters.AddWithValue("@DepartmentID", DepartmentID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void InsertDepartmentKnowledge(DepartmentKnowledge d)
        {
            String sqlQuery = "INSERT INTO DepartmentKnowledge (KnowledgeID, DepartmentID) VALUES (@KnowledgeID, @DepartmentID);";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@KnowledgeID", d.KnowledgeID);
            cmdUserRead.Parameters.AddWithValue("@DepartmentID", d.DepartmentID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }

        public static void InsertDepartmentDataset(DepartmentDataset d)
        {
            String sqlQuery = "INSERT INTO DepartmentDataset (DatasetID, DepartmentID) VALUES (@DatasetID, @DepartmentID);";

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = KnowledgeDBConnection;
            cmdUserRead.Connection.ConnectionString = KnowledgeDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Parameters.AddWithValue("@DatasetID", d.DatasetID);
            cmdUserRead.Parameters.AddWithValue("@DepartmentID", d.DepartmentID);
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
        }
    }
}
