using System.ComponentModel.DataAnnotations;

namespace TMSClient.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        [Required, MaxLength(30)]
        public string AdminName { get; set; }

        [Required, MaxLength(30)]
        public string Role { get; set; } = "ADMIN";

        [Required, MaxLength(15)]
        public string PhoneNumber { get; set; }
        [Required, MaxLength(30)]
        public string EmailID { get; set; }
        [Required, MaxLength(15)]
        public string Password { get; set; }
    }
}
