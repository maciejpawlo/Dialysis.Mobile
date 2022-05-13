using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.ApiClient.Responses
{
    public class GetPatientsResponse : BaseResponse
    {
        public IEnumerable<PatientDTO> Patients { get; set; }
    }
}
