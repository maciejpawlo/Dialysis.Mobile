using Dialysis.Mobile.Core.ApiClient;
using Dialysis.Mobile.Core.ApiClient.Requests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Dialysis.Mobile.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> logger;
        private readonly IDialysisAPI dialysisAPI;

        public AuthService(ILogger<AuthService> logger,
            IDialysisAPI dialysisAPI)
        {
            this.logger = logger;
            this.dialysisAPI = dialysisAPI;
        }

        public async Task<bool> Authencticate(string username, string password)
        {
            var authResposne = await dialysisAPI.Authenticate(new AuthenticateRequest { UserName = username, Password = password });
            if (authResposne.IsSuccessStatusCode)
            {
                await SecureStorage.SetAsync("jwt_token", authResposne.Content.AccessToken);
                await SecureStorage.SetAsync("refresh_token", authResposne.Content.RefreshToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RefreshToken()
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            var refreshTokenResponse = await dialysisAPI.RefreshToken(new RefreshTokenRequest { RefreshToken = refreshToken }).ConfigureAwait(false);
            if (refreshTokenResponse.IsSuccessStatusCode)
            {
                await SecureStorage.SetAsync("jwt_token", refreshTokenResponse.Content.AccessToken);
                await SecureStorage.SetAsync("refresh_token", refreshTokenResponse.Content.RefreshToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Logout()
        {
            _ = SecureStorage.Remove("jwt_token");
            _ = SecureStorage.Remove("refresh_token");
            return true;
        }

        public async Task<bool> IsAuthenticated()
        {
            var jwt = await SecureStorage.GetAsync("jwt_token");
            var refresh = await SecureStorage.GetAsync("refresh_token");
            if (jwt != null && refresh != null)
            {
                return true;
            }
            return false;
        }
    }
}
