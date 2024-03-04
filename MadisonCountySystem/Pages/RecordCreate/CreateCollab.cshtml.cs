using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

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

        public CreateCollabModel()
        {
            UserList = new List<SysUser>();
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
                CollabCreatedDate = DateTime.Now.ToString()
            };
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
