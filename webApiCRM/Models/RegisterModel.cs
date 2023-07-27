using System.ComponentModel.DataAnnotations;

namespace webApiCRM.Models
{
    public class RegisterModel
    {

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(130)]
        public string Email { get; set; }

        [Required, MaxLength(120)]
        public string password { get; set; }
    }
}
