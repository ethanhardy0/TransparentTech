using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountySystem.Pages.DataClasses;
using MadisonCountySystem.Pages.DB;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace MadisonCountySystem.Pages.Collabs
{
    public class CollabChatModel : PageModel
    {
        public static String CollabID { get; set; }
        public String CollabName { get; set; }
        public List<CollabChat> ChatList { get; set; }
        public CollabChat NewChat { get; set; }
        public static int UserID { get; set; }

        [BindProperty]
        [Required] public String Message { get; set; }

        public CollabChatModel()
        {
            ChatList = new List<CollabChat>();
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
                HttpContext.Session.SetString("LibType", "Collab");
                CollabName = HttpContext.Session.GetString("collabName");
                CollabID = HttpContext.Session.GetString("collabID");
                UserID = Int32.Parse(HttpContext.Session.GetString("userID"));

                SqlDataReader chatReader = DBClass.ChatReader();
                while (chatReader.Read())
                {
                    if (CollabID == chatReader["CollabID"].ToString())
                    {
                        ChatList.Add(new CollabChat
                        {
                            ChatContents = chatReader["ChatContents"].ToString(),
                            PostedDate = chatReader["PostedDate"].ToString(),
                            Username = chatReader["Username"].ToString(),
                            FirstName = chatReader["FirstName"].ToString(),
                            LastName = chatReader["LastName"].ToString(),
                        });
                    }
                }
                DBClass.KnowledgeDBConnection.Close();
                ChatList.Reverse();
            }
        }

        public IActionResult OnPostMessage()
        {
            NewChat = new CollabChat
            {
                ChatContents = Message,
                PostedDate = DateTime.Now.ToString(),
                UserID = UserID,
                CollabID = Int32.Parse(CollabID)
            };
            DBClass.InsertChat(NewChat);
            DBClass.KnowledgeDBConnection.Close();

            SqlDataReader chatReader = DBClass.ChatReader();
            while (chatReader.Read())
            {
                if (CollabID == chatReader["CollabID"].ToString())
                {
                    ChatList.Add(new CollabChat
                    {
                        ChatContents = chatReader["ChatContents"].ToString(),
                        PostedDate = chatReader["PostedDate"].ToString(),
                        Username = chatReader["Username"].ToString(),
                        FirstName = chatReader["FirstName"].ToString(),
                        LastName = chatReader["LastName"].ToString(),
                    });
                }
            }
            DBClass.KnowledgeDBConnection.Close();
            ChatList.Reverse();
            ModelState.Clear();
            Message = null;
            CollabName = HttpContext.Session.GetString("collabName");
            CollabID = HttpContext.Session.GetString("collabID");
            UserID = Int32.Parse(HttpContext.Session.GetString("userID"));

            return RedirectToPage();
        }
    }
}
