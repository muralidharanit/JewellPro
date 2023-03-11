using GalaSoft.MvvmLight;
using CommonLayer;
using System.Collections.ObjectModel;
using System;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;
using Npgsql;
using System.Data;
using Newtonsoft.Json;
using System.Net.Http;

namespace JewellPro
{
    public class MainWindowViewModel : ViewModelBase
    {
        Helper helper;
        public MainWindowViewModel()
        {
            helper = new Helper();
            OnLoad();
        }

        #region Properties
        
        private ObservableCollection<Rate> _Rates;
        public ObservableCollection<Rate> Rates
        {
            get { return _Rates; }
            set { _Rates = value; RaisePropertyChanged("Rates"); }
        }

        private ObservableCollection<Rate> _ShowRates;
        public ObservableCollection<Rate> ShowRates
        {
            get { return _ShowRates; }
            set { _ShowRates = value; RaisePropertyChanged("ShowRates"); }
        }
        

        #endregion Properties


        #region iCommands        

        RelayCommand _EditCommand = null;
        public ICommand EditCommand
        {
            get
            {
                if (_EditCommand == null)
                    _EditCommand = new RelayCommand(() => OnEditCommandclick());
                return _EditCommand;
            }
        }

        RelayCommand _LogoutCommand = null;
        public ICommand LogoutCommand
        {
            get
            {
                if (_LogoutCommand == null)
                    _LogoutCommand = new RelayCommand(() => OnLogoutCommandclick());
                return _LogoutCommand;
            }
        }

        #endregion iCommands


        #region Methods
       
        void OnLogoutCommandclick()
        {
            System.Environment.Exit(0);
        }

        void OnEditCommandclick()
        {
            MetalRateDetailsDialog dialog = new MetalRateDetailsDialog(this);
            dialog.ShowDialog();
        }

        public void OnLoad()
        {
            ShowRates = new ObservableCollection<Rate>();
            foreach (Rate rate in Helper.GetStandardMetalRates())
            {
                if(rate.isChecked)
                {
                    ShowRates.Add(rate);
                }
            }
        }

        #endregion Methods

    }
}
