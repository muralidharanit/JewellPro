using System;
using System.Collections.ObjectModel;
using System.Text;
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

        private readonly dynamic viewmodelObj;
        public AddDetectionWindow(dynamic ViewModelObj, ObservableCollection<DetectionControl> DetectionControls, ObservableCollection<ChargesControl> ChargesControls, bool isHideDetection = false, bool isHideCharges = false)
        {
            InitializeComponent();
            empOrderTab.SelectedIndex = 0;
            tabCharges.Visibility = isHideCharges ? Visibility.Collapsed : Visibility.Visible;
            tabDetections.Visibility = isHideDetection ? Visibility.Collapsed : Visibility.Visible;
            viewmodelObj = ViewModelObj;
            detectionControls = DetectionControls;
            chargesControls = ChargesControls;
            GenerateDetectionControls();
            GenerateChargesControls();
        }

        private void GenerateChargesControls()
        {
            foreach (ChargesControl ctrl in chargesControls)
            {
                StackPanel spnl = Helper.AddDynamicControls(ctrl.description, ctrl.value, true, null, "", 
                    Application.Current.Resources["JPTextBlock"] as Style, 
                    Application.Current.Resources["JPTextBox"] as Style, ctrl.name, true, ctrl.isDisplayInBill);
                spnlCharges.Children.Add(spnl);
            }
            this.Height = chargesControls.Count * 50 + 150;
        }

        private void GenerateDetectionControls()
        {
            foreach (DetectionControl ctrl in detectionControls)
            {
                StackPanel spnl = Helper.AddDynamicControls(ctrl.description, ctrl.value, true, null, "", 
                    Application.Current.Resources["JPTextBlock"] as Style, 
                    Application.Current.Resources["JPTextBox"] as Style, ctrl.name);
                spnlDetection.Children.Add(spnl);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement ctrl in spnlDetection.Children)
            {
                ((ctrl as StackPanel).Children[1] as TextBox).Text = "";
            }

            foreach (UIElement ctrl in spnlCharges.Children)
            {
                ((ctrl as StackPanel).Children[1] as TextBox).Text = "";
            }
        }
       
        private void AddDetectionCharges_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errorControl = new StringBuilder();

            foreach (UIElement ctrl in spnlDetection.Children)
            {
                foreach (DetectionControl detection in detectionControls)
                {
                    if ((ctrl as StackPanel).Tag.ToString() == detection.name)
                    {
                        if(!string.IsNullOrWhiteSpace(detection.value) && !Helper.IsValidDecimal(detection.value))
                        {
                            errorControl.AppendLine("Enter valid detection.");
                            errorControl.AppendLine();
                        }
                        else
                        {
                            detection.value = ((ctrl as StackPanel).Children[1] as TextBox).Text;
                        }
                        break;
                    }
                }
            }

            foreach (UIElement ctrl in spnlCharges.Children)
            {
                foreach (ChargesControl charge in chargesControls)
                {
                    if ((ctrl as StackPanel).Tag.ToString() == charge.name)
                    {
                        if (!string.IsNullOrWhiteSpace(charge.value) && !Helper.IsValidDecimal(charge.value))
                        {
                            errorControl.AppendLine("Enter valid charges.");
                            errorControl.AppendLine();
                        }
                        else
                        {
                            charge.value = ((ctrl as StackPanel).Children[2] as TextBox).Text;

                            if((bool)((ctrl as StackPanel).Children[0] as CheckBox).IsChecked)
                                charge.isDisplayInBill = true;
                        }
                        break;
                    }
                }
            }
            if (errorControl.Length != 0)
            {
                MessageBox.Show(errorControl.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                this.Hide();
            }
        }
    }
}