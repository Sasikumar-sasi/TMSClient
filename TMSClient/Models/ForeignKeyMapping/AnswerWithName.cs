namespace TMSClient.Models.ForeignKeyMapping
{
    public class AnswerWithName
    {
        public int AnswerID { get; set; }
        public string AnswerPath { get; set; }
        public string AssessmentName { get; set; }
        public string TraineeName { get; set; }
        public int TraineeID { get; set; }
        public int AssessmentID { get; set; }
    }
}
