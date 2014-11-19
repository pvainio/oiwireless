using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OI_Wireless_Camera
{
    class MediaFile
    {
        private static Settings settings = Settings.Instance;

        private string parent;
        public string Name {get; private set;}
        private int size;
        private int type;
        public int Date { get; private set;}
        public int Time { get; private set; }
        public bool Checked { get; set; }

        public int Timestamp
        {
            get {
                return Date * 0xffff + Time;
            }
        }

        public string Image
        {
            get
            {
                string src = "http://192.168.0.10/get_thumbnail.cgi?DIR=" + parent + "/" + Name;
                return src;
            }
        }

        public string Path
        {
            get
            {
                return parent + "/" + Name;
            }
        }

        public DateTime DateTime
        {
            get
            {
                DateTime dt = new DateTime(1966, 7, 10, 0, 0, 0, 0, DateTimeKind.Local);
                dt = dt.AddDays(Date);
                dt = dt.AddHours(Convert.ToDouble(Time) / 2048);
                return dt;
            }
        }


        public MediaFile(string parent, string name, int size, int type, int date, int time)
        {
            this.parent = parent;
            this.Name = name;
            this.size = size;
            this.type = type;
            this.Date = date;
            this.Time = time;
        }

        public string DLURL
        {
            get
            {
                if (settings.downloadSize.Equals("Original", StringComparison.OrdinalIgnoreCase))
                {
                    return settings.BaseURL + Path;
                }
                else
                {
                    return settings.BaseURL + "/get_resizeimg.cgi?DIR=" + Path + "&size=" + settings.downloadSize;
                }
            }
        }
    }
}
