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
    public class UpdateUserModel : PageModel
    {
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
		public static int ExistingUserID { get; set; }


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
			if (ExistingID != 0)
			{
                ExistingUserID = ExistingID;
				SqlDataReader UserReader = DBClass.UserReader();
				while (UserReader.Read())
				{
					if (ExistingID == Int32.Parse(UserReader["UserID"].ToString()))
					{
						Username = UserReader["Username"].ToString();
						Email = UserReader["Email"].ToString();
                        FirstName = UserReader["FirstName"].ToString();
						LastName = UserReader["LastName"].ToString();
						Phone = UserReader["Phone"].ToString();
						Street = UserReader["Street"].ToString();
						City = UserReader["City"].ToString();
						State = UserReader["State"].ToString();
						Zip = UserReader["Zip"].ToString();
						UserType = UserReader["UserType"].ToString();
					}
				}
				DBClass.KnowledgeDBConnection.Close();
			}
            else
            {
				HttpContext.Response.Redirect("/Main/UserLib");
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
                UserType = UserType,
                UserID = ExistingUserID
            };
            DBClass.UpdateExistingUser(SysUser);
            DBClass.KnowledgeDBConnection.Close();
            DBClass.UpdateHashedUsername(Username, ExistingUserID);
			DBClass.KnowledgeDBConnection.Close();

			return RedirectToPage("/Main/UserLib");
        }

        public IActionResult OnPostClear()
        {

            ModelState.Clear();
            Username = null;
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
