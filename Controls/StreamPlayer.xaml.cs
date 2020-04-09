using Orpheus.AppData;
using Orpheus.CsCore;
using Orpheus.DataContext;
using Orpheus.Helpers;
using Orpheus.Models;
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
    /// Interaction logic for StreamPlayer.xaml
    /// </summary>
    public partial class StreamPlayer : UserControl
    {
        private readonly AppDataManager _appDataManager;

        public StreamPlayer()
        {
            InitializeComponent();
            VolumeSlider.Value = Settings.Default.Player_Volume;
            var appData = new AppData.AppDataContent(MainContext.Instance.MainWindow);
            _appDataManager = new AppDataManager(appData);
           
            try
            {
                _appDataManager.GetData();
            }
            catch
            {
                // ignored
            }

            var directoryList = new IceCastDirectoryList();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddStreamName.Text = string.Empty;
            AddStreamUrl.Text = string.Empty;
            AddStreamPopup.IsOpen = true;
        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (!MainContext.Instance.MainWindow.IsPlayerPlaying && MainContext.Instance.MainWindow.PlayerStreams.Count > 0)
            {
                var streamToPlay = MainContext.Instance.MainWindow.PlayerStreams.FirstOrDefault(s => s.IsPlaying) ?? MainContext.Instance.MainWindow.PlayerStreams.FirstOrDefault();
                MainContext.Instance.MainWindow.IsPlayerPlaying = true;

                Player.Instance.Play(streamToPlay.Url);
                Player.Instance.Volume = (int)VolumeSlider.Value;

                streamToPlay.IsPlaying = true;
            }
            else
            {
                MainContext.Instance.MainWindow.IsPlayerPlaying = false;
                Player.Instance.Stop();
            }
        }

        private void AddStreamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainContext.Instance.MainWindow.PlayerStreams == null) MainContext.Instance.MainWindow.PlayerStreams = new List<PlayerStream>();
            MainContext.Instance.MainWindow.PlayerStreams.Add(new PlayerStream { Name = AddStreamName.Text, Url = AddStreamUrl.Text });
            _appDataManager.SaveData();
            AddStreamPopup.IsOpen = false;
            PlayerStreamsListView.Items.Refresh();
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volumeValue = (int)VolumeSlider.Value;
            Player.Instance.Volume = volumeValue;
            Settings.Default.Player_Volume = volumeValue;
            Settings.Default.Save();
        }

        private async void PlayerStreamsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            ListViewItem item = ListViewRowBeingClicked(((ListView)sender), e);
            if (item != null)
            {
                var itemContext = item.DataContext;
                var stream = (itemContext.GetType() == typeof(KeyValuePair<int, IceCastStream>)) ? ((KeyValuePair<int, IceCastStream>)itemContext).Value
                                    : item.DataContext as BaseStream;

                if (stream != null)
                {
                    //MainContext.Instance.MainWindow.PlayerStreams.ToList().ForEach(x => x.IsPlaying = false);

                    if (!MainContext.Instance.MainWindow.IsPlayerPlaying)
                    {
                        MainContext.Instance.MainWindow.IsPlayerPlaying = true;
                        stream.IsPlaying = true;
                    }
                    else
                    {
                        Player.Instance.Stop();
                    }

                    Player.Instance.Play(stream.Url);
                    Player.Instance.Volume = (int)VolumeSlider.Value;

                    CurrentlyPlayingTitle.Content = string.Empty;
                    var currentlyPlaying = await IceCastParser.GetCurrentlyPlayingAsync(stream.Url);
                    CurrentlyPlayingTitle.Content = currentlyPlaying;
                }
            }

        }

        private void PlayerStreamsListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //listview.Items.Remove(listview.Items.CurrentItem);
                object currentItem = PlayerStreamsListView.Items.CurrentItem;
                if (currentItem != null)
                {
                    foreach (var streamToDelete in PlayerStreamsListView.SelectedItems)
                    {
                        var streamEntry = (PlayerStream)streamToDelete;

                        MainContext.Instance.MainWindow.PlayerStreams.Remove(streamEntry);
                    }

                    PlayerStreamsListView.Items.Refresh(); // TO DO : Refresh too soon must be done when all delete commands ended.
                }
            }
            _appDataManager.SaveData();

        }

        private ListViewItem ListViewRowBeingClicked(ListView listView, MouseButtonEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));

            if (hit != null)
            {
                DependencyObject component = hit.VisualHit;

                while (component != null)
                {
                    if (component is ListViewItem)
                    {
                        return (ListViewItem)component;
                    }
                    else
                    {
                        component = VisualTreeHelper.GetParent(component);
                    }
                }
            }

            return null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox).Text;

            MainContext.Instance.MainWindow.IceCastStreams = IceCastDirectoryList.Stations
                .Where(x =>( x.Value.ServerName != null  && x.Value.ServerName.IndexOf(text, 0, StringComparison.CurrentCultureIgnoreCase) != -1 )
                        || (x.Value.Genre != null && x.Value.Genre.IndexOf(text, 0, StringComparison.CurrentCultureIgnoreCase) != -1))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private void CopyIceCastStreamMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (KeyValuePair<int, IceCastStream>)IceCastListView.SelectedItem;
            Clipboard.SetText(item.Value.Url);
        }
    }
}
