namespace TMSClient.Models.ForeignKeyMapping
{
    public class TraineesBatchAdmin
    {
        
        public int TraineeID { get; set; }

        
        public string Name { get; set; } = "NA";

        
        public string PhoneNumber { get; set; } = "NA";

        
        public string SkillSet { get; set; } = "NA";

        
        public int Experience { get; set; } = 0;
        
        public string EducationQualification { get; set; } = "NA";
        
        public string DOB { get; set; } = "NA";

        
        public string Address { get; set; } = "NA";

        
        public string Role { get; set; } = "Trainer";

        
        public string Position { get; set; }

        public string EmailID { get; set; }

        public string Password { get; set; }
        public string BatchName { get; set; }
        
    }
}
