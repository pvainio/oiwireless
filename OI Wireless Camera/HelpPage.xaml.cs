using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Reflection;

namespace OI_Wireless_Camera
{
    public partial class HelpPage : PhoneApplicationPage
    {
        public HelpPage()
        {
            InitializeComponent();
            AssemblyName an = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            version.Text = "OI Wireless version " + an.Version;
       
     }

        private void contactTwitter_Click(object sender, RoutedEventArgs e)
        {
            ShareStatusTask t = new ShareStatusTask();
            t.Status = "@oiwireless ";
            t.Show();
        }

        private void contactEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "oiwireless@outlook.com";
            emailComposeTask.Subject = "OI Wireless Feedback";
            emailComposeTask.Show();
        }
    }
}