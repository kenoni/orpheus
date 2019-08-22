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
            dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer1.Tick += dispatcherTimer_Tick1;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer1.Start();
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

        

       

        

        

       
    }
}
