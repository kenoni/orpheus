/*This file is part of Orpheus.

   Orpheus is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Orpheus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Orpheus.  If not, see<http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Orpheus.Properties;
using Orpheus.DataContext;
using CSCore.CoreAudioAPI;

namespace Orpheus
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private MainWindow m_Parent = null;
        public SettingsWindow(MainWindow parent)
        {
            InitializeComponent();
            m_Parent = parent;

            DataContext = new SettingsWindowDataContext();

            LoadSettings();
        }

        public void LoadSettings()
        {
            TbMpdAddress.Text = Settings.Default.Mpd_Address;
            TbMpdRefreshInterval.Text = Settings.Default.Mpd_RefreshInterval.ToString();
            CbOutputDevices.SelectedValue = Settings.Default.OutputDeviceId;
        }

        public void SaveSettings()
        {
            Settings.Default.Mpd_Address = TbMpdAddress.Text;
            Settings.Default.Mpd_RefreshInterval = TbMpdRefreshInterval.Text.ToInt();
            var outputDevice = CbOutputDevices.SelectedValue;
            Settings.Default.OutputDeviceId = outputDevice.ToString();
            Settings.Default.Save();
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Parent.OnChildWindowClosing(this);
        }
    }
}
