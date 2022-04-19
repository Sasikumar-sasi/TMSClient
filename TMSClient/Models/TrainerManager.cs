using System.ComponentModel.DataAnnotations;

namespace TMSClient.Models
{
    public class TrainerManager
    {
        [Key]
        public int TMID { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; } = "N/A";

        [Required, MaxLength(15)]
        public string PhoneNumber { get; set; } = "N/A";

        [Required, Range(0, 20)]
        public int Experience { get; set; } = 0;
        [Required, MaxLength(30)]
        public string EducationQualification { get; set; } = "N/A";
        [Required]
        public string DOB { get; set; } = "N/A";

        [Required, MaxLength(100)]
        public string Address { get; set; } = "N/A";

        [Required, MaxLength(30)]
        public string Role { get; set; } = "Training Manager";

        [Required, MaxLength(30)]
        public string Position { get; set; }

        [Required, MaxLength(30)]
        public string EmailID { get; set; }

        [Required, MaxLength(15)]
        public string Password { get; set; }
    }
}
