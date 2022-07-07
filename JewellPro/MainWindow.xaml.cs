using JewellPro;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mainWindowViewModel;

            usercontrolPane.Children.Clear();
            List<ItemMenu> menuItems = new List<ItemMenu>();

            ItemMenu dashboard = new ItemMenu() { Header = "Dashboard", Icon = PackIconKind.TabletDashboard, Id = AppMenus.Dashboard };
            menuItems.Add(dashboard);

            ItemMenu customer = new ItemMenu() { Header = "Customer", Icon = PackIconKind.AccountPlus, IsExpanded = true, IsSelected = false };
            customer.Members.Add(new SubItem() { Header = "New Order", Icon = PackIconKind.CreateNewFolder, Id = AppMenus.CustomerNewOrder });
            customer.Members.Add(new SubItem() { Header = "Generate Estimation", Icon = PackIconKind.Abacus, Id = AppMenus.CustomerEstimation });
            customer.Members.Add(new SubItem() { Header = "Order Delivery", Icon = PackIconKind.CashOnDelivery, Id = AppMenus.CustomerDelivery });
            customer.Members.Add(new SubItem() { Header = "Modify Order", Icon = PackIconKind.DatabaseEdit, Id = AppMenus.CustomerModifyOrder });
            menuItems.Add(customer);

            ItemMenu employee = new ItemMenu() { Header = "Employee", Icon = PackIconKind.AccountPlus, IsExpanded = true, IsSelected = false };
            employee.Members.Add(new SubItem() { Header = "New Order", Icon = PackIconKind.CreateNewFolder, Id = AppMenus.EmployeeNewOrder });
            employee.Members.Add(new SubItem() { Header = "Order Delivery", Icon = PackIconKind.CashOnDelivery, Id = AppMenus.EmployeeDelivery });
            employee.Members.Add(new SubItem() { Header = "Modify Order", Icon = PackIconKind.DatabaseEdit, Id = AppMenus.EmployeeModifyOrder });
            menuItems.Add(employee);

            ItemMenu manage = new ItemMenu() { Header = "Manage", Icon = PackIconKind.Manufacturing, IsExpanded = true, IsSelected = false };
            manage.Members.Add(new SubItem() { Header = "Customer", Icon = PackIconKind.CreateNewFolder, Id = AppMenus.ManageCustomer });
            manage.Members.Add(new SubItem() { Header = "Employee", Icon = PackIconKind.CashOnDelivery, Id = AppMenus.ManageEmployee });
            menuItems.Add(manage);

            ItemMenu report = new ItemMenu() { Header = "Report", Icon = PackIconKind.Report, Id = AppMenus.Report };
            menuItems.Add(report);
            trvMenuItem.ItemsSource = menuItems;
        }

        private void trvMenuItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (trvMenuItem.SelectedItem is SubItem)
            {
                if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.CustomerNewOrder)
                {
                    usercontrolPane.Children.Clear();
                    txtblkAppHeader.Text = "Customer Order";
                    CustomerOrder customerOrder = new CustomerOrder(contentPane.ActualHeight - 260);
                    usercontrolPane.Children.Add(customerOrder);
                }

                if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.CustomerEstimation)
                {
                    usercontrolPane.Children.Clear();
                    txtblkAppHeader.Text = "Customer Estimation";
                    GenerateEstimation customerOrder = new GenerateEstimation(contentPane.ActualHeight - 190);
                    usercontrolPane.Children.Add(customerOrder);
                }

                if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.CustomerDelivery)
                {
                    usercontrolPane.Children.Clear();
                    txtblkAppHeader.Text = "Customer Delivery";
                    CustomerDelivery customerDelivery = new CustomerDelivery(contentPane.ActualHeight - 260);
                    usercontrolPane.Children.Add(customerDelivery);
                }

                if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.EmployeeNewOrder)
                {
                    usercontrolPane.Children.Clear();
                    txtblkAppHeader.Text = "Employee Order";
                    EmployeeOrder employeeOrder = new EmployeeOrder(contentPane.ActualHeight - 260);
                    usercontrolPane.Children.Add(employeeOrder);
                }

                //if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.EmployeeDelivery)
                //{
                //    usercontrolPane.Children.Clear();
                //    EmployeeDelivery employeeOrderDelivery = new EmployeeDelivery(contentPane.ActualHeight - 50);
                //    usercontrolPane.Children.Add(employeeOrderDelivery);
                //}

                if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.ManageCustomer)
                {
                    usercontrolPane.Children.Clear();
                    CustomerDetails customerDetails = new CustomerDetails(contentPane.ActualHeight - 60);
                    usercontrolPane.Children.Add(customerDetails);
                }

                //if ((trvMenuItem.SelectedItem as SubItem).Id == AppMenus.ManageEmployee)
                //{
                //    usercontrolPane.Children.Clear();
                //    EmployeeDetails employeeDetails = new EmployeeDetails(contentPane.ActualHeight - 60);
                //    usercontrolPane.Children.Add(employeeDetails);
                //}
            }
            else if (trvMenuItem.SelectedItem is ItemMenu)
            {
                //if ((trvMenuItem.SelectedItem as ItemMenu).Id == AppMenus.Report)
                //{
                //    usercontrolPane.Children.Clear();
                //    Reports reports = new Reports(contentPane.ActualHeight - 60);
                //    usercontrolPane.Children.Add(reports);
                //}
            }
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
