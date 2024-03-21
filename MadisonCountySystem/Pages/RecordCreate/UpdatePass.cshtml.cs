using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class UpdatePassModel : PageModel
    {
		public string usertype { get; set; }
		public static int ExistingUserID { get; set; }

		[BindProperty,Required] public String Password { get; set; }
        [BindProperty, Required] public String ConfirmPassword { get; set; }

        public void OnGet(int ExistingID)
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

			ExistingUserID = ExistingID;
			SqlDataReader userReader = DBClass.UserReader();
			while(userReader.Read())
			{
				if (Int32.Parse(userReader["UserID"].ToString()) == ExistingUserID)
				{
					HttpContext.Session.SetString("pwdName", userReader["Username"].ToString());
                    HttpContext.Session.SetString("pwdID", ExistingUserID.ToString());
                }
			}
			DBClass.KnowledgeDBConnection.Close();
		}

		public IActionResult OnPostAddDB()
		{
            // Check if model state is valid
            if (!ModelState.IsValid)
            {
				OnGet(ExistingUserID);
            }

            // Check if passwords match
            if (Password != ConfirmPassword)
            {
                ViewData["SignupMessage"] = "Passwords did not match";
				return Page();
            }
			DBClass.UpdateHashedPassword(Password, Int32.Parse(HttpContext.Session.GetString("pwdID")));
			DBClass.KnowledgeDBConnection.Close();

            HttpContext.Session.Remove("pwdID");
            HttpContext.Session.Remove("pwdName");
			return RedirectToPage("/Main/UserLib");
        }
    }
}
