using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OI_Wireless_Camera
{
    class Settings
    {
        private static Settings instance;

        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        private Settings() { }

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }

        public string downloadSize
        {
            get
            {
                if (settings.Contains("saveSize"))
                {
                    return settings["saveSize"] as string;
                }
                else
                {
                    return "1024";
                }

            }
            set
            {
                settings["saveSize"] = value;
                settings.Save();
            }
        }

        public string BaseURL { 
            get {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return "http://" + Server;
                }
                else
                {
                    return "http://" + Server;
                }
            } 
        }

        public string Server = "192.168.0.10";

        public void IncreaseDownloadCount(int count)
        {
            int oldCount = 0;
            if (settings.Contains("dlCount"))
            {
                oldCount = (int) settings["dlCount"];
            }
            oldCount += count;
            settings["dlCount"] = oldCount;
            settings.Save();
        }

        public int DownloadCount
        {
            get
            {
                int oldCount = 0;
                if (settings.Contains("dlCount"))
                {
                    oldCount = (int)settings["dlCount"];
                }
                return oldCount;
            }
        }

        public bool HasReviewed { 
            get {
                if (!settings.Contains("reviewed")) return false;
                return (bool) settings["reviewed"];
            }
           set {
               settings["reviewed"] = value;
               settings.Save();
           }
        }
    }
}
