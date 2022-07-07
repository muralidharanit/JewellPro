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
using WhatsAppApi;

namespace JewellPro
{
    public class CustomerDeliveryViewModel : ViewModelBase
    {
        Helper helper;
        public CustomerDeliveryViewModel()
        {
            helper = new Helper();
            OnLoad();
        }

        private void OnLoad()
        {
            BalanceGoldWeightPanelVisibility = Visibility.Collapsed;
            IsEnableTabItem = false;
            IsEnableSaveButton = false;
            LoadNextOrderRefNo();
            LoadCustomerDetails();
            LoadJewelType();
            LoadAdvanceType();
            OnTabItemSelectionChangeCommand();
            IsOrderCompleted = true;
            IsEnableGenerateOrder = true;           
        }

        #region Properties

        private string _AdditionalInfo;
        public string AdditionalInfo
        {
            get { return _AdditionalInfo; }
            set { _AdditionalInfo = value; RaisePropertyChanged("AdditionalInfo"); }
        }

        private string _OrderRefNo;
        public string OrderRefNo
        {
            get { return _OrderRefNo; }
            set { _OrderRefNo = value; RaisePropertyChanged("OrderRefNo"); }
        }

        private decimal _TotalAdvanceDetailsPureGoldWeight;
        public decimal TotalAdvanceDetailsPureGoldWeight
        {
            get { return _TotalAdvanceDetailsPureGoldWeight; }
            set { _TotalAdvanceDetailsPureGoldWeight = value; RaisePropertyChanged("TotalAdvanceDetailsPureGoldWeight"); }
        }

        private string _TotalOrderDetailsPureGoldWeight;
        public string TotalOrderDetailsPureGoldWeight
        {
            get { return _TotalOrderDetailsPureGoldWeight; }
            set { _TotalOrderDetailsPureGoldWeight = value; RaisePropertyChanged("TotalOrderDetailsPureGoldWeight"); }
        }

        private decimal _TotalConsolidatedWeight;
        public decimal TotalConsolidatedWeight
        {
            get { return _TotalConsolidatedWeight; }
            set { _TotalConsolidatedWeight = value; RaisePropertyChanged("TotalConsolidatedWeight"); }
        }

        private decimal _RawGoldWeight;
        public decimal RawGoldWeight
        {
            get { return _RawGoldWeight; }
            set { _RawGoldWeight = value; RaisePropertyChanged("RawGoldWeight"); }
        }

        private decimal _CurrentOrderConsolidatedWeight;
        public decimal CurrentOrderConsolidatedWeight
        {
            get { return _CurrentOrderConsolidatedWeight; }
            set { _CurrentOrderConsolidatedWeight = value; RaisePropertyChanged("CurrentOrderConsolidatedWeight"); }
        }

        private decimal _TotalCreaditDebitGoldWeight;
        public decimal TotalCreaditDebitGoldWeight
        {
            get { return _TotalCreaditDebitGoldWeight; }
            set { _TotalCreaditDebitGoldWeight = value; RaisePropertyChanged("TotalCreaditDebitGoldWeight"); }
        }
        

        private decimal _TotalCustomerPureGoldWeightDue;
        public decimal TotalCustomerPureGoldWeightDue
        {
            get { return _TotalCustomerPureGoldWeightDue; }
            set { _TotalCustomerPureGoldWeightDue = value; RaisePropertyChanged("TotalCustomerPureGoldWeightDue"); }
        }

        private bool _IsOrderCompleted;
        public bool IsOrderCompleted
        {
            get { return _IsOrderCompleted; }
            set { _IsOrderCompleted = value; RaisePropertyChanged("IsOrderCompleted"); }
        }

        private bool _IsOrderNotCompleted;
        public bool IsOrderNotCompleted
        {
            get { return _IsOrderNotCompleted; }
            set { _IsOrderNotCompleted = value; RaisePropertyChanged("IsOrderNotCompleted"); }
        }

