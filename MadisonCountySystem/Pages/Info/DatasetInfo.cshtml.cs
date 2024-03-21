using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages.Info
{
    public class DatasetInfoModel : PageModel
    {
        public static int DatasetID { get; set; }
        public Dataset Dataset { get; set; }
        public List<String> ColNames { get; set; }
        public List<List<String>> Rows { get; set; }
        public List<String> RowValues { get; set; }
        [BindProperty]
        public String? sqlQuery { get; set; }

        public DatasetInfoModel()
        {
            Dataset = new Dataset();
        }

        public void OnGet(int dataID)
        {
            HttpContext.Session.SetInt32("CurrentDataset", dataID);

            DatasetID = (int)HttpContext.Session.GetInt32("CurrentDataset");

            SqlDataReader DatasetReader = DBClass.DatasetReader();
            while (DatasetReader.Read())
            {
                if (DatasetID == Int32.Parse(DatasetReader["DatasetID"].ToString()))
                {
                    Dataset.DatasetName = DatasetReader["DatasetName"].ToString();
                    Dataset.DatasetType = DatasetReader["DatasetType"].ToString();
                    Dataset.DatasetContents = DatasetReader["DatasetContents"].ToString();
                    Dataset.DatasetCreatedDate = DatasetReader["DatasetCreatedDate"].ToString();
                    Dataset.OwnerName = DatasetReader["Username"].ToString();
                    Dataset.OwnerFirst = DatasetReader["FirstName"].ToString();
                    Dataset.OwnerLast = DatasetReader["LastName"].ToString();

                }
            }
            DBClass.KnowledgeDBConnection.Close();

            if (ColNames == null)
            {

                ColNames = new List<String>();
                SqlDataReader reader = DBClass.AuxGeneralReader("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('" + Dataset.DatasetName + "');");

                while (reader.Read())
                {
                    ColNames.Add(reader[0].ToString());
                }

                DBClass.KnowledgeDBConnection.Close();

                if (Rows == null)
                {
                    reader = DBClass.AuxGeneralReader("Select TOP(100) * from " + Dataset.DatasetName + ";");
                    Rows = new List<List<String>>();

                    while (reader.Read())
                    {
                        RowValues = new List<String>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            RowValues.Add(reader[i].ToString());
                        }
                        Rows.Add(RowValues);
                    }

                    DBClass.KnowledgeDBConnection.Close();

                }
            }
        }

        public IActionResult OnPostRunQuery()
        {

            DBClass.KnowledgeDBConnection.Close();

            SqlDataReader reader = DBClass.DatasetQueryReader(sqlQuery);

            if (reader == null)
            {
                ViewData["QueryError"] = "Error: " + DBClass.QueryError;
                DBClass.KnowledgeDBConnection.Close();
                OnGet(DatasetID);
                return Page();
            }

            ColNames = new List<String>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                ColNames.Add(reader.GetName(i));
            }

            Rows = new List<List<String>>();

            while (reader.Read())
            {
                RowValues = new List<String>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    RowValues.Add(reader[i].ToString());
                }
                Rows.Add(RowValues);
            }
            ViewData["QueryError"] = "";
            DBClass.KnowledgeDBConnection.Close();
            OnGet(DatasetID);
            return Page();
        }

    }
}
