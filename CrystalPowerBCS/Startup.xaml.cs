using CrystalPowerBCS.Commands;
using CrystalPowerBCS.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Linq;

namespace CrystalPowerBCS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class Startup : Application
    {
        public Startup()
        {

        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigureDefaultDateTimeFormat();
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                // If the user is not an administrator, restart the application with administrator privileges
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Assembly.GetEntryAssembly().Location;
                startInfo.Verb = "runas"; // This requests elevation to run as an administrator

                try
                {
                    Process.Start(startInfo);
                    Application.Current.Shutdown();
                }
                catch (Exception)
                {
                    // Handle any exceptions that may occur
                }
            }
            var loc = Environment.CurrentDirectory;
            if(loc != null)
            {
                loc = loc + "\\crystalPowerDb.db";
                if (!File.Exists(loc))
                {
                    try
                    {
                        using (var dbContext = new ApplicationDBContext())
                        {
                            dbContext.Database.EnsureCreated();
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    //using (var dbContext = new ApplicationDBContext())
                    //{
                    //    var pendingMigrations = dbContext.Database.GetPendingMigrations();
                    //    if (pendingMigrations.Any())
                    //    {
                    //        dbContext.Database.Migrate();
                    //    }
                    //}
                }
            }

            WindowHelper.StartWindow();

            // release windows that is not closed
            if (Application.Current != null)
            {
                foreach (Window win in Application.Current.Windows)
                {
                    win.Close();
                }

                Application.Current.Shutdown();
            }
        }

        private void ConfigureDefaultDateTimeFormat()
        {
            // Set the default DateTime format globally
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-IN");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-IN");

            // You can set the desired DateTime format patterns
            DateTimeFormatInfo dtfi = CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat;
            dtfi.ShortDatePattern = "dd-MM-yyy";
            dtfi.LongTimePattern = "HH:mm:ss";
        }

        /// <summary>
        /// on close the application lean the tray icon
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {

            MeterConnectionCommand meterConnection = new MeterConnectionCommand();
            
            System.Threading.Tasks.Task.Run(async () => await meterConnection.DisconnectMeter());

            base.OnExit(e);
            // base.Shutdown();
        }
    }
}
