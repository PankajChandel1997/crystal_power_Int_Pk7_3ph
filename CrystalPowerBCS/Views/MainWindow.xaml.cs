using CrystalPowerBCS.DbFunctions;
using CrystalPowerBCS.Helpers;
using CrystalPowerBCS.ViewModels;
using CrystalPowerBCS.Views;
using Infrastructure.DTOs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CrystalPowerBCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Helpers.IWindow
    {
        public WindowCloseMode WindowCloseMode { get; set; }
        public ObservableCollection<ComboBoxItem> meters { get; set; }
        public ComboBoxItem SelectedMeter { get; set; }
        public MeterCommand MeterCommand;
        public MeterFetchDataLogCommand MeterFetchDataLogCommand;
        public List<MeterDto> metersList;
        public List<MeterFetchDataLogDto> metersLogList;
        public CurrentMeterTypeViewModel currentMeterTypeViewModel;
        public int pageSize = 10;
        public string search = string.Empty;
        public MainWindow()
        {
            this.SizeChanged += OnWindowSizeChanged;

            InitializeComponent();

            this.SizeChanged += OnWindowSizeChanged;

            WpfSingleInstance.Make("Crystal Power", uniquePerUser: false);

            currentMeterTypeViewModel = new CurrentMeterTypeViewModel();

            // Enable "minimize to tray" behavior for this Window
            MinimizeToTrayHelper.Enable(this);

            System.Threading.Tasks.Task.Run(() => FetchMeterType()).Wait();

            var bc = new BrushConverter();
            NamePlate.Background = bc.ConvertFrom("#293846") as Brush;
            NamePlate.Foreground = Brushes.White;
            LeftnavBorder13.BorderThickness = new Thickness(4, 0, 0, 0);
            HostGrid.Children.Add(new NamePlate());

            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterType();
            });
            
        }   

        private async Task FetchMeterType()
        {
            MeterCommand = new MeterCommand();
            meters = new ObservableCollection<ComboBoxItem>();
            metersList = await Task.Run(() => MeterCommand.GetAll(pageSize));
        }

        private async Task BindMeterType()
        {
            if (metersList != null && metersList.Count > 0)
            {
                List<string> UniqueMetersList = new List<string>();
                foreach (var meter in metersList)
                {
                    if (meter.MeterNumber != null && !UniqueMetersList.Contains(meter.MeterNumber))
                    {
                        UniqueMetersList.Add(meter.MeterNumber);
                        MetersList.Items.Add(meter.MeterNumber);
                    }
                }
               // MetersList.SelectedIndex = 0;
                MeterTypeBox.Text = metersList[0].MeterType == Domain.Enums.MeterType.SinglePhase ? Constants.SinglePhaseMeter : metersList[0].MeterType == Domain.Enums.MeterType.ThreePhase ? Constants.ThreePhaseMeter : metersList[0].MeterType == Domain.Enums.MeterType.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
            }
        }
        private void MenuItem_Click(object sender, MouseButtonEventArgs e)
        {
            HostGrid.Children.Clear();
            ClearLeftNavSelection();
            var source = (FrameworkElement)e.Source;
            var bc = new BrushConverter();
            switch (source.Uid)
            {
                case Constants.NamePlate:
                    HostGrid.Children.Add(new NamePlate());
                    NamePlate.Background = bc.ConvertFrom("#293846") as Brush;
                    NamePlate.Foreground = Brushes.White;
                    LeftnavBorder13.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.Instanteneousparameter:
                    HostGrid.Children.Add(new Instanteneousparameter());
                    Instanteneousparameter.Background = bc.ConvertFrom("#293846") as Brush;
                    Instanteneousparameter.Foreground = Brushes.White;
                    LeftnavBorder.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.BlockLoadProfile:
                    HostGrid.Children.Add(new BlockLoadProfile());
                    BlockLoadProfile.Background = bc.ConvertFrom("#293846") as Brush;
                    BlockLoadProfile.Foreground = Brushes.White;
                    LeftnavBorder1.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.BillingProfile:
                    HostGrid.Children.Add(new Billingprofile());
                    BillingProfile.Background = bc.ConvertFrom("#293846") as Brush;
                    BillingProfile.Foreground = Brushes.White;
                    LeftnavBorder2.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.DailyLoadProfileGrid:
                    HostGrid.Children.Add(new DailyLoadProfile());
                    DailyLoadProfile.Background = bc.ConvertFrom("#293846") as Brush;
                    DailyLoadProfile.Foreground = Brushes.White;
                    LeftnavBorder3.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.Events:
                    HostGrid.Children.Add(new Events());
                    Events.Background = bc.ConvertFrom("#293846") as Brush;
                    Events.Foreground = Brushes.White;
                    LeftnavBorder5.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.SelfDiagnostic:
                    HostGrid.Children.Add(new SelfDiagnostic());
                    SelfDiagnostic.Background = bc.ConvertFrom("#293846") as Brush;
                    SelfDiagnostic.Foreground = Brushes.White;
                    LeftnavBorder7.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.Report:
                    HostGrid.Children.Add(new Reports());
                    Reports.Background = bc.ConvertFrom("#293846") as Brush;
                    Reports.Foreground = Brushes.White;
                    LeftnavBorder8.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.Consumption:
                    HostGrid.Children.Add(new Consumption());
                    Consumption.Background = bc.ConvertFrom("#293846") as Brush;
                    Consumption.Foreground = Brushes.White;
                    LeftnavBorder9.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.TOD:
                    HostGrid.Children.Add(new TOD());
                    TOD.Background = bc.ConvertFrom("#293846") as Brush;
                    TOD.Foreground = Brushes.White;
                    LeftnavBorder10.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.DailyLoadConsumption:
                    HostGrid.Children.Add(new DailyLoadConsumption());
                    DailyLoadConsumption.Background = bc.ConvertFrom("#293846") as Brush;
                    DailyLoadConsumption.Foreground = Brushes.White;
                    LeftnavBorder11.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;

                case Constants.ConsumerDetails:
                    HostGrid.Children.Add(new ConsumerDetails());
                    ConsumerDetails.Background = bc.ConvertFrom("#293846") as Brush;
                    ConsumerDetails.Foreground = Brushes.White;
                    LeftnavBorder12.BorderThickness = new Thickness(4, 0, 0, 0);
                    break;
            }
        }

        private void ClearLeftNavSelection()
        {
            var bc = new BrushConverter();

            Instanteneousparameter.Background = bc.ConvertFrom("#2f4050") as Brush;
            Instanteneousparameter.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder.BorderThickness = new Thickness(0, 0, 0, 0);

            BlockLoadProfile.Background = bc.ConvertFrom("#2f4050") as Brush;
            BlockLoadProfile.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder1.BorderThickness = new Thickness(0, 0, 0, 0);

            BillingProfile.Background = bc.ConvertFrom("#2f4050") as Brush;
            BillingProfile.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder2.BorderThickness = new Thickness(0, 0, 0, 0);

            DailyLoadProfile.Background = bc.ConvertFrom("#2f4050") as Brush;
            DailyLoadProfile.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder3.BorderThickness = new Thickness(0, 0, 0, 0);

            Events.Background = bc.ConvertFrom("#2f4050") as Brush;
            Events.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder5.BorderThickness = new Thickness(0, 0, 0, 0);

            SelfDiagnostic.Background = bc.ConvertFrom("#2f4050") as Brush;
            SelfDiagnostic.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder7.BorderThickness = new Thickness(0, 0, 0, 0);

            Reports.Background = bc.ConvertFrom("#2f4050") as Brush;
            Reports.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder8.BorderThickness = new Thickness(0, 0, 0, 0);

            Consumption.Background = bc.ConvertFrom("#2f4050") as Brush;
            Consumption.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder9.BorderThickness = new Thickness(0, 0, 0, 0);

            TOD.Background = bc.ConvertFrom("#2f4050") as Brush;
            TOD.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder10.BorderThickness = new Thickness(0, 0, 0, 0);

            DailyLoadConsumption.Background = bc.ConvertFrom("#2f4050") as Brush;
            DailyLoadConsumption.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder11.BorderThickness = new Thickness(0, 0, 0, 0);

            ConsumerDetails.Background = bc.ConvertFrom("#2f4050") as Brush;
            ConsumerDetails.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder12.BorderThickness = new Thickness(0, 0, 0, 0);

            NamePlate.Background = bc.ConvertFrom("#2f4050") as Brush;
            NamePlate.Foreground = bc.ConvertFrom("#A7B1C2") as Brush;
            LeftnavBorder13.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void ShowLoader()
        {
            Spinner.IsLoading = true;
            PART_Host.Opacity = 0.7;
            PART_Host.IsEnabled = false;
        }

        private void hideLoader()
        {
            PART_Host.Opacity = 1;
            Spinner.IsLoading = false;
            PART_Host.IsEnabled = true;
        }

        private async void ChangeMeterNumberDropDown(object sender, SelectionChangedEventArgs e)
        {
            string value = MetersList.SelectedValue?.ToString();

            if (value != null)
            {
                if (metersList != null && metersList.Count > 0)
                {
                    search = string.Empty;
                    foreach (var meter in metersList)
                    {
                        if (meter.MeterNumber == value)
                        {
                            currentMeterTypeViewModel.MeterType = meter.MeterType.ToString();
                            currentMeterTypeViewModel.MeterNo = meter.MeterNumber;

                            MeterPhaseType.Text = meter.MeterType.ToString() + "&&" + meter.MeterNumber.ToString();
                            MeterTypeBox.Text = meter.MeterType == Domain.Enums.MeterType.SinglePhase ? Constants.SinglePhaseMeter : meter.MeterType == Domain.Enums.MeterType.ThreePhase ? Constants.ThreePhaseMeter : meter.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;

                            break;
                        }
                    }
                }
            }
        }
        private async void SearchComboBox(object sender, TextCompositionEventArgs e)
        {

            MetersList.IsDropDownOpen = true;
            //MetersList.Items.Clear();

            string currentCharacter = e.Text.ToString();

            search = MetersList.Text;

            search += currentCharacter;
            
            //MetersList.Text = search;

            if (search.Length > 0 )
            {
                MeterCommand = new MeterCommand();
                metersList = await Task.Run(() => MeterCommand.GetAll(pageSize));

                //var searchMetersList = await Task.Run(() => MeterCommand.Filter(search,10));
                var searchMetersList = metersList.Where(x => x.MeterNumber.ToLower().StartsWith(search?.ToLower())).ToList();// await Task.Run(() => MeterCommand.Filter(search,10));

                //if(searchMetersList.Count > 0)
                //{
                    MetersList.Items.Clear();

                    metersList = searchMetersList;

                  //  MetersList.SelectedIndex = 0;

                    await this.Dispatcher.Invoke(async () =>
                    {
                        await BindMeterType();
                    });
                //}
            }        
            else
            {
                MetersList.Items.Clear();

                metersList = await Task.Run(() => MeterCommand.GetAll(pageSize));

                await this.Dispatcher.Invoke(async () =>
                {
                    await BindMeterType();
                });
            }
        }

//to add scroll bar in left menu
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CurrentHeight.Text = e.NewSize.Height.ToString();
        }

        private void GoToHome(object sender, RoutedEventArgs e)
        {
            var currentWindow = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var currentWidth = currentWindow.ActualWidth;
            var left = currentWindow.Left;
            var top = currentWindow.Top;
            var windowSatate = currentWindow.WindowState;

            StartupWindow startupWindow = new StartupWindow();
            startupWindow.Owner = this;
            startupWindow.Width = currentWidth;
            startupWindow.Left = left;
            startupWindow.Top = top;

            this.Hide(); // not required if using the child events below
            startupWindow.ShowDialog();
        }
        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollBarVisibility();
            SizeChanged += Window_SizeChanged;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScrollBarVisibility();
        }

        private void UpdateScrollBarVisibility()
        {
            if (ScrollViewer != null)
            {
                if (ActualHeight < 1200)
                {
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    nav_pnl.MaxHeight=500;
                }
                else
                {
                    nav_pnl.MaxHeight = 1000;
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
            }
        }

        private async void BackButtonSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                search = MetersList.Text.Length > 0 ? MetersList.Text.Remove(MetersList.Text.Length - 1) : MetersList.Text;
                if (search.Length  != 0)
                {
                    MeterCommand = new MeterCommand();

                    metersList = await Task.Run(() => MeterCommand.GetAll(pageSize));
                   
                    var searchMetersList = metersList.Where(x => x.MeterNumber.ToLower().StartsWith(search?.ToLower())).ToList();

                    //if (searchMetersList.Count > 0)
                    //{
                        MetersList.Items.Clear();

                        metersList = searchMetersList;

                        //MetersList.SelectedIndex = 0;

                        await this.Dispatcher.Invoke(async () =>
                        {
                            await BindMeterType();
                        });
                    //}
                }
                else
                {
                    MetersList.Items.Clear();

                    metersList = await Task.Run(() => MeterCommand.GetAll(pageSize));

                    await this.Dispatcher.Invoke(async () =>
                    {
                        await BindMeterType();
                    });
                }
            }
        
        }

    }
}
