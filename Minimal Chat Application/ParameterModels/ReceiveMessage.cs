using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_Chat_Application.Models
{
    public class ReceiveMessage
    {
        [Key]
        public int ReceiverID { get; set; }
        public string Content { get; set; }

        // Navigation property
        //public ICollection<Message> Messages { get; set; }
    }
}
