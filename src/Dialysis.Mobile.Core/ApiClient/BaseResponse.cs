using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.ApiClient
{
    public class BaseResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }
    }
}
