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
            set { _OrderDetails = value; RaisePropertyChanged("SelectedOrderDetails"); }
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


        #endregion Properties

        public CustomerOrderViewModel()
        {
            helper = new Helper();
            OnLoad();
        }

        #region iCommands

        //RelayCommand _AddOrderCommand = null;
        //public ICommand AddOrderCommand
        //{
        //    get
        //    {
        //        if (_AddOrderCommand == null)
        //            _AddOrderCommand = new RelayCommand(() => OnAddOrderCommandclick());
        //        return _AddOrderCommand;
        //    }
        //}
        
        //RelayCommand<object> _OrderDetailsEditCommand = null;
        //public ICommand OrderDetailsEditCommand
        //{
        //    get
        //    {
        //        if (_OrderDetailsEditCommand == null)
        //            _OrderDetailsEditCommand = new RelayCommand<object>((obj) => OnOrderDetailsEditCommand(obj));
        //        return _OrderDetailsEditCommand;
        //    }
        //}

        //RelayCommand<object> _OrderDetailsDeleteCommand = null;
        //public ICommand OrderDetailsDeleteCommand
        //{
        //    get
        //    {
        //        if (_OrderDetailsDeleteCommand == null)
        //            _OrderDetailsDeleteCommand = new RelayCommand<object>((obj) => OnOrderDetailsDeleteCommand(obj));
        //        return _OrderDetailsDeleteCommand;
        //    }
        //}

        //RelayCommand _AddAdvanceCommand = null;
        //public ICommand AddAdvanceCommand
        //{
        //    get
        //    {
        //        if (_AddAdvanceCommand == null)
        //            _AddAdvanceCommand = new RelayCommand(() => OnAddAdvanceCommandclick());
        //        return _AddAdvanceCommand;
        //    }
        //}

        //RelayCommand<object> _AdvanceDetailsEditCommand = null;
        //public ICommand AdvanceDetailsEditCommand
        //{
        //    get
        //    {
        //        if (_AdvanceDetailsEditCommand == null)
        //            _AdvanceDetailsEditCommand = new RelayCommand<object>((obj) => OnAdvanceDetailsEditCommand(obj));
        //        return _AdvanceDetailsEditCommand;
        //    }
        //}

        //RelayCommand<object> _AdvanceDetailsDeleteCommand = null;
        //public ICommand AdvanceDetailsDeleteCommand
        //{
        //    get
        //    {
        //        if (_AdvanceDetailsDeleteCommand == null)
        //            _AdvanceDetailsDeleteCommand = new RelayCommand<object>((obj) => OnAdvanceDetailsDeleteCommand(obj));
        //        return _AdvanceDetailsDeleteCommand;
        //    }
        //}

        //RelayCommand _GenerateInvoiceCommand = null;
        //public ICommand GenerateInvoiceCommand
        //{
        //    get
        //    {
        //        if (_GenerateInvoiceCommand == null)
        //            _GenerateInvoiceCommand = new RelayCommand(() => OnGenerateInvoiceCommandclick());
        //        return _GenerateInvoiceCommand;
        //    }
        //}

        //RelayCommand _AddAttachementCommand = null;
        //public ICommand AddAttachementCommand
        //{
        //    get
        //    {
        //        if (_AddAttachementCommand == null)
        //            _AddAttachementCommand = new RelayCommand(() => OnAddAttachementCommandclick());
        //        return _AddAttachementCommand;
        //    }
        //}

        //RelayCommand _ResetCommand = null;
        //public ICommand ResetCommand
        //{
        //    get
        //    {
        //        if (_ResetCommand == null)
        //            _ResetCommand = new RelayCommand(() => OnResetCommandclick());
        //        return _ResetCommand;
        //    }
        //}

        //RelayCommand _AdvanceTabResetCommand = null;
        //public ICommand AdvanceTabResetCommand
        //{
        //    get
        //    {
        //        if (_AdvanceTabResetCommand == null)
        //            _AdvanceTabResetCommand = new RelayCommand(() => OnAdvanceTabResetCommandclick());
        //        return _AdvanceTabResetCommand;
        //    }
        //}

        //RelayCommand _AdvanceTypeSelectionChangeCommand = null;
        //public ICommand AdvanceTypeSelectionChangeCommand
        //{
        //    get
        //    {
        //        if (_AdvanceTypeSelectionChangeCommand == null)
        //            _AdvanceTypeSelectionChangeCommand = new RelayCommand(() => OnAdvanceTypeSelectionChangeCommandclick());
        //        return _AdvanceTypeSelectionChangeCommand;
        //    }
        //}

        #endregion Properties

        #region Methods

        private void OnLoad()
        {
            //Load Master data
            CustomerDetails = helper.GetAllCustomerDetails();
            JewelTypes = helper.GetAllLoadJewelTypes();
            //OrderRefNo = helper.GetNextOrderRefNo();
            //AdvanceTypesCollection = helper.GetAllAdvanceTypes();
            ////Puritys = helper.GetAllPurityDetails();
            
            //TotalGoldWeight = string.Empty;
            //IsGenerateInvoiceEnabled = false;

            //OrderDetails = new OrderDetails();
            //OrderDetails.orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            //OrderDetails.orderDate = DateTime.Now.ToString();
            //OrderDetailsCollection = new ObservableCollection<OrderDetails>();
            
            //AdvanceDetail = new AdvanceDetails();
            //AdvanceDetail.advanceDate = DateTime.Now.ToString();

            //AdvanceDetailsCollection = new ObservableCollection<AdvanceDetails>();

            //for (int i = 0; i <= AdvanceTypesCollection.Count - 1; i++)
            //{
            //    if (AdvanceTypesCollection[i].name.Equals("none", StringComparison.OrdinalIgnoreCase))
            //    {
            //        SelectedAdvanceType = AdvanceTypesCollection[i];
            //        AdvanceDetail.advanceType = AdvanceTypesCollection[i];
            //        break;
            //    }
            //}

            //OrderButtonLabel = Convert.ToString(UserControlState.Add);
            //AdvanceButtonLabel = Convert.ToString(UserControlState.Add);

            //OnAdvanceTypeSelectionChangeCommandclick();
        }

        #endregion Methods
    }
}
