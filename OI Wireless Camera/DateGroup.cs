using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OI_Wireless_Camera
{
    class DateGroup : List<MediaFile>
    {
        public DateGroup() : base()
        {
        }

        public string Name
        {
            get
            {
                // Build name from date
                MediaFile lastItem = this[this.Count - 1];
                DateTime dt = lastItem.DateTime;
                return dt.ToShortDateString() + " " + dt.ToShortTimeString();
            }
            set { }
        }

        /**
         * Convert List of MediaFiles to DateGrouped list.
         */
        internal static List<DateGroup> Create(List<MediaFile> files)
        {
            List<DateGroup> groups = new List<DateGroup>();
            MediaFile previousFile = null;
            foreach (MediaFile mediaFile in files)
            {
                if (previousFile == null || IsItTimeForNewGroup(previousFile, mediaFile))
                {
                    groups.Add(new DateGroup());
                }

                groups.Last().Add(mediaFile);
                previousFile = mediaFile;
            }
            return groups;
        }

        private static bool IsItTimeForNewGroup(MediaFile previousFile, MediaFile mediaFile)
        {
            if (previousFile == null) return true;

            int timeDiff = previousFile.Timestamp - mediaFile.Timestamp;
            if (timeDiff > 2048)
            {
                return true;
            }
            return false;
        }

    }
}
