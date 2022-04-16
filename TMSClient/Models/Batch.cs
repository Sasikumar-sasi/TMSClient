using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMSClient.Models
{
    public class Batch
    {
        [Key]
        public int BatchID { get; set; }
        [Required, MaxLength(30)]
        public string BatchName { get; set; }

        public string Stream { get; set; }

        [ForeignKey("TrainerID")]
        public int? TrainerID { get; set; }
        public virtual Trainer Trainers { get; set; }
        ICollection<Trainee> Trainees { get; set; }


    }
}
