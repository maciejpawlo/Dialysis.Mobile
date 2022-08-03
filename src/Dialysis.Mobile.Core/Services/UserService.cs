using Dialysis.Mobile.Core.ApiClient;
using Dialysis.Mobile.Core.ApiClient.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Dialysis.Mobile.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IAuthService authService;
        private readonly IDialysisAPI dialysisAPI;

        public UserService(ILogger<UserService> logger,
            IAuthService authService,
            IDialysisAPI dialysisAPI)
        {
            this.logger = logger;
            this.authService = authService;
            this.dialysisAPI = dialysisAPI;
        }

        public async Task GetAndSaveUserInfo()
        {
            var unauthorizedPolicy = Policy
               .HandleResult<IApiResponse<GetUserInfoResponse>>(r => r.StatusCode == HttpStatusCode.Unauthorized)
               .RetryAsync(retryCount: 1, async (exception, retryCount) =>
               {
                   await authService.RefreshToken().ConfigureAwait(false);
               });

            var token = await SecureStorage.GetAsync("jwt_token");
            logger.LogInformation("Attempting to get user info from API");
            var result = await unauthorizedPolicy.ExecuteAsync(async()=>await dialysisAPI.GetUserInfo(token));
            if (result.IsSuccessStatusCode)
            {
                logger.LogInformation($"Successfully received user data");
                await SecureStorage.SetAsync("UserInfo", JsonConvert.SerializeObject(result.Content));
            }
            else
            {
                logger.LogError($"Could not send data to API, status code: {result.StatusCode}");
            }
        }
    }
}
