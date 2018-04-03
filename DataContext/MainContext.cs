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
        private readonly MainWindowDataContext _mainWindowDataContext = null;
        private readonly SettingsWindowDataContext _settingsDataContext = null;

        private MainContext()
        {
            _mainWindowDataContext = new MainWindowDataContext();
            _settingsDataContext = new SettingsWindowDataContext();
        }

        private static MainContext _instance;

        public static MainContext Instance => _instance ?? (_instance = new MainContext());

        public MainWindowDataContext MainWindow { get => _mainWindowDataContext; } 
        public SettingsWindowDataContext Settings { get => _settingsDataContext; }
    }
}
