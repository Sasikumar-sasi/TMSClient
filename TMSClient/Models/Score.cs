using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMSClient.Models
{
    public class Score
    {
        [Key]
        public int ScoreID { get; set; }
        [ForeignKey("AssessmentID")]
        public int AssessmentID { get; set; }
        [ForeignKey("TraineeID")]
        public int TraineeID { get; set; }
        [Required]
        public int GainedScore { get; set; }
        public int TotalScore { get; set; } = 100;

        public virtual Assessment Assessment { get; set; }
    }
}
