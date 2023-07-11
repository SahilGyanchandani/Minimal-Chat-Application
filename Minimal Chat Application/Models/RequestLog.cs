using System.ComponentModel.DataAnnotations;
namespace Minimal_Chat_Application.Models
{
   

    public class RequestLog
    {
        [Key]
        public int LogId { get; set; }
        public string IpAddress { get; set; }
        //public string? RequestBody { get; set; }=null;
        public DateTime Timestamp { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
    }

}
