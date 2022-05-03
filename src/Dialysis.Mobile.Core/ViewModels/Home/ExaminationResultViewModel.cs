using Dialysis.Mobile.Core.Models;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Core;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.ViewModels.Home
{

    public class ExaminationResultViewModel : MvxViewModel<Examination, Examination>
    {
        private readonly ILogger<ExaminationResultViewModel> logger;
        private readonly IMvxNavigationService navigationService;

        #region Properties
        private double? _weight;
        public double? Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }

        private int? _diastolicPressure;
        public int? DiastolicPressure
        {
            get => _diastolicPressure;
            set => SetProperty(ref _diastolicPressure, value);
        }

        private int? _systolicPressure;
        public int? SystolicPressure
        {
            get => _systolicPressure;
            set => SetProperty(ref _systolicPressure, value);
        }

        private Examination _examinationToSend;
        public Examination ExaminationToSend
        {
            get => _examinationToSend;
            set => SetProperty(ref _examinationToSend, value);
        }
        #endregion

        #region Commands
        public MvxAsyncCommand SaveExaminationCommand { get; set; }
        #endregion

        public ExaminationResultViewModel(ILogger<ExaminationResultViewModel> logger,
            IMvxNavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            SaveExaminationCommand = new MvxAsyncCommand(SaveExaminationAsync);
        }

        private async Task SaveExaminationAsync()
        {
            ExaminationToSend.SystolicPressure = SystolicPressure.Value;
            ExaminationToSend.DiastolicPressure = DiastolicPressure.Value;
            ExaminationToSend.Weight = Weight.Value;
            logger.LogInformation("Returning filled examination: \n{examination}", ExaminationToSend);
            await navigationService.Close(this, ExaminationToSend);
        }

        public override void Prepare(Examination parameter)
        {
            this.ExaminationToSend = parameter;
        }
    }
}
