using System;
using Orpheus.Mpd.Commands;

namespace Orpheus.Mpd
{
    public class MpdServerDecorator : IMpdServer
    {
        protected readonly IMpdServer _server;

        public MpdServerDecorator(IMpdServer server)
        {
            _server = server;
        }

        public string ConnectionAsString => _server.ConnectionAsString;

        public void RunCommand<T>(string message, IMpdCommand<T> task, Action<T> callback = null)
        {
            _server.RunCommand(message, task, callback);
        }
    }
}
