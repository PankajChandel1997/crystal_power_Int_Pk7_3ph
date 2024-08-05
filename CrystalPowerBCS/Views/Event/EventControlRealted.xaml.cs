using CrystalPowerBCS.DbFunctions.EventFunctions;
using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for EventConnectDisconnect.xaml
    /// </summary>
    public partial class EventControlRealted : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<ControlEventDto> ControlRealtedEvent;
        public ControlEventCommand ControlEventCommand;
        public List<ControlEventSinglePhaseDto> ControlEventSinglePhase;
        public ControlEventSinglePhaseCommand ControlEventSinglePhaseCommand;
        public ErrorHelper _errorHelper;
        public int pageSize = 10;
        public string MeterType;
        public string MeterNumber = "00000000";
        public ListSortDirection SortDirection = ListSortDirection.Descending;
        public EventControlRealted()
        {
            InitializeComponent();

            string MeterTypess = MeterPhaseType.Text;

            ControlRelatedEventSingleGrid.Visibility = Visibility.Collapsed;
            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;
            _errorHelper = new ErrorHelper();
        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                ControlRelatedEventSingleGrid.MaxHeight = height - 217;
                ControlRelatedEventThreeGrid.MaxHeight = height - 217;
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

        private async Task Filter(string startDate, string endDate, int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ControlEventSinglePhaseCommand = new ControlEventSinglePhaseCommand();

                var gridData = await ControlEventSinglePhaseCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                ControlRelatedEventSingleGrid.ItemsSource = gridData;
                ControlEventSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                ControlEventCommand = new ControlEventCommand();

                var gridData = await ControlEventCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                ControlRelatedEventThreeGrid.ItemsSource = gridData;
                ControlRealtedEvent = gridData;
            }
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
            if(MeterPhaseType.Text.Split("&&").Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                    await Filter(startDate, endDate, pageSize); ;
                });
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ControlEventSinglePhaseCommand = new ControlEventSinglePhaseCommand();

                var gridData = await ControlEventSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                ControlRelatedEventSingleGrid.ItemsSource = gridData;
                ControlEventSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                ControlEventCommand = new ControlEventCommand();

                var gridData = await ControlEventCommand.GetAll(pageSize, MeterNumber);

                ControlRelatedEventThreeGrid.ItemsSource = gridData;
                ControlRealtedEvent = gridData;
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
                ControlRelatedEventThreeGrid.Visibility = Visibility.Collapsed;
                ControlRelatedEventSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ControlRelatedEventSingleGrid.Visibility = Visibility.Collapsed;
                ControlRelatedEventThreeGrid.Visibility = Visibility.Visible;
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
                List<ControlEventSinglePhaseDto> ControlEventSinglePhaseData = new();
                ControlEventSinglePhaseData = ControlRelatedEventSingleGrid.Items.OfType<ControlEventSinglePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ControlEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(ControlEventSinglePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.ControlEventSinglePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ControlEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(ControlEventSinglePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.ControlEventSinglePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<ControlEventDto> ControlRealtedEventData = new();
                ControlRealtedEventData = ControlRelatedEventThreeGrid.Items.OfType<ControlEventDto>().ToList();

                string meterName = MeterType == Constants.ThreePhaseMeter ? Constants.ThreePhaseMeter : MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ControlEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(ControlRealtedEventData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.ControlEvent + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ControlEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(ControlRealtedEventData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.ControlEvent + meterName, MeterNumber);
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
                        var controlRelatedEventThreePhaseList = ControlRelatedEventThreeGrid.Items.OfType<ControlEventDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventThreeGrid.ItemsSource = controlRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var controlRelatedEventSinglePhaseList = ControlRelatedEventSingleGrid.Items.OfType<ControlEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventSingleGrid.ItemsSource = controlRelatedEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var controlRelatedEventThreePhaseList = ControlRelatedEventThreeGrid.Items.OfType<ControlEventDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventThreeGrid.ItemsSource = controlRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var controlRelatedEventSinglePhaseList = ControlRelatedEventSingleGrid.Items.OfType<ControlEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventSingleGrid.ItemsSource = controlRelatedEventSinglePhaseList;

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
                        var controlRelatedEventThreePhaseList = ControlRelatedEventThreeGrid.Items.OfType<ControlEventDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventThreeGrid.ItemsSource = controlRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var controlRelatedEventSinglePhaseList = ControlRelatedEventSingleGrid.Items.OfType<ControlEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventSingleGrid.ItemsSource = controlRelatedEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var controlRelatedEventThreePhaseList = ControlRelatedEventThreeGrid.Items.OfType<ControlEventDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventThreeGrid.ItemsSource = controlRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var controlRelatedEventSinglePhaseList = ControlRelatedEventSingleGrid.Items.OfType<ControlEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        ControlRelatedEventSingleGrid.ItemsSource = controlRelatedEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }

    }
}
