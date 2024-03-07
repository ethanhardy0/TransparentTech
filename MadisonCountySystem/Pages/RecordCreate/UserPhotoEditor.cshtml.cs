using MadisonCountySystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MadisonCountySystem.Pages.RecordCreate
{
    public class UserPhotoEditorModel : PageModel
    {
        [BindProperty, Required]
        public IFormFile Image {  get; set; }
        public String? imgDir {  get; set; } 
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                HttpContext.Response.Redirect("/DBLogin");
            }
        }

        public void OnPostPreview()
        {
            if (Image.Length > 0)
            {
                // Full path to file in temp location
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", Image.FileName);

                // Save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }

                // Set imgDir to the virtual path
                imgDir = "/images/" + Image.FileName; // Assuming wwwroot is the web root
            }

            HttpContext.Session.SetString("imgDir", imgDir);
            // Other logic as needed
            OnGet();
        }

        public IActionResult OnPostUpload()
        {
            DBClass.InsertUserPhoto(Int32.Parse(HttpContext.Session.GetString("userID")), HttpContext.Session.GetString("imgDir"));
            DBClass.KnowledgeDBConnection.Close();
            return RedirectToPage("/Main/UserHome");
        }
    }
}
