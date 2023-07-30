using GalaSoft.MvvmLight;
using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for GenerateEstimation.xaml
    /// </summary>
    public partial class GenerateEstimation : UserControl
    {
        public GenerateEstimation(double ucHeight, ViewModelBase viewModelBase)
        {
            InitializeComponent();
            this.Height = ucHeight;
            
            this.DataContext = viewModelBase;
        }        
    }
}
