using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewellPro
{
    public class EnumInfo
    {

        public enum SearchTypes { Customer, Empolyee}

        public enum AppMenus
        {
            CustomerNewOrder,
            CustomerEstimation,
            ModifyCustomerEstimation,
            CustomerModifyOrder,
            CustomerDelivery,
            EmployeeNewOrder,
            EmployeeModifyOrder,
            EmployeeDelivery,
            Report,
            Dashboard,
            ManageCustomer,
            ManageEmployee
        }
        public enum UserControlState { Add, Update, Delete };

        public enum OrderType { Customer, Employee, Estimation };

        public enum EstimationReportKeys { Date, Name, Address, Phone, TotalItems, TotalWeight, TotalAmount };
    }
}
