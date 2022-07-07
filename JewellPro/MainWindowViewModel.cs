using GalaSoft.MvvmLight;
using CommonLayer;
using System.Collections.ObjectModel;
using System;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;
using Npgsql;
using System.Data;

namespace JewellPro
{
    public class MainWindowViewModel : ViewModelBase
    {
        Helper helper;
        public MainWindowViewModel()
        {
            helper = new Helper();
            EditPane = false;
            SaveButtonVisibility = Visibility.Collapsed;
            EditButtonVisibility = Visibility.Visible;
            Kt24GoldChecked = false;
            Kt22GoldChecked = true;
            Kt20GoldChecked = false;
            Kt18GoldChecked = false;
            GetGoldRates();
        }

        #region Properties
        
        private ObservableCollection<Rate> _Rates;
        public ObservableCollection<Rate> Rates
        {
            get { return _Rates; }
            set { _Rates = value; RaisePropertyChanged("Rates"); }
        }

        private string _22KtGoldRate;
        public string Kt22GoldRate
        {
            get { return _22KtGoldRate; }
            set { _22KtGoldRate = value; RaisePropertyChanged("Kt22GoldRate"); }
        }

        private string _20KtGoldRate;
        public string Kt20GoldRate
        {
            get { return _20KtGoldRate; }
            set { _20KtGoldRate = value; RaisePropertyChanged("Kt20GoldRate"); }
        }

        private string _18KtGoldRate;
        public string Kt18GoldRate
        {
            get { return _18KtGoldRate; }
            set { _18KtGoldRate = value; RaisePropertyChanged("Kt18GoldRate"); }
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

        private bool _kt24GoldChecked;
        public bool Kt24GoldChecked
        {
            get { return _kt24GoldChecked; }
            set { _kt24GoldChecked = value; RaisePropertyChanged("Kt24GoldChecked"); }
        }

        private bool _kt22GoldChecked;
        public bool Kt22GoldChecked
        {
            get { return _kt22GoldChecked; }
            set { _kt22GoldChecked = value; RaisePropertyChanged("Kt22GoldChecked"); }
        }

        private bool _kt20GoldChecked;
        public bool Kt20GoldChecked
        {
            get { return _kt20GoldChecked; }
            set { _kt20GoldChecked = value; RaisePropertyChanged("Kt20GoldChecked"); }
        }

        private bool _kt18GoldChecked;
        public bool Kt18GoldChecked
        {
            get { return _kt18GoldChecked; }
            set { _kt18GoldChecked = value; RaisePropertyChanged("Kt18GoldChecked"); }
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

        public void GetGoldRates()
        {
            Rates = helper.GetRates();

            if(Rates != null)
            {
                foreach(var obj in Rates)
                {
                    if(obj.name.ToLower() =="gold")
                    {
                        PureGoldRate = (Convert.ToInt16(obj.rate)).ToString("F");
                        Configuration.PureGoldRate = PureGoldRate;

                        Kt22GoldRate = (Convert.ToInt16(obj.rate) * 92 / 100).ToString("F");
                        Kt20GoldRate = (Convert.ToInt16(obj.rate) * 84 / 100).ToString("F");
                        Kt18GoldRate = (Convert.ToInt16(obj.rate) * 75 / 100).ToString("F");
                    }
                    else if (obj.name.ToLower() == "silver")
                    {
                        SilverRate = (Convert.ToInt16(obj.rate)).ToString("F");
                        Configuration.SilverRate = SilverRate;
                    }
                }
            }
        }

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
                        GetGoldRates();
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
                GetGoldRates();
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
