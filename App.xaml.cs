using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
        private const string eventName = "84bb9974-fb13-4927-bf47-91f9fca1601c";
        private EventWaitHandle singleInstanceEvent;


        protected override void OnStartup(StartupEventArgs e)
        {
            bool created = false;
            singleInstanceEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName, out created);

            if (!created)
            {
                singleInstanceEvent.Set();
                Shutdown();
            }
            else
            {
                SynchronizationContext ctx = SynchronizationContext.Current;
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        singleInstanceEvent.WaitOne();
                        ctx.Post(_ => MakeActiveApplication(), null);
                    }
                });
            }


            base.OnStartup(e);
        }

        private void MakeActiveApplication()
        {
            MainWindow.Activate();
            MainWindow.Topmost = true;
            MainWindow.Topmost = false;
            MainWindow.Focus();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();

            Logger.Info("Application started.");
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

            Logger.Info(message);
        }
    }

}
