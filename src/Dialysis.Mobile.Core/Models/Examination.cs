using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.Models
{
    public class Examination
    {
        public double Weight { get; set; }
        public double TurbidityNTU { get; set; }
        public double TurbidityFAU { get; set; }
        public int DiastolicPressure { get; set; }
        public int SystolicPressure { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PatientID { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
