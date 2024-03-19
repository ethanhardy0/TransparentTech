using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateCollabModel : PageModel
    {
        public Collab Collab { get; set; }
        public UserCollab UserCollab { get; set; }
        public List<SysUser> UserList { get; set; }
        [BindProperty]
        [Required] public string CollabName { get; set; }
        [BindProperty]
        public List<int> SelectedUserIds { get; set; } = new List<int>(); // To hold IDs of selected users for invitations
        [BindProperty]
        [Required]
        public string CollabNotes { get; set; }

        public static int ExistingCollabID { get; set; }
        public String CreateorUpdate { get; set; }
        public List<SysUser> ExistingUsers { get; set; }

        public CreateCollabModel()
        {
            UserList = new List<SysUser>();
            ExistingUsers = new List<SysUser>();
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
                SqlDataReader userReader = DBClass.UserReader();
                while (userReader.Read())
                {
                    UserList.Add(new SysUser
                    {
                        Username = userReader["Username"].ToString(),
                        UserID = Int32.Parse(userReader["UserID"].ToString())

                    });
                }
                DBClass.KnowledgeDBConnection.Close();


                CreateorUpdate = "Create";
                ExistingCollabID = ExistingID;
                if (ExistingCollabID != 0)
                {
                    CreateorUpdate = "Update";
                    SqlDataReader CollabReader = DBClass.CollabReader();
                    while (CollabReader.Read())
                    {
                        if (ExistingCollabID == Int32.Parse(CollabReader["CollabID"].ToString()))
                        {
                            CollabName = CollabReader["CollabName"].ToString();
                            CollabNotes = CollabReader["CollabNotes"].ToString();
                        }
                    }
                    DBClass.KnowledgeDBConnection.Close();

                    //SqlDataReader existingUserReader = DBClass.UserCollabReader();
                    //while (existingUserReader.Read())
                    //{
                    //    if (ExistingCollabID == Int32.Parse(existingUserReader["CollabID"].ToString()))
                    //    {
                    //        foreach(var user in UserList)
                    //        {
                    //            if(user.UserID == Int32.Parse(existingUserReader["UserID"].ToString()))
                    //            {
                    //                ExistingUsers.Add(user);
                    //            }
                    //        }
                    //    }
                    //}
                    //DBClass.KnowledgeDBConnection.Close();
                }
            }
        }
        public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                SqlDataReader userReader = DBClass.UserReader();
                while (userReader.Read())
                {
                    UserList.Add(new SysUser
                    {
                        Username = userReader["Username"].ToString(),
                        UserID = Int32.Parse(userReader["UserID"].ToString())
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
                return Page();
            }
            Collab = new Collab()
            {
                CollabName = CollabName,
                CollabNotes = CollabNotes,
                CollabCreatedDate = DateTime.Now.ToString(),
                CollabID = ExistingCollabID
            };
            if (ExistingCollabID == 0)
            {
                int newCollabID = DBClass.InsertCollab(Collab);
                DBClass.KnowledgeDBConnection.Close();

                DBClass.InsertUserCollab(new UserCollab
                {
                    UserRole = "Owner",
                    UserID = Int32.Parse(HttpContext.Session.GetString("userID")),
                    CollabID = newCollabID
                });
                DBClass.KnowledgeDBConnection.Close();
                foreach (int userId in SelectedUserIds)
                {
                    DBClass.InsertUserCollab(new UserCollab
                    {
                        UserRole = "Member", // Or any appropriate role
                        UserID = userId,
                        CollabID = newCollabID
                    });
                }
                DBClass.KnowledgeDBConnection.Close();
            }
            else
            {
                DBClass.UpdateExistingCollab(Collab);
                DBClass.KnowledgeDBConnection.Close();
            }
            return RedirectToPage("/Main/Collaborations");

        }
        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear(); //clears form
            CollabName = "Finance Team";
            CollabNotes = "For only the greatest employees";
            SqlDataReader userReader = DBClass.UserReader();
            while (userReader.Read())
            {
                UserList.Add(new SysUser
                {
                    Username = userReader["Username"].ToString(),
                    UserID = Int32.Parse(userReader["UserID"].ToString())
                });
            }
            DBClass.KnowledgeDBConnection.Close();
            return Page();
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            CollabName = null;
            CollabNotes = null;
            SqlDataReader userReader = DBClass.UserReader();
            while (userReader.Read())
            {
                UserList.Add(new SysUser
                {
                    Username = userReader["Username"].ToString(),
                    UserID = Int32.Parse(userReader["UserID"].ToString())
                });
            }
            DBClass.KnowledgeDBConnection.Close();
            return Page();
        }

    }
}
