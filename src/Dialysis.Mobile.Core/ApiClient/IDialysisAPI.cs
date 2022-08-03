using Dialysis.Mobile.Core.ApiClient.Requests;
using Dialysis.Mobile.Core.ApiClient.Responses;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.ApiClient
{
    public interface IDialysisAPI
    {
        [Post("/auth/authenticate")]
        Task<IApiResponse<AuthenticateResponse>> Authenticate([Body] AuthenticateRequest request);

        [Post("/auth/refreshToken")]
        Task<IApiResponse<AuthenticateResponse>> RefreshToken([Body] RefreshTokenRequest request);

        [Get("/user/userinfo")]
        Task<IApiResponse<GetUserInfoResponse>> GetUserInfo([Authorize("Bearer")] string authorization);

        [Post("/examinations")]
        Task<IApiResponse> CreateExaminations([Authorize("Bearer")] string authorization, [Body] ExaminationDTO examinationDTO);
    }
}
