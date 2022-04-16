using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMSClient.Models.ViewModel
{
    public class AnswerViewModel
    {
        [Required]
        public IFormFile AnswerFile { get; set; }
        [ForeignKey("AssessmentID")]
        public int AssessmentID { get; set; }

    }
}
