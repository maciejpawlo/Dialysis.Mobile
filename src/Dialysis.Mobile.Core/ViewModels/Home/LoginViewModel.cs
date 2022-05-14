using Dialysis.Mobile.Core.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.ViewModels.Home
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ILogger<LoginViewModel> logger;
        private readonly IAuthService authService;
        private readonly IMvxNavigationService navigationService;

        #region Properties
        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        #endregion

        #region Commands
        public MvxAsyncCommand LoginCommand { get; set; }

        #endregion

        public LoginViewModel(ILogger<LoginViewModel> logger,
            IAuthService authService, 
            IMvxNavigationService navigationService)
        {
            this.logger = logger;
            this.authService = authService;
            this.navigationService = navigationService;
            LoginCommand = new MvxAsyncCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
            var loginResult = await authService.Authencticate(Login, Password);
            if (!loginResult)
            {
                return;
            }
            await navigationService.Navigate<HomeViewModel>();
        }
    }
}
