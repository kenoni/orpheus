using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Orpheus.AppData;
using Orpheus.DataContext;
using Orpheus.Models;
using Orpheus.Mpd;
using Orpheus.Properties;
using Orpheus.CsCore;

namespace Orpheus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly SettingsWindowDataContext _settingsDataContext = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer1 = null;
        

        public MainWindow()
        {
            
            InitializeComponent();
            
            
            InitializeDataContext();
            InitializeAppDataManager();
            
            StartTimers();
        }

        private void InitializeDataContext()
        {
            DataContext = MainContext.Instance;
            MpdServer.CreateInstance(MainContext.Instance.MainWindow.DisplayMessage, MainContext.Instance.MainWindow.UpdateUI);
        }

        public void InitializeAppDataManager()
        {
           
        }

        public MainWindowDataContext MainWindowDataContext
        {
            get;
            private set;
        }

        private void StartTimers()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer1.Tick += dispatcherTimer_Tick1;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer1.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (MainContext.Instance.MainWindow.ElapsedTime != 0 && MainContext.Instance.MainWindow.Duration != 0)
            {
                MainContext.Instance.MainWindow.ElapsedTime++;
            }
        }

        private void dispatcherTimer_Tick1(object sender, EventArgs e)
        {

            MainContext.Instance.MainWindow.UpdateStatus();
        }

        

        private void OnPauseClicked(object sender, RoutedEventArgs e)
        {
            Player.Instance.Stop();
        }

        private void OnStopClicked(object sender, RoutedEventArgs e)
        {
            Player.Instance.Pause();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Player.Instance.Stop();
        }

        

        private void OnPlaylistViewDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = ListViewRowBeingClicked(PlayListView, e);
            if (item != null)
            {
                dispatcherTimer.Stop();
                var song = item.DataContext as MpdPlaylistEntry;

                if (song != null)
                {
                    MpdServer.Instance?.PlayId(song.Id);
                    MainContext.Instance?.MainWindow?.UpdateStatus();
                    dispatcherTimer.Start();
                }
            }
        }

        private void PlayListView_PreviewDrop(object sender, DragEventArgs e)
        {
            int targetRow = DropTargetRowIndex(e);
            //string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string[] songsToBeAdded = (string[])e.Data.GetData("AddToPlaylist");

            foreach (var song in songsToBeAdded)
            {
                MpdServer.Instance?.AddId(song);
            }

            MainContext.Instance.MainWindow.UpdatePlayList(); //TO DO : Query only added records
        }

        private int DropTargetRowIndex(DragEventArgs e)
        {
            for (int i = 0; i < PlayListView.Items.Count; ++i)
            {
                DataGridRow row = PlayListView.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;

                if (row != null)
                {
                    Point pt = e.GetPosition(row);
                    double yCoord = row.TranslatePoint(pt, row).Y;
                    double halfHeight = row.ActualHeight / 2;

                    if (yCoord < halfHeight)
                    {
                        return i;
                    }
                }
            }

            return PlayListView.Items.Count;
        }

        private ListViewItem ListViewRowBeingClicked(ListView listView, MouseButtonEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));

            if (hit != null)
            {
                DependencyObject component = (DependencyObject)hit.VisualHit;

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

        private void PlaylisItemSlider_OnValueChanged(object sender, MouseButtonEventArgs e)
        {
            var sliderValue = PlaylisItemSlider.Value;
            MpdServer.Instance?.SeekCur(sliderValue.ToString(CultureInfo.InvariantCulture));
        }

        private void SetSplitterVisible(bool show)
        {
            MainGrid.ColumnDefinitions[2].Width = (show) ? new GridLength(50, GridUnitType.Star) : new GridLength(0);
            GridSplitter1.Visibility = (show) ? Visibility.Visible : Visibility.Hidden;
        }

       

       

       


        

        


        private void PlayListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //listview.Items.Remove(listview.Items.CurrentItem);
                object currentItem = PlayListView.Items.CurrentItem;
                if (currentItem != null)
                {
                    foreach(var songToDelete in PlayListView.SelectedItems)
                    {
                        var playlistEntry = (MpdPlaylistEntry)songToDelete;

                        MainContext.Instance.MainWindow.CurrentPlaylist.Remove(playlistEntry);
                        MpdServer.Instance.DeleteId(playlistEntry.Id);
                    }

                    PlayListView.Items.Refresh(); // TO DO : Refresh too soon must be done when all delete commands ended.
                }
            }
        }

       

        

        

       

        

       

        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl)
            {
                string tabName = ((sender as TabControl).SelectedItem as TabItem).Header as string;

                switch (tabName)
                {
                    //case "Player": if (MainContext.Instance.MainWindow.PlayerStreams == null) _appDataManager.GetData(); break;
                    default: break;
                }
                e.Handled = true;
            }
           
        }

        

        private void GridSplitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(TabControlHolder.Width.Value != 30)
            {
                PreviousGridSplitterWIdth = TabControlHolder.Width.Value;
                TabControlHolder.Width = new GridLength(30, GridUnitType.Pixel);
            }
            else
            {
                TabControlHolder.Width = new GridLength(PreviousGridSplitterWIdth, GridUnitType.Pixel);
            }
        }

        private double PreviousGridSplitterWIdth = 0;

       

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var firstSongInCurrentPlaylist = MainContext.Instance.MainWindow.CurrentPlaylist.FirstOrDefault();
            if(firstSongInCurrentPlaylist != null)
            {
                MpdServer.Instance.PlayId(firstSongInCurrentPlaylist.Id);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Next();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Stop();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Previous();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Random("1");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MpdServer.Instance.Repeat("1");
        }
    }
}
