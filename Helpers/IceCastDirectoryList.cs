using Orpheus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Threading;
using System.ComponentModel;
using Orpheus.DataContext;

namespace Orpheus.Helpers
{
    class IceCastDirectoryList
    {
        private readonly string listPath;
        public static Dictionary<int, IceCastStream> Stations = new Dictionary<int, IceCastStream>();
        private static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static string defaultYpPath = Path.Combine(baseDir, "yp.xml");
        private static string ypFileUrl = "http://dir.xiph.org/yp.xml";

        public IceCastDirectoryList() : this(defaultYpPath)
        {
            try
            {
                Task.Factory.StartNew(() => UpdateYpFile());
            }
            catch { }
        }

        private void UpdateYpFile()
        {
            if (!File.Exists(defaultYpPath))
            {
                DownloadYpFile();
            }
            else if(File.GetCreationTime(defaultYpPath).Date < DateTime.Now.Date)
            {
                DownloadYpFile();
            }

            //Parse();
        }

        private void DownloadYpFile()
        {
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                client.DownloadFileTaskAsync(ypFileUrl, defaultYpPath);
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Parse();
            
        }

        public IceCastDirectoryList(string listPath)
        {
            this.listPath = listPath;
            Parse();
        }

        private void Parse()
        {
            int i = 0;

            if (!File.Exists(listPath) || new FileInfo(listPath).Length == 0) return;

            try
            {
                using (XmlReader reader = XmlReader.Create(listPath))
                {
                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "entry")
                        {
                            XElement el = (XElement)XNode.ReadFrom(reader);

                            Stations.Add(i++, new IceCastStream
                            {
                                Name = $"{el.Elements("server_name").FirstOrDefault()?.Value} [{el.Elements("bitrate").FirstOrDefault()?.Value}]",
                                ServerName = el.Elements("server_name").FirstOrDefault()?.Value,
                                ListenUrl = el.Elements("listen_url").FirstOrDefault()?.Value,
                                Url = el.Elements("listen_url").FirstOrDefault()?.Value,
                                ServerType = el.Elements("server_type").FirstOrDefault()?.Value,
                                Bitrate = el.Elements("bitrate").FirstOrDefault()?.Value,
                                Channels = el.Elements("chanells").FirstOrDefault()?.Value,
                                SampleRate = el.Elements("samplerate").FirstOrDefault()?.Value,
                                Genre = el.Elements("genre").FirstOrDefault()?.Value,
                                CurrentSong = el.Elements("current_song").FirstOrDefault()?.Value
                            });
                        }
                    }
                }

                MainContext.Instance.MainWindow.IceCastStreams = Stations.OrderBy(x => x.Value.ServerName).ToDictionary(x => x.Key, x => x.Value);
            }
            catch { }
        }
    }
}
