using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class CreateUserModel : PageModel
    {
        public SysUser SysUser { get; set; }
        [BindProperty]
        [Required]
        public String Username { get; set; }

        [BindProperty]
        [Required]
        public String Email { get; set; }

        [BindProperty]
        [Required]
        public String FirstName { get; set; }

        [BindProperty]
        [Required]
        public String LastName { get; set; }

        [BindProperty]
        [Required]
        public String Phone { get; set; }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            Username = null;
            Email = null;
            FirstName = null;
            LastName = null;
            Phone = null;
            return Page();
        }

        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            Username = "User 12345";
            Email = "user123@email.com";
            FirstName = "UserFirst";
            LastName = "UserLast";
            Phone = "123-456-7890";
            return Page();
        }

        public IActionResult OnPostAddDB()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            SysUser = new SysUser
            {
                Username = Username,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone
            };
            DBClass.InsertUser(SysUser);
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Main/UserLib");
        }
    }
}
