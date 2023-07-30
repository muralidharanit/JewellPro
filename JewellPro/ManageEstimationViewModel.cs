using CommonLayer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Npgsql;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Twilio.TwiML.Voice;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    public class ManageEstimationViewModel : ViewModelBase
    {
        public ManageEstimationViewModel() 
        {
            helper = new Helper();
            OnLoad();
        }

        #region Properties

        private Helper helper;

        private string _OrderButtonLabel;
        public string OrderButtonLabel
        {
            get { return _OrderButtonLabel; }
            set { _OrderButtonLabel = value; RaisePropertyChanged("OrderButtonLabel"); }
        }

        private ObservableCollection<OrderDetails> _EstimationRefNos;
        public ObservableCollection<OrderDetails> EstimationRefNos
        {
            get { return _EstimationRefNos; }
            set { _EstimationRefNos = value; RaisePropertyChanged("EstimationRefNos"); }
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

        private OrderDetails _Order;
        public OrderDetails Order
        {
            get { return _Order; }
            set { _Order = value; RaisePropertyChanged("Order"); }
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

        private OrderDetails _SelectedOrderDetails;
        public OrderDetails SelectedOrderDetails
        {
            get { return _SelectedOrderDetails; }
            set { _SelectedOrderDetails = value; RaisePropertyChanged("SelectedOrderDetails"); }
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
            try
            {
                CustomerDetails = helper.GetAllCustomerDetails();
                JewelTypes = helper.GetAllLoadJewelTypes();
                //EstimationRefNo = "ESTM_APS_" + helper.GetNextOrderRefNo();
                OrderButtonLabel = Convert.ToString(UserControlState.Add);
                GridColumnsVisibility = new GenerateEstimationGridColumns { attachement = false, description = true, dueDate = false, jewelPurity = true, jewelType = true, netWeight = true, seal = true, size = true };
                SelectedOrderDetails = new OrderDetails();
                SelectedOrderDetails.orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                SelectedOrderDetails.orderDate = DateTime.Now.ToString();
                OrderDetailsCollection = new ObservableCollection<OrderDetails>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        void OnCustomerNameSelectionChangeCommand()
        {
            string sqlQuery = string.Format("select * from orders where fk_customer_id = {0} and is_completed = false", SelectedCustomer.id);
            NpgsqlDataReader dataReader = helper.GetTableData(sqlQuery);
            try
            {
                if (dataReader != null)
                {
                    EstimationRefNos = new ObservableCollection<OrderDetails>();
                    while (dataReader.Read())
                    {
                        OrderDetails orderRefNo = new OrderDetails
                        {
                            id = Convert.ToInt32(dataReader["id"]),
                            orderNo = "ESTM_APS_" + Convert.ToInt32(dataReader["id"]),
                            isPriorityOrder = Convert.ToBoolean(dataReader["is_priority_order"]),
                            isRateFreeze = Convert.ToBoolean(dataReader["is_rate_freeze"]),
                            rateFreezeDate = Convert.ToString(dataReader["freeze_date"]),
                            freezeRate = Convert.ToString(dataReader["freeze_amount"]),
                            customer = SelectedCustomer,
                        };
                        EstimationRefNos.Add(orderRefNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        void OnCustomerEstimationSelectionChangeCommand()
        {
            string sqlQuery = string.Format("SELECT * FROM order_details where fk_order_id = {0}", SelectedOrderDetails.id);
            NpgsqlDataReader dataReader = helper.GetTableData(sqlQuery);
            try
            {
                if (dataReader != null)
                {
                    SelectedOrderDetails.orderDetailsList = new ObservableCollection<OrderDetails>();
                    while (dataReader.Read())
                    {
                        OrderDetails orderDetails = new OrderDetails();
                        orderDetails.id = Convert.ToInt32(dataReader["id"]);
                        orderDetails.size = Convert.ToString(dataReader["size"]);
                        orderDetails.netWeight = Convert.ToString(dataReader["net_weight"]);
                        orderDetails.seal = Convert.ToString(dataReader["seal"]);
                        orderDetails.description = Convert.ToString(dataReader["description"]);
                        orderDetails.attachement = Convert.ToString(dataReader["attachement"]);
                        orderDetails.orderDate = Convert.ToString(dataReader["order_date"]);
                        orderDetails.dueDate = Convert.ToString(dataReader["due_date"]);
                        orderDetails.jewelPurity = Convert.ToString(dataReader["ornament_purity"]);
                        orderDetails.wastagePercentage = Convert.ToString(dataReader["wastage"]);
                        //chargesDetails = helper.GetChargesControls("", Convert.ToInt32(dataReader["id"])),
                        orderDetails.jewelType = JewelTypes.FirstOrDefault(o => o.id == Convert.ToInt64(dataReader["fk_ornament_type_id"]));
                        SelectedOrderDetails.orderDetailsList.Add(orderDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        bool ValidateCustomerOrderDetailsCollection()
        {
            if (OrderDetailsCollection == null || OrderDetailsCollection.Count == 0)
            {
                MessageBox.Show("SelectedOrderDetails should not be empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            long customerId = OrderDetailsCollection[0].customer.id;

            foreach (OrderDetails order in OrderDetailsCollection)
            {
                if (customerId != order.customer.id)
                {
                    MessageBox.Show("SelectedOrderDetails should not be created, different customer not allowed", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
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
                                custOrderDetails.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9}, {10})", obj.size,
                                    obj.netWeight, obj.seal, obj.description, obj.attachement, obj.orderDate, obj.dueDate, obj.jewelPurity, obj.wastage,
                                    obj.jewelType.id, fk_customer_order));
                            }

                            string custOrderList = string.Join<string>(",", custOrderDetails);
                            string query_customer_order_details = "Insert into estimation_order_details(size, net_weight, seal, description, attachement, order_date, due_date, ornament_purity, wastage, fk_ornament_type_id, fk_customer_order_id) values " + custOrderList;

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

                SelectedOrderDetails.attachement = fileFullName;
                SelectedOrderDetails.attachementPath = orderPath;
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

            if (!string.IsNullOrEmpty(SelectedOrderDetails.quantity) && !Helper.IsValidInteger(SelectedOrderDetails.quantity))
            {
                errorControl.AppendLine("Enter Valid jewel Quantity");
                errorControl.AppendLine();
            }

            if (string.IsNullOrEmpty(SelectedOrderDetails.jewelPurity))
            {
                errorControl.AppendLine("Enter Jewel Purity");
                errorControl.AppendLine();
            }
            else if (!Helper.IsValidDecimal(SelectedOrderDetails.jewelPurity))
            {
                errorControl.AppendLine("Enter Valid Jewel Purity. Example 88.00");
                errorControl.AppendLine();
            }

            if (!string.IsNullOrEmpty(SelectedOrderDetails.wastage))
            {
                if (!Helper.IsValidDecimal(SelectedOrderDetails.wastage))
                {
                    errorControl.AppendLine("Enter Valid Jewel Wastage. Example 7.50 or 3");
                    errorControl.AppendLine();
                }
            }


            if (string.IsNullOrEmpty(SelectedOrderDetails.netWeight))
            {
                errorControl.AppendLine("Enter Jewellery Net Weight");
                errorControl.AppendLine();
            }
            else if (!Helper.IsValidDecimal(SelectedOrderDetails.netWeight))
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
                SelectedOrderDetails.customer = SelectedCustomer;
                SelectedOrderDetails.jewelType = SelectedJewelType;
                if (OrderButtonLabel == Convert.ToString(UserControlState.Add))
                {
                    SelectedOrderDetails.subOrderNo = "SUB" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    OrderDetailsCollection.Add(SelectedOrderDetails);

                }
                else if (OrderButtonLabel == Convert.ToString(UserControlState.Update))
                {
                    for (int i = OrderDetailsCollection.Count - 1; i >= 0; --i)
                    {
                        if (OrderDetailsCollection[i].subOrderNo == SelectedOrderDetails.subOrderNo)
                        {
                            OrderDetailsCollection.RemoveAt(i);
                            OrderDetailsCollection.Insert(i, SelectedOrderDetails);
                            OrderButtonLabel = Convert.ToString(UserControlState.Add);
                            break;
                        }
                    }
                }
                else
                {
                    throw new CustomException("Unknown state found in OnAddOrderCommandclick");
                }
                SelectedOrderDetails = new OrderDetails();
                SelectedOrderDetails.orderDate = DateTime.Now.ToString();
            }
        }

        void OnOrderDetailsEditCommand(object orderDetail)
        {
            OrderButtonLabel = Convert.ToString(UserControlState.Update);
            SelectedCustomer = (orderDetail as OrderDetails).customer;
            SelectedJewelType = (orderDetail as OrderDetails).jewelType;
            SelectedOrderDetails = CloneObject.DeepClone<OrderDetails>(orderDetail as OrderDetails);
        }

        void OnOrderDetailsDeleteCommand(object orderDetail)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this order", "Delete order", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        NpgsqlTransaction trans = connection.BeginTransaction();
                        string query = "Delete from order_details where id = {0} " + (orderDetail as OrderDetails).id;

                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, query, CommandType.Text))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                }
                catch (SystemException ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
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

        void OnResetCommandclick()
        {
            OrderButtonLabel = Convert.ToString(UserControlState.Add);
            if (OrderDetailsCollection == null || OrderDetailsCollection.Count == 0)
            {
                SelectedCustomer = null;
            }
            SelectedJewelType = null;
            SelectedOrderDetails = new OrderDetails();
            SelectedOrderDetails.orderDate = DateTime.Now.ToString();
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
                ////SelectedOrderDetails orderDetails = new SelectedOrderDetails { orderDate = DateTime.Now.ToString("dd-MM-yyyy"), orderRefNo = EstimationRefNo };
                //ExcelFileArgs excelFileArgs = new ExcelFileArgs { orderDetails = OrderDetailsCollection, selectedCustomer = SelectedCustomer, selectedCustomerOrder = orderDetails };
                //ExcelGenerator excelGenerator = new ExcelGenerator();
                //excelGenerator.GenerateCustomerEstimationInvoice(orderDetails);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        #endregion Methods

    }
}
