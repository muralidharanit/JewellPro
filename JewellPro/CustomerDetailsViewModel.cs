using CommonLayer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JewellPro
{
    public class CustomerDetailsViewModel : ViewModelBase
    {

        #region Properties
        Helper helper;
        
        private ObservableCollection<Customer> _customersDetailsCollection;
        public ObservableCollection<Customer> CustomerDetailsCollection
        {
            get { return _customersDetailsCollection; }
            set { _customersDetailsCollection = value; RaisePropertyChanged("CustomerDetailsCollection"); }
        }

        private ObservableCollection<Gender> _GenderCollection;
        public ObservableCollection<Gender> GenderCollection
        {
            get { return _GenderCollection; }
            set { _GenderCollection = value; RaisePropertyChanged("GenderCollection"); }
        }

        private Gender _SelectedGender;
        public Gender SelectedGender
        {
            get { return _SelectedGender; }
            set { _SelectedGender = value; RaisePropertyChanged("SelectedGender"); }
        }

        private Customer _CustomerDetail;
        public Customer CustomerDetail
        {
            get { return _CustomerDetail; }
            set { _CustomerDetail = value; RaisePropertyChanged("CustomerDetail"); }
        }

        private Visibility _UpdateCustomerButtonVisibility;
        public Visibility UpdateCustomerButtonVisibility
        {
            get { return _UpdateCustomerButtonVisibility; }
            set { _UpdateCustomerButtonVisibility = value; RaisePropertyChanged("UpdateCustomerButtonVisibility"); }
        }

        private Visibility _AddCustomerButtonVisibility;
        public Visibility AddCustomerButtonVisibility
        {
            get { return _AddCustomerButtonVisibility; }
            set { _AddCustomerButtonVisibility = value; RaisePropertyChanged("AddCustomerButtonVisibility"); }
        }

        #endregion Properties

        public CustomerDetailsViewModel()
        {
            helper = new Helper();           
            OnLoad();
        }

        #region iCommands

        RelayCommand<object> _CustomerDetailsEditCommand = null;
        public ICommand CustomerDetailsEditCommand
        {
            get
            {
                if (_CustomerDetailsEditCommand == null)
                    _CustomerDetailsEditCommand = new RelayCommand<object>((obj) => OnCustomerDetailsEditCommandClick(obj));
                return _CustomerDetailsEditCommand;
            }
        }

        RelayCommand<object> _CustomerDetailsDeleteCommand = null;
        public ICommand CustomerDetailsDeleteCommand
        {
            get
            {
                if (_CustomerDetailsDeleteCommand == null)
                    _CustomerDetailsDeleteCommand = new RelayCommand<object>((obj) => OnCustomerDetailsDeleteCommandClick(obj));
                return _CustomerDetailsDeleteCommand;
            }
        }

        RelayCommand _AddCustomerCommand = null;
        public ICommand AddCustomerCommand
        {
            get
            {
                if (_AddCustomerCommand == null)
                    _AddCustomerCommand = new RelayCommand(() => OnAddCustomerCommandClick());
                return _AddCustomerCommand;
            }
        }

        RelayCommand _UpdateCustomerCommand = null;
        public ICommand UpdateCustomerCommand
        {
            get
            {
                if (_UpdateCustomerCommand == null)
                    _UpdateCustomerCommand = new RelayCommand(() => OnUpdateCustomerCommandClick());
                return _UpdateCustomerCommand;
            }
        }

        RelayCommand _ClearCommand = null;
        public ICommand ClearCommand
        {
            get
            {
                if (_ClearCommand == null)
                    _ClearCommand = new RelayCommand(() => OnClearCommandCommandClick());
                return _ClearCommand;
            }
        }

        #endregion iCommands

        #region Methods

        void OnLoad()
        {
            ClearUIControls();
            CustomerDetailsCollection = helper.GetAllCustomerDetails();
            GenderCollection = helper.GetAllGenders();
        }

        void OnClearCommandCommandClick()
        {
            ClearUIControls();
        }

        private void OnCustomerDetailsEditCommandClick(object customerDetail)
        {
            CustomerDetail = CloneObject.DeepClone<Customer>(customerDetail as Customer);
            SetGenderInfo(CustomerDetail);
            UpdateCustomerButtonVisibility = Visibility.Visible;
            AddCustomerButtonVisibility = Visibility.Collapsed;
        }

        private void OnAddCustomerCommandClick()
        {
            if (ValidateCustomerDetails())
            {
                if (AddCustomerButtonVisibility == Visibility.Visible && UpdateCustomerButtonVisibility != Visibility.Visible)
                {
                    string query = String.Format("Insert into Customer (name, description, address, email, aadhaar, gst, mobile, pan, gender, dob, anniversary) VALUES " +
                    "('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}')", 
                    CustomerDetail.name, CustomerDetail.description, CustomerDetail.address, CustomerDetail.email, CustomerDetail.aadhaar, CustomerDetail.gst, CustomerDetail.mobile, CustomerDetail.pan, SelectedGender.name, CustomerDetail.dob, CustomerDetail.anniversary);

                    string msgResult = String.Format("Customer '{0}' added successfully.", CustomerDetail.name);

                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        try
                        {
                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query, CommandType.Text))
                            {
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    trans.Commit();
                                    OnLoad();
                                    MessageBox.Show(msgResult, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Logger.LogError(ex.Message);
                        }
                    }

                }
            }
        }

        private void OnUpdateCustomerCommandClick()
        {
            if (ValidateCustomerDetails())
            {
                if(UpdateCustomerButtonVisibility == Visibility.Visible && AddCustomerButtonVisibility != Visibility.Visible)
                {
                    string query = String.Format("Update Customer set name ='{0}', description ='{1}', address = '{2}', email='{3}', aadhaar ='{4}', gst='{5}', mobile='{6}', pan ='{7}', gender ='{8}', dob ='{9}', anniversary ='{10}' WHERE id = {11}",
                        CustomerDetail.name, CustomerDetail.description, CustomerDetail.address, CustomerDetail.email, CustomerDetail.aadhaar, CustomerDetail.gst, CustomerDetail.mobile, CustomerDetail.pan, SelectedGender.name, CustomerDetail.dob, CustomerDetail.anniversary, CustomerDetail.id);

                    string msgResult = String.Format("Customer '{0}' updated successfully.", CustomerDetail.name);

                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        try
                        {
                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query, CommandType.Text))
                            {
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    trans.Commit();
                                    OnLoad();
                                    MessageBox.Show(msgResult, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Logger.LogError(ex.Message);
                        }
                    }
                }
            }
        }

        private void OnCustomerDetailsDeleteCommandClick(object customerDetail)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to Delete " + (customerDetail as Customer).name + " ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if ((customerDetail as Customer).id != 0)
                {
                    string Query = String.Format("Update Customer set isDeleted = True where id = {0}", (customerDetail as Customer).id);
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        try
                        {
                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, Query, CommandType.Text))
                            {
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    trans.Commit();
                                    OnLoad();
                                    MessageBox.Show(String.Format("Customer '{0}' deleted successfully.", (customerDetail as Customer).name), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            trans.Rollback();
                            Logger.LogError(ex.Message);
                        }
                    }
                }
            }
        }

        bool ValidateCustomerDetails()
        {
            string errorControl = string.Empty;

            
            if(UpdateCustomerButtonVisibility == Visibility.Visible && CustomerDetail.id == 0)
            {
                errorControl += " Customer Id not found for Update\n";
            }

            if (string.IsNullOrWhiteSpace(CustomerDetail.name))
            {
                errorControl += " Name,";
            }

            if (SelectedGender == null)
            {
                errorControl += " Gender,";
            }
            
            if (string.IsNullOrWhiteSpace(CustomerDetail.address))
            {
                errorControl += " Address,";
            }

            if (!string.IsNullOrWhiteSpace(CustomerDetail.email))
            {
                if(!Helper.IsValidEmail(CustomerDetail.email))
                {
                    errorControl += " Email,";
                }
            }
            

            if (string.IsNullOrWhiteSpace(CustomerDetail.mobile) && CustomerDetail.mobile.All(char.IsDigit))
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
        
        void ClearUIControls()
        {
            CustomerDetail = new Customer();
            SelectedGender =  null;
            AddCustomerButtonVisibility = Visibility.Visible;
            UpdateCustomerButtonVisibility = Visibility.Collapsed;
        }

        void SetGenderInfo(User commonUserInfo)
        {
            foreach (var obj in GenderCollection)
            {
                if (string.Equals(commonUserInfo.gender, obj.name, StringComparison.InvariantCultureIgnoreCase))
                {
                    SelectedGender = obj;
                    break;
                }
            }
        }

        #endregion Methods
    }
}
