using System;
using System.Collections.Generic;
using Orpheus.Models;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public interface  IMpdServer
    {
        void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null);
        string ConnectionAsString { get; }
    }
}