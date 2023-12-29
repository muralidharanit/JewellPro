using CommonLayer;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace JewellPro
{
    public class Helper
    {
        public static UserPreference LoggedInUserPreference;
        public static int RateDisplayCount = 3;
        public static int MaxDescriptionLimit = 80;
        public static string SenderEmail = "";
        public static string SenderPassword = "";

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " lakhs ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words;
        }

        public NpgsqlDataReader GetTableData(string sqlQuery)
        {
            try
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, sqlQuery, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
                                return dataReader;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return null;
        }

        public static StackPanel AddDynamicControls(string lblContent, string value, 
            bool isReadonly = true, MouseButtonEventHandler Image_MouseLeftButtonUp = null, 
            string symbol = "", Style textBlockStyle = null, Style textBoxStyle = null, 
            object identifier = null, bool showDisplayInBillCheckBox = false, bool displayInBillCheckBoxValue = false)
        {
            StackPanel contentPanel = new StackPanel();
            contentPanel.Orientation = Orientation.Horizontal;
            contentPanel.Margin = new Thickness(5, 5, 5, 5);
            contentPanel.Tag = identifier;
            contentPanel.Name = "dt" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            Console.WriteLine(contentPanel.Name);

            if(showDisplayInBillCheckBox)
            {
                CheckBox checkBox = new CheckBox
                {
                    IsChecked = displayInBillCheckBoxValue,
                    Margin = new Thickness(0, 0, 3, 10)
                };
                contentPanel.Children.Add(checkBox);
            }

            TextBlock label = new TextBlock();
            label.Text = lblContent + " : " + symbol;
            label.Tag = lblContent;
            label.Style = textBlockStyle;

            TextBox textBox = new TextBox();
            textBox.Style = textBoxStyle;
            textBox.Text = Convert.ToString(value);
            contentPanel.Children.Add(label);
            contentPanel.Children.Add(textBox);

            if (Image_MouseLeftButtonUp != null)
            {
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Source = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + @"\images\remove.png"));
                image.Margin = new Thickness(10, 0, 0, 0);

                image.MouseLeftButtonUp += (Image_MouseLeftButtonUp as MouseButtonEventHandler);
                image.Width = 20;
                image.Height = 20;
                image.Cursor = Cursors.Hand;
                contentPanel.Children.Add(image);
            }
            return contentPanel;
        }

        public string CalculateWastage()
        {
            string wastage = string.Empty;

            return wastage;
        }

        public static string CalculateCharges(StackPanel stackPanel)
        {
            double charges = 0;
            foreach (UIElement ctrl in stackPanel.Children)
            {
                if ((ctrl as StackPanel).Children[1] is TextBox)
                {
                    string txtbxValue = ((ctrl as StackPanel).Children[1] as TextBox).Text;
                    charges = charges + Convert.ToDouble(txtbxValue);
                }
            }

            if (charges > 0)
                return Convert.ToString(charges);

            return string.Empty;
        }

        public static string TextFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        public static string GetPurity(Purity selectedPurity, OrderDetails orderDetails)
        {
            if (selectedPurity == null)
            {
                decimal _purity;
                if (!Decimal.TryParse(orderDetails.jewelPurity, out _purity))
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToString(_purity);
                }
            }
            else if (selectedPurity != null)
            {
                return selectedPurity.purity;
            }

            return string.Empty;
        }

        public static string GetGrandTotal(ObservableCollection<AdvanceDetails> goldeDetails)
        {
            decimal total = 0;
            foreach (AdvanceDetails obj in goldeDetails)
            {
                _ = Decimal.TryParse(obj.pureGoldWeight, out decimal pureGoldWeight);
                total = total + pureGoldWeight;
            }
            return Convert.ToString(Math.Round(total, 3));
        }

        public static string GetGrandTotal(ObservableCollection<OrderDetails> goldeDetails)
        {
            decimal total = 0;
            foreach (OrderDetails obj in goldeDetails)
            {
                _ = Decimal.TryParse(obj.pureGoldWeight, out decimal pureGoldWeight);
                total = total + pureGoldWeight;
            }
            return Convert.ToString(Math.Round(total, 3));
        }

        public static string GetTotal(OrderDetails orderDetails)
        {
            decimal total = 0;

            _ = Decimal.TryParse(orderDetails.jewellRecivedWeight, out decimal jewellRecivedWeight);
            _ = Decimal.TryParse(orderDetails.detection, out decimal detection);
            _ = Decimal.TryParse(orderDetails.wastagePercentage, out decimal wastagePercentage);

            decimal goldWeightinJewellery = jewellRecivedWeight - detection;
            decimal wastage = goldWeightinJewellery * wastagePercentage / 100;
            total = goldWeightinJewellery + wastage;
            return Convert.ToString(Math.Round(total, 3));
        }

        public static string GetWastage(OrderDetails orderDetails)
        {
            _ = Decimal.TryParse(orderDetails.jewellRecivedWeight, out decimal jewellRecivedWeight);
            _ = Decimal.TryParse(orderDetails.detection, out decimal detection);
            _ = Decimal.TryParse(orderDetails.detection, out decimal wastagePercentage);

            decimal goldWeightinJewellery = jewellRecivedWeight - detection;
            decimal wastage = goldWeightinJewellery * wastagePercentage / 100;
            return Convert.ToString(Math.Round(wastage, 3));
        }

        public static string ConvertToPureGold(OrderDetails orderDetails)
        {
            decimal total;
            _ = Decimal.TryParse(orderDetails.grandTotal, out decimal grandTotal);
            _ = Decimal.TryParse(orderDetails.jewelPurity, out decimal jewelPurity);
            total = grandTotal * jewelPurity / 100;
            return Convert.ToString(Math.Round(total, 3));
        }

        public static string GetDetectionTotal(OrderDetails orderDetails)
        {
            decimal total = 0.000M;
            foreach (DetectionControl ctrl in orderDetails.detectionDetails)
            {
                if (!string.IsNullOrEmpty(ctrl.value))
                {
                    _ = Decimal.TryParse(ctrl.value, out decimal value);
                    total = total + value;
                }
            }
            return Convert.ToString(Math.Round(total, 3));
        }

        public static ObservableCollection<DetectionControl> GetDetectionControls(string entity = "", int orderId = 0)
        {
            ObservableCollection<DetectionControl> detectionControls = GetAllDetectionControls();

            if (orderId > 0)
            {
                string order_charges_details = "SELECT * FROM customer_order_detection_details where fk_customer_order_details_id = " + orderId;
                ObservableCollection<DetectionControl> detectionDetails = null;
                try
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, order_charges_details, CommandType.Text))
                        {
                            using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                            {
                                if (dataReader != null)
                                {
                                    detectionDetails = new ObservableCollection<DetectionControl>();
                                    while (dataReader.Read())
                                    {
                                        DetectionControl detection = new DetectionControl
                                        {
                                            id = Convert.ToInt32(dataReader["fk_detection_id"]),
                                            pkId = Convert.ToInt32(dataReader["id"]),
                                            value = Convert.ToString(dataReader["detection"]),
                                        };
                                        detectionDetails.Add(detection);
                                    }
                                }
                            }
                        }
                    }

                    if (detectionControls != null && detectionControls.Count > 0 && detectionDetails != null && detectionDetails.Count > 0)
                    {
                        foreach (DetectionControl ctrl in detectionControls)
                        {
                            foreach (DetectionControl chrgeVal in detectionDetails)
                            {
                                if (ctrl.id == chrgeVal.id)
                                {
                                    ctrl.value = chrgeVal.value;
                                    ctrl.pkId = chrgeVal.pkId;
                                    break;
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
            return detectionControls;
        }

        public static ObservableCollection<ChargesControl> GetChargesControls(string entity = "", int orderId = 0)
        {
            ObservableCollection<ChargesControl> chargesControls = GetAllChargesControls();

            if (orderId > 0)
            {
                string order_charges_details = "SELECT * FROM customer_order_charges_details where fk_order_details_id = " + orderId;
                ObservableCollection<ChargesControl> chargesDetails = null;
                try
                {
                    using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                    {
                        using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, order_charges_details, CommandType.Text))
                        {
                            using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                            {
                                if (dataReader != null)
                                {
                                    chargesDetails = new ObservableCollection<ChargesControl>();
                                    while (dataReader.Read())
                                    {
                                        ChargesControl charges = new ChargesControl
                                        {
                                            id = Convert.ToInt32(dataReader["fk_charges_id"]),
                                            pkId = Convert.ToInt32(dataReader["id"]),
                                            value = Convert.ToString(dataReader["charge"]),
                                        };
                                        chargesDetails.Add(charges);
                                    }
                                }
                            }
                        }
                    }

                    if (chargesControls != null && chargesControls.Count > 0 && chargesDetails != null && chargesDetails.Count > 0)
                    {
                        foreach (ChargesControl ctrl in chargesControls)
                        {
                            foreach (ChargesControl chrgeVal in chargesDetails)
                            {
                                if (ctrl.id == chrgeVal.id)
                                {
                                    ctrl.value = chrgeVal.value;
                                    ctrl.pkId = chrgeVal.pkId;
                                    break;
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
            return chargesControls;
        }

        public ObservableCollection<Customer> GetAllCustomerDetails(string isDeleted = "False")
        {
            string sqlQuery = string.Format("SELECT * FROM Customer where isDeleted = {0} Order by Name", isDeleted);
            NpgsqlDataReader dataReader = GetTableData(sqlQuery);
            ObservableCollection<Customer> CustomerDetails = null;
            try
            {
                if (dataReader != null)
                {
                    CustomerDetails = new ObservableCollection<Customer>();
                    while (dataReader.Read())
                    {
                        Customer customer = new Customer
                        {
                            id = Convert.ToInt32(dataReader["Id"]),
                            name = Convert.ToString(dataReader["name"]),
                            description = Convert.ToString(dataReader["description"]),
                            address = Convert.ToString(dataReader["address"]),
                            email = Convert.ToString(dataReader["email"]),
                            aadhaar = Convert.ToString(dataReader["aadhaar"]),
                            mobile = Convert.ToString(dataReader["mobile"]),
                            pan = Convert.ToString(dataReader["pan"]),
                            gender = Convert.ToString(dataReader["gender"]),
                            dob = Convert.ToString(dataReader["dob"]),
                            anniversary = Convert.ToString(dataReader["anniversary"]),
                            gst = Convert.ToString(dataReader["gst"])
                        };
                        CustomerDetails.Add(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return CustomerDetails;
        }

        public ObservableCollection<Employee> GetAllEmployeeDetails(string isDeleted = "False")
        {
            string sqlQuery = string.Format("SELECT * FROM employee where isDeleted = {0} Order by Name", isDeleted);
            NpgsqlDataReader dataReader = GetTableData(sqlQuery);
            ObservableCollection<Employee> employees = null;
            try
            {
                if (dataReader != null)
                {
                    employees = new ObservableCollection<Employee>();
                    while (dataReader.Read())
                    {
                        Employee employee = new Employee
                        {
                            id = Convert.ToInt32(dataReader["Id"]),
                            name = Convert.ToString(dataReader["name"]),
                            description = Convert.ToString(dataReader["description"]),
                            address = Convert.ToString(dataReader["address"]),
                            email = Convert.ToString(dataReader["email"]),
                            aadhaar = Convert.ToString(dataReader["aadhaar"]),
                            mobile = Convert.ToString(dataReader["mobile"]),
                            pan = Convert.ToString(dataReader["pan"]),
                            gender = Convert.ToString(dataReader["gender"]),
                        };
                        employees.Add(employee);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return employees;
        }

        public ObservableCollection<JewelType> GetAllLoadJewelTypes()
        {
            string sqlQuery = "SELECT * FROM ornament_type Order by Name";
            NpgsqlDataReader dataReader = GetTableData(sqlQuery);
            ObservableCollection<JewelType> JewelTypes = null;
            try
            {
                if (dataReader != null)
                {
                    JewelTypes = new ObservableCollection<JewelType>();
                    while (dataReader.Read())
                    {
                        JewelType jewelType = new JewelType
                        {
                            id = Convert.ToInt32(dataReader["Id"]),
                            name = Convert.ToString(dataReader["name"]),
                            description = Convert.ToString(dataReader["description"])
                        };
                        JewelTypes.Add(jewelType);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return JewelTypes;
        }

        public ObservableCollection<Gender> GetAllGenders()
        {
            string sqlQuery = "SELECT * FROM gender Order by id";
            NpgsqlDataReader dataReader = GetTableData(sqlQuery);
            ObservableCollection<Gender> GenderCollection = null;
            try
            {
                if (dataReader != null)
                {
                    GenderCollection = new ObservableCollection<Gender>();
                    while (dataReader.Read())
                    {
                        Gender gender = new Gender
                        {
                            id = Convert.ToInt32(dataReader["Id"]),
                            name = Convert.ToString(dataReader["gender"])
                        };
                        GenderCollection.Add(gender);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return GenderCollection;
        }

        public ObservableCollection<AdvanceType> GetAllAdvanceTypes()
        {
            string sqlQuery = "SELECT * FROM advance_type where is_active = True Order by id";
            NpgsqlDataReader dataReader = GetTableData(sqlQuery);
            ObservableCollection<AdvanceType> AdvanceTypesCollection = null;
            try
            {
                if (dataReader != null)
                {
                    AdvanceTypesCollection = new ObservableCollection<AdvanceType>();
                    while (dataReader.Read())
                    {
                        AdvanceType advanceType = new AdvanceType
                        {
                            id = Convert.ToInt32(dataReader["Id"]),
                            name = Convert.ToString(dataReader["name"]),
                            description = Convert.ToString(dataReader["description"])
                        };
                        AdvanceTypesCollection.Add(advanceType);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return AdvanceTypesCollection;
        }

        public static ObservableCollection<ChargesControl> GetAllChargesControls()
        {
            string sqlQuery = "SELECT * FROM charges_type Order by Id";
            ObservableCollection<ChargesControl> chargesControls = null;
            try
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, sqlQuery, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
                                chargesControls = new ObservableCollection<ChargesControl>();
                                while (dataReader.Read())
                                {
                                    ChargesControl charges = new ChargesControl
                                    {
                                        id = Convert.ToInt32(dataReader["Id"]),
                                        name = Convert.ToString(dataReader["name"]),
                                        description = Convert.ToString(dataReader["description"]),
                                        controlType = "TextBox",
                                        value = string.Empty
                                    };
                                    chargesControls.Add(charges);
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
            return chargesControls;
        }

        public static ObservableCollection<DetectionControl> GetAllDetectionControls()
        {
            string sqlQuery = "SELECT * FROM detection_type Order by Id";
            ObservableCollection<DetectionControl> detectionControls = null;
            try
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, sqlQuery, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
                                detectionControls = new ObservableCollection<DetectionControl>();
                                while (dataReader.Read())
                                {
                                    DetectionControl detection = new DetectionControl
                                    {
                                        id = Convert.ToInt32(dataReader["Id"]),
                                        name = Convert.ToString(dataReader["name"]),
                                        description = Convert.ToString(dataReader["description"]),
                                        controlType = "TextBox",
                                        value = string.Empty
                                    };
                                    detectionControls.Add(detection);
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
            return detectionControls;
        }

        public string GetNextOrderRefNo()
        {
            long orderRefNo = 1;
            string sqlQuery = "select max(id) as Id from orders";

            NpgsqlDataReader dataReader = GetTableData(sqlQuery);
            try
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        if (!(dataReader["id"] is DBNull))
                        {
                            orderRefNo = Convert.ToInt64(dataReader["Id"]);
                            orderRefNo = orderRefNo + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            return Convert.ToString(orderRefNo);
        }

        public static string Truncate(string value)
        {
            return value.Length <= MaxDescriptionLimit ? value : value.Substring(0, MaxDescriptionLimit) + "...";
        }

        public static void SaveImageForFixedSize(string srcPath, string destPath, int maximumWidth, int maximumHeight, bool enforceRatio = true, bool addPadding = true)
        {
            var image = System.Drawing.Image.FromFile(srcPath);
            var imageEncoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            var canvasWidth = maximumWidth;
            var canvasHeight = maximumHeight;
            var newImageWidth = maximumWidth;
            var newImageHeight = maximumHeight;
            var xPosition = 0;
            var yPosition = 0;


            if (enforceRatio)
            {
                var ratioX = maximumWidth / (double)image.Width;
                var ratioY = maximumHeight / (double)image.Height;
                var ratio = ratioX < ratioY ? ratioX : ratioY;
                newImageHeight = (int)(image.Height * ratio);
                newImageWidth = (int)(image.Width * ratio);

                if (addPadding)
                {
                    xPosition = (int)((maximumWidth - (image.Width * ratio)) / 2);
                    yPosition = (int)((maximumHeight - (image.Height * ratio)) / 2);
                }
                else
                {
                    canvasWidth = newImageWidth;
                    canvasHeight = newImageHeight;
                }
            }

            var thumbnail = new Bitmap(canvasWidth, canvasHeight);
            var graphic = Graphics.FromImage(thumbnail);

            if (enforceRatio && addPadding)
            {
                graphic.Clear(Color.White);
            }

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(image, xPosition, yPosition, newImageWidth, newImageHeight);

            thumbnail.Save(destPath, imageEncoders[1], encoderParameters);
        }

        public static string GetMotherBoardID()
        {
            string mbInfo = String.Empty;
            ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
            scope.Connect();
            ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());

            foreach (PropertyData propData in wmiClass.Properties)
            {
                if (propData.Name == "SerialNumber")
                    mbInfo = Convert.ToString(propData.Value);
            }

            return mbInfo;
        }

        public static ObservableCollection<Rate> GetRates()
        {
            string sqlQuery = "SELECT * FROM Rate Order by Id";
            ObservableCollection<Rate> Rates = null;
            try
            {
                using (NpgsqlConnection connection = DBWrapper.GetNpgsqlConnection())
                {
                    using (NpgsqlCommand cmd = DBWrapper.GetNpgsqlCommand(connection, sqlQuery, CommandType.Text))
                    {
                        using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            if (dataReader != null)
                            {
                                Rates = new ObservableCollection<Rate>();
                                while (dataReader.Read())
                                {
                                    Rate rate = new Rate
                                    {
                                        id = Convert.ToInt32(dataReader["Id"]),
                                        name = Convert.ToString(dataReader["name"]),
                                        rate = (int)Convert.ToDecimal(dataReader["rate"]),
                                        description = Convert.ToString(dataReader["description"]),
                                    };
                                    Rates.Add(rate);
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
            return Rates;
        }

        public static ObservableCollection<Rate> GetStandardMetalRates()
        {
            ObservableCollection<Rate> StandardRates = new ObservableCollection<Rate>();
            ObservableCollection<Rate> rates = Helper.GetRates();

            decimal pureGoldRate = rates.First(x => x.name.ToLower() == "gold").rate;
            Configuration.PureGoldRate = Convert.ToString(pureGoldRate);
            int silverRate = rates.First(x => x.name.ToLower() == "silver").rate;
            Configuration.SilverRate = Convert.ToString(silverRate);

            StandardRates.Add(new Rate { name = "24 K", rate = (int)Math.Round(pureGoldRate / 22 * 24), purity = 24, description = "24 K Gold :", id = 1 });
            StandardRates.Add(new Rate { name = "22 K", rate = (int)pureGoldRate, purity = 22, description = "22 K Gold :", id = 2 });
            StandardRates.Add(new Rate { name = "20 K", rate = (int)Math.Round(pureGoldRate / 22 * 20), purity = 20, description = "20 K Gold :", id = 3 });
            StandardRates.Add(new Rate { name = "18 K", rate = (int)Math.Round(pureGoldRate / 22 * 18), purity = 18, description = "18 K Gold :", id = 4 });
            StandardRates.Add(new Rate { name = "Silver", rate = silverRate, purity = 100, description = "Silver :", id = 5 });

            foreach (var obj in CommanDetails.user.userPreference.Rates)
            {
                foreach (var rate in StandardRates)
                {
                    if (obj.name == rate.name && obj.isChecked)
                    {
                        rate.isChecked = true;
                        break;
                    }
                }
            }
            return StandardRates;
        }

        public static bool IsValidEmail(string value)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            bool isValidEmail = regex.IsMatch(value);
            if (isValidEmail)
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDecimal(string value, decimal limit = 0)
        {
            if (decimal.TryParse(value, out decimal number) && number >= 1 && (limit <= 1 || number <= limit))
            {
                return true;
            }

            return false;
        }

        public static bool IsValidInteger(string value)
        {
            if (decimal.TryParse(value, out decimal number) && number >= 1)
            {
                return true;
            }
            return false;
        }

        public static decimal GetGoldCharges(OrderDetails orderDetails)
        {
            return Math.Round(Convert.ToDecimal(orderDetails.freezeRate) * Convert.ToDecimal(orderDetails.netWeight));
        }

        public static decimal GetGoldChargesWithWastage(OrderDetails orderDetails)
        {
            decimal goldCharge = GetGoldCharges(orderDetails);
            decimal goldWastage = Math.Round(goldCharge * Convert.ToDecimal(orderDetails.wastage) / 100);
            return goldCharge + goldWastage;
        }

        public static decimal GetCharges(OrderDetails orderDetails)
        {
            decimal totalcharges = 0;
            foreach (var charges in from charges in orderDetails.chargesDetails
                                    where !string.IsNullOrWhiteSpace(charges.value) && IsValidDecimal(charges.value)
                                    select charges)
            {
                totalcharges += Convert.ToDecimal(charges.value);
            }

            if (!string.IsNullOrWhiteSpace(orderDetails.makingCharge) && IsValidDecimal(orderDetails.makingCharge))
                totalcharges += Convert.ToDecimal(orderDetails.makingCharge);

            return totalcharges;
        }

        public static decimal GetEstimatedValue(OrderDetails orderDetails)
        {
            var t = (Convert.ToDecimal(Configuration.PureGoldRate) * Convert.ToDecimal(orderDetails.jewelPurity)) / 100;

            decimal goldCharge = t * Convert.ToDecimal(orderDetails.netWeight);

            decimal watageVal = 0;

            if (!string.IsNullOrWhiteSpace(orderDetails.wastage))
                watageVal = (goldCharge * Convert.ToDecimal(orderDetails.wastage)) / 100;

            return goldCharge + watageVal;
        }

        public static string FormatRupees(decimal rupee)
        {
            string formattedPrice = (Math.Round(rupee)).ToString("N", new CultureInfo("hi-IN"));
            return formattedPrice;
        }

        public static OrderDetails GenerateNewOrderDetailsInstance()
        {
            OrderDetails OrderDetails = new OrderDetails();
            OrderDetails.id = 0;
            OrderDetails.subOrderNo = GetGuid();
            OrderDetails.rateFreezeDate = DateTime.Now.ToString();
            OrderDetails.freezeRate = Configuration.PureGoldRate;
            OrderDetails.orderDate = DateTime.Now.ToString();
            OrderDetails.detectionDetails = GetAllDetectionControls();
            OrderDetails.chargesDetails = GetAllChargesControls();
            return OrderDetails;
        }

        public static string GetGuid()
        {
            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString().Substring(1, 6);
            uuid += DateTime.Now.ToString("yyyyMMddHHmmss");
            return uuid;
        }
    }

    public static class CloneObject
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }

    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }
    }
}

//public ObservableCollection<Purity> GetAllPurityDetails(string metal = "Gold")
//{
//    string sqlQuery = string.Format("SELECT * FROM purity where metal = '{0}' Order by purity", metal);
//    NpgsqlDataReader dataReader = GetTableData(sqlQuery);
//    ObservableCollection<Purity> puritys = null;
//    try
//    {
//        if (dataReader != null)
//        {
//            puritys = new ObservableCollection<Purity>();
//            while (dataReader.Read())
//            {
//                Purity purity = new Purity
//                {
//                    id = Convert.ToInt32(dataReader["id"]),
//                    metalType = Convert.ToString(dataReader["metal"]),
//                    description = Convert.ToString(dataReader["description"]),
//                    purity = Convert.ToString(dataReader["purity"]),
//                    karat = Convert.ToString(dataReader["karat"]),
//                    displayText = Convert.ToString(dataReader["display_text"]),
//                };
//                puritys.Add(purity);
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        Logger.LogError(ex.Message);
//    }
//    return puritys;
//}

//public ICollection<T> getSortedData(ICollection<T> collection, string property, string direction)
//{
//    switch (direction.Trim())
//    {
//        case "asc":
//            collection = ((from n in collection
//                           orderby
//                           n.GetType().GetProperty(property).GetValue(n, null)
//                           select n).ToList<T>()) as ICollection<T>;
//            break;
//        case "desc":
//            collection = ((from n in collection
//                           orderby
//                           n.GetType().GetProperty(property).GetValue(n, null)
//                           descending
//                           select n).ToList<T>()) as ICollection<T>;
//            break;
//    }
//    return collection;
//}


//public sealed class AppSharedContext
//{
//    AppSharedContext() { }
//    private static readonly object _lock = new object();
//    private static AppSharedContext instance = null;
//    public static AppSharedContext Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                lock (_lock)
//                    {
//                        if (instance == null)
//                        {
//                            instance = new AppSharedContext();
//                        }
//                    }
//            }
//            return instance;
//        }
//    }
//}



//chargesControls.Add(new ChargesControl { id = 1, name = "HallMarking", description = "Hall Marking", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 2, name = "Enamal", description = "Enamal", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 3, name = "Polish", description = "Polish", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 4, name = "StoneFitting", description = "Stone Fitting", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 5, name = "Melting", description = "Melting", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 6, name = "Making", description = "Making", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 7, name = "StoneCost", description = "Stone Cost", controlType = "TextBox", value = 0.0M });
//chargesControls.Add(new ChargesControl { id = 8, name = "Others", description = "Others", controlType = "TextBox", value = 0.0M });


//public SelectedOrderDetails CloneOrderDetails(SelectedOrderDetails orderDetails)
//{
//    SelectedOrderDetails order = new SelectedOrderDetails();
//    order.seal = !string.IsNullOrEmpty(orderDetails.seal) ? orderDetails.seal : string.Empty;
//    order.quantity = !string.IsNullOrEmpty(orderDetails.quantity) ? orderDetails.quantity : string.Empty;
//    order.size = !string.IsNullOrEmpty(orderDetails.size) ? orderDetails.size : string.Empty;
//    order.wastage = !string.IsNullOrEmpty(orderDetails.wastage) ? orderDetails.wastage : string.Empty;
//    order.wastagePercentage = !string.IsNullOrEmpty(orderDetails.wastagePercentage) ? orderDetails.wastagePercentage : string.Empty;
//    order.jewelPurity = !string.IsNullOrEmpty(orderDetails.jewelPurity) ? orderDetails.jewelPurity : string.Empty;
//    order.attachement = !string.IsNullOrEmpty(orderDetails.attachement) ? orderDetails.attachement : string.Empty;
//    order.description = !string.IsNullOrEmpty(orderDetails.description) ? orderDetails.description : string.Empty;
//    order.detection = !string.IsNullOrEmpty(orderDetails.detection) ? orderDetails.detection : string.Empty;
//    order.dueDate = !string.IsNullOrEmpty(orderDetails.dueDate) ? orderDetails.dueDate : string.Empty;
//    order.orderDate = !string.IsNullOrEmpty(orderDetails.orderDate) ? orderDetails.orderDate : string.Empty;
//    order.jewellRecivedWeight = !string.IsNullOrEmpty(orderDetails.jewellRecivedWeight)  ? orderDetails.jewellRecivedWeight : string.Empty;
//    order.netWeight = !string.IsNullOrEmpty(orderDetails.netWeight) ? orderDetails.netWeight : string.Empty;
//    order.subOrderNo = !string.IsNullOrEmpty(orderDetails.subOrderNo) ? orderDetails.subOrderNo : string.Empty;
//    return order;
//}

//public AdvanceDetails CloneAdvanceDetails(AdvanceDetails advanceDetails)
//{
//    AdvanceDetails advance = new AdvanceDetails();
//    advance.advanceDate = !string.IsNullOrEmpty(advanceDetails.advanceDate) ? advanceDetails.advanceDate : string.Empty;
//    advance.description = !string.IsNullOrEmpty(advanceDetails.description) ? advanceDetails.description : string.Empty;
//    advance.goldPurity = advanceDetails.goldPurity > 0 ? advanceDetails.goldPurity : 0;
//    advance.goldWeight = advanceDetails.goldWeight > 0 ? advanceDetails.goldWeight : 0;
//    advance.id = advanceDetails.id > 0 ? advanceDetails.id : 0;
//    return advance;
//}