using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for ChargesWindow.xaml
    /// </summary>
    public partial class ChargesWindow : MetroWindow
    {
        private List<CustomControls> chargesControls = new List<CustomControls>();
        private UserControl callBackUserControl;
        public ChargesWindow(UserControl userControl = null)
        {
            InitializeComponent();
            callBackUserControl = userControl;
            GenerateControls();
            txtblkError.Text = "";
        }

        private void GenerateControls()
        {
            //string sqlQuery = "select * from charges_type order by name asc";
            //Helper helper = new Helper();
            //NpgsqlDataReader dataReader = helper.GetTableData(sqlQuery);
            //chargesControls.Clear();
            //spnlCharges.Children.Clear();

            //if (dataReader != null)
            //{
            //    while (dataReader.Read())
            //    {
            //        string lblContent = Convert.ToString(dataReader["name"]);
            //        StackPanel spnl = Helper.AddDynamicControls(lblContent, "", false);
            //        spnlCharges.Children.Add(spnl);
            //    }
            //}
        }

        private void GenerateCharges(object sender, System.Windows.RoutedEventArgs e)
        {
            chargesControls.Clear();
            bool isValid = true;

            foreach (UIElement ctrl in spnlCharges.Children)
            {
                string txtbxValue = ((ctrl as StackPanel).Children[1] as TextBox).Text.Trim();
                string lblContent = Convert.ToString(((ctrl as StackPanel).Children[0] as Label).Tag);

                if(txtbxValue.Length > 0)
                {
                    CustomControls customControl = new CustomControls();
                    if ((ctrl as StackPanel).Children[1] is TextBox)
                    {
                        if (!Helper.IsValidDecialValue(txtbxValue))
                        {
                            txtblkError.Text = string.Format("Invalid value present in {0} field", lblContent);
                            isValid = false;
                            break;
                        }
                        else
                        {
                            customControl.description = lblContent;
                            customControl.value = Convert.ToDecimal(txtbxValue);
                        }
                    }
                    chargesControls.Add(customControl);
                }
            }

            if(isValid)
            {
                if (callBackUserControl != null && callBackUserControl.GetType().Name == "EmployeeDelivery")
                {
                    (callBackUserControl as EmployeeDelivery).AddChargesControl(chargesControls);
                }

                this.Hide();
            }
        }
    }
}
