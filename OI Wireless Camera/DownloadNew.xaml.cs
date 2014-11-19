using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace OI_Wireless_Camera
{
    public partial class DownloadNew : PhoneApplicationPage
    {
        private List<MediaFile> files = new List<MediaFile>();
        private Queue<string> dirsToProcess = new Queue<string>();
        private Settings settings = Settings.Instance;
        private bool downloading = false;

        public DownloadNew()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            files.Clear();
            dirsToProcess.Clear();
            dirsToProcess.Enqueue("/DCIM");
            processRemainingDirs();
            
        }

        private void processRemainingDirs()
        {
            if (!dirsToProcess.Any())
            {
                processFiles();
                return;
            } 
            string nextDir = dirsToProcess.Dequeue();
            HttpWebRequest request = HttpWebRequest.CreateHttp(settings.BaseURL + "/get_imglist.cgi?DIR=" + nextDir);
            request.BeginGetResponse(callback, request);
        }

        private void processFiles()
        {
            if (!files.Any())
            {
                // No files, display instructions
                Dispatcher.BeginInvoke(() => {
                    filesSelector.Visibility = System.Windows.Visibility.Collapsed;
                    textInstruction.Visibility = System.Windows.Visibility.Visible;
                    dlProgressBar.IsIndeterminate = false;
                    dlProgressBar.Value = 0;
                    dlStatus.Text = "No photos found.";
 
                });
                return;
            }

            files = files.OrderBy(t => -t.Timestamp).ToList();
            List<DateGroup> dateGrouped = DateGroup.Create(files);

            Dispatcher.BeginInvoke(() => {
                dlProgressBar.IsIndeterminate = false;
                dlProgressBar.Value = 0;
                dlStatus.Text = "Select photos";
                filesSelector.ItemsSource = dateGrouped;
            });
        }

        private void callback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            try
            {
                using (WebResponse response = request.EndGetResponse(ar))
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    String version = reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        processResponseLine(line);
                    }

                    processRemainingDirs();
                }
            }
            catch (Exception)
            {
                //throw;
                // DIR not found, skip it
                processRemainingDirs();
            }

        }

        private void processResponseLine(string line)
        {
            string[] split = line.Split(',');
            string parent = split[0];
            string name = split[1];
            int size = Convert.ToInt32(split[2]);
            int type = Convert.ToInt32(split[3]);
            int date = Convert.ToInt32(split[4]);
            int time = Convert.ToInt32(split[5]);
            if ((type & 0x10) != 0) // directory
            {
                dirsToProcess.Enqueue(parent + "/" + name);
            }
            else
            {
                if (name.EndsWith("jpg") || name.EndsWith("JPG"))
                {
                    // Only jpeg for now
                    files.Add(new MediaFile(parent, name, size, type, date, time));
                }
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Task t = DownloadFiles();
        }

        private async Task DownloadFiles()
        {
            System.Collections.IList items2 = filesSelector.SelectedItems;
            List<MediaFile> itemsToDl = new List<MediaFile>(items2.OfType<MediaFile>());
            int totalSize = itemsToDl.Count;
            if (totalSize < 1) return;

            downloading = true;

            int errors = 0;

            Dispatcher.BeginInvoke(() =>
            {
                saveBtn.IsEnabled = false;
                dlProgressBar.IsIndeterminate = true;
                dlProgressBar.Value = 0;
                filesSelector.IsEnabled = false;
                dlStatus.Text = "Transferring...";
            });

            MediaLibrary lib = new MediaLibrary();
            int count = 0;
            foreach (MediaFile mf in itemsToDl)
            {
                bool error = false;
                try
                {
                    await downloadFile(mf, lib);
                }
                catch (Exception e)
                {
                    error = true;
                    System.Diagnostics.Debug.WriteLine("Exception " + e.Message);
                    System.Diagnostics.Debug.WriteLine("Exception " + e.StackTrace);
                    errors++;
                }
                count++;
                Dispatcher.BeginInvoke(() =>
                {
                    if (!error) filesSelector.SelectedItems.Remove(mf);
                    dlProgressBar.IsIndeterminate = false;
                    dlProgressBar.Value = 100*count/totalSize;
                });

            }

            downloading = false;

            Dispatcher.BeginInvoke(() =>
            {
                if (errors > 0)
                {
                    CustomMessageBox error = new CustomMessageBox()
                    {
                        Caption = "Errors downloading photos",
                        Message = "Unable to download " + errors + "/" + totalSize + " photos.  Photos not loaded are still checked for retry.",
                        RightButtonContent = "OK"
                    };
                    error.Show();
                }
                else
                {
                    settings.IncreaseDownloadCount(totalSize);
                }
                filesSelector.IsEnabled = true;
                saveBtn.IsEnabled = true;
                dlStatus.Text = "Select photos";
            }); 
        }

        private async Task downloadFile(MediaFile mf, MediaLibrary lib)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(GetDLURL(mf));
            WebResponse response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
            using (Stream rstream = response.GetResponseStream())
            using (Stream buffer = new MemoryStream())
            {
                await rstream.CopyToAsync(buffer);
                System.Diagnostics.Debug.WriteLine("To lib " + mf.Name);
                buffer.Seek(0, SeekOrigin.Begin);
                try
                {
                    lib.SavePictureToCameraRoll(mf.Name, buffer);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    System.Diagnostics.Debugger.Break();
                    throw;
                }
            }
        }

        private string GetDLURL(MediaFile mf)
        {
            return mf.DLURL;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image src = (Image)sender;
            src.Source = null;
            src.SetBinding(Image.SourceProperty,new Binding("Image"));
        }

        private void filesSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            saveBtn.IsEnabled = !downloading && filesSelector.SelectedItems.Count > 0;
        }

    }
}