        private Visibility _BalanceGoldWeightPanelVisibility;
        public Visibility BalanceGoldWeightPanelVisibility
        {
            get { return _BalanceGoldWeightPanelVisibility; }
            set { _BalanceGoldWeightPanelVisibility = value; RaisePropertyChanged("BalanceGoldWeightPanelVisibility"); }
        }

        private Visibility _IsSaveBtnlVisibility;
        public Visibility IsSaveBtnlVisibility
        {
            get { return _IsSaveBtnlVisibility; }
            set { _IsSaveBtnlVisibility = value; RaisePropertyChanged("IsSaveBtnlVisibility"); }
        }
        
        private bool _IsEnableTabItem;
        public bool IsEnableTabItem
        {
            get { return _IsEnableTabItem; }
            set { _IsEnableTabItem = value; RaisePropertyChanged("IsEnableTabItem"); }
        }

        private bool _IsEnableSaveButton;
        public bool IsEnableSaveButton
        {
            get { return _IsEnableSaveButton; }
            set { _IsEnableSaveButton = value; RaisePropertyChanged("IsEnableSaveButton"); }
        }
        

        private bool _IsEnableGenerateOrder;
        public bool IsEnableGenerateOrder
        {
            get { return _IsEnableGenerateOrder; }
            set { _IsEnableGenerateOrder = value; RaisePropertyChanged("IsEnableGenerateOrder"); }
        }

        private ObservableCollection<Customer> _customersDetails;
        public ObservableCollection<Customer> CustomerDetails
        {
            get { return _customersDetails; }
            set { _customersDetails = value; RaisePropertyChanged("CustomerDetails"); }
        }

        private Customer _SelectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set { _SelectedCustomer = value; RaisePropertyChanged("selectedCustomer"); }
        }

        private ObservableCollection<OrderDetails> _OrderRefNos;
        public ObservableCollection<OrderDetails> OrderRefNos
        {
            get { return _OrderRefNos; }
            set { _OrderRefNos = value; RaisePropertyChanged("OrderRefNos"); }
        }

        private OrderDetails _SelectedCustomerOrder;
        public OrderDetails SelectedCustomerOrder
        {
            get { return _SelectedCustomerOrder; }
            set 
            {
                _SelectedCustomerOrder = value;
                if(value != null)
                {
                    IsEnableTabItem = true;
                    IsEnableSaveButton = true;
                    IsSaveBtnlVisibility = Visibility.Visible;
                }
                RaisePropertyChanged("selectedCustomerOrder"); 
            }
        }

        private Visibility _IsGenerateCustomerDeliveryBtnlVisibility;
        public Visibility IsGenerateCustomerDeliveryBtnlVisibility
        {
            get { return _IsGenerateCustomerDeliveryBtnlVisibility; }
            set
            {
                _IsGenerateCustomerDeliveryBtnlVisibility = value;
                RaisePropertyChanged("IsGenerateCustomerDeliveryBtnlVisibility");
            }
        }        

        private ObservableCollection<OrderDetails> _OrderDetails;
        public ObservableCollection<OrderDetails> OrderDetails
        {
            get { return _OrderDetails; }
            set { _OrderDetails = value; RaisePropertyChanged("OrderDetails"); }
        }

        private ObservableCollection<JewelType> _jewelTypes;
        public ObservableCollection<JewelType> JewelTypes
        {
            get { return _jewelTypes; }
            set { _jewelTypes = value; RaisePropertyChanged("JewelTypes"); }
        }

        private ObservableCollection<AdvanceType> _advanceTypes;
        public ObservableCollection<AdvanceType> AdvanceTypes
        {
            get { return _advanceTypes; }
            set { _advanceTypes = value; RaisePropertyChanged("AdvanceTypes"); }
        }
        

