using CommonLayer;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for AdvancedSearch.xaml
    /// </summary>
    public partial class AdvancedSearch : Window
    {
        #region Properties
       
        private ObservableCollection<Customer> _customersDetails;
        public ObservableCollection<Customer> CustomerDetails
        {
            get { return _customersDetails; }
            set { _customersDetails = value; }
        }

        private dynamic viewModel;

        #endregion Properties

        public AdvancedSearch(SearchTypes searchType, dynamic _viewModel)
        {
            InitializeComponent();
            viewModel = _viewModel;
        }

        private void OnSearchDetailsclick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtbxSearchKey.Text))
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    string query_customer = String.Format("SELECT * FROM customer WHERE(name LIKE '%{0}%') OR (email LIKE '%{0}%') OR (mobile LIKE '%{0}%') and isDeleted = False", txtbxSearchKey.Text);

                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
                                CustomerDetails = new ObservableCollection<Customer>();
                                while (dataReader.Read())
                                {
                                    Customer customer = new Customer
                                    {
                                        id = Convert.ToInt32(dataReader["Id"]),
                                        name = Convert.ToString(dataReader["name"]),
                                        description = Convert.ToString(dataReader["description"]),
                                        address = Convert.ToString(dataReader["address"]),
                                        email = Convert.ToString(dataReader["email"]),
                                        aadhaar = Convert.ToString(dataReader["aadhaar"]),
                                        mobile = Convert.ToString(dataReader["mobile"]),
                                        pan = Convert.ToString(dataReader["pan"]),
                                        gender = Convert.ToString(dataReader["gender"]),
                                        gst = Convert.ToString(dataReader["gst"])
                                    };
                                    CustomerDetails.Add(customer);
                                }
                            }
                        }
                    }
                    grdSearchDetails.ItemsSource = CustomerDetails;
                }
            }
            else
            {
                MessageBox.Show("Search string should not be Empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnOkCommandClick(object sender, RoutedEventArgs e)
        {
            this.Close();

            if (viewModel != null && CustomerDetails != null)
            {
                foreach (Customer customer in CustomerDetails)
                {
                    if (customer.isSelected)
                    {
                        viewModel.SearchDetailsCallback(customer);
                        break;
                    }
                }
            }
        }


    }
}
