using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrystalPowerBCS.Helpers
{
    public class SingleInstance
    {
        #region Constants and Fields

        /// <summary>The event mutex name.</summary>
        private const string UniqueEventName = "AIzaSyAhySl9LVEFtCgnzbxtmB_T3hiLdECmAGY";

        /// <summary>The event wait handle.</summary>
        private static EventWaitHandle eventWaitHandle;
        #endregion

        #region Methods

        /// <summary>The app on startup.</summary>
        public static bool StartInstance()
        {
            try
            {
                // try to open it - if another instance is running, it will exist
                eventWaitHandle = EventWaitHandle.OpenExisting(UniqueEventName);

                // Notify other instance so it could bring itself to foreground.
                eventWaitHandle.Set();

                // Terminate this instance.
                Application.Current.Shutdown();
                return false;
            }
            catch (Exception)
            {
                // listen to a new event
                eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            }

            // if this instance gets the signal to show the main window
            new Task(() =>
            {
                while (eventWaitHandle.WaitOne())
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var mw = Application.Current.MainWindow;
                        if (mw != null)
                        {
                            if (mw.WindowState == WindowState.Minimized || mw.Visibility != Visibility.Visible)
                            {
                                mw.Show();
                                mw.WindowState = WindowState.Normal;
                            }

                            // According to some sources these steps gurantee that an app will be brought to foreground.
                            mw.Activate();
                            mw.Topmost = true;
                            mw.Topmost = false;
                            mw.Focus();
                        }
                    }));
                }
            }).Start();

            return true;
        }

        #endregion
    }
}
