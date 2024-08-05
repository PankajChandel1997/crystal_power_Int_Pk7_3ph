using CrystalPowerBCS.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CrystalPowerBCS.Helpers
{
    public interface IWindow
    {
        public WindowCloseMode WindowCloseMode { get; set; }
    }

    public enum WindowCloseMode
    {
        Current,
        Parent,
        Top,
        Application
    }

    public static class WindowHelper
    {
        public static bool IsApplicationExited { get; set; }

        // NewWindow show windown as top window
        public static void NewWindow<TWindow>(TWindow window) where TWindow : Window, IWindow
        {
            Application.Current.MainWindow = window;
            window.ShowDialog();

            if (window.WindowCloseMode == WindowCloseMode.Application)
            {
                IsApplicationExited = true;
                Application.Current.Shutdown(0);
            }
        }

        // NewWindow create a new top window , and show it
        public static void NewWindow<TWindow>(object dataContext = null) where TWindow : Window, IWindow, new()
        {
            NewWindow<TWindow>(null, dataContext);
        }

        // NewWindow create a new top window , show it, and close old top window
        public static void NewWindow<TWindow>(this Window top, object dataContext = null) where TWindow : Window, IWindow, new()
        {
            if (top != null)
            {
                IWindow win = top as IWindow;
                if (win != null)
                {
                    win.WindowCloseMode = WindowCloseMode.Top;
                }
                top.Close();
            }

            TWindow window = new TWindow();
            //  window.DataContext = dataContext;
            Application.Current.MainWindow = window;

            window.ShowDialog();
            if (window.WindowCloseMode == WindowCloseMode.Application)
            {
                IsApplicationExited = true;
                Application.Current.Shutdown(0);
            }
            //window.MouseDown -= mouseDown;
        }

        // ShowWindow create a new child window, and show it.
        public static async void ShowWindow<TWindow>(this Window parent, object dataContext = null) where TWindow : Window, IWindow, new()
        {

            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TWindow child = new TWindow();
                //  child.DataContext = dataContext;
                ShowWindow<TWindow>(parent, child);
            }), DispatcherPriority.Background);
        }

        // ShowWindow show child window
        public static async void ShowWindow<TWindow>(this Window parent, TWindow child) where TWindow : Window, IWindow
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                parent.Hide();
                parent.ShowInTaskbar = false;

                Application.Current.MainWindow = child;

                child.ShowDialog();
                //child.MouseDown -= mouseDown;

                if (child.WindowCloseMode == WindowCloseMode.Parent || child.WindowCloseMode == WindowCloseMode.Top || child.WindowCloseMode == WindowCloseMode.Application)
                {
                    IWindow win = parent as IWindow;
                    if (win != null)
                    {
                        win.WindowCloseMode = child.WindowCloseMode;
                    }
                    parent.Close();
                }
                else
                {
                    Application.Current.MainWindow = parent;
                    parent.ShowInTaskbar = true;
                    parent.ShowDialog();
                }

            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// get time interval for backgroud sync
        /// </summary>
        /// <param name="interval">user selected interval (it can be 30 sec, 60, 90)</param>
        /// <returns></returns>
        public static int SyncInterval(string interval)
        {
            int syncInterval = 10000;
            if (string.IsNullOrEmpty(interval) == false)
            {
                switch (interval.ToLower())
                {
                    case "thirtysec":
                        syncInterval = 30000; // 30 secs
                        break;
                    case "sixtysec":
                        syncInterval = 60000; // 60 secs
                        break;
                    case "nintysec":
                        syncInterval = 90000; // 90 secs
                        break;
                    default:
                        syncInterval = 10000; // 10 secs
                        break;
                }
            }
            return syncInterval;
        }

        /// <summary>
        /// opne window
        /// </summary>
        /// <param name="dataContext"></param>
        public static void StartWindow(object dataContext = null)
        {
            if (IsApplicationExited == false)
            {
                NewWindow<StartupWindow>(dataContext);
            }
            return;
        }
    }
}