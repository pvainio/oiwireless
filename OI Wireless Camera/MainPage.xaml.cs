using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OI_Wireless_Camera.Resources;
using System.IO;
using System.Xml.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using System.Windows.Threading;

namespace OI_Wireless_Camera
{
    public partial class MainPage : PhoneApplicationPage
    {
        private int connectionAttempts = 0;
        private Settings settings = Settings.Instance;

        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DownloadNew.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (settings.DownloadCount > 30 && !settings.HasReviewed)
            {
                reviewPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                reviewPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                cameraStatus.Text = "Connected: debugger";
                progressBar.IsIndeterminate = false;
                downloadButton.IsEnabled = true;
                CheckConnection(true);
            }
            else
            {
                CheckConnection(true);
            }
        }

        private void CheckConnection(bool resetRetryCounter = false)
        {
            if (resetRetryCounter) connectionAttempts = 0;
            connectionAttempts++;
            if (connectionAttempts > 30)
            {
                progressBar.IsIndeterminate = false;
                cameraStatus.Text = "No camera found";
                return;
            }
            progressBar.IsIndeterminate = true;
            cameraStatus.Text = "Searching for camera";
          
                HttpWebRequest request = HttpWebRequest.CreateHttp(settings.BaseURL + "/get_caminfo.cgi");
                request.BeginGetResponse(callback, request);
        }

      

        private void callback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest) ar.AsyncState;

            try
            {
                using (WebResponse response = request.EndGetResponse(ar))
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    XDocument asd = XDocument.Load(responseStream);
                    string camera = asd.Element("caminfo").Element("model").Value;
                    response.Close();
                    Dispatcher.BeginInvoke(() =>
                    {
                        cameraStatus.Text = "Connected: " + camera;
                        progressBar.IsIndeterminate = false;
                        downloadButton.IsEnabled = true;
                    }
                    );
                }
 
            }
            catch (Exception)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = new TimeSpan(0, 0, 1);
                    timer.Start();
                });

            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            CheckConnection();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionSettingsTask ct = new ConnectionSettingsTask();
            ct.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            ct.Show();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void Roll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Review_Click(object sender, RoutedEventArgs e)
        {
            settings.HasReviewed = true;
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "oiwireless@outlook.com";
            emailComposeTask.Subject = "OI Wireless Feedback";
            emailComposeTask.Show();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}