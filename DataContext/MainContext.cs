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

        public MainContext(MpdServer mpd)
        {
            _mainWindowDataContext = new MainWindowDataContext(mpd);
            _settingsDataContext = new SettingsWindowDataContext();
        }

        public MainWindowDataContext MainWindow { get => _mainWindowDataContext; } 
        public SettingsWindowDataContext Settings { get => _settingsDataContext; }
    }
}
