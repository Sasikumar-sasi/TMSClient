using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMSClient.Models
{
    public class Trainee
    {
        [Key]
        public int TraineeID { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; } = "N/A";

        [Required, MaxLength(15)]
        public string PhoneNumber { get; set; } = "N/A";

        [Required, MaxLength(30)]
        public string SkillSet { get; set; } = "N/A";

        [Required, Range(0, 20)]
        public int Experience { get; set; } = 0;
        [Required, MaxLength(30)]
        public string EducationQualification { get; set; } = "N/A";
        [Required]
        public string DOB { get; set; } = "N/A";

        [Required, MaxLength(100)]
        public string Address { get; set; } = "N/A";

        [Required, MaxLength(30)]
        public string Role { get; set; } = "Trainee";

        [Required, MaxLength(30)]
        public string Position { get; set; }

        [Required, MaxLength(30)]
        public string EmailID { get; set; }

        [Required, MaxLength(15)]
        public string Password { get; set; }

        [ForeignKey("BatchID")]
        public int? BatchID { get; set; } 
        public virtual Batch Batchs { get; set; }

    }
}

