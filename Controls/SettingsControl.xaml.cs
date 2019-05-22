using Orpheus.DataContext;
using Orpheus.Mpd;
using Orpheus.Properties;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orpheus.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
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

        private void OnSaveSettingsClicked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MpdServer.CreateInstance(MainContext.Instance.MainWindow.DisplayMessage, MainContext.Instance.MainWindow.UpdateUI);
        }
    }
}
