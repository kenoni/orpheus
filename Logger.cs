using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Orpheus
{
    public static class Logger
    {
        private static ILog _log4NetLogger = LogManager.GetLogger(typeof(App));

        public static void Info(string message)
        {
            _log4NetLogger?.Info(message);
        }
    }
}
