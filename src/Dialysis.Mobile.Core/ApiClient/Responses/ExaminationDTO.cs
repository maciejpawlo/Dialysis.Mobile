namespace Dialysis.Mobile.Core.ApiClient.Responses
{
    public class ExaminationDTO
    {
        public int ExaminationID { get; set; }
        public double Weight { get; set; }
        public double TurbidityNTU { get; set; }
        public double TurbidityFAU { get; set; }
        public int SystolicPressure { get; set; }
        public int DiastolicPressure { get; set; }
        public string ImageURL { get; set; }
        public int PatientID { get; set; }
    }
}