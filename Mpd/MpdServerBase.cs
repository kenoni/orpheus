using System;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public abstract class  MpdServerBase
    {
        public abstract void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null);
        public abstract string ConnectionAsString { get; }
    }
}