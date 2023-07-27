namespace webApiCRM.Models
{
    public class AuthModel
    {

        public string message { get; set; }
        public Boolean isAuthenticated { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateTime  ExpiresOn { get; set; }

        public List<string> Roles { get; set; }

        public string Token { get; set; }

    }
}
