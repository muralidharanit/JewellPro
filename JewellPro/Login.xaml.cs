using CommonLayer;
using JewellPro;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Data;
using System.Windows;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            txtUsername.Text = "Murali";
            txtPassword.Password = "m";

            string motherBoardId = Helper.GetMotherBoardID().Trim();

            //if(motherBoardId != "W2KS99T109W")
            //{
            //    loginBtn.IsEnabled = false;
            //    cancelBtn.IsEnabled = false;
            //    lblWarning.Content = "Invalid Client code found, contact for License +91 97152 86757";
            //}
        }

        private void btnclkLogin(object sender, RoutedEventArgs e)
        {
            string Query = "SELECT * FROM Login";
            try
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, Query, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
                                while (dataReader.Read())
                                {
                                    if (string.Equals(Convert.ToString(dataReader["Username"]), txtUsername.Text) && string.Equals(Convert.ToString(dataReader["Password"]), txtPassword.Password))
                                    {
                                        CommanDetails.user = new JewellPro.Login
                                        {
                                            id = Convert.ToInt32(dataReader["Id"]),
                                            userName = Convert.ToString(dataReader["Username"]),
                                            lastLoggedIn = Convert.ToString(DateTime.Now),
                                            userPreference = GetUserPreference(Convert.ToString(dataReader["preference"]))
                                        };
                                        this.Hide();

                                        MainWindow mainWindow = new MainWindow();
                                        mainWindow.ShowDialog();
                                    }
                                }

                                if (CommanDetails.user == null)
                                {
                                    lblWarning.Content = "Invalid Username and Password";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnclkClear(object sender, System.Windows.RoutedEventArgs e)
        {
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;
            lblWarning.Content = string.Empty;
        }

        public UserPreference GetUserPreference(string userPreferenceInfo)
        {
            return JsonConvert.DeserializeObject<UserPreference>(userPreferenceInfo);
        }
    }
}
