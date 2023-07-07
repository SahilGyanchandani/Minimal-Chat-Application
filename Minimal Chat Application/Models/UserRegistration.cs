using System.ComponentModel.DataAnnotations;

namespace Minimal_Chat_Application.Models
{
    public class UserRegistration
    {
        [Key]
        public int UserID { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PhoneNumber { get; set; }
        public Boolean IsActive { get; set; }

        // Navigation property
        //public ICollection<Message> SendMessages { get; set; }
        //public ICollection<Message> ReceiveMessages { get; set; }

    }
}
