using CommonLayer;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using Twilio.Rest.Api.V2010.Account.Usage.Record;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for MetalRateDetailsDialog.xaml
    /// </summary>
    public partial class MetalRateDetailsDialog : Window
    {
        //ObservableCollection<Rate> StandardRates = new ObservableCollection<Rate>();
        MainWindowViewModel mainWindowViewModel;
        public MetalRateDetailsDialog(MainWindowViewModel _mainWindowViewModel)
        {
            InitializeComponent();
            mainWindowViewModel = _mainWindowViewModel;
            
            lstbxStandardRates.ItemsSource = Helper.GetStandardMetalRates();

            txtPureGoldRate.Text = (lstbxStandardRates.ItemsSource as ObservableCollection<Rate>).First(x => x.name == "24 K").rate.ToString();
            txtSilverRate.Text = (lstbxStandardRates.ItemsSource as ObservableCollection<Rate>).First(x => x.name == "Silver").rate.ToString();
        }
        
        void OnSaveCommandclick(object sender, RoutedEventArgs e)
        {
            if (ValidateRateDetails())
            {
                OnSaveBaseMetalRate();
                OnSaveUserPreference();

                lstbxStandardRates.ItemsSource = Helper.GetStandardMetalRates();

                mainWindowViewModel.ShowRates = new ObservableCollection<Rate>();
                foreach (var rate in lstbxStandardRates.ItemsSource as ObservableCollection<Rate>)
                {
                    if (rate.isChecked)
                    {
                        mainWindowViewModel.ShowRates.Add(rate);
                    }
                }
                this.Close();
            }
        }

        bool ValidateRateDetails()
        {
            int counter = 0;
            btnOk.IsEnabled = true;
            txtWarning.Text = "";

            if(lstbxStandardRates.ItemsSource != null)
            {
                foreach (Rate obj in lstbxStandardRates.ItemsSource)
                {
                    if (obj.isChecked)
                    {
                        counter++;
                    }

                    if (counter > 3)
                    {
                        txtWarning.Text = "More than three rate selection not allowed";
                        btnOk.IsEnabled = false;
                        return false;
                    }
                }
            }

            if (!Helper.IsValidDecimal(txtPureGoldRate.Text) || !Helper.IsValidDecimal(txtSilverRate.Text))
            {
                txtWarning.Text = "In Valid Gold/Silver rate";
                btnOk.IsEnabled = false;
                return false;
            }

            return true;
        }

        void OnSaveUserPreference()
        {
            CommanDetails.user.userPreference.Rates = lstbxStandardRates.ItemsSource as ObservableCollection<Rate>;
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
                    //MessageBox.Show("Login preference information saved successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Logger.LogError(ex);
                }
            }
        }

        void OnSaveBaseMetalRate()
        {
            using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
            {
                NpgsqlTransaction trans = connection.BeginTransaction();
                try
                {
                    string query_update_gold_details = string.Format("update rate set rate = '{0}' where id = {1};", txtPureGoldRate.Text, 1);
                    string query_update_silver_details = string.Format("update rate set rate = '{0}' where id = {1};", txtSilverRate.Text, 2);

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

        private void btnClear(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RateCheckEvent(object sender, RoutedEventArgs e)
        {
            ValidateRateDetails();
        }

        private void txtPureGoldRate_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateRateDetails();
        }

        private void txtSilverRate_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateRateDetails();
        }
    }
}
