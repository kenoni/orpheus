using Orpheus.Mpd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orpheus.DataContext
{
    class MainContext
    {
        private MainContext()
        {
            MainWindow = new MainWindowDataContext();
            Settings = new SettingsWindowDataContext();
        }

        private static MainContext _instance;

        public static MainContext Instance => _instance ?? (_instance = new MainContext());

        public MainWindowDataContext MainWindow { get; } 
        public SettingsWindowDataContext Settings { get; }
    }
}
