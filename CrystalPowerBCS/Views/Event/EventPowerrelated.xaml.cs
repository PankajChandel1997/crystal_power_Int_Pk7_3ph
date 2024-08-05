using CrystalPowerBCS.DbFunctions.EventFunctions;
using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for EventPowerrelated.xaml
    /// </summary>
    public partial class EventPowerrelated : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<PowerRelatedEventDto> PowerRelatedEvent;
        public PowerRelatedEventCommand PowerRelatedEventCommand;
        public List<PowerRelatedEventSinglePhaseDto> PowerRelatedEventSinglePhase;
        public PowerRelatedEventSinglePhaseCommand PowerRelatedEventSinglePhaseCommand;
        public int pageSize = 10;
        public string MeterType = Constants.SinglePhaseMeter;
        public string MeterNumber = "00000000";
        public ErrorHelper _errorHelper;
        public ListSortDirection SortDirection = ListSortDirection.Descending;

        public EventPowerrelated()
        {
            InitializeComponent();

            PowerRelatedEventSingleGrid.Visibility = Visibility.Visible;
            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;
            _errorHelper = new ErrorHelper();
            //PowerRelatedEvent = new ObservableCollection<PowerRelatedEventDto>();
        }

        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                PowerRelatedEventSingleGrid.MaxHeight = height - 217;
                PowerRelatedEventThreeGrid.MaxHeight = height - 217;
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void ChangeView(object sender, RoutedEventArgs e)
        {
            IsGraph = !IsGraph;
            if (IsGraph == true)
            {
                TableView.Visibility = Visibility.Collapsed;
                listView.Visibility = Visibility.Collapsed;
                listViewDisabled.Visibility = Visibility.Visible;
                GraphView.Visibility = Visibility.Visible;
                graphView.Visibility = Visibility.Visible;
                graphView.IsHitTestVisible = false;
                graphViewDisabled.Visibility = Visibility.Collapsed;

                double[] dataX1 = new double[] { 0, 2, 3, 4, 5 };
                double[] dataY1 = new double[] { 0, 4, 9, 16, 25 };

                double[] dataX2 = new double[] { 0, 4, 6, 7, 8 };
                double[] dataY2 = new double[] { 0, 12, 19, 26, 35 };

                IPGraph.Plot.AddScatter(dataX1, dataY1);
                IPGraph.Plot.AddScatter(dataX2, dataY2);
                IPGraph.Plot.Palette = ScottPlot.Palette.Frost;
                IPGraph.Refresh();
            }
            else
            {
                TableView.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Visible;
                listView.IsHitTestVisible = false;
                listViewDisabled.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                graphView.Visibility = Visibility.Collapsed;
                graphViewDisabled.Visibility = Visibility.Visible;
            }
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void FilterByDate(object sender, RoutedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate, endDate, pageSize);;
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    FilterByPageSize(null, null);
                });
            }
        }

        private void FilterByPageSize(object sender, SelectionChangedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

            if (MeterPhaseType.Text.Split("&&").Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                    await Filter(startDate, endDate, pageSize);
                });
            }

        }

        private async Task Filter(string startDate, string endDate, int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                PowerRelatedEventSinglePhaseCommand = new PowerRelatedEventSinglePhaseCommand();

                var gridData = await PowerRelatedEventSinglePhaseCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                PowerRelatedEventSingleGrid.ItemsSource = gridData;
                PowerRelatedEventSinglePhase = gridData;
              
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                PowerRelatedEventCommand = new PowerRelatedEventCommand();

                var gridData = await PowerRelatedEventCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                PowerRelatedEventThreeGrid.ItemsSource = gridData;
                PowerRelatedEvent = gridData;
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                PowerRelatedEventSinglePhaseCommand = new PowerRelatedEventSinglePhaseCommand();

                var gridData = await PowerRelatedEventSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                PowerRelatedEventSingleGrid.ItemsSource = gridData;
                PowerRelatedEventSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                PowerRelatedEventCommand = new PowerRelatedEventCommand();

                var gridData = await PowerRelatedEventCommand.GetAll(pageSize, MeterNumber);

                PowerRelatedEventThreeGrid.ItemsSource = gridData;
                PowerRelatedEvent = gridData;
            }
        }

        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });
            if (MeterType == Constants.SinglePhaseMeter)
            {
                PowerRelatedEventThreeGrid.Visibility = Visibility.Collapsed;
                PowerRelatedEventSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                PowerRelatedEventSingleGrid.Visibility = Visibility.Collapsed;
                PowerRelatedEventThreeGrid.Visibility = Visibility.Visible;
            }

            if (MeterPhaseType.Text.Split("&&").Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                    FilterByPageSize(null, null);
                });
            }

        }

        private async Task BindMeterTypeAndNumber()
        {
            string[] meterData = MeterPhaseType.Text.Split("&&");
            MeterType = meterData[0].ToString();
            MeterNumber = meterData[1].ToString();
        }

        private void ExportData(object sender, SelectionChangedEventArgs e)
        {

            var exportType = ((System.Windows.Controls.ContentControl)cbExport.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;

            if (MeterType == Constants.SinglePhaseMeter)
            {
                List<PowerRelatedEventSinglePhaseDto> PowerRelatedEventSinglePhaseData = new();
                PowerRelatedEventSinglePhaseData = PowerRelatedEventSingleGrid.Items.OfType<PowerRelatedEventSinglePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.PowerRelatedEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(PowerRelatedEventSinglePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.PowerRelatedEventSinglePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.PowerRelatedEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(PowerRelatedEventSinglePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.PowerRelatedEventSinglePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<PowerRelatedEventDto> PowerRelatedEventData = new();
                PowerRelatedEventData = PowerRelatedEventThreeGrid.Items.OfType<PowerRelatedEventDto>().ToList();

                string meterName = MeterType == Constants.ThreePhaseMeter ? Constants.ThreePhaseMeter : MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.PowerRelatedEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(PowerRelatedEventData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.PowerRelatedEventThreePhaseMeter + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.PowerRelatedEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(PowerRelatedEventData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.PowerRelatedEventThreePhaseMeter + meterName, MeterNumber);
                }
            }
            cbExport.SelectedIndex = 0;
        }

        public void ClearGridFilter(object sender, RoutedEventArgs e)
        {
            FilterDatePicker.SelectedDate = null;
            FilterToDatePicker.SelectedDate = null;

            this.Dispatcher.Invoke(() =>
            {
                FilterByPageSize(null, null);
            });

        }

        private void CustomSort(object sender, DataGridSortingEventArgs e)
        {
            string column = e.Column.SortMemberPath.ToString();

            ListSortDirection currentSortDirection = e.Column.SortDirection ?? SortDirection;

            if (currentSortDirection == ListSortDirection.Ascending)
            {
                SortDirection = ListSortDirection.Descending;
                if (column == "CreatedOn")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var powerRelatedEventsList = PowerRelatedEventThreeGrid.Items.OfType<PowerRelatedEventDto>().OrderBy(x=> DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventThreeGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                    else
                    {
                        var powerRelatedEventsList = PowerRelatedEventSingleGrid.Items.OfType<PowerRelatedEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventSingleGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var powerRelatedEventsList = PowerRelatedEventThreeGrid.Items.OfType<PowerRelatedEventDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventThreeGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                    else
                    {
                        var powerRelatedEventsList = PowerRelatedEventSingleGrid.Items.OfType<PowerRelatedEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventSingleGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                }
            }
            else
            {
                SortDirection = ListSortDirection.Ascending;
                if (column == "CreatedOn")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var powerRelatedEventsList = PowerRelatedEventThreeGrid.Items.OfType<PowerRelatedEventDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventThreeGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                    else
                    {
                        var powerRelatedEventsList = PowerRelatedEventSingleGrid.Items.OfType<PowerRelatedEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventSingleGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var powerRelatedEventsList = PowerRelatedEventThreeGrid.Items.OfType<PowerRelatedEventDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventThreeGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                    else
                    {
                        var powerRelatedEventsList = PowerRelatedEventSingleGrid.Items.OfType<PowerRelatedEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        PowerRelatedEventSingleGrid.ItemsSource = powerRelatedEventsList;

                        e.Handled = true;
                    }
                }
            }
        }
    }
}
