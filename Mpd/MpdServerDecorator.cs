using System;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public class MpdServerDecorator : MpdServerBase
    {
        protected readonly MpdServerBase Server;

        protected MpdServerDecorator(MpdServerBase server)
        {
            Server = server;
        }

        public override string ConnectionAsString => Server.ConnectionAsString;

        public override void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null)
        {
            Server.RunCommand(message, task, callback);
        }
    }
}
