using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Orpheus.Helpers
{
    class IceCastParser
    {
        

        public static string GetCurrentlyPlaying(string url)
        {
            return GetHttpStream(url);
        }
        public static Task<string> GetCurrentlyPlayingAsync(string url)
        {
            return Task.Factory.StartNew(() => GetCurrentlyPlaying(url));
        }

        private static void ProcessStreamData(ref int offset, int length)
        {
            if (length < 1)
                return;

            offset += length;
        }

        static string ExtractTitle (string title)
        {
            Match match = Regex.Match(title, @"StreamTitle='([\s\S]+)'");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            
            return title;
        }

        private static string GetHttpStream(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("icy-metadata", "1");
                request.ReadWriteTimeout = 10 * 1000;
                request.Timeout = 10 * 1000;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    int metaInt = 0;
                    if (!string.IsNullOrEmpty(response.GetResponseHeader("icy-metaint")))
                        metaInt = Convert.ToInt32(response.GetResponseHeader("icy-metaint"));

                    using (Stream socketStream = response.GetResponseStream())
                    using (MemoryStream metadataData = new MemoryStream())
                    {
                        byte[] buffer = new byte[16384];
                        int metadataLength = 0;
                        int streamPosition = 0;
                        int bufferPosition = 0;
                        int readBytes = 0;

                        
                        if (bufferPosition >= readBytes)
                        {
                            Thread.Sleep(1000);
                            readBytes = socketStream.Read(buffer, 0, buffer.Length);
                            bufferPosition = 0;
                        }
                        if (readBytes <= 0)
                        {
                            return string.Empty;
                        }

                        if (metadataLength == 0)
                        {
                            if (metaInt == 0 || streamPosition + readBytes - bufferPosition <= metaInt)
                            {
                                streamPosition += readBytes - bufferPosition;
                                ProcessStreamData(ref bufferPosition, readBytes - bufferPosition);
                                return string.Empty;
                            }

                            ProcessStreamData(ref bufferPosition, metaInt - streamPosition);
                            metadataLength = Convert.ToInt32(buffer[bufferPosition++]) * 16;
                            if (metadataLength == 0) return string.Empty;
                        }

                        while (bufferPosition < readBytes)
                        {
                            metadataData.WriteByte(buffer[bufferPosition++]);
                            metadataLength--;
                            if (metadataLength == 0)
                            {
                                var metadataBuffer = metadataData.ToArray();
                                return ExtractTitle(Encoding.UTF8.GetString(metadataBuffer));
                            }
                        }
                    }
                }
            }
            catch { }

            return string.Empty;
        }
    }
}
