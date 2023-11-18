using GalaSoft.MvvmLight;
using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for CustomerOrder.xaml
    /// </summary>
    public partial class CustomerOrder : UserControl
    {
        public CustomerOrder(double ucHeight, ViewModelBase viewModelBase)
        {
            InitializeComponent();
            this.Height = ucHeight;

            this.DataContext = viewModelBase;
        }
    }
}
