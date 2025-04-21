namespace TestProjectAnnur.Data.Models
{
    public class A_T_NonWorking_Date
    {
        public Guid ID { get; set; }
        public string Factory_ID { get; set; }
        public string YYYYMM { get; set; }  // Changed from PU_Code to Team_Code
        public DateOnly Data_Date { get; set; }
        public string Section_Code { get; set; }
        public string PU_Code { get; set; }
    }
}
