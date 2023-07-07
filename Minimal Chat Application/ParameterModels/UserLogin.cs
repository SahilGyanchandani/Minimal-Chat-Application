using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_Chat_Application.ParameterModels
{
    public class UserLogin
    {
        [Key]
        [Required]
        public int LoginID { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }



    }
}
