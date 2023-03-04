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
            ShowRates = new ObservableCollection<Rate>();
            EditPane = false;
            SaveButtonVisibility = Visibility.Collapsed;
            EditButtonVisibility = Visibility.Visible;           
            LoadUserPreference();
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
        
        private string _PureGoldRate;
        public string PureGoldRate
        {
            get { return _PureGoldRate; }
            set { _PureGoldRate = value; RaisePropertyChanged("PureGoldRate"); }
        }

        private string _SilverRate;
        public string SilverRate
        {
            get { return _SilverRate; }
            set { _SilverRate = value; RaisePropertyChanged("SilverRate"); }
        }

        private bool _EditPane;
        public bool EditPane
        {
            get { return _EditPane; }
            set { _EditPane = value; RaisePropertyChanged("EditPane"); }
        }

        private Visibility _EditButtonVisibility;
        public Visibility EditButtonVisibility
        {
            get { return _EditButtonVisibility; }
            set { _EditButtonVisibility = value; RaisePropertyChanged("EditButtonVisibility"); }
        }

        private Visibility _SaveButtonVisibility;
        public Visibility SaveButtonVisibility
        {
            get { return _SaveButtonVisibility; }
            set { _SaveButtonVisibility = value; RaisePropertyChanged("SaveButtonVisibility"); }
        }

        private bool _IsEnabledRateListBox;
        public bool IsEnabledRateListBox
        {
            get { return _IsEnabledRateListBox; }
            set { _IsEnabledRateListBox = value; RaisePropertyChanged("IsEnabledRateListBox"); }
        }
        

        #endregion Properties


        #region iCommands

        RelayCommand _SaveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new RelayCommand(() => OnSaveCommandclick());
                return _SaveCommand;
            }
        }

        RelayCommand _SaveUserPreferenceCommand = null;
        public ICommand SaveUserPreferenceCommand
        {
            get
            {
                if (_SaveUserPreferenceCommand == null)
                    _SaveUserPreferenceCommand = new RelayCommand(() => OnSaveUserPreferenceCommandclick());
                return _SaveUserPreferenceCommand;
            }
        }

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

        RelayCommand _RateSelectionChangeCommand = null;
        public ICommand RateSelectionChangeCommand
        {
            get
            {
                if (_RateSelectionChangeCommand == null)
                    _RateSelectionChangeCommand = new RelayCommand(() => OnRateSelectionChangeCommandclick());
                return _RateSelectionChangeCommand;
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

        void OnSaveCommandclick()
        {
            EditPane = false;
            EditButtonVisibility = Visibility.Visible;
            SaveButtonVisibility = Visibility.Collapsed;

            if (ValidateRateDetails())
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    NpgsqlTransaction trans = connection.BeginTransaction();
                    try
                    {
                        string query_update_gold_details = string.Format("update rate set rate = '{0}' where id = {1};", PureGoldRate, 1);
                        string query_update_silver_details = string.Format("update rate set rate = '{0}' where id = {1};", SilverRate, 2);

                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_update_gold_details, CommandType.Text))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_update_silver_details, CommandType.Text))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.LogError(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Gold/Silver rate are entered Incorrect format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadUserPreference();
        }
       
        void OnSaveUserPreferenceCommandclick()
        {
            UserPreference userPreferenceInfo = new UserPreference();
            userPreferenceInfo.Rates = Rates;
            
            CommanDetails.user.userPreference.Rates = Rates;
            string userInfo = JsonConvert.SerializeObject(CommanDetails.user.userPreference);
            string sqlQuery = string.Format("update login set preference ='{0}' where id ={1}", userInfo, CommanDetails.user.id);

            using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
            {
                NpgsqlTransaction trans = connection.BeginTransaction();
                try
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, sqlQuery, CommandType.Text))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    MessageBox.Show("Login preference information saved successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Logger.LogError(ex);
                }
            }
            LoadUserPreference();
        }

        void LoadUserPreference()
        {
            UserPreference userPreferenceInfo = CommanDetails.user.userPreference;
            if (userPreferenceInfo.Rates != null)
            {
                foreach(var userPref in userPreferenceInfo.Rates)
                {
                    //foreach (var uiRates in Rates)
                    //{
                    //    if (userPref.isChecked && uiRates.name == userPref.name)
                    //    {
                    //        ShowRates.Add(userPref);
                    //        uiRates.isChecked = true;
                    //        break;
                    //    }
                    //}
                }
            }
            //OnRateSelectionChangeCommandclick();
        }

        void OnRateSelectionChangeCommandclick()
        {
            int count = 0;
            foreach(Rate rate in Rates)
            {
                if(rate.isChecked)
                {
                    count = count + 1;
                }
            }

            if(count >= Helper.RateDisplayCount)
            {
                foreach (Rate rate in Rates)
                {
                    if (!rate.isChecked)
                    {
                        rate.isEnabled = false;
                    }
                }
            }
            else
            {
                foreach (Rate rate in Rates)
                {
                    rate.isEnabled = true;
                }
            }

            ShowRates = new ObservableCollection<Rate>();
            foreach (Rate rate in Rates)
            {
                if (rate.isChecked)
                {
                    ShowRates.Add(rate);
                }
            }
        }

        void OnLogoutCommandclick()
        {
            System.Environment.Exit(0);
        }

        bool ValidateRateDetails()
        {
            if (string.IsNullOrWhiteSpace(PureGoldRate) || !decimal.TryParse(PureGoldRate, out decimal goldRate))
            {
                return false;
            }
            else if(string.IsNullOrWhiteSpace(SilverRate) || !decimal.TryParse(SilverRate, out decimal silverRate))
            {
                return false;
            }
            return true;
        }

        void OnEditCommandclick()
        {
            EditPane = true;
            EditButtonVisibility = Visibility.Collapsed;
            SaveButtonVisibility = Visibility.Visible;
        }

        #endregion Methods

    }
}
