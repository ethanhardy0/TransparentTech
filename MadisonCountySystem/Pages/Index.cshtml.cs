using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountySystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public static int UserID { get; set; }
        [BindProperty]
        public String? FirstName { get; set; }
        [BindProperty]
        public String? LastName { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            HttpContext.Session.SetString("LibType", "Main");
            if (HttpContext.Session.GetString("username") == null)
            {
                UserID = 6;
            }
            else
            {
                UserID = Int32.Parse(HttpContext.Session.GetString("userID"));
            }
            using (SqlDataReader loggedUserReader = DBClass.LoggedUserReader(UserID))
            {
                if (loggedUserReader.Read()) // Assuming there's only one user per UserID
                {
                    FirstName = loggedUserReader["FirstName"].ToString();
                    LastName = loggedUserReader["LastName"].ToString();
                }
            }
            DBClass.KnowledgeDBConnection.Close();
        }

        public IActionResult OnPostStart()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Main/ReportLib");
            }
            else
            {
                return RedirectToPage("/Main/Collaborations");
            }
        }
    }
}
