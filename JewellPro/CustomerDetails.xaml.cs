using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for CustomerDetails.xaml
    /// </summary>
    public partial class CustomerDetails : UserControl
    {
        public CustomerDetails(double ucHeight)
        {
            InitializeComponent();
            this.Height = ucHeight;

            CustomerDetailsViewModel customerDetailsViewModel = new CustomerDetailsViewModel();
            this.DataContext = customerDetailsViewModel;
        }
    }
}
