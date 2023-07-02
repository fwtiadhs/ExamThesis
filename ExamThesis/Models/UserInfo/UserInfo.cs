namespace ExamThesis.Models.UserInfo
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Cn { get; set; }
        public string Title { get; set; }
        public string Email { get; internal set; }
    }
}
