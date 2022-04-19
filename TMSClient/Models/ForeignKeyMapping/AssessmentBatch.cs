namespace TMSClient.Models.ForeignKeyMapping
{
    public class AssessmentBatch
    {
       
        public int AssessmentID { get; set; }
        
        public string AssessmentName { get; set; }
        
        public string Date { get; set; } 
        
        public string Duration { get; set; }
        
        public string StartingTime { get; set; }
        
        public string EndingTime { get; set; }

        public string Question { get; set; } = "NA";


        public string BatchName { get; set; }
        
        
    }
}
