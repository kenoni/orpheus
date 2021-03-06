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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orpheus.Mpd
{
    public class MpdResponse
    {
        private readonly Regex _okRegex = new Regex(@"^OK$", RegexOptions.Compiled);
        private readonly Regex _ackRegex = new Regex(@"^ACK \[(?<error>[^@]+)@(?<command_listNum>[^\]]+)] {(?<command>[^}]*)} (?<message>.*)", RegexOptions.Compiled);

        private string _errorMessage;

        public IList<string> Lines { get; } = new List<string>();

        public bool AddLine(string line)
        {
            var matchResponseEnd = _okRegex.Match(line);

            if (matchResponseEnd.Success)
            {
                return false;
            }
            
            var matchError = _ackRegex.Match(line);
            if (matchError.Success)
            {
                _errorMessage = matchError.Groups["message"].Value;
                return false;
            }

            Lines.Add(line);

            return true;
        }
    }
}
