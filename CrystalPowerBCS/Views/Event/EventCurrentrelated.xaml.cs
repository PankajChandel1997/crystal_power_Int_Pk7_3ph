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
    /// Interaction logic for EventCurrentrelated.xaml
    /// </summary>
    public partial class EventCurrentrelated : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<CurrentRelatedEventDto> CurrentRelatedEvent;
        public CurrentRelatedEventCommand CurrentRelatedEventCommand;
        public List<CurrentRelatedEventSinglePhaseDto> CurrentRelatedEventSinglePhase;
        public CurrentRelatedEventSinglePhaseCommand CurrentRelatedEventSinglePhaseCommand;
        public int pageSize = 10;
        public string MeterType;
        public string MeterNumber = "00000000";
        public ErrorHelper _errorHelper;
        public ListSortDirection SortDirection = ListSortDirection.Descending;

        public EventCurrentrelated()
        {
            InitializeComponent();

            CurrentRelatedEventSingleGrid.Visibility = Visibility.Visible;
            graphView.Visibility = Visibility.Collapsed;
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
                CurrentRelatedEventSingleGrid.MaxHeight = height - 217;
                CurrentRelatedEventThreeGrid.MaxHeight = height - 217;
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
                CurrentRelatedEventSingleGrid.Items.Clear();

                CurrentRelatedEventSinglePhaseCommand = new CurrentRelatedEventSinglePhaseCommand();

                var gridData = await CurrentRelatedEventSinglePhaseCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                CurrentRelatedEventSingleGrid.ItemsSource = gridData;
                CurrentRelatedEventSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                CurrentRelatedEventCommand = new CurrentRelatedEventCommand();

                var gridData = await CurrentRelatedEventCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    await GridCalcualtion(gridData);
                }

                CurrentRelatedEventThreeGrid.ItemsSource = gridData;
                CurrentRelatedEvent = gridData;
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

            if (MeterPhaseType.Text.Split("&&").Length > 1)
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
                CurrentRelatedEventSingleGrid.Items.Clear();

                CurrentRelatedEventSinglePhaseCommand = new CurrentRelatedEventSinglePhaseCommand();

                var gridData = await CurrentRelatedEventSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                CurrentRelatedEventSingleGrid.ItemsSource = gridData;
                CurrentRelatedEventSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                CurrentRelatedEventCommand = new CurrentRelatedEventCommand();

                var gridData = await CurrentRelatedEventCommand.GetAll(pageSize, MeterNumber);

                if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    await GridCalcualtion(gridData);
                }

                CurrentRelatedEventThreeGrid.ItemsSource = gridData;
                CurrentRelatedEvent = gridData;
            }
        }

        private async Task<List<CurrentRelatedEventDto>> GridCalcualtion(List<CurrentRelatedEventDto> gridData)
        {
            foreach (var item in gridData)
            {
                item.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                item.Event = item.Event;
                item.CurrentIr = double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any).ToString(); ;
                item.CurrentIy =double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any).ToString(); ;
                item.CurrentIb = double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any).ToString(); ;
                item.VoltageVrn = double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any).ToString(); ;
                item.VoltageVyn = double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any).ToString(); ;
                item.VoltageVbn = double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any).ToString(); ;
                item.SignedPowerFactorRPhase =double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any).ToString(); ;
                item.SignedPowerFactorYPhase =double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any).ToString(); ;
                item.SignedPowerFactorBPhase =double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any).ToString(); ;
                item.CumulativeEnergykWhImport = double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any).ToString();
                item.CumulativeTamperCount = item.CumulativeTamperCount;
                item.CumulativeEnergykWhExport = double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any).ToString();
                item.CreatedOn = item.CreatedOn;
                item.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;
            }

            return gridData;
        }
      
        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });
            if (MeterType == Constants.SinglePhaseMeter)
            {
                CurrentRelatedEventThreeGrid.Visibility = Visibility.Collapsed;
                CurrentRelatedEventSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                CurrentRelatedEventSingleGrid.Visibility = Visibility.Collapsed;
                CurrentRelatedEventThreeGrid.Visibility = Visibility.Visible;
            }
        
            if(MeterPhaseType.Text.Split("&&").Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                   FilterByPageSize(null,null);
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
                List<CurrentRelatedEventSinglePhaseDto> CurrentRelatedEventSinglePhaseData = new();
                CurrentRelatedEventSinglePhaseData = CurrentRelatedEventSingleGrid.Items.OfType<CurrentRelatedEventSinglePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.CurrentRelatedEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(CurrentRelatedEventSinglePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName,Constants.CurrentRelatedEventSinglePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.CurrentRelatedEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(CurrentRelatedEventSinglePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.CurrentRelatedEventSinglePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<CurrentRelatedEventDto> CurrentRelatedEventData = new();
                CurrentRelatedEventData = CurrentRelatedEventThreeGrid.Items.OfType<CurrentRelatedEventDto>().ToList();

                string meterName = MeterType == Constants.ThreePhaseMeter ? Constants.ThreePhaseMeter : MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.CurrentRelatedEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(CurrentRelatedEventData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.CurrentRelatedEvent + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.CurrentRelatedEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(CurrentRelatedEventData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.CurrentRelatedEvent + meterName, MeterNumber);
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
                        var currentRelatedEventThreePhaseList = CurrentRelatedEventThreeGrid.Items.OfType<CurrentRelatedEventDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventThreeGrid.ItemsSource = currentRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var currentRelatedEventSinglePhaseList = CurrentRelatedEventSingleGrid.Items.OfType<CurrentRelatedEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventSingleGrid.ItemsSource = currentRelatedEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "DateAndTimeOfEvent")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var currentRelatedEventThreePhaseList = CurrentRelatedEventThreeGrid.Items.OfType<CurrentRelatedEventDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventThreeGrid.ItemsSource = currentRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var currentRelatedEventSinglePhaseList = CurrentRelatedEventSingleGrid.Items.OfType<CurrentRelatedEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.DateAndTimeOfEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventSingleGrid.ItemsSource = currentRelatedEventSinglePhaseList;

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
                        var currentRelatedEventThreePhaseList = CurrentRelatedEventThreeGrid.Items.OfType<CurrentRelatedEventDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventThreeGrid.ItemsSource = currentRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var currentRelatedEventSinglePhaseList = CurrentRelatedEventSingleGrid.Items.OfType<CurrentRelatedEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventSingleGrid.ItemsSource = currentRelatedEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "DateAndTimeOfEvent")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var currentRelatedEventThreePhaseList = CurrentRelatedEventThreeGrid.Items.OfType<CurrentRelatedEventDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventThreeGrid.ItemsSource = currentRelatedEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var currentRelatedEventSinglePhaseList = CurrentRelatedEventSingleGrid.Items.OfType<CurrentRelatedEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.DateAndTimeOfEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        CurrentRelatedEventSingleGrid.ItemsSource = currentRelatedEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }

    }
}
