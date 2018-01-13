using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using log4net;
using log4net.Repository.Hierarchy;

namespace Orpheus
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application

    {
        private static  ILog _log4NetLogger;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _log4NetLogger = LogManager.GetLogger(typeof(App));
            log4net.Config.XmlConfigurator.Configure();

            LogMessage("Application started.");
            Dispatcher.UnhandledException += LogUnhandledExceptions;
        }

        private static volatile bool _insideUnhandledExceptionHandler;
        static void LogUnhandledExceptions(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            if (_insideUnhandledExceptionHandler) return;

            _insideUnhandledExceptionHandler = true;

            try
            {
                LogException(args.Exception);
            }
            catch
            {
                // You have to catch all exceptions inside this method
            }
            finally
            {
                _insideUnhandledExceptionHandler = false;
            }
        }

        public static void LogException(Exception e)
        {
            string exceptionMessage = (!string.IsNullOrEmpty(e.Message)) ? e.Message : string.Empty;
            string message = $"{exceptionMessage} {Environment.NewLine}  {e.StackTrace}";
            message = (message.Length > 2000) ? message.Substring(0, 2000) : message;

            _log4NetLogger?.Error(message);
        }
        public static void LogMessage(string message)
        {
            _log4NetLogger?.Info(message);
        }

    }

}
