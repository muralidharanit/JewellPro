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
    }
}
