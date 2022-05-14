using Dialysis.Mobile.Core.ViewModels.Home;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dialysis.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    //[MvxContentPagePresentation(WrapInNavigationPage = true)]
    [MvxContentPagePresentation(NoHistory = true)]
    public partial class LoginPage : MvxContentPage<LoginViewModel>
    {
        public LoginPage()
        {
            InitializeComponent();
        }
    }
}