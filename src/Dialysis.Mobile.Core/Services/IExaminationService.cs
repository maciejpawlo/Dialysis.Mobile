using Dialysis.Mobile.Core.ApiClient.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.Services
{
    public interface IExaminationService
    {
        Task<bool> CreateExaminations(ExaminationDTO examinationDTO);
    }
}
