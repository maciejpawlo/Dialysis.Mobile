using Dialysis.Mobile.Core.Services;
using Dialysis.Mobile.Core.ViewModels.Home;
using MvvmCross.Exceptions;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.Utils
{
    public class AppStart : MvxAppStart
    {
        private readonly IAuthService authService;

        public AppStart(IMvxApplication application, IMvxNavigationService navigationService, IAuthService authService) : base(application, navigationService)
        {
            this.authService = authService;
        }

        protected override async Task NavigateToFirstViewModel(object hint = null)
        {
			try
			{
				// You need to run Task sync otherwise code would continue before completing.
				var tcs = new TaskCompletionSource<bool>();
				Task.Run(async () => tcs.SetResult(await authService.IsAuthenticated()));
				var isAuthenticated = tcs.Task.Result;

				if (isAuthenticated)
				{
					//You need to Navigate sync so the screen is added to the root before continuing.
					NavigationService.Navigate<HomeViewModel>().GetAwaiter().GetResult();
				}
				else
				{
					NavigationService.Navigate<LoginViewModel>().GetAwaiter().GetResult();
				}
			}
			catch (System.Exception exception)
			{
				throw exception.MvxWrap("Problem navigating to ViewModel");
			}
		}
    }
}
