using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.ApiClient.Responses
{
    public class GetUserInfoResponse : BaseResponse
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int InternalUserID { get; set; } //doctor id/patient id
    }
}
