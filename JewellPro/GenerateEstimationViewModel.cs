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

        private Customer _SelectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set { _SelectedCustomer = value; RaisePropertyChanged("SelectedCustomer"); }
        }

        private OrderDetails _Order;
        public OrderDetails Order
        {
            get { return _Order; }
            set { _Order = value; RaisePropertyChanged("Order"); }
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

        RelayCommand _ShowDetectionWindowCommand = null;
        public ICommand ShowDetectionWindowCommand
        {
            get
            {
                if (_ShowDetectionWindowCommand == null)
                    _ShowDetectionWindowCommand = new RelayCommand(() => OnShowDetectionWindowCommand());
                return _ShowDetectionWindowCommand;
            }
        }

        #endregion iCommands


        #region Methods

        void OnLoad()
        {
            CustomerDetails = helper.GetAllCustomerDetails();
            JewelTypes = helper.GetAllLoadJewelTypes();
            EstimationRefNo = "ESTM_APS_" + helper.GetNextOrderRefNo();
            OrderButtonLabel = Convert.ToString(UserControlState.Add);
            GridColumnsVisibility = new GenerateEstimationGridColumns { attachement = false, description = true, dueDate = false, jewelPurity = true, jewelType = true, netWeight = true, seal = true, size = true };

            Order = new OrderDetails();
            Order.rateFreezeDate = DateTime.Now.ToString();
            Order.freezeRate = Configuration.PureGoldRate;
            //Order.isPriorityOrder = fal;

            OrderDetails = Helper.GenerateNewOrderDetailsInstance();
            OrderDetailsCollection = new ObservableCollection<OrderDetails>();
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
                        long fk_order_id = 0;

                        string query_customer_order = string.Format("Insert into orders " +
                            "(order_no, fk_customer_id, fk_order_type, order_date) values('{0}', {1}, {2}, '{3}') RETURNING id",
                            DateTime.Now.ToString("yyyyMMddHHmmss"), SelectedCustomer.id, (int)OrderType.Estimation, DateTime.Now.ToString("dd-MM-yyyy"));

                        if (Order.isRateFreeze && Order.isPriorityOrder)
                        {
                            query_customer_order = string.Format("Insert into orders " +
                            "(order_no, fk_customer_id, fk_order_type, order_date, is_rate_freeze, freeze_date, freeze_amount, is_priority_order) values('{0}', {1}, {2}, '{3}', '{4}','{5}', '{6}','{7}') RETURNING id",
                            DateTime.Now.ToString("yyyyMMddHHmmss"), SelectedCustomer.id, (int)OrderType.Estimation, DateTime.Now.ToString("dd-MM-yyyy"), Order.isRateFreeze, Order.rateFreezeDate, Order.freezeRate, Order.isPriorityOrder);
                        }
                        else if (Order.isRateFreeze)
                        {
                            query_customer_order = string.Format("Insert into orders " +
                            "(order_no, fk_customer_id, fk_order_type, order_date, is_rate_freeze, freeze_date, freeze_amount) values('{0}', {1}, {2}, '{3}', '{4}','{5}', '{6}') RETURNING id",
                            DateTime.Now.ToString("yyyyMMddHHmmss"), SelectedCustomer.id, (int)OrderType.Estimation, DateTime.Now.ToString("dd-MM-yyyy"), Order.isRateFreeze, Order.rateFreezeDate, Order.freezeRate);
                        }
                        else if (Order.isPriorityOrder)
                        {
                            query_customer_order = string.Format("Insert into orders " +
                            "(order_no, fk_customer_id, fk_order_type, order_date, is_priority_order) values('{0}', {1}, {2}, '{3}', '{4}') RETURNING id",
                            DateTime.Now.ToString("yyyyMMddHHmmss"), SelectedCustomer.id, (int)OrderType.Estimation, DateTime.Now.ToString("dd-MM-yyyy"), Order.isPriorityOrder);
                        }

                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order, CommandType.Text))
                        {
                            fk_order_id = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        if (fk_order_id > 0)
                        {
                            List<string> custOrderDetails = new List<string>();
                            foreach (OrderDetails obj in OrderDetailsCollection)
                            {
                                custOrderDetails.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9}, {10})",
                                    obj.size, obj.netWeight, obj.seal, obj.description, obj.attachement, obj.orderDate, obj.dueDate, obj.jewelPurity, obj.wastage,
                                    obj.jewelType.id, fk_order_id));
                            }

                            string custOrderList = string.Join<string>(",", custOrderDetails);
                            string query_customer_order_details = "Insert into order_details(size, net_weight, seal, description, attachement, order_date, due_date, ornament_purity, wastage, fk_ornament_type_id, fk_order_id) values " + custOrderList;

                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_details, CommandType.Text))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (fk_order_id == 0)
                        {
                            throw new Exception("Order creation failed. close and open the tool.");
                        }
                        trans.Commit();                        
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.LogError(ex);
                    }

                    try
                    {
                        var reportStatus = OnGenerateExcelCommand();
                        if (reportStatus.status)
                        {
                            ConfirmationDialog confirmationDialog = new ConfirmationDialog(SelectedCustomer, reportStatus);
                            confirmationDialog.ShowDialog();
                            //OnLoad();
                        }
                        else
                        {
                            MessageBox.Show("Customer estimation report not generated", "Record", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.LogError(ex);
                    }
                }
            }
        }

        void OnShowDetectionWindowCommand()
        {
            AddDetectionWindow addDetectionWindow = new AddDetectionWindow(this, OrderDetails.detectionDetails, OrderDetails.chargesDetails, isHideDetection:true);
            addDetectionWindow.ShowDialog();
            addDetectionWindow.Owner = System.Windows.Application.Current.MainWindow;
        }

        public void UpdateDetectionDetails(ObservableCollection<DetectionControl> DetectionControls, ObservableCollection<ChargesControl> ChargesControls)
        {
            //Console.WriteLine();
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

        bool ValidateOrder()
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

            if(OrderDetails.isRateFreeze)
            {
                if (string.IsNullOrEmpty(OrderDetails.rateFreezeDate))
                {
                    errorControl.AppendLine("Rate freeze selected please select the freeze date");
                    errorControl.AppendLine();
                }
                else if (string.IsNullOrEmpty(OrderDetails.freezeRate) || !Helper.IsValidInteger(OrderDetails.freezeRate))
                {
                    errorControl.AppendLine("Rate freeze selected please enter valid amount");
                    errorControl.AppendLine();
                }
            }

            if (!string.IsNullOrEmpty(OrderDetails.wastage))
            {
                if (!Helper.IsValidDecimal(OrderDetails.wastage))
                {
                    errorControl.AppendLine("Enter Valid Jewel Wastage. Example 7.50 or 3");
                    errorControl.AppendLine();
                }
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
            if (ValidateOrder())
            {
                OrderDetails.customer = SelectedCustomer;
                OrderDetails.jewelType = SelectedJewelType;
                if (OrderButtonLabel == Convert.ToString(UserControlState.Add))
                {
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
                OrderDetails = Helper.GenerateNewOrderDetailsInstance();
            }
        }

        void OnOrderDetailsEditCommand(object orderDetail)
        {
            OrderButtonLabel = Convert.ToString(UserControlState.Update);
            SelectedCustomer = (orderDetail as OrderDetails).customer;
            SelectedJewelType = (orderDetail as OrderDetails).jewelType;
            OrderDetails = CloneObject.DeepClone<OrderDetails>(orderDetail as OrderDetails);
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
                        OrderDetails = Helper.GenerateNewOrderDetailsInstance();
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
            OrderDetails = Helper.GenerateNewOrderDetailsInstance();
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

        ReportStatus OnGenerateExcelCommand()
        {
            try
            {
                Order.customer = SelectedCustomer;
                Order.orderDetailsList = OrderDetailsCollection;
                Order.orderDate = DateTime.Now.ToString("dd-MM-yyyy");
                Order.orderRefNo = EstimationRefNo;
                ExcelGenerator excelGenerator = new ExcelGenerator();
                return excelGenerator.GenerateCustomerEstimationInvoice(Order);          
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            return null;
        }


        #endregion Methods

    }
}
