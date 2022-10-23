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
    public class CustomerOrderViewModel : ViewModelBase
    {
        #region Properties
        private Helper helper;

        private string _OrderRefNo;
        public string OrderRefNo
        {
            get { return _OrderRefNo; }
            set { _OrderRefNo = value; RaisePropertyChanged("OrderRefNo"); }
        }

        private string _OrderButtonLabel;
        public string OrderButtonLabel
        {
            get { return _OrderButtonLabel; }
            set { _OrderButtonLabel = value; RaisePropertyChanged("OrderButtonLabel"); }
        }

        private string _AdvanceButtonLabel;
        public string AdvanceButtonLabel
        {
            get { return _AdvanceButtonLabel; }
            set { _AdvanceButtonLabel = value; RaisePropertyChanged("AdvanceButtonLabel");}
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
            set
            { 
                _orderDetailsCollection = value; 
                RaisePropertyChanged("OrderDetailsCollection");
            }
        }

        private ObservableCollection<JewelType> _jewelTypes;
        public ObservableCollection<JewelType> JewelTypes
        {
            get { return _jewelTypes; }
            set { _jewelTypes = value; RaisePropertyChanged("JewelTypes"); }
        }

        private ObservableCollection<AdvanceType> _AdvanceTypesCollection;
        public ObservableCollection<AdvanceType> AdvanceTypesCollection
        {
            get { return _AdvanceTypesCollection; }
            set { _AdvanceTypesCollection = value; RaisePropertyChanged("AdvanceTypesCollection"); }
        }

        private Customer _SelectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set { _SelectedCustomer = value; RaisePropertyChanged("selectedCustomer"); }
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

        private AdvanceType _SelectedAdvanceType;
        public AdvanceType SelectedAdvanceType
        {
            get { return _SelectedAdvanceType; }
            set { _SelectedAdvanceType = value; RaisePropertyChanged("SelectedAdvanceType"); }
        }

        private AdvanceDetails _AdvanceDetail;
        public AdvanceDetails AdvanceDetail
        {
            get { return _AdvanceDetail; }
            set { _AdvanceDetail = value; RaisePropertyChanged("AdvanceDetail");}
        }

        private ObservableCollection<AdvanceDetails> _advanceDetailsCollection;
        public ObservableCollection<AdvanceDetails> AdvanceDetailsCollection
        {
            get { return _advanceDetailsCollection; }
            set { _advanceDetailsCollection = value; RaisePropertyChanged("AdvanceDetailsCollection"); }
        }

        private string _TotalGoldWeight;
        public string TotalGoldWeight
        {
            get { return _TotalGoldWeight; }
            set { _TotalGoldWeight = value; RaisePropertyChanged("TotalGoldWeight"); }
        }

        private bool _isAdvanceButtonEnabled;
        public bool IsAdvanceButtonEnabled
        {
            get { return _isAdvanceButtonEnabled; }
            set { _isAdvanceButtonEnabled = value; RaisePropertyChanged("IsAdvanceButtonEnabled"); }
        }

        private bool _isGenerateInvoiceEnabled;
        public bool IsGenerateInvoiceEnabled
        {
            get { return _isGenerateInvoiceEnabled; }
            set { _isGenerateInvoiceEnabled = value; RaisePropertyChanged("IsGenerateInvoiceEnabled"); }
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

        #endregion Properties

        public CustomerOrderViewModel()
        {
            helper = new Helper();
            OnLoad();
        }

        #region iCommands

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

        RelayCommand _AddAdvanceCommand = null;
        public ICommand AddAdvanceCommand
        {
            get
            {
                if (_AddAdvanceCommand == null)
                    _AddAdvanceCommand = new RelayCommand(() => OnAddAdvanceCommandclick());
                return _AddAdvanceCommand;
            }
        }

        RelayCommand<object> _AdvanceDetailsEditCommand = null;
        public ICommand AdvanceDetailsEditCommand
        {
            get
            {
                if (_AdvanceDetailsEditCommand == null)
                    _AdvanceDetailsEditCommand = new RelayCommand<object>((obj) => OnAdvanceDetailsEditCommand(obj));
                return _AdvanceDetailsEditCommand;
            }
        }

        RelayCommand<object> _AdvanceDetailsDeleteCommand = null;
        public ICommand AdvanceDetailsDeleteCommand
        {
            get
            {
                if (_AdvanceDetailsDeleteCommand == null)
                    _AdvanceDetailsDeleteCommand = new RelayCommand<object>((obj) => OnAdvanceDetailsDeleteCommand(obj));
                return _AdvanceDetailsDeleteCommand;
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

        RelayCommand _AdvanceTabResetCommand = null;
        public ICommand AdvanceTabResetCommand
        {
            get
            {
                if (_AdvanceTabResetCommand == null)
                    _AdvanceTabResetCommand = new RelayCommand(() => OnAdvanceTabResetCommandclick());
                return _AdvanceTabResetCommand;
            }
        }

        RelayCommand _AdvanceTypeSelectionChangeCommand = null;
        public ICommand AdvanceTypeSelectionChangeCommand
        {
            get
            {
                if (_AdvanceTypeSelectionChangeCommand == null)
                    _AdvanceTypeSelectionChangeCommand = new RelayCommand(() => OnAdvanceTypeSelectionChangeCommandclick());
                return _AdvanceTypeSelectionChangeCommand;
            }
        }

        #endregion Properties

        #region Methods

        private void OnLoad()
        {
            //Load Master data
            CustomerDetails = helper.GetAllCustomerDetails();
            JewelTypes = helper.GetAllLoadJewelTypes();
            OrderRefNo = helper.GetNextOrderRefNo(OrderType.Customer);
            AdvanceTypesCollection = helper.GetAllAdvanceTypes();
            Puritys = helper.GetAllPurityDetails();
            
            TotalGoldWeight = string.Empty;
            IsGenerateInvoiceEnabled = false;

            OrderDetails = new OrderDetails();
            OrderDetails.orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            OrderDetails.orderDate = DateTime.Now.ToString();
            OrderDetailsCollection = new ObservableCollection<OrderDetails>();
            
            AdvanceDetail = new AdvanceDetails();
            AdvanceDetail.advanceDate = DateTime.Now.ToString();

            AdvanceDetailsCollection = new ObservableCollection<AdvanceDetails>();

            for (int i = 0; i <= AdvanceTypesCollection.Count - 1; i++)
            {
                if (AdvanceTypesCollection[i].name.Equals("none", StringComparison.OrdinalIgnoreCase))
                {
                    SelectedAdvanceType = AdvanceTypesCollection[i];
                    AdvanceDetail.advanceType = AdvanceTypesCollection[i];
                    break;
                }
            }

            OrderButtonLabel = Convert.ToString(UserControlState.Add);
            AdvanceButtonLabel = Convert.ToString(UserControlState.Add);

            OnAdvanceTypeSelectionChangeCommandclick();
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

        void OnAdvanceTabResetCommandclick()
        {
            AdvanceButtonLabel = Convert.ToString(UserControlState.Add);
            SelectedAdvanceType = null;
            AdvanceDetail = new AdvanceDetails();
        }

        private void OnAddAttachementCommandclick()
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

                string orderPath = Path.Combine(assetsPath, OrderRefNo);
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


            //ValidatePurity
            if (string.IsNullOrWhiteSpace(Helper.GetPurity(SelectedPurity, OrderDetails)))
            {
                errorControl.AppendLine("Enter Jewel Purity");
                errorControl.AppendLine();
            }

            
            if (string.IsNullOrEmpty(OrderDetails.netWeight))
            {
                errorControl.AppendLine("Enter Jewellery Net Weight");
                errorControl.AppendLine();
            }

            if (errorControl.Length != 0)
            {
                MessageBox.Show(errorControl.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        bool ValidateAdvanceDetails()
        {
            StringBuilder errorControl = new StringBuilder();

            if (SelectedAdvanceType == null || SelectedAdvanceType.name.Trim().Length == 0)
            {
                errorControl.AppendLine("Advance Info not selected");
                errorControl.AppendLine();
            }

            if (AdvanceDetail == null || AdvanceDetail.goldWeight <= 0)
            {
                errorControl.AppendLine("Gold Weight Empty");
                errorControl.AppendLine();
            }

            if (AdvanceDetail.goldPurity <= 0)
            {
                errorControl.AppendLine("Gold Purity Empty");
                errorControl.AppendLine();
            }

            if (errorControl.Length != 0)
            {
                MessageBox.Show(errorControl.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        bool ValidateIfAdvanceRequired()
        {
            if (AdvanceDetailsCollection == null || AdvanceDetailsCollection.Count == 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Advance Info Empty or Not Given.\nDo you want generate order with out advance?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
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

        void OnAddOrderCommandclick()
        {
            if (ValidateCustomerOrder())
            {
                OrderDetails.customer = SelectedCustomer;
                OrderDetails.jewelType = SelectedJewelType;
                OrderDetails.jewelPurity = Helper.GetPurity(SelectedPurity, OrderDetails);
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
                        OrderDetails = new OrderDetails();
                        OrderDetails.orderDate = DateTime.Now.ToString();
                        OrderButtonLabel = Convert.ToString(UserControlState.Add);
                        break;
                    }
                }
            }
        }

        void OnAddAdvanceCommandclick()
        {
            if (ValidateAdvanceDetails())
            {
                AdvanceDetail.advanceType = SelectedAdvanceType;
                AdvanceDetail.pureGoldWeight = Convert.ToString(Math.Round((AdvanceDetail.goldWeight * AdvanceDetail.goldPurity) / 100, 3));
                if (AdvanceButtonLabel == Convert.ToString(UserControlState.Add))
                {
                    AdvanceDetail.id = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    AdvanceDetailsCollection.Add(AdvanceDetail);
                }
                else if (AdvanceButtonLabel == Convert.ToString(UserControlState.Update))
                {
                    for (int i = AdvanceDetailsCollection.Count - 1; i >= 0; --i)
                    {
                        if (AdvanceDetailsCollection[i].id == AdvanceDetail.id)
                        {
                            AdvanceButtonLabel = Convert.ToString(UserControlState.Add);
                            AdvanceDetailsCollection.RemoveAt(i);
                            AdvanceDetailsCollection.Insert(i, AdvanceDetail);
                            break;
                        }
                    }
                }
                else
                {
                    throw new CustomException("Unknown state found in OnAddOrderCommandclick");
                }

                //New Advance
                AdvanceDetail = new AdvanceDetails();
                AdvanceDetail.advanceType = AdvanceTypesCollection[0];
                AdvanceDetail.advanceDate = DateTime.Now.ToString();

                //Grand total
                decimal total = 0m;
                foreach (AdvanceDetails obj in AdvanceDetailsCollection)
                {
                    total = total + Convert.ToDecimal(obj.pureGoldWeight);
                }
                TotalGoldWeight = Convert.ToString(Math.Round(total, 3));
            }
        }

        void OnAdvanceDetailsEditCommand(object advanceDetails)
        {
            AdvanceButtonLabel = Convert.ToString(UserControlState.Update);
            SelectedAdvanceType = (advanceDetails as AdvanceDetails).advanceType;
            AdvanceDetail = helper.CloneAdvanceDetails((advanceDetails as AdvanceDetails));
        }

        void OnAdvanceDetailsDeleteCommand(object advanceDetails)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this Information", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (AdvanceDetails advance in AdvanceDetailsCollection)
                {
                    if (advance.id == (advanceDetails as AdvanceDetails).id)
                    {
                        AdvanceButtonLabel = Convert.ToString(UserControlState.Add);
                        AdvanceDetailsCollection.Remove(advance);
                        AdvanceDetail = new AdvanceDetails();
                        AdvanceDetail.advanceDate = DateTime.Now.ToString();
                        break;
                    }
                }
            }
        }

        void OnAdvanceTypeSelectionChangeCommandclick()
        {
            IsAdvanceButtonEnabled = true;
            if (SelectedAdvanceType != null && SelectedAdvanceType.name.ToLower().Equals("none"))
            {
                IsAdvanceButtonEnabled = false;
            }
        }

        void OnGenerateInvoiceCommandclick()
        {
            if (ValidateCustomerOrderDetailsCollection() && ValidateIfAdvanceRequired())
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    NpgsqlTransaction trans = connection.BeginTransaction();
                    try
                    {
                        //Insert into customer_order table
                        long fk_customer_order = 0;
                        string query_customer_order = String.Format("Insert into customer_order (order_no, order_ref_no, fk_customer_id) values('{0}','{1}', {2}) RETURNING id", DateTime.Now.ToString("yyyyMMddHHmmss"), OrderRefNo, SelectedCustomer.id);
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
                            string query_customer_order_details = "Insert into customer_order_details(size, net_weight, seal, description, attachement, order_date, due_date, ornament_purity, fk_ornament_type_id, fk_customer_order_id) values " + custOrderList;

                            using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_details, CommandType.Text))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            if (AdvanceDetailsCollection.Count > 0)
                            {
                                List<string> custAdvanceDetails = new List<string>();
                                foreach (AdvanceDetails obj in AdvanceDetailsCollection)
                                {
                                    custAdvanceDetails.Add(string.Format("({0},'{1}','{2}','{3}','{4}',{5})", obj.advanceType.id, obj.advanceDate,
                                        obj.goldWeight, obj.goldPurity, obj.description, fk_customer_order));
                                }

                                string custAdvanceList = string.Join<string>(",", custAdvanceDetails);
                                string query_customer_order_advance_details = "Insert into customer_order_advance_details(fk_advance_type_id, advance_date, gold_weight, gold_purity, description, fk_customer_order_id) values " + custAdvanceList;

                                using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_customer_order_advance_details, CommandType.Text))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            //Advance type None
                            else
                            {
                                string query_none_advance_details = string.Format("Insert into customer_order_advance_details" +
                                    "(fk_advance_type_id, advance_date, gold_weight, gold_purity, description, fk_employee_order_id) values ", SelectedAdvanceType.id, DateTime.Now.ToString(), 0.000M, 0.00M, string.Empty, SelectedCustomer.id);

                                using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query_none_advance_details, CommandType.Text))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Order creation failed. close and open the tool.");
                        }
                        trans.Commit();
                        MessageBoxResult messageBoxResult = MessageBox.Show("Customer order added successfully", "Record", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (MessageBoxResult.Yes == messageBoxResult)
                        {
                            OnGeneratePrintCommand();
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

        void OnGeneratePrintCommand()
        {
            try
            {
                ExcelFileArgs excelFileArgs = new ExcelFileArgs { orderDetails = OrderDetailsCollection, selectedCustomer = SelectedCustomer, advanceDetails = AdvanceDetailsCollection };
                ExcelGenerator excelGenerator = new ExcelGenerator();
                excelGenerator.GenerateCustomerOrderInvoice(excelFileArgs);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        #endregion Methods
    }
}
