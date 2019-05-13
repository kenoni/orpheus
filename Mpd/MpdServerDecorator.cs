using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orpheus.Mpd
{
    class MpdServerDecorator : MpdServerBase
    {
        protected readonly MpdServerBase _server;

        public MpdServerDecorator(MpdServerBase server)
        {
            _server = server;
        }
        public override string ConnectionAsString => _server.ConnectionAsString;
    }
}
