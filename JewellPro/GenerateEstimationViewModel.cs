using CommonLayer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Npgsql;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    public class GenerateEstimationViewModel : ViewModelBase
    {
        Helper helper;
        long generatedOrderId = 0;
        UserControlState userControlState;

        #region Properties

        private string _OrderButtonLabel;
        public string OrderButtonLabel
        {
            get { return _OrderButtonLabel; }
            set { _OrderButtonLabel = value; RaisePropertyChanged("OrderButtonLabel"); }
        }

        private string _OrderDetailsButtonLabel;
        public string OrderDetailsButtonLabel
        {
            get { return _OrderDetailsButtonLabel; }
            set { _OrderDetailsButtonLabel = value; RaisePropertyChanged("OrderDetailsButtonLabel"); }
        }

        private bool _IsEnableOrderDetailsTab;
        public bool IsEnableOrderDetailsTab
        {
            get { return _IsEnableOrderDetailsTab; }
            set { _IsEnableOrderDetailsTab = value; RaisePropertyChanged("IsEnableOrderDetailsTab"); }
        }
        private int _EstimationTabSelectedIndex;
        public int EstimationTabSelectedIndex
        {
            get { return _EstimationTabSelectedIndex; }
            set { _EstimationTabSelectedIndex = value; RaisePropertyChanged("EstimationTabSelectedIndex"); }
        }


        private string _EstimationRefNo;
        public string EstimationRefNo
        {
            get { return _EstimationRefNo; }
            set { _EstimationRefNo = value; RaisePropertyChanged("EstimationRefNo"); }
        }

        private ObservableCollection<OrderDetails> _EstimationDetails;
        public ObservableCollection<OrderDetails> EstimationDetails
        {
            get { return _EstimationDetails; }
            set { _EstimationDetails = value; RaisePropertyChanged("EstimationDetails"); }
        }

        private OrderDetails _SelectedOrderDetail;
        public OrderDetails SelectedOrderDetail
        {
            get { return _SelectedOrderDetail; }
            set { _SelectedOrderDetail = value; RaisePropertyChanged("SelectedOrderDetail"); }
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
            set 
            { 
                _Order = value; 
                RaisePropertyChanged("Order");
                if(_Order != null && userControlState == UserControlState.Update)
                    IsEnableOrderDetailsTab = true;
            }
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

        public GenerateEstimationViewModel(UserControlState userControlState)
        {
            this.userControlState = userControlState;
            helper = new Helper();
            OnLoad();
            OrderDetailsCollection.CollectionChanged += OrderDetailsCollection_CollectionChanged;
        }

        private void OrderDetailsCollection_CollectionChanged(object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessageBox.Show("");
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

        RelayCommand _CustomerNameSelectionChangeCommand = null;
        public ICommand CustomerNameSelectionChangeCommand
        {
            get
            {
                if (_CustomerNameSelectionChangeCommand == null)
                    _CustomerNameSelectionChangeCommand = new RelayCommand(() => OnCustomerNameSelectionChangeCommand());
                return _CustomerNameSelectionChangeCommand;
            }
        }

        RelayCommand _CustomerEstimationSelectionChangeCommand = null;
        public ICommand CustomerEstimationSelectionChangeCommand
        {
            get
            {
                if (_CustomerEstimationSelectionChangeCommand == null)
                    _CustomerEstimationSelectionChangeCommand = new RelayCommand(() => OnCustomerEstimationSelectionChangeCommand());
                return _CustomerEstimationSelectionChangeCommand;
            }
        }

        RelayCommand _CreateOrderCommand = null;
        public ICommand CreateOrderCommand
        {
            get
            {
                if (_CreateOrderCommand == null)
                    _CreateOrderCommand = new RelayCommand(() => OnCreateOrderCommand());
                return _CreateOrderCommand;
            }
        }

        RelayCommand _ResetOrderCommand = null;
        public ICommand ResetOrderCommand
        {
            get
            {
                if (_ResetOrderCommand == null)
                    _ResetOrderCommand = new RelayCommand(() => OnResetOrderCommand());
                return _ResetOrderCommand;
            }
        }

        RelayCommand _CreateOrderDetailsCommand = null;
        public ICommand CreateOrderDetailsCommand
        {
            get
            {
                if (_CreateOrderDetailsCommand == null)
                    _CreateOrderDetailsCommand = new RelayCommand(() => OnCreateOrderDetailsCommandclick());
                return _CreateOrderDetailsCommand;
            }
        }

        RelayCommand _ResetOrderDetailsCommand = null;
        public ICommand ResetOrderDetailsCommand
        {
            get
            {
                if (_ResetOrderDetailsCommand == null)
                    _ResetOrderDetailsCommand = new RelayCommand(() => OnResetOrderDetailsCommand());
                return _ResetOrderDetailsCommand;
            }
        }

        #endregion iCommands


        #region Methods

        void OnLoad()
        {
            CustomerDetails = helper.GetAllCustomerDetails();
            JewelTypes = helper.GetAllLoadJewelTypes();

            OrderDetails = Helper.GenerateNewOrderDetailsInstance();
            OrderDetailsCollection = new ObservableCollection<OrderDetails>();
            EstimationDetails = new ObservableCollection<OrderDetails>();
            Order = Helper.GenerateNewOrderDetailsInstance();
            IsEnableOrderDetailsTab = false;
            if (userControlState == UserControlState.Create)
            {
                Order.orderNo = "ESTM_APS_" + helper.GetNextOrderRefNo();
                EstimationDetails.Add(Order);
                OrderButtonLabel = Convert.ToString(UserControlState.Create);
            }
            else if (userControlState == UserControlState.Update)
            {
                OrderButtonLabel = Convert.ToString(UserControlState.Update);
            }

            SelectedJewelType = null;
            OrderDetailsButtonLabel = Convert.ToString(UserControlState.Create);
        }

        void OnUpdateLoad()
        {
            OrderDetails = Helper.GenerateNewOrderDetailsInstance();
            Order = Helper.GenerateNewOrderDetailsInstance(); ;

            SelectedJewelType = null;
            OrderButtonLabel = Convert.ToString(UserControlState.Add);

            if (OrderDetailsCollection.Count == 0)
            {
                OnCustomerNameSelectionChangeCommand();
            }
        }


        #region Orders tab

        void OnCreateOrderCommand()
        {
            if (ValidateOrder())
            {
                if (OrderButtonLabel == Convert.ToString(UserControlState.Create))
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        try
                        {
                            InsertDataIntoOrdersTable(connection);
                            trans.Commit();
                            IsEnableOrderDetailsTab = true;
                            EstimationTabSelectedIndex = 1;
                            OrderButtonLabel = Convert.ToString(UserControlState.Update);
                            MessageBox.Show("New Estimation Order created successfully.", "Estimation",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Logger.LogError(ex);
                        }
                    }
                }
                else if (OrderButtonLabel == Convert.ToString(UserControlState.Update))
                {

                }
            }
        }

        void InsertDataIntoOrdersTable(NpgsqlConnection connection)
        {
            if (Order.id == 0)
            {
                string rateFreezeDate = Order.isRateFreeze ? Order.rateFreezeDate : null;
                string freezeRate = Order.isRateFreeze ? Order.freezeRate : null;

                string query_customer_order = string.Format("Insert into orders " +
                    "(order_no, fk_customer_id, fk_order_type, order_date," +
                    "is_rate_freeze, freeze_date, freeze_amount, is_priority_order, is_gst_order) " +
                    "values('{0}', {1}, {2}, '{3}'," +
                    "'{4}','{5}', '{6}','{7}', '{8}') RETURNING id",
                    DateTime.Now.ToString("yyyyMMddHHmmss"), SelectedCustomer.id, (int)EnumInfo.OrderType.Estimation, DateTime.Now.ToString("dd-MM-yyyy"),
                    Order.isRateFreeze, rateFreezeDate, freezeRate, Order.isPriorityOrder, Order.isGSTOrder);

                using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order, CommandType.Text))
                {
                    Order.id = Convert.ToInt64(cmd.ExecuteScalar());
                }

                if (Order.id == 0)
                {
                    throw new Exception("Order creation failed. close and open the tool.");
                }
            }
        }

        void UpdateDataIntoOrdersTable(NpgsqlConnection connection)
        {
            string rateFreezeDate = Order.isRateFreeze ? Order.rateFreezeDate : null;
            string freezeRate = Order.isRateFreeze ? Order.freezeRate : null;
            string orderDate = DateTime.Now.ToString("dd-MM-yyyy");

            string query_customer_order = string.Format("update orders set " +
                "fk_customer_id =  {0} , order_date     = '{1}', is_rate_freeze     = '{2}', " +
                "freeze_date    = '{3}', freeze_amount  = '{4}', is_priority_order  = '{5}', " +
                "is_gst_order   = '{6}'  where id = {7}",
                SelectedCustomer.id, orderDate, Order.isRateFreeze,
                rateFreezeDate, freezeRate, Order.isPriorityOrder,
                Order.isGSTOrder, Order.id);

            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order, CommandType.Text))
            {
                Order.id = generatedOrderId = Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        void OnResetOrderCommand()
        {
            OnLoad();
        }

        bool ValidateOrder()
        {
            StringBuilder errorControl = new StringBuilder();

            if (SelectedCustomer == null || SelectedCustomer.name.Trim().Length == 0)
            {
                errorControl.AppendLine("Select Customer Name");
                errorControl.AppendLine();
            }

            if (Order == null || Order.orderNo == null)
            {
                errorControl.AppendLine("Select Order Number");
                errorControl.AppendLine();
            }

            if (errorControl.Length != 0)
            {
                MessageBox.Show(errorControl.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        #endregion Orders tab





        void OnCustomerNameSelectionChangeCommand()
        {
            if (SelectedCustomer != null && userControlState == UserControlState.Update)
            {
                EstimationDetails = new ObservableCollection<OrderDetails>();
                string sqlQuery = string.Format("select * from orders where fk_customer_id = {0} and is_completed = false", SelectedCustomer.id);
                NpgsqlDataReader dataReader = helper.GetTableData(sqlQuery);
                try
                {
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            OrderDetails orderRefNo = new OrderDetails
                            {
                                id = Convert.ToInt32(dataReader["id"]),
                                orderNo = "ESTM_APS_" + Convert.ToInt32(dataReader["id"]),
                                isPriorityOrder = Convert.ToBoolean(dataReader["is_priority_order"]),
                                isGSTOrder = Convert.ToBoolean(dataReader["is_gst_order"]),
                                isRateFreeze = Convert.ToBoolean(dataReader["is_rate_freeze"]),
                                rateFreezeDate = Convert.ToString(dataReader["freeze_date"]),
                                freezeRate = Convert.ToString(dataReader["freeze_amount"]),
                                orderDate = Convert.ToString(dataReader["order_date"]),
                                customer = SelectedCustomer,
                            };
                            EstimationDetails.Add(orderRefNo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        void OnCustomerEstimationSelectionChangeCommand()
        {
            if (Order != null && userControlState == UserControlState.Update)
            {
                string sqlQuery = string.Format("SELECT * FROM order_details where fk_order_id = {0}", Order.id);
                NpgsqlDataReader dataReader = helper.GetTableData(sqlQuery);
                try
                {
                    if (dataReader != null)
                    {
                        OrderDetailsCollection = new ObservableCollection<OrderDetails>();
                        while (dataReader.Read())
                        {
                            OrderDetails orderDetails = new OrderDetails();
                            orderDetails.id = Convert.ToInt32(dataReader["id"]);
                            orderDetails.orderNo = Helper.GetGuid();
                            orderDetails.size = Convert.ToString(dataReader["size"]);
                            orderDetails.netWeight = Convert.ToString(dataReader["net_weight"]);
                            orderDetails.seal = Convert.ToString(dataReader["seal"]);
                            orderDetails.description = Convert.ToString(dataReader["description"]);
                            orderDetails.attachement = Convert.ToString(dataReader["attachement"]);
                            orderDetails.orderDate = Convert.ToString(dataReader["order_date"]);
                            orderDetails.dueDate = Convert.ToString(dataReader["due_date"]);
                            orderDetails.jewelPurity = Convert.ToString(dataReader["ornament_purity"]);
                            orderDetails.makingCharge = Convert.ToString(dataReader["making_charges"]);
                            orderDetails.wastage = Convert.ToString(dataReader["wastage"]);
                            orderDetails.chargesDetails = Helper.GetChargesControls("", Convert.ToInt32(dataReader["id"]));
                            orderDetails.detectionDetails = Helper.GetAllDetectionControls();
                            orderDetails.jewelType = JewelTypes.FirstOrDefault(o => o.id == Convert.ToInt64(dataReader["fk_ornament_type_id"]));
                            OrderDetailsCollection.Add(orderDetails);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        bool ValidateCustomerOrderDetailsCollection()
        {
            if (OrderDetailsCollection == null || OrderDetailsCollection.Count == 0)
            {
                MessageBox.Show("SelectedOrderDetails should not be empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        void OnGenerateInvoiceCommandclick()
        {
            if (ValidateCustomerOrderDetailsCollection())
            {
                try
                {
                    var reportStatus = OnGenerateExcelCommand();
                    if (reportStatus.status)
                    {
                        ConfirmationDialog confirmationDialog = new ConfirmationDialog(SelectedCustomer, reportStatus);
                        confirmationDialog.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Customer estimation report not generated", "Record", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        void InsertDataIntoOrderDetailsTable(NpgsqlConnection connection)
        {
            if (Order.id > 0)
            {
                string custOrderDetails = string.Format("('{0}','{1}','{2}','{3}','{4}','{5}'," +
                                                        "'{6}','{7}','{8}','{9}', '{10}', {11},{12})",
                            OrderDetails.size, OrderDetails.netWeight, OrderDetails.seal,
                            OrderDetails.description, OrderDetails.attachement, OrderDetails.orderDate,
                            OrderDetails.dueDate, OrderDetails.jewelPurity, OrderDetails.wastage,
                            OrderDetails.quantity, OrderDetails.makingCharge, SelectedJewelType.id, Order.id);

                string query_customer_order_details = "Insert into order_details" +
                    "(size, net_weight, seal, description, attachement, order_date, due_date, " +
                    "ornament_purity, wastage, quantity, making_charges, fk_ornament_type_id, fk_order_id) values" +
                    custOrderDetails + " RETURNING id";

                using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_details, CommandType.Text))
                {
                    OrderDetails.id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (OrderDetails.id == 0)
                {
                    throw new Exception("Order creation failed. close and open the tool.");
                }
            }
        }

        void InsertDataIntoChargesDetails(NpgsqlConnection connection)
        {
            if (OrderDetails.id > 0)
            {
                List<string> _chargesList = new List<string>();
                foreach (ChargesControl charges in OrderDetails.chargesDetails)
                {
                    if (!string.IsNullOrEmpty(charges.value))
                    {
                        _chargesList.Add(string.Format("({0},'{1}',{2})", charges.id, charges.value, OrderDetails.id));
                    }
                }

                if (_chargesList.Count > 0)
                {
                    string detectLst = string.Join<string>(",", _chargesList);
                    string query_customer_order_charges_details = "Insert into customer_order_charges_details" +
                        "(fk_charges_id, charge, fk_order_details_id) values " + detectLst;

                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_charges_details, CommandType.Text))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }



        void OnShowDetectionWindowCommand()
        {
            AddDetectionWindow addDetectionWindow = new AddDetectionWindow(this, OrderDetails.detectionDetails, OrderDetails.chargesDetails, isHideDetection: true);
            addDetectionWindow.ShowDialog();
            addDetectionWindow.Owner = System.Windows.Application.Current.MainWindow;
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

                string orderPath = Path.Combine(assetsPath, Convert.ToString(Order.id));
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

        bool ValidateOrderDetails()
        {
            StringBuilder errorControl = new StringBuilder();

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

            if (OrderDetails.isRateFreeze)
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

        void OnCreateOrderDetailsCommandclick()
        {
            if (ValidateOrderDetails())
            {
                if (OrderDetailsButtonLabel == Convert.ToString(UserControlState.Create))
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        try
                        {
                            InsertDataIntoOrderDetailsTable(connection);
                            InsertDataIntoChargesDetails(connection);
                            trans.Commit();

                            OrderDetails.jewelType = SelectedJewelType;
                            OrderDetailsCollection.Add(OrderDetails);
                            OrderDetails = Helper.GenerateNewOrderDetailsInstance();
                            SelectedJewelType = null;
                            MessageBox.Show("Record added successfully.", "Estimation",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Logger.LogError(ex);
                        }
                    }
                }
                else if (OrderDetailsButtonLabel == Convert.ToString(UserControlState.Update))
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        try
                        {
                            UpdateDataIntoOrderDetailsTable(connection);
                            DeleteDataIntoChargesDetails(connection);
                            InsertDataIntoChargesDetails(connection);
                            trans.Commit();

                            for (int i = OrderDetailsCollection.Count - 1; i >= 0; --i)
                            {
                                if (OrderDetailsCollection[i].subOrderNo == OrderDetails.subOrderNo)
                                {
                                    OrderDetailsCollection.RemoveAt(i);
                                    OrderDetailsCollection.Insert(i, OrderDetails);
                                    break;
                                }
                            }
                            OrderDetails = Helper.GenerateNewOrderDetailsInstance();
                            OrderDetailsButtonLabel = Convert.ToString(UserControlState.Create);
                            SelectedJewelType = null;
                            MessageBox.Show("Record updated successfully.", "Estimation",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Logger.LogError(ex);
                        }
                    }
                }
            }
        }

        void UpdateDataIntoOrderDetailsTable(NpgsqlConnection connection)
        {
            string query_customer_orderDetails = string.Format("update order_details set " +
                "size           = '{0}', net_weight         = '{1}', seal       = '{2}', " +
                "description    = '{3}', attachement        = '{4}', order_date = '{5}', " +
                "due_date       = '{6}', ornament_purity    = '{7}', wastage    = '{8}', " +
                "quantity       = '{9}', fk_ornament_type_id=  {10}, making_charges =  '{11}'" +
                " where id   =  {12}  ",
                OrderDetails.size, OrderDetails.netWeight, OrderDetails.seal,
                OrderDetails.description, OrderDetails.attachement, OrderDetails.orderDate,
                OrderDetails.dueDate, OrderDetails.jewelPurity, OrderDetails.wastage,
                OrderDetails.quantity, OrderDetails.jewelType.id, OrderDetails.makingCharge,
                OrderDetails.id);

            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_orderDetails, CommandType.Text))
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        void DeleteDataIntoChargesDetails(NpgsqlConnection connection)
        {
            string query_customer_order_charges_details = string.Format("delete from customer_order_charges_details where id = {0} ", OrderDetails.id);

            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_charges_details, CommandType.Text))
            {
                Convert.ToInt32(cmd.ExecuteScalar());
            }

        }

        void OnOrderDetailsEditCommand(object orderDetail)
        {
            OrderDetailsButtonLabel = Convert.ToString(UserControlState.Update);
            SelectedJewelType = (orderDetail as OrderDetails).jewelType;
            OrderDetails = CloneObject.DeepClone<OrderDetails>(orderDetail as OrderDetails);
        }

        void OnOrderDetailsDeleteCommand(object orderDetail)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this Order", "Delete Order", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (OrderDetails dgOrder in OrderDetailsCollection)
                {
                    if (dgOrder.orderNo == (orderDetail as OrderDetails).orderNo)
                    {                        
                        if (userControlState == UserControlState.Update)
                        {
                            using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                            {
                                NpgsqlTransaction trans = connection.BeginTransaction();
                                try
                                {
                                    string deleteOrderDetailQuery = string.Format("DELETE FROM order_details WHERE id = {0}", dgOrder.id);
                                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, deleteOrderDetailQuery, CommandType.Text))
                                    {
                                        cmd.ExecuteNonQuery();
                                    }

                                    List<string> deleteOrderChargeDetailIds = new List<string>();
                                    foreach (ChargesControl charges in dgOrder.chargesDetails)
                                    {
                                        deleteOrderChargeDetailIds.Add(Convert.ToString(charges.pkId));
                                    }
                                    string detectLst = string.Join<string>(",", deleteOrderChargeDetailIds);
                                    string deleteOrderChargeDetailQuery = string.Format("DELETE FROM customer_order_charges_details WHERE id IN ({0})", detectLst);
                                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, deleteOrderChargeDetailQuery, CommandType.Text))
                                    {
                                        cmd.ExecuteNonQuery();
                                    }

                                    trans.Commit();
                                    OrderDetailsButtonLabel = Convert.ToString(UserControlState.Create);
                                    OrderDetailsCollection.Remove(dgOrder);
                                    OrderDetails = Helper.GenerateNewOrderDetailsInstance();
                                    SelectedJewelType = null;
                                    MessageBox.Show("Record deleted successfully.", "Estimation",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    Logger.LogError(ex);
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        void OnShowSearchDetailsCommandclick()
        {
            AdvancedSearch advancedSearch = new AdvancedSearch(SearchTypes.Customer, this);
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

        void OnResetOrderDetailsCommand()
        {
            OrderButtonLabel = Convert.ToString(UserControlState.Create);
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
                Order.orderDetailsList = OrderDetailsCollection;
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


//GridColumnsVisibility = new GenerateEstimationGridColumns { attachement = false, description = true, dueDate = false, jewelPurity = true, jewelType = true, netWeight = true, seal = true, size = true };