        private OrderDetails _selectedOrderDetails;
        public OrderDetails selectedOrderDetails
        {
            get { return _selectedOrderDetails; }
            set
            {
                _selectedOrderDetails = value;
                RaisePropertyChanged("selectedOrderDetails");
            }
        }

        private ObservableCollection<AdvanceDetails> _advanceDetailsCollection;
        public ObservableCollection<AdvanceDetails> AdvanceDetailsCollection
        {
            get { return _advanceDetailsCollection; }
            set { _advanceDetailsCollection = value; RaisePropertyChanged("AdvanceDetailsCollection"); }
        }

        private ObservableCollection<CreditDebitDetails> _creditDebitDetailsCollection;
        public ObservableCollection<CreditDebitDetails> CreditDebitDetailsCollection
        {
            get { return _creditDebitDetailsCollection; }
            set { _creditDebitDetailsCollection = value; RaisePropertyChanged("CreditDebitDetailsCollection"); }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                RaisePropertyChanged(() => SelectedTabIndex);
            }
        }

        #endregion Properties


        #region iCommands

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

        RelayCommand _CustomerOrderSelectionChangeCommand = null;
        public ICommand CustomerOrderSelectionChangeCommand
        {
            get
            {
                if (_CustomerOrderSelectionChangeCommand == null)
                    _CustomerOrderSelectionChangeCommand = new RelayCommand(() => OnCustomerOrderSelectionChangeCommand());
                return _CustomerOrderSelectionChangeCommand;
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

        RelayCommand _CalculateOrderDetailsCommand = null;
        public ICommand CalculateOrderDetailsCommand
        {
            get
            {
                if (_CalculateOrderDetailsCommand == null)
                    _CalculateOrderDetailsCommand = new RelayCommand(() => OnCalculateOrderDetailsCommand());
                return _CalculateOrderDetailsCommand;
            }
        }

        RelayCommand _GenerateCustomerDeliveryInvoiceCommand = null;
        public ICommand GenerateCustomerDeliveryInvoiceCommand
        {
            get
            {
                if (_GenerateCustomerDeliveryInvoiceCommand == null)
                    _GenerateCustomerDeliveryInvoiceCommand = new RelayCommand(() => OnGenerateCustomerDeliveryInvoiceCommand());
                return _GenerateCustomerDeliveryInvoiceCommand;
            }
        }

        RelayCommand _SaveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new RelayCommand(() => OnSaveCommand());
                return _SaveCommand;
            }
        }
        
        RelayCommand _TabItemSelectionChangeCommand = null;
        public ICommand TabItemSelectionChangeCommand
        {
            get
            {
                if (_TabItemSelectionChangeCommand == null)
                    _TabItemSelectionChangeCommand = new RelayCommand(() => OnTabItemSelectionChangeCommand());
                return _TabItemSelectionChangeCommand;
            }
        }

        RelayCommand<string> _OrderCompletedCommand = null;
        public ICommand OrderCompletedCommand
        {
            get
            {
                if (_OrderCompletedCommand == null)
                    _OrderCompletedCommand = new RelayCommand<string>((obj) => OnOrderCompletedCommand(obj));
                return _OrderCompletedCommand;
            }
        }

        RelayCommand _UpdateCreditDebitCommand = null;
        public ICommand UpdateCreditDebitCommand
        {
            get
            {
                if (_UpdateCreditDebitCommand == null)
                    _UpdateCreditDebitCommand = new RelayCommand(() => OnUpdateCreditDebitCommand());
                return _UpdateCreditDebitCommand;
            }
        }

        #endregion iCommands


        #region Methods

        void LoadNextOrderRefNo()
        {
            OrderRefNo = string.Concat("CUS_DEL_", helper.GetNextSerialValue("customer_delivery") + 1);
        }

        void LoadCustomerDetails()
        {
            CustomerDetails = helper.GetAllCustomerDetails();            
        }

        void LoadJewelType()
        {
            JewelTypes = helper.GetAllLoadJewelTypes();            
        }

        void LoadAdvanceType()
        {
            AdvanceTypes = helper.GetAllAdvanceTypes();            
        }

        void OnCustomerNameSelectionChangeCommand()
        {
            string sqlQuery = string.Format("select * from customer_order where fk_customer_id = {0} and is_completed = false", SelectedCustomer.id);
            NpgsqlDataReader dataReader = helper.GetTableData(sqlQuery);
            try
            {
                if (dataReader != null)
                {
                    OrderRefNos = new ObservableCollection<OrderDetails>();
                    while (dataReader.Read())
                    {
                        OrderDetails order = new OrderDetails
                        {
                            id = Convert.ToInt32(dataReader["id"]),
                            orderNo = Convert.ToString(dataReader["order_no"]),
                            orderRefNo = Convert.ToString(dataReader["order_ref_no"]),
                            
                        };
                        OrderRefNos.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        
        void OnCustomerOrderSelectionChangeCommand()
        {
            if(SelectedCustomerOrder != null)
            {
                string sqlOrderDetailsQuery = string.Format("select * from customer_order_details where fk_customer_order_id = {0} ", SelectedCustomerOrder.id);
                NpgsqlDataReader dataReader = helper.GetTableData(sqlOrderDetailsQuery);
                try
                {
                    if (dataReader != null)
                    {
                        OrderDetails = new ObservableCollection<OrderDetails>();
                        while (dataReader.Read())
                        {
                            OrderDetails order = new OrderDetails
                            {
                                id = Convert.ToInt32(dataReader["id"]),
                                size = Convert.ToString(dataReader["size"]),
                                netWeight = Convert.ToString(dataReader["net_weight"]),
                                seal = Convert.ToString(dataReader["seal"]),
                                description = Convert.ToString(dataReader["description"]),
                                attachement = Convert.ToString(dataReader["attachement"]),
                                orderDate = Convert.ToString(dataReader["order_date"]),
                                dueDate = Convert.ToString(dataReader["due_date"]),
                                jewelPurity = Convert.ToString(dataReader["ornament_purity"]),
                                jewellRecivedWeight = Convert.ToString(dataReader["recived_weight"]),
                                wastagePercentage = Convert.ToString(dataReader["wastage"]),
                                detectionDetails = helper.GetDetectionControls("", Convert.ToInt32(dataReader["id"])),
                                chargesDetails = helper.GetChargesControls("", Convert.ToInt32(dataReader["id"])),
                                jewelType = JewelTypes.FirstOrDefault(o => o.id == Convert.ToInt64(dataReader["fk_ornament_type_id"]))
                            };
                            //Dont change the order
                            order.detection = Helper.GetDetectionTotal(order);
                            order.grandTotal = Helper.GetTotal(order);
                            order.pureGoldWeight = Convert.ToString(Math.Round((Convert.ToDecimal(order.grandTotal) * Convert.ToDecimal(order.jewelPurity)) / 100, 3));
                            OrderDetails.Add(order);
                        }
                        TotalOrderDetailsPureGoldWeight = Helper.GetGrandTotal(OrderDetails);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }

                string sqlAdvanceDetailsQuery = string.Format("select * from customer_order_advance_details where fk_customer_order_id = {0} ", SelectedCustomerOrder.id);
                NpgsqlDataReader dataReader1 = helper.GetTableData(sqlAdvanceDetailsQuery);
                try
                {
                    if (dataReader1 != null)
                    {
                        AdvanceDetailsCollection = new ObservableCollection<AdvanceDetails>();
                        while (dataReader1.Read())
                        {
                            AdvanceDetails advance = new AdvanceDetails
                            {
                                id = Convert.ToInt32(dataReader1["id"]),
                                goldWeight = Convert.ToDecimal(dataReader1["gold_weight"]),
                                goldPurity = Convert.ToDecimal(dataReader1["gold_purity"]),
                                pureGoldWeight = Convert.ToString(Math.Round(Convert.ToDecimal(dataReader1["gold_weight"]) * Convert.ToDecimal(dataReader1["gold_purity"]) / 100, 3))
                            };

                            foreach (AdvanceType advanceType in AdvanceTypes)
                            {
                                if (advanceType.id == Convert.ToInt16(dataReader1["fk_advance_type_id"]))
                                {
                                    advance.advanceType = advanceType;
                                    break;
                                }
                            }
                            AdvanceDetailsCollection.Add(advance);
                        }
                        TotalAdvanceDetailsPureGoldWeight = Convert.ToDecimal(Helper.GetGrandTotal(AdvanceDetailsCollection));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }

                string sqlCreditDebitDetailsQuery = string.Format("select * from customer_credit_debit_details where fk_customer_order_id = {0} ", SelectedCustomerOrder.id);
                NpgsqlDataReader dataReader2 = helper.GetTableData(sqlCreditDebitDetailsQuery);
                try
                {
                    if (dataReader2 != null)
                    {
                        while (dataReader2.Read())
                        {
                            AdditionalInfo = Convert.ToString(dataReader2["description"]);

                            if(RawGoldWeight == CurrentOrderConsolidatedWeight)
                            {
                                IsOrderCompleted = true;
                            }
                            else
                            {
                                IsOrderCompleted = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }

                string sqlCreditDebitDetailsForCustomerQuery = string.Format("select * from customer_credit_debit_details where fk_customer_id = {0}", SelectedCustomer.id);
                NpgsqlDataReader dataReader3 = helper.GetTableData(sqlCreditDebitDetailsForCustomerQuery);
                try
                {
                    if (dataReader3 != null)
                    {
                        decimal temp = 0;
                        CreditDebitDetailsCollection = new ObservableCollection<CreditDebitDetails>();
                        while (dataReader3.Read())
                        {
                            CreditDebitDetails advance = new CreditDebitDetails
                            {
                                id = Convert.ToInt32(dataReader3["id"]),
                                goldWeight = Convert.ToString(dataReader3["gold_weight"]),
                                description = Convert.ToString(dataReader3["description"]),
                                orderId = Convert.ToInt32(dataReader3["fk_customer_order_id"]),
                            };
                            temp = temp + Convert.ToDecimal(advance.goldWeight);
                            CreditDebitDetailsCollection.Add(advance);
                        }
                        TotalCreaditDebitGoldWeight = temp;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }

                ObservableCollection<OrderDetails>  tempOrderCollection = new ObservableCollection<OrderDetails>();
                string query_customer_order = string.Format("select * from customer_order where fk_customer_id = {0} ", SelectedCustomer.id);
                NpgsqlDataReader dataReader4 = helper.GetTableData(query_customer_order);
                try
                {
                    if (dataReader4 != null)
                    {
                        while (dataReader4.Read())
                        {
                            OrderDetails order = new OrderDetails
                            {
                                id = Convert.ToInt32(dataReader4["id"]),
                                orderNo = Convert.ToString(dataReader4["order_no"]),
                                orderRefNo = Convert.ToString(dataReader4["order_ref_no"]),

                            };
                            tempOrderCollection.Add(order);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }


                foreach (OrderDetails order in tempOrderCollection)
                {
                    foreach (CreditDebitDetails credit in CreditDebitDetailsCollection)
                    {
                        if(order.id == credit.orderId)
                        {
                            credit.orderRefNo = order.orderRefNo;
                            break;
                        }
                    }
                }
                //CreditDebitDetailsCollection = new ObservableCollection<CreditDebitDetails>(CreditDebitDetailsCollection.OrderBy(x => x.orderRefNo));  
            }
        }

        void OnShowDetectionWindowCommand()
        {
            AddDetectionWindow addDetectionWindow = new AddDetectionWindow(this, selectedOrderDetails.detectionDetails, selectedOrderDetails.chargesDetails);
            addDetectionWindow.ShowDialog();
            addDetectionWindow.Owner = System.Windows.Application.Current.MainWindow;
        }  

        public void UpdateDetectionDetails(ObservableCollection<DetectionControl> detectionControls, ObservableCollection<ChargesControl> ChargesControls)
        {
            decimal total = 0.0M;
            foreach (DetectionControl ctrl in detectionControls)
            {
                if (Decimal.TryParse(ctrl.value, out decimal val))
                {
                    total = total + Convert.ToDecimal(val);
                }                        
            }
            selectedOrderDetails.detection = Convert.ToString(total);
            OnCalculateOrderDetailsCommand();
        }

        

        void OnCalculateOrderDetailsCommand()
        {
            if (string.IsNullOrEmpty(selectedOrderDetails.jewellRecivedWeight))
            {
                MessageBox.Show("Jewellery Weight should be greater then 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(selectedOrderDetails.wastagePercentage))
            {
                MessageBox.Show("Wastage Weight should be greater then 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            selectedOrderDetails.grandTotal = Helper.GetTotal(selectedOrderDetails);
            selectedOrderDetails.pureGoldWeight = Helper.ConvertToPureGold(selectedOrderDetails);

            updateGrandTotalForOrderDetails();
        }
        
        void updateGrandTotalForOrderDetails()
        {
            TotalOrderDetailsPureGoldWeight = Helper.GetGrandTotal(OrderDetails);
        }

        void OnSaveCommand()
        {
            if (ValidateCustomerDelivery())
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    NpgsqlTransaction trans = connection.BeginTransaction();
                    try
                    {
                        foreach (OrderDetails obj in OrderDetails)
                        {
                            string query_customer_order_details = string.Format("UPDATE customer_order_details SET wastage = '{0}', recived_weight = '{1}' WHERE id ={2}", obj.wastagePercentage, obj.jewellRecivedWeight, obj.id);
                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_details, CommandType.Text))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            bool isDetectionExist = false;
                            string query_select_detection = string.Format("select * from customer_order_detection_details where fk_customer_order_details_id = {0}", obj.id);
                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_select_detection, CommandType.Text))
                            {
                                NpgsqlDataReader dataReader = cmd.ExecuteReader();

                                if (dataReader != null)
                                {
                                    isDetectionExist = dataReader.HasRows;
                                }
                            }

                            //No Entry found in Detection table
                            List<string> _detectionList = new List<string>();
                            if (!isDetectionExist)
                            {
                                foreach (DetectionControl detection in obj.detectionDetails)
                                {
                                    if (!string.IsNullOrEmpty(detection.value))
                                    {
                                        _detectionList.Add(string.Format("({0},'{1}',{2})", detection.id, detection.value, obj.id));
                                    }
                                }

                                if (_detectionList.Count > 0)
                                {
                                    string detectLst = string.Join<string>(",", _detectionList);
                                    string query_customer_order_detection_details = "Insert into customer_order_detection_details(fk_detection_id, detection, fk_customer_order_details_id) values " + detectLst;

                                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_detection_details, CommandType.Text))
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            else
                            {
                                foreach (DetectionControl detection in obj.detectionDetails)
                                {
                                    if (detection.pkId > 0 )
                                    {
                                        if(string.IsNullOrEmpty(detection.value))
                                        {
                                            string query_delete_customer_order_detection_details = string.Format("delete from customer_order_detection_details where id = {0}", detection.pkId.ToString());
                                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_delete_customer_order_detection_details, CommandType.Text))
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            string query_update_customer_order_detection_details = string.Format("update customer_order_detection_details set detection = '{0}' where id = {1} ", detection.value, detection.pkId.ToString());
                                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_update_customer_order_detection_details, CommandType.Text))
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                        }                                    
                                    }

                                    if (detection.pkId == 0 && !string.IsNullOrEmpty(detection.value))
                                    {
                                        string query_insert_customer_order_detection_details = string.Format("Insert into customer_order_detection_details(fk_detection_id, detection, fk_customer_order_details_id) values ({0},'{1}',{2})" , detection.id, detection.value, obj.id);

                                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_insert_customer_order_detection_details, CommandType.Text))
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            bool isChargesExist = false;
                            string query_select_charges = string.Format("select * from customer_order_charges_details where fk_customer_order_details_id = {0}", obj.id);
                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_select_charges, CommandType.Text))
                            {
                                NpgsqlDataReader dataReader = cmd.ExecuteReader();

                                if (dataReader != null)
                                {
                                    isChargesExist = dataReader.HasRows;
                                }
                            }

                            if (!isChargesExist)
                            {
                                List<string> _chargesList = new List<string>();
                                foreach (ChargesControl charge in obj.chargesDetails)
                                {
                                    if (!string.IsNullOrEmpty(charge.value))
                                    {
                                        _chargesList.Add(string.Format("({0},'{1}',{2})", charge.id, charge.value, obj.id));
                                    }
                                }

                                if (_chargesList.Count > 0)
                                {
                                    string chargLst = string.Join<string>(",", _chargesList);
                                    string query_customer_order_charges_details = "Insert into customer_order_charges_details(fk_charges_id, charge, fk_customer_order_details_id) values " + chargLst;

                                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_charges_details, CommandType.Text))
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            else
                            {
                                foreach (ChargesControl charge in obj.chargesDetails)
                                {
                                    if (charge.pkId > 0)
                                    {
                                        if (string.IsNullOrEmpty(charge.value))
                                        {
                                            string query_delete_customer_order_charges_details = string.Format("delete from customer_order_charges_details where id = {0}", charge.pkId.ToString());
                                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_delete_customer_order_charges_details, CommandType.Text))
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            string query_update_customer_order_charges_details = string.Format("update customer_order_charges_details set charge = '{0}' where id = {1} ", charge.value, charge.pkId.ToString());
                                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_update_customer_order_charges_details, CommandType.Text))
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }

                                    if (charge.pkId == 0 && !string.IsNullOrEmpty(charge.value))
                                    {
                                        string query_insert_customer_order_charges_details = string.Format("Insert into customer_order_charges_details(fk_charges_id, charge, fk_customer_order_details_id) values ({0},'{1}',{2})", charge.id, charge.value, obj.id);

                                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_insert_customer_order_charges_details, CommandType.Text))
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        trans.Commit();
                        MessageBox.Show("Customer order saved successfully", "Record", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.LogError(ex.Message);
                    }
                }
            }
        }

        void OnGenerateCustomerDeliveryInvoiceCommand()
        {
            if (ValidateCustomerDelivery())
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    NpgsqlTransaction trans = connection.BeginTransaction();
                    try
                    {
                        string gold_weight = string.Empty;
                        if (!IsOrderCompleted)
                        {
                            if(CurrentOrderConsolidatedWeight < 0 && (CurrentOrderConsolidatedWeight + RawGoldWeight) < 0)
                            {
                                gold_weight = Convert.ToString(CurrentOrderConsolidatedWeight + RawGoldWeight);
                            }
                            else if(CurrentOrderConsolidatedWeight < 0 && (CurrentOrderConsolidatedWeight + RawGoldWeight) > 0)
                            {
                                gold_weight = "+" + Convert.ToString(CurrentOrderConsolidatedWeight + RawGoldWeight) + "";
                            }
                            else if (CurrentOrderConsolidatedWeight < 0 && (CurrentOrderConsolidatedWeight + RawGoldWeight) == 0)
                            {
                                gold_weight = "0.0";
                            }
                            //Case 1 
                            else if(CurrentOrderConsolidatedWeight > RawGoldWeight)
                            {
                                gold_weight = "-" + Convert.ToString(CurrentOrderConsolidatedWeight - RawGoldWeight);
                            }
                            //Case 2
                            else if(RawGoldWeight > CurrentOrderConsolidatedWeight)
                            {
                                gold_weight = "+" + Convert.ToString(RawGoldWeight - CurrentOrderConsolidatedWeight);
                            }
                            //Case 3
                            else if (RawGoldWeight == CurrentOrderConsolidatedWeight)
                            {
                                gold_weight = "0.0";
                            }
                        }
                        else
                        {
                            gold_weight = "0.0";
                        }


                        gold_weight = Convert.ToString(Math.Round(Convert.ToDecimal(gold_weight), 3));

                        string query_customer_credit_debit_details = string.Format("Insert into customer_credit_debit_details(gold_weight, description, fk_customer_id, fk_customer_order_id) values ('{0}','{1}',{2},{3})", gold_weight, AdditionalInfo, SelectedCustomer.id, SelectedCustomerOrder.id);
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_credit_debit_details, CommandType.Text))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        string query_customer_order_completed = "UPDATE customer_order set is_completed = true where id = " + SelectedCustomerOrder.id;
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_completed, CommandType.Text))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();

                        MessageBoxResult messageBoxResult = MessageBox.Show("Customer delivery order successfully created.\n Do you want share the Invoice in PDF fromat click Yes", "Customer delivery order successfully created", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (MessageBoxResult.Yes == messageBoxResult)
                        {
                            OnGeneratePrintCommand();
                        }
                        //IsSaveBtnlVisibility 
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.LogError(ex.Message);
                    }
                }
            }
        }

        bool ValidateCustomerDelivery()
        {
            StringBuilder errorControl = new StringBuilder();
            
            if (errorControl.Length != 0)
            {
                MessageBox.Show(errorControl.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }
        
        void OnTabItemSelectionChangeCommand()
        {
            if (SelectedTabIndex == 0)
            {
                IsSaveBtnlVisibility = Visibility.Visible;
                IsGenerateCustomerDeliveryBtnlVisibility = Visibility.Collapsed;
            }
            else if (SelectedTabIndex == 2)
            {
                IsSaveBtnlVisibility = Visibility.Collapsed;
                IsGenerateCustomerDeliveryBtnlVisibility = Visibility.Visible;
            }
            else
            {
                IsSaveBtnlVisibility = Visibility.Collapsed;
                IsGenerateCustomerDeliveryBtnlVisibility = Visibility.Collapsed;
            }
            CurrentOrderConsolidatedWeight = (Convert.ToDecimal(TotalAdvanceDetailsPureGoldWeight) - Convert.ToDecimal(TotalOrderDetailsPureGoldWeight));
            TotalConsolidatedWeight = CurrentOrderConsolidatedWeight + (TotalCreaditDebitGoldWeight);
        }

        void OnOrderCompletedCommand(string orderStatus)
        {
            BalanceGoldWeightPanelVisibility = Visibility.Visible;
            if (IsOrderCompleted)
            {
                RawGoldWeight = 0;
                BalanceGoldWeightPanelVisibility = Visibility.Collapsed;
                //RawGoldWeight = CurrentOrderConsolidatedWeight;
            }
        }

        void OnUpdateCreditDebitCommand()
        {
            TotalCreaditDebitGoldWeight = TotalCreaditDebitGoldWeight + RawGoldWeight;
        }

        void OnGeneratePrintCommand()
        {
            try
            {                
                ExcelFileArgs excelFileArgs = new ExcelFileArgs { orderDetails = OrderDetails, selectedCustomer = SelectedCustomer, selectedCustomerOrder = SelectedCustomerOrder, advanceDetails = AdvanceDetailsCollection };
                ExcelGenerator excelGenerator = new ExcelGenerator();
                excelGenerator.GenerateCustomerOrderDeliveryInvoice(excelFileArgs);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        #endregion Methods

    }
}
