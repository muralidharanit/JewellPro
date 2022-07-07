using System;
using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for CustomerDelivery.xaml
    /// </summary>
    public partial class CustomerDelivery : UserControl
    {
        public CustomerDelivery(double ucHeight)
        {
            InitializeComponent();
            this.Height = ucHeight;

            CustomerDeliveryViewModel customerDeliveryViewModel = new CustomerDeliveryViewModel();
            this.DataContext = customerDeliveryViewModel;
        }
    }
}
