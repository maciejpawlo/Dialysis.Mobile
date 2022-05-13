namespace Dialysis.Mobile.Core.ApiClient.Responses
{
    public class ExaminationDTO
    {
        public int ExaminationID { get; set; }
        public double Weight { get; set; }
        public double Turbidity { get; set; }
        public string ImageURL { get; set; }
        public int PatientID { get; set; }
    }
}