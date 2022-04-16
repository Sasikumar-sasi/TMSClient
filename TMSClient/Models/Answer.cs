using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMSClient.Models
{
    public class Answer
    {
        [Key]
        public int AnswerID { get; set; }
        [Required]
        public string AnswerPath { get; set; }
        [ForeignKey("AssessmentID")]
        public int AssessmentID { get; set; }
        [ForeignKey("TraineeID")]
        public int TraineeID { get; set; }

        public virtual Trainee Trainee { get; set; }
        public virtual Assessment assessment { get; set; }
    }
}
