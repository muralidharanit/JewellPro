using Npgsql;
using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text;
using CommonLayer;
using System.Data;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for CustomerOrder.xaml
    /// </summary>
    public partial class CustomerOrder : UserControl
    {
        public CustomerOrder(double ucHeight)
        {
            InitializeComponent();
            this.Height = ucHeight;

            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            this.DataContext = customerOrderViewModel;
        }
    }
}
