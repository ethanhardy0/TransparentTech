using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
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
            }
            return Page();
        }

        public IActionResult OnPostAddDB()
        {
            if (ModelState.IsValid)
            {
                if (Password == ConfirmPassword)
                {
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
                        UserType = "Standard User"
                    };
                    int newUserID = DBClass.InsertUserFull(SysUser);

                    DBClass.CreateHashedUser(Username, Password, newUserID);
                    DBClass.KnowledgeDBConnection.Close();

                    //make sure user was successfully created before creating User and redirecting
                }
                else
                {
                    ViewData["SignupMessage"] = "Passwords did not match";
                    return Page();
                }
            }
            return RedirectToPage("/DBLogin", new { logout = "false" });
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
            return Page();
        }
    }
}
