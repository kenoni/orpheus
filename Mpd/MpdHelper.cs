﻿/*This file is part of Orpheus.

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
using System.Text.RegularExpressions;

namespace Orpheus.Mpd
{
    internal static class MpdHelper
    {
        public static string[] ToMpdAddress(this string address)
        {
            var mpdAddressRegEx = new Regex(@"([\S]+)[:]{1}([\d]+)[:]([\S]+)");
            var matchMpdAddress = mpdAddressRegEx.Match(address);

            return (matchMpdAddress.Success) ? new[] { matchMpdAddress.Groups[1].Value, matchMpdAddress.Groups[2].Value, matchMpdAddress.Groups[3].Value }
                                             : new[] { address };
        }
    }
}
