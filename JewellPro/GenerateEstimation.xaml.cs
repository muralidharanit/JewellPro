using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for GenerateEstimation.xaml
    /// </summary>
    public partial class GenerateEstimation : UserControl
    {
        public GenerateEstimation(double ucHeight)
        {
            InitializeComponent();
            this.Height = ucHeight;

            GenerateEstimationViewModel generateEstimationViewModel = new GenerateEstimationViewModel();
            this.DataContext = generateEstimationViewModel;
        }
    }
}
