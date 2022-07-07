using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for AddDetectionWindow.xaml
    /// </summary>
    public partial class AddDetectionWindow : Window
    {
        private ObservableCollection<DetectionControl> detectionControls;
        private ObservableCollection<ChargesControl> chargesControls;

        private dynamic viewmodelObj;
        public AddDetectionWindow(dynamic ViewmodelObj, ObservableCollection<DetectionControl> DetectionControls, ObservableCollection<ChargesControl> ChargesControls)
        {
            InitializeComponent();
            viewmodelObj = ViewmodelObj;
            detectionControls = DetectionControls;
            chargesControls = ChargesControls;
            GenerateDetectionControls();
            GenerateChargesControls();
        }

        private void GenerateChargesControls()
        {
            foreach (ChargesControl ctrl in chargesControls)
            {
                StackPanel spnl = Helper.AddDynamicControls(ctrl.description, ctrl.value, true, null, "", Application.Current.Resources["JPTextBlock"] as Style, Application.Current.Resources["JPTextBox"] as Style, ctrl.name);
                spnlCharges.Children.Add(spnl);
            }
            this.Height = chargesControls.Count * 50 + 150;
        }

        private void GenerateDetectionControls()
        {
            foreach (DetectionControl ctrl in detectionControls)
            {
                StackPanel spnl = Helper.AddDynamicControls(ctrl.description, ctrl.value, true, null, "", Application.Current.Resources["JPTextBlock"] as Style, Application.Current.Resources["JPTextBox"] as Style, ctrl.name);
                spnlDetection.Children.Add(spnl);
            }
        }
        
        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddDetectionCharges_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement ctrl in spnlDetection.Children)
            {
                foreach (DetectionControl obj in detectionControls)
                {
                    if ((ctrl as StackPanel).Tag.ToString() == obj.name)
                    {
                        obj.value = ((ctrl as StackPanel).Children[1] as TextBox).Text;
                        break;
                    }
                }
            }

            foreach (UIElement ctrl in spnlCharges.Children)
            {
                foreach (ChargesControl obj in chargesControls)
                {
                    if ((ctrl as StackPanel).Tag.ToString() == obj.name)
                    {
                        obj.value = ((ctrl as StackPanel).Children[1] as TextBox).Text;
                        break;
                    }
                }
            }

            if (viewmodelObj != null)
            {
                viewmodelObj.UpdateDetectionDetails(detectionControls, chargesControls);
            }

            this.Hide();
        }
    }
}