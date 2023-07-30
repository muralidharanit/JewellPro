using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    [Serializable]
    public class BaseClass 
    {
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    [Serializable]
    public class OrderDetails :  NotifyObject
    {
        private long _id;
        public long id
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("id"); }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("name"); }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("description"); }
        }

        private string _orderNo;
        public string orderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; RaisePropertyChanged("orderNo"); }
        }

        private OrderType _orderType;
        public OrderType orderType
        {
            get { return _orderType; }
            set { _orderType = value; RaisePropertyChanged("orderType"); }
        }

        private string _orderRefNo;
        public string orderRefNo
        {
            get { return _orderRefNo; }
            set { _orderRefNo = value; RaisePropertyChanged("orderRefNo"); }
        }

        private string _subOrderNo;
        public string subOrderNo
        {
            get { return _subOrderNo; }
            set { _subOrderNo = value; RaisePropertyChanged("subOrderNo"); }
        }

        private Employee _employee;
        public Employee employee
        {  
            get { return _employee; }
            set { _employee = value;  RaisePropertyChanged("employee"); }
        }

        private Customer _customer;
        public Customer customer
        {
            get { return _customer; }
            set {_customer = value; RaisePropertyChanged("customer"); }
        }

        private JewelType _jewelType;
        public JewelType jewelType 
        {
            get { return _jewelType; }
            set { _jewelType = value; RaisePropertyChanged("jewelType");}
        }
        
        private string _size;
        public string size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged("size"); }
        }

        private string _seal;
        public string seal
        {
            get { return _seal; }
            set { _seal = value; RaisePropertyChanged("seal"); }
        }

        private string _attachement;
        public string attachement
        {
            get { return _attachement; }
            set { _attachement = value; RaisePropertyChanged("attachement"); }
        }

        private string _attachementPath;
        public string attachementPath
        {
            get { return _attachementPath; }
            set { _attachementPath = value; RaisePropertyChanged("attachementPath"); }
        }

        private string _orderDate;
        public string orderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; RaisePropertyChanged("orderDate"); }
        }

        private string _dueDate;
        public string dueDate 
        {
            get { return _dueDate; }
            set { _dueDate = value; RaisePropertyChanged("dueDate"); }
        }

        private string _makingCharge;
        public string makingCharge
        {
            get { return _makingCharge; }
            set { _makingCharge = value; RaisePropertyChanged("makingCharge"); }
        }
        
        private string _rateFreezeDate;
        public string rateFreezeDate
        {
            get { return _rateFreezeDate; }
            set { _rateFreezeDate = value; RaisePropertyChanged("rateFreezeDate"); }
        }

        private bool _isRateFrozen;
        public bool isRateFreeze
        {
            get { return _isRateFrozen; }
            set { _isRateFrozen = value; RaisePropertyChanged("isRateFreeze"); }
        }

        private bool _isPriorityOrder;
        public bool isPriorityOrder
        {
            get { return _isPriorityOrder; }
            set { _isPriorityOrder = value; RaisePropertyChanged("isPriorityOrder"); }
        }

        private bool _isGSTOrder;
        public bool isGSTOrder
        {
            get { return _isGSTOrder; }
            set { _isGSTOrder = value; RaisePropertyChanged("isGSTOrder"); }
        }

        private string _freezeRate;
        public string freezeRate
        {
            get { return _freezeRate; }
            set { _freezeRate = value; RaisePropertyChanged("freezeRate"); }
        }

        private string _jewelPurity;
        public string jewelPurity
        {
            get { return _jewelPurity; }
            set { _jewelPurity = value; RaisePropertyChanged("jewelPurity"); }
        }

        private string _netWeight;
        public string netWeight
        {
            get { return _netWeight; }
            set { _netWeight = value; RaisePropertyChanged("netWeight"); }
        }

        private string _jewellRecivedWeight;
        public string jewellRecivedWeight
        {
            get { return _jewellRecivedWeight; }
            set { _jewellRecivedWeight = value; RaisePropertyChanged("jewellRecivedWeight"); }
        }

        private string _pureGoldWeight;
        public string pureGoldWeight
        {
            get { return _pureGoldWeight; }
            set { _pureGoldWeight = value; RaisePropertyChanged("pureGoldWeight"); }
        }

        private string _wastagePercentage;
        public string wastagePercentage
        {
            get { return _wastagePercentage; }
            set { _wastagePercentage = value; RaisePropertyChanged("wastagePercentage"); }
        }

        private string _wastage;
        public string wastage
        {
            get { return _wastage; }
            set { _wastage = value; RaisePropertyChanged("wastage"); }
        }

        private string _detection;
        public string detection
        {
            get { return _detection; }
            set { _detection = value;  RaisePropertyChanged("detection"); }
        }       

        private string _grandTotal;
        public string grandTotal
        {
            get { return _grandTotal; }
            set { _grandTotal = value; RaisePropertyChanged("grandTotal"); }
        }

        private string _quantity;
        public string quantity
        {
            get { return _quantity; }
            set { _quantity = value; RaisePropertyChanged("quantity"); }
        }

        ObservableCollection<DetectionControl> _detectionDetails;
        public ObservableCollection<DetectionControl> detectionDetails
        {
            get { return _detectionDetails; }
            set { _detectionDetails = value; RaisePropertyChanged("detectionDetails"); }
        }

        ObservableCollection<ChargesControl> _chargesDetails;
        public ObservableCollection<ChargesControl> chargesDetails
        {
            get { return _chargesDetails; }
            set { _chargesDetails = value; RaisePropertyChanged("chargesDetails"); }
        }


        ObservableCollection<AdvanceDetails> _advanceDetails;
        public ObservableCollection<AdvanceDetails> advanceDetails
        {
            get { return _advanceDetails; }
            set { _advanceDetails = value; RaisePropertyChanged("advanceDetails"); }
        }

        ObservableCollection<OrderDetails> _orderDetailsList;
        public ObservableCollection<OrderDetails> orderDetailsList
        {
            get { return _orderDetailsList; }
            set { _orderDetailsList = value; RaisePropertyChanged("orderDetailsList"); }
        }
    }

    [Serializable]
    public class DetectionControl : CustomControls
    {
        
    }

    [Serializable]
    public class ChargesControl : CustomControls
    {

    }

    [Serializable]
    public class CustomControls 
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string controlType { get; set; }
        public string value { get; set; }
        public int pkId { get; set; }
    }

    [Serializable]
    public class Login 
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string lastLoggedIn { get; set; }
        public UserPreference userPreference { get; set; }
        public UserType userType { get; set; }
    }

    [Serializable]
    public class MailInfo
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
        public string AutoTrigger { get; set; }
    }

    [Serializable]
    public class UserType 
    {
        public int id { get; set; }
        public string userType { get; set; }
    }

    [Serializable]
    public class Employee : User
    {

    }

    [Serializable]
    public class GenerateEstimationGridColumns : NotifyObject
    {
        private bool _jewelType;
        public bool jewelType
        {
            get { return _jewelType; }
            set { _jewelType = value; RaisePropertyChanged("jewelType"); }
        }

        private bool _jewelPurity;
        public bool jewelPurity
        {
            get { return _jewelPurity; }
            set { _jewelPurity = value; RaisePropertyChanged("jewelPurity"); }
        }

        private bool _size;
        public bool size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged("size"); }
        }

        private bool _netWeight;
        public bool netWeight
        {
            get { return _netWeight; }
            set { _netWeight = value; RaisePropertyChanged("netWeight"); }
        }

        private bool _seal;
        public bool seal
        {
            get { return _seal; }
            set { _seal = value; RaisePropertyChanged("seal"); }
        }

        private bool _description;
        public bool description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("description"); }
        }

        private bool _attachement;
        public bool attachement
        {
            get { return _attachement; }
            set { _attachement = value; RaisePropertyChanged("attachement"); }
        }

        private bool _dueDate;
        public bool dueDate
        {
            get { return _dueDate; }
            set { _dueDate = value; RaisePropertyChanged("dueDate"); }
        }
    }

    [Serializable]
    public class Purity 
    {
        public int id { get; set; }
        public string metalType { get; set; }
        public string description { get; set; }
        public string purity { get; set; }
        public string displayText { get; set; }
        public string karat { get; set; }
    }

    [Serializable]
    public class Customer : User
    {
        public string gst { get; set; }
    }

    [Serializable]
    public class User : BaseClass
    {        
        public string address { get; set; }
        public string email { get; set; }
        public string bloodgroup { get; set; }
        public string mobile { get; set; }
        public string dob { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }
        public string aadhaar { get; set; }
        public string pan { get; set; }
        public bool isSelected { get; set; }
        public string anniversary { get; set; }
    }

    [Serializable]
    public class Gender
    {
        public int id { get; set; }

        public string name { get; set; }

    }

    [Serializable]
    public class AdvanceType : BaseClass
    {

    }

    [Serializable]
    public class AdvanceDetails : NotifyObject
    {
        public long id { get; set; }
        public string name { get; set; }
        public string transferRefNo { get; set; }
        public string bankName { get; set; }
        public string advanceAmount { get; set; }
        private decimal _goldPurity { get; set; }
        public decimal goldPurity
        {
            get { return _goldPurity; }
            set { _goldPurity = Math.Round(value, 2); RaisePropertyChanged("goldPurity"); }
        }

        private decimal _goldWeight;
        public decimal goldWeight
        {
            get { return _goldWeight; }
            set { _goldWeight = Math.Round(value, 3); RaisePropertyChanged("goldWeight"); }
        }
        public decimal goldMeltingLossWeight { get; set; }
        public string goldRate { get; set; }

        private AdvanceType _advanceType;
        public AdvanceType advanceType
        {
            get { return _advanceType; }
            set { _advanceType = value; RaisePropertyChanged("advanceType"); }
        }

        private string _advanceDate;
        public string advanceDate
        {
            get { return _advanceDate; }
            set { _advanceDate = value; RaisePropertyChanged("advanceDate"); }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value;  RaisePropertyChanged("description"); }
        }

        private string _pureGoldWeight;
        public string pureGoldWeight
        {
            get { return _pureGoldWeight; }
            set { _pureGoldWeight = value; RaisePropertyChanged("pureGoldWeight"); }
        }

        private string _total;
        public string total
        {
            get { return _total; }
            set { _total = value; RaisePropertyChanged("total"); }
        }
    }

    [Serializable]
    public class CreditDebitDetails : NotifyObject
    {
        public long id { get; set; }

        private string _goldWeight;
        public string goldWeight
        {
            get { return _goldWeight; }
            set { _goldWeight = value;  RaisePropertyChanged("goldWeight"); }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("description"); }
        }

        public long orderId { get; set; }

        public string orderRefNo { get; set; }
    }

    [Serializable]
    public class JewelType : BaseClass
    {

    }

    [Serializable]
    public class Wastage
    {
        public string jewellWeight { get; set; }
        public string wastageInPercentage { get; set; }
        public string wastageInGram { get; set; }
        public string detection { get; set; }
    }

    [Serializable]
    public class GoldeDetails
    {
        public long id { get; set; }
        public bool isPureGold { get; set; }
        public string goldPurity { get; set; }
        public string goldWeight { get; set; }
        public string itemPurity { get; set; }
        public string total { get; set; }
    }

    [Serializable]
    public class UserPreference
    {
        public string pageId { get; set; }
        public ObservableCollection<Rate> Rates { get; set; }
    }

    [Serializable]
    public class Rate : NotifyObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public int rate { get; set; }
        public int purity { get; set; }
        public string description { get; set; }
        
        private bool _isChecked { get; set; }
        public bool isChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; RaisePropertyChanged("isChecked"); }
        }

        private bool _isEnabled { get; set; }
        public bool isEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; RaisePropertyChanged("isEnabled"); }
        }
    }

    [Serializable]
    public class ExcelFileArgs
    {
        public string orderRefNo { get; set; }
        public string orderDate { get; set; }
        public Employee selectedEmployee { get; set; }
        public Customer selectedCustomer { get; set; }
        public OrderDetails selectedCustomerOrder { get; set; }
        public ObservableCollection<AdvanceDetails> advanceDetails { get; set; }
        public ObservableCollection<OrderDetails> orderDetails { get; set; }
    }

    [Serializable]
    public class ReportStatus
    {
        public string reportPath { get; set; }
        public bool status { get; set; }
        public string errorInfo { get; set; }
    }

    [Serializable]
    public class NotifyObject : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }

    }
}
