using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using MadisonCountySystem.Pages.DB;

namespace MadisonCountySystem.Pages
{
    public class DBLoginModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Username { get; set; }
        [BindProperty]
        [Required]
        public string Password { get; set; }

        public IActionResult OnGet(String logout)
        {
            if (logout == "true")
            {
                HttpContext.Session.Clear();
                ViewData["LoginMessage"] = "Successfully Logged Out!";
            }
            else if (logout == "false")
            {
                ViewData["LoginMessage"] = "Success! Account creation was successful!";
            }
            return Page();
        }

        public IActionResult OnPostTryLogin()
        {
            if (ModelState.IsValid)
            {
                if (DBClass.HashedParameterLogin(Username, Password))
                {
                    HttpContext.Session.SetString("username", Username);
                    DBClass.KnowledgeDBConnection.Close();
                    SqlDataReader LoginUser = DBClass.LoginUser(Username);
                    while (LoginUser.Read())
                    {
                        HttpContext.Session.SetString("userID", LoginUser["UserID"].ToString());
                        HttpContext.Session.SetString("typeUser", LoginUser["UserType"].ToString());
                    }
                    DBClass.KnowledgeDBConnection.Close();

                    // Sets all departments to false
                    HttpContext.Session.SetInt32("dep1", 0);
                    HttpContext.Session.SetInt32("dep2", 0);
                    HttpContext.Session.SetInt32("dep3", 0);
                    HttpContext.Session.SetInt32("dep4", 0);
                    HttpContext.Session.SetInt32("dep5", 0);

                    SqlDataReader userDepReader = DBClass.UserDepartmentReader();

                    // Sets department to true if user is in the department
                    while(userDepReader.Read())
                    {
                        

                        if (Int32.Parse(userDepReader["UserID"].ToString()) == Int32.Parse(HttpContext.Session.GetString("userID")))
                        {
                            switch (Int32.Parse(userDepReader["DepartmentID"].ToString()))
                            {
                                case 1:
                                    HttpContext.Session.SetInt32("dep1", 1);
                                    break;
                                case 2:
                                    HttpContext.Session.SetInt32("dep2", 1);
                                    break;
                                case 3:
                                    HttpContext.Session.SetInt32("dep3", 1);
                                    break;
                                case 4:
                                    HttpContext.Session.SetInt32("dep4", 1);
                                    break;
                                case 5:
                                    HttpContext.Session.SetInt32("dep5", 1);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }

                    DBClass.KnowledgeDBConnection.Close();

                    return RedirectToPage("/Main/UserHome");

                }
                else
                {
                    HttpContext.Session.Clear();
                    ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                    DBClass.KnowledgeDBConnection.Close();
                    return Page();
                }
            }
            else { return Page(); }
        }

        public IActionResult OnPostLogoutHandler()
        {
            HttpContext.Session.Clear();
            return RedirectToPage(new { logout = "true" });
        }

        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            Username = "ezelljd";
            Password = "12345";
            return Page();
        }
        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            Username = null;
            Password = null;
            return Page();
        }
        public IActionResult OnPostSignup()
        {
            return RedirectToPage("/RecordCreate/SignupForm");
        }
    }
}
