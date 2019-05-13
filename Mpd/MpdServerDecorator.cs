using System;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public class MpdServerDecorator : MpdServerBase
    {
        protected readonly MpdServerBase _server;

        public MpdServerDecorator(MpdServerBase server)
        {
            _server = server;
        }

        public override string ConnectionAsString => _server.ConnectionAsString;

        public override void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null)
        {
            _server.RunCommand(message, task, callback);
        }
    }
}
