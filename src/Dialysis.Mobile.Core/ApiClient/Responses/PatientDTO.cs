using System;

namespace Dialysis.Mobile.Core.ApiClient.Responses
{
    public class PatientDTO
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PESEL { get; set; }
        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
    }
}