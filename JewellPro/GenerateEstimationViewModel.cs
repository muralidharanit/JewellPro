using CommonLayer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    public class GenerateEstimationViewModel : ViewModelBase
    {
        #region Properties
        
        private Helper helper;

        private string _OrderButtonLabel;
        public string OrderButtonLabel
        {
            get { return _OrderButtonLabel; }
            set { _OrderButtonLabel = value; RaisePropertyChanged("OrderButtonLabel"); }
        }

        private string _EstimationRefNo;
        public string EstimationRefNo
        {
            get { return _EstimationRefNo; }
            set { _EstimationRefNo = value; RaisePropertyChanged("EstimationRefNo"); }
        }

        private ObservableCollection<Customer> _customersDetails;
        public ObservableCollection<Customer> CustomerDetails
        {
            get { return _customersDetails; }
            set { _customersDetails = value; RaisePropertyChanged("CustomerDetails"); }
        }

        private ObservableCollection<OrderDetails> _orderDetailsCollection;
        public ObservableCollection<OrderDetails> OrderDetailsCollection
        {
            get { return _orderDetailsCollection; }
            set { _orderDetailsCollection = value; RaisePropertyChanged("OrderDetailsCollection"); }
        }

        private ObservableCollection<JewelType> _jewelTypes;
        public ObservableCollection<JewelType> JewelTypes
        {
            get { return _jewelTypes; }
            set { _jewelTypes = value; RaisePropertyChanged("JewelTypes"); }
        }

        private Purity _SelectedPurity;
        public Purity SelectedPurity
        {
            get { return _SelectedPurity; }
            set { _SelectedPurity = value; RaisePropertyChanged("SelectedPurity"); }
        }

        private ObservableCollection<Purity> _puritys;
        public ObservableCollection<Purity> Puritys
        {
            get { return _puritys; }
            set { _puritys = value; RaisePropertyChanged("Puritys"); }
        }
        

        private Customer _SelectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set { _SelectedCustomer = value; RaisePropertyChanged("SelectedCustomer"); }
        }

        private OrderDetails _OrderDetails;
        public OrderDetails OrderDetails
        {
            get { return _OrderDetails; }
            set { _OrderDetails = value; RaisePropertyChanged("OrderDetails"); }
        }

        private JewelType _SelectedJewelType;
        public JewelType SelectedJewelType
        {
            get { return _SelectedJewelType; }
            set { _SelectedJewelType = value; RaisePropertyChanged("SelectedJewelType"); }
        }

        private string _TotalGoldWeight;
        public string TotalGoldWeight
        {
            get { return _TotalGoldWeight; }
            set { _TotalGoldWeight = value; RaisePropertyChanged("TotalGoldWeight"); }
        }

        private GenerateEstimationGridColumns _GridColumnsVisibility;
        public GenerateEstimationGridColumns GridColumnsVisibility
        {
            get { return _GridColumnsVisibility; }
            set { _GridColumnsVisibility = value; RaisePropertyChanged("GridColumnsVisibility"); }
        }

        #endregion Properties


        public GenerateEstimationViewModel()
        {
            helper = new Helper();
            OnLoad();
        }


        #region iCommands

        RelayCommand _ShowSearchDetailsCommand = null;
        public ICommand ShowSearchDetailsCommand
        {
            get
            {
                if (_ShowSearchDetailsCommand == null)
                    _ShowSearchDetailsCommand = new RelayCommand(() => OnShowSearchDetailsCommandclick());
                return _ShowSearchDetailsCommand;
            }
        }

        RelayCommand _AddOrderCommand = null;
        public ICommand AddOrderCommand
        {
            get
            {
                if (_AddOrderCommand == null)
                    _AddOrderCommand = new RelayCommand(() => OnAddOrderCommandclick());
                return _AddOrderCommand;
            }
        }

        RelayCommand<object> _OrderDetailsEditCommand = null;
        public ICommand OrderDetailsEditCommand
        {
            get
            {
                if (_OrderDetailsEditCommand == null)
                    _OrderDetailsEditCommand = new RelayCommand<object>((obj) => OnOrderDetailsEditCommand(obj));
                return _OrderDetailsEditCommand;
            }
        }

        RelayCommand<object> _OrderDetailsDeleteCommand = null;
        public ICommand OrderDetailsDeleteCommand
        {
            get
            {
                if (_OrderDetailsDeleteCommand == null)
                    _OrderDetailsDeleteCommand = new RelayCommand<object>((obj) => OnOrderDetailsDeleteCommand(obj));
                return _OrderDetailsDeleteCommand;
            }
        }

        RelayCommand _ResetCommand = null;
        public ICommand ResetCommand
        {
            get
            {
                if (_ResetCommand == null)
                    _ResetCommand = new RelayCommand(() => OnResetCommandclick());
                return _ResetCommand;
            }
        }

        RelayCommand _GenerateInvoiceCommand = null;
        public ICommand GenerateInvoiceCommand
        {
            get
            {
                if (_GenerateInvoiceCommand == null)
                    _GenerateInvoiceCommand = new RelayCommand(() => OnGenerateInvoiceCommandclick());
                return _GenerateInvoiceCommand;
            }
        }

        RelayCommand _AddAttachementCommand = null;
        public ICommand AddAttachementCommand
        {
            get
            {
                if (_AddAttachementCommand == null)
                    _AddAttachementCommand = new RelayCommand(() => OnAddAttachementCommandclick());
                return _AddAttachementCommand;
            }
        }

        #endregion iCommands


        #region Methods

        void OnLoad()
        {
            CustomerDetails = helper.GetAllCustomerDetails();
            JewelTypes = helper.GetAllLoadJewelTypes();
            Puritys = helper.GetAllPurityDetails();
            EstimationRefNo = helper.GetNextOrderRefNo(OrderType.Estimation);
            OrderButtonLabel = Convert.ToString(UserControlState.Add);
            GridColumnsVisibility = new GenerateEstimationGridColumns { attachement = false, description = true, dueDate = false, jewelPurity = true, jewelType = true, netWeight = true, seal = true, size = true };
            OrderDetails = new OrderDetails();
            OrderDetails.orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            OrderDetails.orderDate = DateTime.Now.ToString();
            OrderDetailsCollection = new ObservableCollection<OrderDetails>();

            TotalGoldWeight = string.Empty;
        }

        bool ValidateCustomerOrderDetailsCollection()
        {
            if (OrderDetailsCollection == null || OrderDetailsCollection.Count == 0)
            {
                MessageBox.Show("OrderDetails should not be empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            long customerId = OrderDetailsCollection[0].customer.id;

            foreach (OrderDetails order in OrderDetailsCollection)
            {
                if (customerId != order.customer.id)
                {
                    MessageBox.Show("OrderDetails should not be created, different customer not allowed", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        void OnGenerateInvoiceCommandclick()
        {
            if (ValidateCustomerOrderDetailsCollection())
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    NpgsqlTransaction trans = connection.BeginTransaction();
                    try
                    {
                        //Insert into customer_order table
                        long fk_customer_order = 0;
                        string query_customer_order = String.Format("Insert into estimation_order (order_no, order_ref_no, fk_customer_id) values('{0}','{1}', {2}) RETURNING id", DateTime.Now.ToString("yyyyMMddHHmmss"), EstimationRefNo, SelectedCustomer.id);
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order, CommandType.Text))
                        {
                            fk_customer_order = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        if (fk_customer_order > 0)
                        {
                            List<string> custOrderDetails = new List<string>();
                            foreach (OrderDetails obj in OrderDetailsCollection)
                            {
                                custOrderDetails.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9})", obj.size,
                                    obj.netWeight, obj.seal, obj.description, obj.attachement, obj.orderDate, obj.dueDate, obj.jewelPurity,
                                    obj.jewelType.id, fk_customer_order));
                            }

                            string custOrderList = string.Join<string>(",", custOrderDetails);
                            string query_customer_order_details = "Insert into estimation_order_details(size, net_weight, seal, description, attachement, order_date, due_date, ornament_purity, fk_ornament_type_id, fk_customer_order_id) values " + custOrderList;

                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_details, CommandType.Text))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (fk_customer_order == 0)
                        {
                            throw new Exception("Order creation failed. close and open the tool.");
                        }
                        
                        trans.Commit();
                        MessageBoxResult messageBoxResult = MessageBox.Show("Customer estimation order added successfully", "Record", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (MessageBoxResult.Yes == messageBoxResult)
                        {
                            OnGenerateExcelCommand();
                        }
                        OnLoad();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.LogError(ex.Message);
                    }
                }
            }
        }

        void OnAddAttachementCommandclick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image Files| *.jpg; *.jpeg; *.png;";

            if (openFileDialog.ShowDialog() == true)
            {
                string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "assets");

                if (!Directory.Exists(assetsPath))
                {
                    Directory.CreateDirectory(assetsPath);
                }

                string orderPath = Path.Combine(assetsPath, EstimationRefNo);
                if (!Directory.Exists(orderPath))
                {
                    Directory.CreateDirectory(orderPath);
                }

                string fileName = Convert.ToString(Directory.GetFiles(orderPath, "*", SearchOption.AllDirectories).Length + 1);
                string fileExt = Path.GetExtension(openFileDialog.SafeFileName);
                string fileFullName = string.Concat(fileName, fileExt);

                File.Copy(openFileDialog.FileName, Path.Combine(orderPath, fileFullName));

                OrderDetails.attachement = fileFullName;
                OrderDetails.attachementPath = orderPath;
            }
        }

        bool ValidateCustomerOrder()
        {
            StringBuilder errorControl = new StringBuilder();

            if (SelectedCustomer == null || SelectedCustomer.name.Trim().Length == 0)
            {
                errorControl.AppendLine("Select Customer Name");
                errorControl.AppendLine();
            }

            if (SelectedJewelType == null || SelectedJewelType.name.Trim().Length == 0)
            {
                errorControl.AppendLine("Select Ornament Type");
                errorControl.AppendLine();
            }

            if (!string.IsNullOrEmpty(OrderDetails.quantity) && !Helper.IsValidInteger(OrderDetails.quantity))
            {
                errorControl.AppendLine("Enter Valid jewel Quantity");
                errorControl.AppendLine();
            }

            if (string.IsNullOrEmpty(OrderDetails.jewelPurity))
            {
                errorControl.AppendLine("Enter Jewel Purity");
                errorControl.AppendLine();
            }
            else if (!Helper.IsValidDecimal(OrderDetails.jewelPurity))
            {
                errorControl.AppendLine("Enter Valid Jewel Purity. Example 88.00");
                errorControl.AppendLine();
            }

            if (string.IsNullOrEmpty(OrderDetails.netWeight))
            {
                errorControl.AppendLine("Enter Jewellery Net Weight");
                errorControl.AppendLine();
            }
            else if (!Helper.IsValidDecimal(OrderDetails.netWeight))
            {
                errorControl.AppendLine("Enter Valid Weight. Example 48.560");
                errorControl.AppendLine();
            }

            if (errorControl.Length != 0)
            {
                MessageBox.Show(errorControl.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        void OnAddOrderCommandclick()
        {
            if (ValidateCustomerOrder())
            {
                OrderDetails.customer = SelectedCustomer;
                OrderDetails.jewelType = SelectedJewelType;
                if (OrderButtonLabel == Convert.ToString(UserControlState.Add))
                {
                    OrderDetails.subOrderNo = "SUB" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    OrderDetailsCollection.Add(OrderDetails);

                }
                else if (OrderButtonLabel == Convert.ToString(UserControlState.Update))
                {
                    for (int i = OrderDetailsCollection.Count - 1; i >= 0; --i)
                    {
                        if (OrderDetailsCollection[i].subOrderNo == OrderDetails.subOrderNo)
                        {
                            OrderDetailsCollection.RemoveAt(i);
                            OrderDetailsCollection.Insert(i, OrderDetails);
                            OrderButtonLabel = Convert.ToString(UserControlState.Add);
                            break;
                        }
                    }
                }
                else
                {
                    throw new CustomException("Unknown state found in OnAddOrderCommandclick");
                }
                OrderDetails = new OrderDetails();
                OrderDetails.orderDate = DateTime.Now.ToString();
            }
        }

        void OnOrderDetailsEditCommand(object orderDetail)
        {
            OrderButtonLabel = Convert.ToString(UserControlState.Update);
            SelectedCustomer = (orderDetail as OrderDetails).customer;
            SelectedJewelType = (orderDetail as OrderDetails).jewelType;
            OrderDetails = helper.CloneOrderDetails(orderDetail as OrderDetails);
        }

        void OnOrderDetailsDeleteCommand(object orderDetail)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this order", "Delete order", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (OrderDetails order in OrderDetailsCollection)
                {
                    if (order.subOrderNo == (orderDetail as OrderDetails).subOrderNo)
                    {
                        OrderDetailsCollection.Remove(order);
                        OrderDetails = new OrderDetails();
                        OrderDetails.orderDate = DateTime.Now.ToString();
                        OrderButtonLabel = Convert.ToString(UserControlState.Add);
                        break;
                    }
                }
            }
        }

        void OnShowSearchDetailsCommandclick()
        {
            AdvancedSearch advancedSearch  = new AdvancedSearch(SearchTypes.Customer, this);
            advancedSearch.ShowDialog();
        }

        public void SearchDetailsCallback(Customer searchCustomer)
        {
            foreach (Customer customer in CustomerDetails)
            {
                if (customer.id == searchCustomer.id)
                {
                    SelectedCustomer = customer;
                    break;
                }
            }
        }

        void OnResetCommandclick()
        {
            OrderButtonLabel = Convert.ToString(UserControlState.Add);
            if (OrderDetailsCollection == null || OrderDetailsCollection.Count == 0)
            {
                SelectedCustomer = null;
            }
            SelectedJewelType = null;
            OrderDetails = new OrderDetails();
            OrderDetails.orderDate = DateTime.Now.ToString();
        }

        void OnGeneratePrintCommand()
        {
            try
            {
               
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        void OnGenerateExcelCommand()
        {
            try
            {
                OrderDetails orderDetails = new OrderDetails { orderDate = DateTime.Now.ToString("dd-mm-yyyy"), orderRefNo = EstimationRefNo };
                ExcelFileArgs excelFileArgs = new ExcelFileArgs { orderDetails = OrderDetailsCollection, selectedCustomer = SelectedCustomer, selectedCustomerOrder = orderDetails };
                ExcelGenerator excelGenerator = new ExcelGenerator();
                excelGenerator.GenerateCustomerEstimationInvoice(excelFileArgs);                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        #endregion Methods

    }
}
