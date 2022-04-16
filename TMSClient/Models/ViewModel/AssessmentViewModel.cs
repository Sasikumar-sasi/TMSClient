using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMSClient.Models.ViewModel
{
    public class AssessmentViewModel
    {
        [Required, MaxLength(30)]
        public string AssessmentName { get; set; }
        [Required, MaxLength(30)]
        public string Date { get; set; } = DateTime.Now.ToString();
        [Required]
        public string Duration { get; set; }
        [Required]
        public string StartingTime { get; set; }
        [Required]
        public string EndingTime { get; set; }

        public IFormFile QuestionFile { get; set; } 


        [ForeignKey("BatchID")]
        public int BatchID { get; set; }
        public virtual Batch Batchs { get; set; }
    }
}
