using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace JewellPro
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        ReportStatus reportStatus { get; set; }
        Customer customer { get; set; }
        public ConfirmationDialog(Customer user, ReportStatus reportStatus)
        {
            InitializeComponent();
            this.reportStatus = reportStatus;
            this.customer = user;
            txtEmail.Text = user.email;
            txtMobile.Text = user.mobile;
            txtReportFileLocation.Text = reportStatus.reportPath;
            hyperLinkReportFileLocation.NavigateUri = new Uri(reportStatus.reportPath, UriKind.Absolute);
            if (string.IsNullOrWhiteSpace(user.email) && string.IsNullOrWhiteSpace(user.mobile))
            {
                btnOk.IsEnabled= false;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fromAddress = new MailAddress("murali.ash@gmail.com", "Varsid ");
                var toAddress = new MailAddress("muralidharan_it@hotmail.com", "To Varsid");
                const string fromPassword = "liveHappy123";
                const string subject = "test";
                const string body = "Hey now!!\\n Hello Test";

                MailMessage mail = new MailMessage();
                mail.From = fromAddress;
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.Body = body;

                Attachment attachment = new Attachment(reportStatus.reportPath);
                mail.Attachments.Add(attachment);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 20000
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
                {
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            //const string accountSid = "ACc4f51bb3764b3ee1d99b9f90cb3c98dc";
            //const string authToken = "fafeddf0b1e46f3f76369eccea061fab";
            //TwilioClient.Init(accountSid, authToken);
            //var mediaUrl = new[] {
            //        new Uri(reportStatus.reportPath)
            //    }.ToList();

            //var message = MessageResource.Create(mediaUrl: mediaUrl,
            //    from: new Twilio.Types.PhoneNumber("whatsapp:+919715286757"), 
            //    body: "Profile", 
            //    to: new Twilio.Types.PhoneNumber("whatsapp:+919715286757"));

            //Console.WriteLine(message.Sid);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenReport(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = this.reportStatus.reportPath;
            proc.Start();
        }
    }
}
