using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_Chat_Application.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }     
        public int UserID { get; set; } //SenderID Of User
        public int ReceiverID { get; set; } //ReceiverID of User
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Boolean IsActive { get; set; }

        //public UserRegistration Sender { get; set; }
        //public UserRegistration Receiver { get; set; }
    }
}
