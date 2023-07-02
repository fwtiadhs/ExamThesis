namespace ExamThesis.Models.AuthModel
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public ErrorResponse error { get; set; }
    }
    public class ErrorResponse
    {
        public string error { get; set; }
        public string error_description { get; set; }
        public string message { get; set; }
    }
}
