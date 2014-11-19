using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace OI_Wireless_Camera
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private Settings settings = OI_Wireless_Camera.Settings.Instance;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            this.saveSize.SelectedItem = settings.downloadSize;
        }

        private void Settings_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.saveSize.SelectedItem == null) return;
            settings.downloadSize = this.saveSize.SelectedItem as string;
        }
        
       
    }
}