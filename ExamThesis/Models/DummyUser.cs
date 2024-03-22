
namespace ExamThesis.Models
{
    public class DummyUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EduPersonAffiliation { get; set; }

        public DummyUser(string username, string password, string eduPersonAffiliation)
        {
            Username = username;
            Password = password;
            EduPersonAffiliation = eduPersonAffiliation;
        }
    }

    public static class DummyUsers
    {
        public static List<DummyUser> AllUsers { get; } = new List<DummyUser>
    {
        new DummyUser("teacher1", "password1", "professor"),
        new DummyUser("teacher2", "password2", "professor"),
        new DummyUser("student1", "password1", "student"),
        new DummyUser("student2", "password2", "student")
    };
    }

}
