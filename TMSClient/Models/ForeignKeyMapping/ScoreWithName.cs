namespace TMSClient.Models.ForeignKeyMapping
{
    public class ScoreWithName
    {

        public int ScoreID { get; set; }

        public string AssessmentName { get; set; }
        public string TraineeName { get; set; }
        public int GainedScore { get; set; }
        public int TotalScore { get; set; } = 100;

    }
}
