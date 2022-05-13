using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Dialysis.Mobile.Core.ViewModels.Home;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dialysis.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [MvxModalPresentation(WrapInNavigationPage = true)]
    public partial class ExaminationResultPage : MvxContentPage<ExaminationResultViewModel>
    {
        public ExaminationResultPage()
        {
            InitializeComponent();
        }

        //TODO: disable back button OR display some kind of alert
    }
}