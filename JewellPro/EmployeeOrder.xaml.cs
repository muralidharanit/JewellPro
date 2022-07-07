using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for EmployeeOrder.xaml
    /// </summary>
    public partial class EmployeeOrder : UserControl
    {
        public EmployeeOrder(double ucHeight)
        {
            InitializeComponent();
            this.Height = ucHeight;

            EmployeeOrderViewModel supplierOrderViewModel = new EmployeeOrderViewModel();
            this.DataContext = supplierOrderViewModel;
        }
    }
}
