/*This file is part of Orpheus.

   Orpheus is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Orpheus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Orpheus.  If not, see<http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orpheus.Mpd
{
    public static class MpdHelper
    {
        public static bool IsConnected(this Socket client)
        {
            
            // This is how you can determine whether a socket is still connected.
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                client.Blocking = false;
                client.Send(tmp, 0, 0);

                DataContext.MainContext.Instance.MainWindow.MpdConnected = true;

                return true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                
                DataContext.MainContext.Instance.MainWindow.MpdConnected = (e.NativeErrorCode.Equals(10035));
                return (e.NativeErrorCode.Equals(10035));
                
            }
            finally
            {
                client.Blocking = blockingState;
            }
        }

        public static string[] ToMpdAddress(this string address)
        {
            var mpdAddressRegEx = new Regex(@"([\S]+)[:]{1}([\d]+)");
            var matchMpdAddress = mpdAddressRegEx.Match(address);

            return (matchMpdAddress.Success) ? new[] { matchMpdAddress.Groups[1].Value, matchMpdAddress.Groups[2].Value }
                                             : new[] { address };
        }
    }
}
