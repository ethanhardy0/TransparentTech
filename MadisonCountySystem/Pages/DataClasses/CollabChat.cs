namespace MadisonCountySystem.Pages.DataClasses
{
    public class CollabChat
    {
        public int CollabChatID { get; set; }
        public String? ChatContents { get; set; }
        public String? PostedDate { get; set; }
        public int UserID { get; set; }
        public int CollabID { get; set; }
        public String? Username { get; set; }
        public String? Password { get; set; }
        public String? Email { get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public String? Phone { get; set; }
        public String? Street { get; set; }
        public String? City { get; set; }
        public String? State { get; set; }
        public int Zip { get; set; }
        public String? UserType { get; set; }
        public String? UserRole { get; set; }
    }
}
