using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class SignupFormModel : PageModel
    {
        [BindProperty]
        [Required]
        public String Username { get; set; }
        [BindProperty]
        [Required]
        public String Password { get; set; }
        [BindProperty]
        [Required]
        public String ConfirmPassword { get; set; }

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
        [BindProperty]
        [Required]
        public String Street { get; set; }
        [BindProperty]
        [Required]
        public String City { get; set; }
        [BindProperty]
        [Required]
        public String State { get; set; }
        [BindProperty]
        [Required]
        public String Zip { get; set; }
        [BindProperty]
        [Required]
        public String UserType { get; set; }
        public SysUser SysUser { get; set; }

        public string usertype { get; set; }

        public void OnGet()
        {
            usertype = HttpContext.Session.GetString("typeUser");

            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }

            if (usertype != "Admin")
            {
                HttpContext.Response.Redirect("/Index");
            }
        }
        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                Username = "testuser";
                Email = "testuser1@email.com";
                FirstName = "Bartholemew";
                LastName = "Jackson";
                Phone = "123-456-1234";
                Street = "765 Main Street";
                City = "Spokane";
                State = "Washington";
                Zip = "77321";
                UserType = "Admin";
            }
            return Page();
        }

        public IActionResult OnPostAddDB()
        {
            // Check if model state is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if passwords match
            if (Password != ConfirmPassword)
            {
                ViewData["SignupMessage"] = "Passwords did not match";
                return Page();
            }

            // All validations passed, proceed with user creation
            SysUser = new SysUser
            {
                Username = Username,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Street = Street,
                City = City,
                State = State,
                Zip = Zip,
                UserType = UserType
            };
            int newUserID = DBClass.InsertUserFull(SysUser);

            DBClass.CreateHashedUser(Username, Password, newUserID);
            DBClass.KnowledgeDBConnection.Close();

            return RedirectToPage("/Main/UserLib");
        }

        public IActionResult OnPostClear()
        {

            ModelState.Clear();
            Username = null;
            Password = null;
            Email = null;
            FirstName = null;
            LastName = null;
            Phone = null;
            Street = null;
            City = null;
            State = null;
            Zip = null;
            UserType = null;
            return Page();
        }
    }
}
