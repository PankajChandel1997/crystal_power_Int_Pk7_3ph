using CrystalPowerBCS.Commands;
using CrystalPowerBCS.Helpers;
using CrystalPowerBCS.ViewModels;
using Domain.Enums;
using Gurux.Common;
using Gurux.DLMS;
using Gurux.Serial;
using Hardcodet.Wpf.TaskbarNotification;
using Infrastructure.API;
using Infrastructure.DTOs;
using Infrastructure.Helpers;
using Notification.Wpf;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window, Helpers.IWindow

    {
        private TaskbarIcon tb;
        public WindowCloseMode WindowCloseMode { get; set; }
        public int BackCount;
        public int projectCode = -1;
        public string Password = "Crystal@123";
        MeterConnectionCommand meterConnection = new MeterConnectionCommand();
        public GXSerial serial = new GXSerial();
        public MeterMethodsCommand meterMethodsCommand;
        MeterConnectionViewModel meterConnectionModel = new MeterConnectionViewModel();
        public string SelectedPort;
        public ErrorHelper _errorHelper;
        public ConsumerDetailsDto consumerDetails = new ConsumerDetailsDto();

        public StartupWindow()
        {
            InitializeComponent();

            WpfSingleInstance.Make(Constants.CrystalPower, uniquePerUser: false);

            MinimizeToTrayHelper.Enable(this);
            _errorHelper = new ErrorHelper();
            AddMeterTypes();

            BackCount = 0;
            meterTypeComboBox.SelectedIndex = 0;
            FetechMeterDataComboBox.SelectedIndex = 0;
        }

        private void AddMeterTypes()
        {
            connectMeterTypeComboBox.Items.Clear();

            foreach (ProjectCode meterType in Enum.GetValues(typeof(ProjectCode)))
            {
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(
                    typeof(ProjectCode).GetField(meterType.ToString()), typeof(DescriptionAttribute));

                // If the description attribute exists, add the description to the ComboBox
                if (descriptionAttribute != null)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = descriptionAttribute.Description;
                    connectMeterTypeComboBox.Items.Add(comboBoxItem);
                }
            }
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
            {
                StartUpWindowGrid.Opacity = 0.7;
                StartUpWindowGrid.IsEnabled = false;

                View_Button.Content = "Please wait...";
                View_Button.IsEnabled = false;
                await Task.Run(() => ChangeView());
                this.Hide();

            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void ChangeView()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
            {
                var currentWindow = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                var currentHeight = currentWindow.ActualHeight;
                var currentWidth = currentWindow.ActualWidth;
                var left = currentWindow.Left;
                var top = currentWindow.Top;
                var windowSatate = currentWindow.WindowState;

                MainWindow mainWindow = new MainWindow();
                mainWindow.Height = currentHeight;
                mainWindow.Width = currentWidth;
                mainWindow.Left = left;
                mainWindow.Top = top;
                mainWindow.WindowState = windowSatate;

                mainWindow.ShowDialog();

            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void ShowFetchOptions(object sender, RoutedEventArgs e)
        {

            List<string> activePorts = await Task.Run(() => meterConnection.GetComPorts());

            if (activePorts != null && activePorts.Count > 0)
            {
                comPortsComboBox.Items.Clear();
                foreach (string activePort in activePorts)
                {
                    comPortsComboBox.Items.Add(activePort);
                }
                comPortsComboBox.SelectedIndex = 0;
            }
            else
            {
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, Constants.UnabletoaccessComPortPleaseCheckOpticalCable, NotificationType.Error, CloseOnClick: true);
            }

            Options.Visibility = Visibility.Collapsed;
            FetchOptions.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
            BackCount = BackCount + 1;

            //Temp Code only for WB so removing form here
            //ShowTODPopup(null, null);
        }

        //Loop through multiple projects to make connection
        private async void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoader();

                if (string.IsNullOrEmpty(SelectedPort))
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, Constants.PleaseSelectComPort, NotificationType.Warning, CloseOnClick: true);

                    return;
                }

                if (string.IsNullOrEmpty(MeterKeys.Active_Authentication) && string.IsNullOrEmpty(MeterKeys.Active_Encryption) && string.IsNullOrEmpty(MeterKeys.Active_Password))
                {
                    string[] keyTypes = { "GUJRAT", "Demo", "Int-LT-Pkg7", "HPSEBL", "ANVARULL", "Tillient" , "Purvanchal"/*, "APDCL-1", "APDCL-2", "Aparva", "JAMMU"*/ };

                    foreach (var item in keyTypes)
                    {

                        if (item == ProjectCode.Demo.GetDescription())
                        {
                            projectCode = (int)ProjectCode.Demo;
                            MeterKeys.Active_Authentication = MeterKeys.Demo_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.Demo_Encryption;
                            MeterKeys.Active_Password = MeterKeys.Demo_Password;
                        }
                        else if (item == ProjectCode.Gujrat.GetDescription())
                        {
                            projectCode = (int)ProjectCode.Gujrat;
                            MeterKeys.Active_Authentication = MeterKeys.GUJRAT_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.GUJRAT_Encryption;
                            MeterKeys.Active_Password = MeterKeys.GUJRAT_Password;
                        }
                        else if (item == ProjectCode.IntelliPk7.GetDescription())
                        {
                            projectCode = (int)ProjectCode.IntelliPk7;
                            MeterKeys.Active_Authentication = MeterKeys.IntelliPk7_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.IntelliPk7_Encryption;
                            MeterKeys.Active_Password = MeterKeys.IntelliPk7_Password;
                        }
                        else if (item == ProjectCode.Tillient.GetDescription())
                        {
                            projectCode = (int)ProjectCode.Tillient;
                            MeterKeys.Active_Authentication = MeterKeys.Tillient_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.Tillient_Encryption;
                            MeterKeys.Active_Password = MeterKeys.Tillient_Password;
                        }
                        else if (item == ProjectCode.HPSEBL.GetDescription())
                        {
                            projectCode = (int)ProjectCode.HPSEBL;
                            MeterKeys.Active_Authentication = MeterKeys.HPSEBL_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.HPSEBL_Encryption;
                            MeterKeys.Active_Password = MeterKeys.HPSEBL_Password;
                        }
                        else if (item == ProjectCode.Purvanchal.GetDescription())
                        {
                            projectCode = (int)ProjectCode.Purvanchal;
                            MeterKeys.Active_Authentication = MeterKeys.Purvanchal_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.Purvanchal_Encryption;
                            MeterKeys.Active_Password = MeterKeys.Purvanchal_Password;
                        }
                        else if (item == ProjectCode.ANVARULL.GetDescription())
                        {
                            MeterKeys.Active_Authentication = MeterKeys.ANVARULL_Authentication;
                            MeterKeys.Active_Encryption = MeterKeys.ANVARULL_Encryption;
                            MeterKeys.Active_Password = MeterKeys.ANVARULL_Password;
                        }

                        if (meterConnectionModel.SerialConnected != 1)
                        {
                            meterConnectionModel = await Task.Run(() => meterConnection.ConnectMeter(SelectedPort));
                        }

                        bool response = await ConnectMeterAsync(projectCode);
                        if (response)
                        {
                            FetchOptions.Visibility = Visibility.Collapsed;
                            FetchMeterData.Visibility = Visibility.Visible;

                            BackCount = BackCount + 1;

                            break;
                        }
                    }
                }
                else
                {
                    if (meterConnectionModel.SerialConnected != 1)
                    {
                        meterConnectionModel = await Task.Run(() => meterConnection.ConnectMeter(SelectedPort));
                    }

                    bool response = await ConnectMeterAsync(projectCode);
                    if (response)
                    {
                        FetchOptions.Visibility = Visibility.Collapsed;
                        FetchMeterData.Visibility = Visibility.Visible;

                        BackCount = BackCount + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("'" + meterConnectionModel.Port.ToUpper() + "' is denied"))
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, Constants.PortisengagedPleasetryagain, NotificationType.Error, CloseOnClick: true);
                }
                _errorHelper.WriteLog(DateTime.UtcNow + " : StartUpWindow : Connect : Exception ==>" + ex.Message);

                return;
            }
            finally
            {
                hideLoader();
            }
        }

        //Meter Connect Functionality
        private async Task<bool> ConnectMeterAsync(int projectCode)
        {
            try
            {
                if (meterConnectionModel != null && meterConnectionModel.IsConnected == true)
                {
                    meterMethodsCommand = new MeterMethodsCommand(meterConnectionModel);

                    //Getting Current Time on Meter
                    string MeterTime = await Task.Run(() => meterMethodsCommand.GetSerialNum_Click(consumerDetails));

                    if (!string.IsNullOrEmpty(MeterTime))
                    {
                        await Task.Run(() => meterMethodsCommand.OpenConnection());

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.MeterConnected + projectCode + ", " + MeterTime, NotificationType.Success, CloseOnClick: true);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : StartUpWindow : Connect : Exception ==>" + ex.Message);

                return false;
            }
        }

        private void ShowLoader()
        {
            Spinner.IsLoading = true;
            StartUpWindowGrid.Opacity = 0.7;
            StartUpWindowGrid.IsEnabled = false;
        }

        private void hideLoader()
        {
            StartUpWindowGrid.Opacity = 1;
            Spinner.IsLoading = false;
            StartUpWindowGrid.IsEnabled = true;
        }


        private void CloseOkBtn(object sender, RoutedEventArgs e)
        {
            FetchedMeterData.IsOpen = false;
        }

        private void CloseFetchedPopup(object sender, RoutedEventArgs e)
        {
            FetchedMeterData.IsOpen = false;
        }


        private async void ShowFetchedPopup(object sender, RoutedEventArgs e)
        {
            ShowLoader();
            bool response = false;
            ComboBoxItem SelectedItem = (ComboBoxItem)FetechMeterDataComboBox.SelectedValue;
            string value = SelectedItem.Content.ToString();

            //Getting From and To Date For filter
            DateTime? startDate = FilterFromDatePicker.SelectedDate.HasValue ? Functions.ToIndiaDateTimeObject((DateTime)FilterFromDatePicker.SelectedDate).Date : null;
            DateTime? endDate = FilterToDatePicker.SelectedDate.HasValue ? Functions.ToIndiaDateTimeObject((DateTime)FilterToDatePicker.SelectedDate).Date.AddDays(1).AddMinutes(-1) : null;

            if (value == MeterDataValueType.All.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadAllMeterData());
            }
            else if (value == MeterDataValueType.IP.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadIPOnly());
            }
            else if (value == MeterDataValueType.BillingProfile.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadBillingProfileOnly(startDate, endDate));
            }
            else if (value == MeterDataValueType.BlockLoadProfile.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadBlockLoadProfileOnly(startDate, endDate));
            }
            else if (value == MeterDataValueType.DailyLoadProfile.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadDailyLoadProfile(startDate, endDate));
            }
            else if (value == MeterDataValueType.EventOnly.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadEventOnly(startDate, endDate));
            }
            else if (value == MeterDataValueType.AllWithoutLoadProfile.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadAllWithoutLoadProfile());
            }
            else if (value == MeterDataValueType.SelfDiagnostic.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadSelfDiagnosticOnly());
            }
            else if (value == MeterDataValueType.NamePlate.GetDescription())
            {
                response = await Task.Run(() => meterMethodsCommand.ReadNamePlateOnly());
            }

            //Getting Current Time on Meter
            if (response)
            {
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, Constants.OperationCompletedSuccessFully, NotificationType.Success, CloseOnClick: true);
            }
            else
            {
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, Constants.SomethingWentWrongPleasetryagain, NotificationType.Error, CloseOnClick: true);
            }
            hideLoader();
        }

        private void BackButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (BackCount == 2)
            {
                FetchOptions.Visibility = Visibility.Visible;
                FetchMeterData.Visibility = Visibility.Collapsed;
                _ = meterMethodsCommand.CloseConnection();
                NotificationManager notificationManager = new NotificationManager();
                notificationManager.Show(Constants.Notification, Constants.Disconnected, NotificationType.Error, CloseOnClick: true);
            }
            else if (BackCount == 1)
            {
                Options.Visibility = Visibility.Visible;
                FetchOptions.Visibility = Visibility.Collapsed;

                BackButton.Visibility = Visibility.Hidden;
            }

            BackCount = BackCount - 1;
        }
        private async Task DisconnectMeterAsync()
        {
            ShowLoader();
            await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
            {
                meterConnectionModel = await System.Threading.Tasks.Task.Run(async () => await meterConnection.DisconnectMeter());

            }, System.Windows.Threading.DispatcherPriority.Send);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {

                System.Threading.Tasks.Task.Run(() => meterMethodsCommand.CloseConnection()).Wait();

            }, System.Windows.Threading.DispatcherPriority.Send);

            FetchMeterData.Visibility = Visibility.Collapsed;
            Options.Visibility = Visibility.Visible;

            BackButton.Visibility = Visibility.Hidden;

            BackCount = 0;

            NotificationManager notificationManager = new NotificationManager();

            notificationManager.Show(Constants.Notification, Constants.Disconnected, NotificationType.Error, CloseOnClick: true);

            hideLoader();
        }
        private void DisconnectMeter(object sender, RoutedEventArgs e)
        {
            _ = DisconnectMeterAsync();
        }

        private void ComboOptions(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem SelectedItem = (ComboBoxItem)meterTypeComboBox.SelectedValue;
            string value = SelectedItem.Content.ToString();
            if (value == "Crystal")
            {
                PassowrdOption.Visibility = Visibility.Collapsed;
            }
            else
            {
                PassowrdOption.Visibility = Visibility.Visible;
            }
        }

        private void ShowSetParameterPopup(object sender, RoutedEventArgs e)
        {
            SetParameterPopup.IsOpen = !SetParameterPopup.IsOpen;
            StartUpWindowGrid.Opacity = 0.4;
            StartUpWindowGrid.IsEnabled = false;
        }

        private void ShowConnectDisconnectPopup(object sender, RoutedEventArgs e)
        {
            ConnectDissconnectPopup.IsOpen = !ConnectDissconnectPopup.IsOpen;
            StartUpWindowGrid.Opacity = 0.4;
            StartUpWindowGrid.IsEnabled = false;
        }

        public void SaveBillingDate(object sender, RoutedEventArgs e)
        {
            NotificationManager notificationManager = new NotificationManager();

            if (BillingDatePicker.SelectedDate != null)
            {
                try
                {
                    DateTime billingDate = BillingDatePicker.SelectedDate.Value;
                    _ = meterMethodsCommand.SetBillingDate(billingDate);

                    SetParameterPopup.IsOpen = false;
                    StartUpWindowGrid.Opacity = 1;
                    StartUpWindowGrid.IsEnabled = true;
                    notificationManager.Show(Constants.Notification, Constants.BillingDateSetSuccessfull, NotificationType.Success, CloseOnClick: true);
                }
                catch (Exception ex)
                {
                    notificationManager.Show("Error", $"An error occurred: {ex.Message}", NotificationType.Error, CloseOnClick: true);
                }
            }
            else
            {
                SetParameterPopup.IsOpen = false;
                StartUpWindowGrid.Opacity = 1;
                StartUpWindowGrid.IsEnabled = true;
                notificationManager.Show("Warning", "Date can't be null", NotificationType.Warning, CloseOnClick: true);
            }
        }

        private void CloseSetParmPopup(object sender, RoutedEventArgs e)
        {
            SetParameterPopup.IsOpen = false;
            StartUpWindowGrid.Opacity = 1;
            StartUpWindowGrid.IsEnabled = true;
        }
        private void CloseConnectDisconnectPopup(object sender, RoutedEventArgs e)
        {
            ConnectDissconnectPopup.IsOpen = false;
            StartUpWindowGrid.Opacity = 1;
            StartUpWindowGrid.IsEnabled = true;
        }

        private void ComPortChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPort = comPortsComboBox.SelectedValue?.ToString();
        }

        // Changing Keys based on Selected Meter Type
        private void connectMeterChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem SelectedItem = (ComboBoxItem)connectMeterTypeComboBox.SelectedValue;
            string value = SelectedItem.Content.ToString();
    
            if (value == ProjectCode.Demo.GetDescription())
            {
                MeterKeys.Active_Authentication = MeterKeys.Demo_Authentication;
                MeterKeys.Active_Encryption = MeterKeys.Demo_Encryption;
                MeterKeys.Active_Password = MeterKeys.Demo_Password;
            }
            else if (value == ProjectCode.IntelliPk7.GetDescription())
            {
                MeterKeys.Active_Authentication = MeterKeys.IntelliPk7_Authentication;
                MeterKeys.Active_Encryption = MeterKeys.IntelliPk7_Encryption;
                MeterKeys.Active_Password = MeterKeys.IntelliPk7_Password;
            }
            else if (value == ProjectCode.Tillient.GetDescription())
            {
                MeterKeys.Active_Authentication = MeterKeys.Tillient_Authentication;
                MeterKeys.Active_Encryption = MeterKeys.Tillient_Encryption;
                MeterKeys.Active_Password = MeterKeys.Tillient_Password;
            }
            else if (value == ProjectCode.HPSEBL.GetDescription())
            {
                MeterKeys.Active_Authentication = MeterKeys.HPSEBL_Authentication;
                MeterKeys.Active_Encryption = MeterKeys.HPSEBL_Encryption;
                MeterKeys.Active_Password = MeterKeys.HPSEBL_Password;
            }
            else if (value == ProjectCode.ANVARULL.GetDescription())
            {
                MeterKeys.Active_Authentication = MeterKeys.ANVARULL_Authentication;
                MeterKeys.Active_Encryption = MeterKeys.ANVARULL_Encryption;
                MeterKeys.Active_Password = MeterKeys.ANVARULL_Password;
            }
            else
            {
                MeterKeys.Active_Authentication = MeterKeys.GUJRAT_Authentication;
                MeterKeys.Active_Encryption = MeterKeys.GUJRAT_Encryption;
                MeterKeys.Active_Password = MeterKeys.GUJRAT_Password;
            }

            //else
            //{
            //    MeterKeys.Active_Authentication = MeterKeys.Aparva_Authentication;
            //    MeterKeys.Active_Encryption = MeterKeys.Aparva_Encryption;
            //    MeterKeys.Active_Password = MeterKeys.Aparva_Password;
            //}
        }

        private void ShowHideDateRangeFilter(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem SelectedItem = (ComboBoxItem)FetechMeterDataComboBox.SelectedValue;
            string value = SelectedItem.Content.ToString();

            if (string.IsNullOrEmpty(value))
            {
                DateRangeFilter.Visibility = Visibility.Collapsed;
                FilterFromDatePicker.SelectedDate = null;
                FilterToDatePicker.SelectedDate = null;
            }

            if (value.Contains(Constants.FetchBlockLoadProfile))
            {
                DateRangeFilter.Visibility = Visibility.Visible;

                //Setting Default Date at start
                FilterFromDatePicker.SelectedDate = Functions.ToIndiaDateTimeObject(DateTime.UtcNow);
                FilterToDatePicker.SelectedDate = Functions.ToIndiaDateTimeObject(DateTime.UtcNow.AddDays(+1));
            }
            else
            {
                DateRangeFilter.Visibility = Visibility.Collapsed;
                FilterFromDatePicker.SelectedDate = null;
                FilterToDatePicker.SelectedDate = null;
            }
        }

        private void AuthenticateUser(object sender, RoutedEventArgs e)
        {
            AuthenticateUser();
        }
        private void AuthenticateUser()
        {
            NotificationManager notificationManager = new NotificationManager();

            var passwordBox = LoginMeterPasswordHidden;
            var VisiblePassword = LoginMeterPasswordVisible.Text;
            if (passwordBox.Password == Password || VisiblePassword == Password)
            {
                LoginView.Visibility = Visibility.Collapsed;
                Options.Visibility = Visibility.Visible;

                notificationManager.Show(Constants.Notification, Constants.Login, NotificationType.Success, CloseOnClick: true);
            }
            else
            {
                notificationManager.Show(Constants.Notification, Constants.IncorrectPassword, NotificationType.Warning, CloseOnClick: true);
            }
        }

        private void AuthenticateUser(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AuthenticateUser();
            }
        }

        private void AddCustomerDetails(object sender, RoutedEventArgs e)
        {
            CustomerDetailsPopup.IsOpen = true;
        }
        private void CloseCustomerPopup(object sender, RoutedEventArgs e)
        {
            consumerDetails = new ConsumerDetailsDto()
            {
                ConsumerNo = string.Empty,
                ConsumerName = string.Empty,
                ConsumerAddress = string.Empty
            };

            CustomerDetailsPopup.IsOpen = false;
        }

        private void ConsumerSubmit(object sender, RoutedEventArgs e)
        {
            consumerDetails = new ConsumerDetailsDto()
            {
                ConsumerNo = ConsumerNo.Text,
                ConsumerName = ConsumerName.Text,
                ConsumerAddress = ConsumerAddress.Text
            };

            CustomerDetailsPopup.IsOpen = false;
        }

        private void ShowPassword(object sender, MouseButtonEventArgs e)
        {
            LoginMeterPasswordHidden.Visibility = Visibility.Hidden;
            LoginMeterPasswordVisible.Text = LoginMeterPasswordHidden.Password;
            LoginMeterPasswordVisible.Visibility = Visibility.Visible;
            showPasswordIcon.Visibility = Visibility.Hidden;
            hidePasswordIcon.Visibility = Visibility.Visible;
        }

        private void HidePassword(object sender, MouseButtonEventArgs e)
        {
            LoginMeterPasswordHidden.Visibility = Visibility.Visible;
            LoginMeterPasswordHidden.Password = LoginMeterPasswordVisible.Text;
            LoginMeterPasswordVisible.Visibility = Visibility.Hidden;
            showPasswordIcon.Visibility = Visibility.Visible;
            hidePasswordIcon.Visibility = Visibility.Hidden;
        }

        private void ConnectMeterCommand(object sender, RoutedEventArgs e)
        {
            _= meterMethodsCommand.SendConnectCommand();
        }

        private void DisconnectMeterCommand(object sender, RoutedEventArgs e)
        {
            _= meterMethodsCommand.SendDisconnectCommand();
        }

        private void MDResetSetPopup(object sender, RoutedEventArgs e)
        {
            _ = meterMethodsCommand.MDResetSet();
        }

        private void ShowTODPopup(object sender, RoutedEventArgs e)
        {
            SetTod.IsOpen = !SetTod.IsOpen;
            StartUpWindowGrid.Opacity = 0.4;
            StartUpWindowGrid.IsEnabled = false;
        }
        private void CloseSetTodPopup(object sender, RoutedEventArgs e)
        {
            SetTod.IsOpen = false;
            StartUpWindowGrid.Opacity = 1;
            StartUpWindowGrid.IsEnabled = true;
        }
        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits, colon (:), and backspace
            if (!Regex.IsMatch(e.Text, @"[0-9:]") && e.Text != "\b")
            {
                e.Handled = true;
            }
        }

    }
}
