using CommonLayer;
using Npgsql;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for CustomerDetails.xaml
    /// </summary>
    public partial class CustomerDetails : UserControl
    {
        string ADD_BUTTON_CAPTION = "Add Customer";
        string UPDATE_BUTTON_CAPTION = "Update Customer";
        long customerId = 0;
        public CustomerDetails(double ucHeight)
        {
            InitializeComponent();
            Init();
            this.Height = ucHeight;
        }

        private void Init()
        {
            loadCustomerDetails();
            clearUIControls();
        }

        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            DataGrid datagrid = ((Button)sender).CommandParameter as DataGrid;
            var selectedRow = datagrid.SelectedItem;

            btnAddUpdate.Content = UPDATE_BUTTON_CAPTION;
            customerId = (selectedRow as Customer).id;

            txtbxName.Text = (selectedRow as Customer).name;
            txtbxDescription.Text = (selectedRow as Customer).description;
            txtbxAddress.Text = (selectedRow as Customer).address;
            txtbxEmail.Text = (selectedRow as Customer).email;
            txtbxAddhaar.Text = (selectedRow as Customer).aadhaar;            

            txtbxMobile.Text = (selectedRow as Customer).mobile;
            txtbxGST.Text = (selectedRow as Customer).gst;
            txtbxPAN.Text = (selectedRow as Customer).pan;

            for (int i = 0; i <= cbxGender.Items.Count - 1; i++)
            {
                if ((cbxGender.Items[i] as ComboBoxItem).Content.ToString() == (selectedRow as Customer).gender)
                {
                    cbxGender.SelectedIndex = i;
                    break;
                }
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            DataGrid datagrid = ((Button)sender).CommandParameter as DataGrid;
            var selectedRow = datagrid.SelectedItem;

            MessageBoxResult result = MessageBox.Show("Do you want to Delete " + (selectedRow as Customer).name + " ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if ((selectedRow as Customer).id != 0)
                {
                    string Query = String.Format("Update Customer set isDeleted = True where id = {0}", (selectedRow as Customer).id);
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, Query, CommandType.Text))
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                loadCustomerDetails();
                                MessageBox.Show(String.Format("Customer '{0}' deleted successfully.", (selectedRow as Customer).name), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
        }

        private void AddUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateCustomerDetails())
            {
                string msgResult = string.Empty;
                string query = string.Empty;
                if (btnAddUpdate.Content.ToString() == ADD_BUTTON_CAPTION)
                {
                    query = String.Format("Insert into Customer (name, description, address, email, aadhaar, gst, mobile, pan, gender) VALUES " +
                    "('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", txtbxName.Text, txtbxDescription.Text, txtbxAddress.Text, txtbxEmail.Text, txtbxAddhaar.Text, txtbxGST.Text, txtbxMobile.Text, txtbxPAN.Text, cbxGender.Text);

                    msgResult = String.Format("Customer '{0}' added successfully.", txtbxName.Text);
                }
                else
                {
                    query = String.Format("Update Customer set name ='{0}', description ='{1}', address = '{2}', email='{3}', aadhaar ='{4}', gst='{5}', mobile='{6}', pan ='{7}', gender ='{8}' WHERE id = {9}", 
                        txtbxName.Text, txtbxDescription.Text, txtbxAddress.Text, txtbxEmail.Text, txtbxAddhaar.Text, txtbxGST.Text, txtbxMobile.Text, txtbxPAN.Text, cbxGender.Text, customerId);

                    msgResult = String.Format("Customer '{0}' updated successfully.", txtbxName.Text);
                }

                try
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query, CommandType.Text))
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                loadCustomerDetails();
                                clearUIControls();
                                MessageBox.Show(msgResult, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clearUIControls();
        }

        void loadCustomerDetails()
        {
            string Query = "SELECT * FROM Customer where isDeleted = False Order by Name";
            List<Customer> customers = new List<Customer>();
            try
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, Query, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
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
                                    customers.Add(customer);
                                }
                                dtGridCustomerDetails.ItemsSource = null;
                                dtGridCustomerDetails.ItemsSource = customers;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        bool ValidateCustomerDetails()
        {
            string errorControl = string.Empty;

            if (txtbxName.Text.Trim().Length == 0)
            {
                errorControl += " Name,";
            }

            if (txtbxAddress.Text.Trim().Length == 0)
            {
                errorControl += " Address,";
            }

            if (txtbxMobile.Text.Trim().Length == 0)
            {
                errorControl += " Mobile,";
            }            

            if (errorControl.Length != 0)
            {
                errorControl = errorControl.Remove(errorControl.Length - 1);
                MessageBox.Show(string.Format("{0} should not be empty", errorControl));
                return false;
            }

            return true;
        }

        void clearUIControls()
        {
            txtbxName.Text = string.Empty;
            txtbxDescription.Text = string.Empty;
            txtbxAddress.Text = string.Empty;
            txtbxEmail.Text = string.Empty;
            txtbxAddhaar.Text = string.Empty;            

            txtbxMobile.Text = string.Empty;
            txtbxGST.Text = string.Empty;
            txtbxPAN.Text = string.Empty;

            cbxGender.Text = string.Empty;
            cbxGender.SelectedIndex = -1;

            btnAddUpdate.Content = ADD_BUTTON_CAPTION;
            customerId = 0;
        }
    }
}
