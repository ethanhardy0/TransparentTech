using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;


namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateKnowledgeModel : PageModel
    {
        public KnowledgeItem KnowledgeItem { get; set; }
        public PEST PEST { get; set; }
        public SWOT SWOT { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeTitle { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeSubject { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeCategory { get; set; }
        [BindProperty]
        [Required]
        public String KnowledgeInformation { get; set; }
        public String KnowledgePostDate { get; set; }
        public static String CurrentLocation { get; set; }
        public KnowledgeItemCollab KnowledgeCollab { get; set; }
        [BindProperty]
        public bool IsSwotItem { get; set; } // This property is bound to the checkbox
        [BindProperty]
        public bool IsPESTItem { get; set; }
        [BindProperty]
        public String Strengths { get; set; }
        [BindProperty]
        public String Weakness { get; set; }
        [BindProperty]
        public String Opportunities { get; set; }
        [BindProperty]
        public String Threats { get; set; }
        [BindProperty]
        public String Political { get; set; }
        [BindProperty]
        public String Economic { get; set; }
        [BindProperty]
        public String Social { get; set; }
        [BindProperty]
        public String Technological { get; set; }
        public static int ExistingKIID { get; set; }
        public int newKnowledgeID { get; set; }
        public int KnowledgeTypeID { get; set; }
        public String CreateorUpdate {  get; set; }
        public List<Department> ActiveDepts { get; set; }

        public void OnGet(int ExistingID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
            else
            {
                CreateorUpdate = "Create";
                CurrentLocation = HttpContext.Session.GetString("LibType");
                ExistingKIID = ExistingID;
                if(ExistingKIID != 0)
                {
                    CreateorUpdate = "Update";
                    SqlDataReader KnowledgeItemReader = DBClass.KnowledgeItemReader();
                    while (KnowledgeItemReader.Read())
                    {
                        if (ExistingKIID == Int32.Parse(KnowledgeItemReader["KnowledgeID"].ToString()))
                        {
                            KnowledgeTitle = KnowledgeItemReader["KnowledgeTitle"].ToString();
                            KnowledgeSubject = KnowledgeItemReader["KnowledgeSubject"].ToString();
                            KnowledgeCategory = KnowledgeItemReader["KnowledgeCategory"].ToString();
                            KnowledgeInformation = KnowledgeItemReader["KnowledgeInformation"].ToString();
                            KnowledgeTypeID = Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString());
                            if(Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 2)
                            {
                                IsSwotItem = true;
                                Strengths = KnowledgeItemReader["Strengths"].ToString();
                                Weakness = KnowledgeItemReader["Weaknesses"].ToString();
                                Opportunities = KnowledgeItemReader["Opportunities"].ToString();
                                Threats = KnowledgeItemReader["Threats"].ToString();
                            } else if (Int32.Parse(KnowledgeItemReader["KnowledgeTypeID"].ToString()) == 3)
                            {
                                IsPESTItem = true;
                                Political = KnowledgeItemReader["Political"].ToString();
                                Economic = KnowledgeItemReader["Economic"].ToString();
                                Social = KnowledgeItemReader["Social"].ToString();
                                Technological = KnowledgeItemReader["Technological"].ToString();
                            }
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();
                }
                GetActiveDepts();
            }

        }

        public IActionResult OnPost()
        {
            ModelState.Clear();
            Strengths = null;
            Weakness = null;
            Opportunities = null;
            Threats = null;
            CreateorUpdate = "Create";
            GetActiveDepts();
            return Page();
        }

        public IActionResult OnPostAddDB(int selectedDep)
        {

            /*if (!ModelState.IsValid)
            {
                GetActiveDepts();
                return Page();
            }*/

            if (!IsSwotItem && !IsPESTItem)
            {
                KnowledgeItem = new KnowledgeItem()
                {
                    KnowledgeTitle = KnowledgeTitle,
                    KnowledgeSubject = KnowledgeSubject,
                    KnowledgeCategory = KnowledgeCategory,
                    KnowledgeInformation = KnowledgeInformation,
                    KnowledgePostDate = DateTime.Now.ToString(),
                    OwnerID = Int32.Parse(HttpContext.Session.GetString("userID")),
                    KnowledgeTypeID = 1,
                    KnowledgeID = ExistingKIID
                };
            } else if (IsSwotItem)
            {
                SWOT = new SWOT()
                {
                    KnowledgeTitle = KnowledgeTitle,
                    KnowledgeSubject = KnowledgeSubject,
                    KnowledgeCategory = KnowledgeCategory,
                    KnowledgeInformation = KnowledgeInformation,
                    KnowledgePostDate = DateTime.Now.ToString(),
                    OwnerID = Int32.Parse(HttpContext.Session.GetString("userID")),
                    KnowledgeTypeID = 2,
                    KnowledgeID = ExistingKIID,
                    Strengths = Strengths,
                    Weaknesses = Weakness,
                    Opportunities = Opportunities,
                    Threats = Threats
                };

            } else if (IsPESTItem)
            {
                PEST = new PEST()
                {
                    KnowledgeTitle = KnowledgeTitle,
                    KnowledgeSubject = KnowledgeSubject,
                    KnowledgeCategory = KnowledgeCategory,
                    KnowledgeInformation = KnowledgeInformation,
                    KnowledgePostDate = DateTime.Now.ToString(),
                    OwnerID = Int32.Parse(HttpContext.Session.GetString("userID")),
                    KnowledgeTypeID = 3,
                    KnowledgeID = ExistingKIID,
                    Political = Political,
                    Economic = Economic,
                    Social = Social,
                    Technological = Technological
                };

            }

            if (ExistingKIID == 0)
            {
                if (IsSwotItem)
                {
                    newKnowledgeID = DBClass.InsertSWOT(SWOT);
                }
                else if (IsPESTItem)
                {
                    newKnowledgeID = DBClass.InsertPEST(PEST);
                } else
                {
                    newKnowledgeID = DBClass.InsertKnowledgeItem(KnowledgeItem);
                }

                DBClass.KnowledgeDBConnection.Close();
                DBClass.InsertDepartmentKnowledge(new DepartmentKnowledge
                {
                    KnowledgeID = newKnowledgeID,
                    DepartmentID = selectedDep
                });
                DBClass.KnowledgeDBConnection.Close();

                if (CurrentLocation == "Collab")
                {
                    KnowledgeCollab = new KnowledgeItemCollab
                    {
                        KnowledgeID = newKnowledgeID,
                        CollabID = Int32.Parse(HttpContext.Session.GetString("collabID"))
                    };
                    DBClass.InsertKnowledgeCollab(KnowledgeCollab);
                    DBClass.KnowledgeDBConnection.Close();
                    return RedirectToPage("/Collabs/KnowledgeList");
                }
                else
                {
                    return RedirectToPage("/Main/KnowledgeLib");
                }
            }
            else
            {
                if (IsSwotItem)
                {
                    DBClass.UpdateExistingKI(SWOT);
                } else if (IsPESTItem) 
                {
                    DBClass.UpdateExistingKI(PEST);
                } else
                {
                    DBClass.UpdateExistingKI(KnowledgeItem);
                }

                DBClass.KnowledgeDBConnection.Close();
                if (CurrentLocation == "Collab")
                {
                    return RedirectToPage("/Collabs/KnowledgeList");
                }
                else
                {
                    return RedirectToPage("/Main/KnowledgeLib");
                }
            }
        }
        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();

                if (IsSwotItem)
                {
                    Strengths = "Learning is good";
                    Weakness = "Learning is tough";
                    Opportunities = "Learning is fun sometimes";
                    Threats = "You should probably go to class to learn";
                }
                KnowledgeTitle = "How to learn";
                KnowledgeSubject = "Wisdom";
                KnowledgeCategory = "Book";
                KnowledgeInformation = "300 Pages";
                CreateorUpdate = "Create";
            }
            GetActiveDepts();
            return Page();
        }
        public IActionResult OnPostCancel()
        {
            ModelState.Clear();
            KnowledgeTitle = null;
            KnowledgeSubject = null;
            KnowledgeCategory = null;
            KnowledgeInformation = null;
            Strengths = null;
            Weakness = null;
            Opportunities = null;
            Threats = null;
            CreateorUpdate = "Create";
            GetActiveDepts();
            return Page();
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
