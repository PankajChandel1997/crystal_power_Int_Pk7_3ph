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

namespace CrystalPowerBCS.Views.Event
{
    /// <summary>
    /// Interaction logic for AllEventsRelated.xaml
    /// </summary>
    /// 
    public partial class AllEventsRelated : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public ObservableCollection<AllEventsDTO> AllEventsThreePhase;
        public ControlEventCommand ControlEventCommand;
        public ObservableCollection<AllEventsSinglePhaseDto> AllEventsSinglePhase;
        public AllEventsCommand AllEventsCommand;
        public AllEventsSinglePhaseCommand AllEventsSinglePhaseCommand;
        public ErrorHelper _errorHelper;
        public int pageSize = int.MaxValue;
        public string MeterType;
        public string MeterNumber = "00000000";
        public ListSortDirection SortDirection = ListSortDirection.Descending;
        public AllEventsRelated()
        {
            InitializeComponent();

            string MeterTypess = MeterPhaseType.Text;

            AllEventsSingleGrid.Visibility = Visibility.Collapsed;
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
                AllEventsSingleGrid.MaxHeight = height - 217;
                AllEventThreeGrid.MaxHeight = height - 217;
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
                AllEventsSinglePhaseCommand = new AllEventsSinglePhaseCommand();
                var gridData = await AllEventsSinglePhaseCommand.GetAll(MeterNumber);


                if (page > 0 && startDate != null && endDate != null)
                {
                    gridData = gridData.Where(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)).Take(page).ToList();

                    await BindAllEventsSinglePhaseDto(gridData);

                }
                else if (page > 0)
                {
                    await BindAllEventsSinglePhaseDto(gridData.Take(page).ToList());
                }
                else
                {
                    await BindAllEventsSinglePhaseDto(gridData);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                AllEventsCommand = new AllEventsCommand();
                var gridData = await AllEventsCommand.GetAll(MeterNumber);


                if (page > 0 && startDate != null && endDate != null)
                {
                    gridData = gridData.Where(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)).Take(page).ToList();

                    await BindAllEventThreePhaseDto(gridData);

                }
                else if (page > 0)
                {
                    await BindAllEventThreePhaseDto(gridData.Take(page).ToList());
                }
                else
                {
                    await BindAllEventThreePhaseDto(gridData);
                }

            }
        }

        private async void FilterByDate(object sender, RoutedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate, endDate, pageSize); ;
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

            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, pageSize);
            });
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                AllEventsSinglePhaseCommand = new AllEventsSinglePhaseCommand();

                var gridData = await AllEventsSinglePhaseCommand.GetAll(MeterNumber);

                await BindAllEventsSinglePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                AllEventsCommand = new AllEventsCommand();

                var gridData = await AllEventsCommand.GetAll(MeterNumber);

                if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    gridData = await GridCalculation(gridData);
                }

                await BindAllEventThreePhaseDto(gridData);
            }
        }

        private async Task<List<AllEventsDTO>> GridCalculation(List<AllEventsDTO> gridData)
        {
            foreach (var item in gridData)
            {
                item.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                item.Event = item.Event;
                item.CurrentIr = !string.IsNullOrEmpty(item.CurrentIr) ? double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any).ToString() : "";
                item.CurrentIy = !string.IsNullOrEmpty(item.CurrentIy) ? double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any).ToString() : "";
                item.CurrentIb = !string.IsNullOrEmpty(item.CurrentIb) ? double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any).ToString() : "";
                item.VoltageVrn = !string.IsNullOrEmpty(item.VoltageVrn) ? double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any).ToString() : "";
                item.VoltageVyn = !string.IsNullOrEmpty(item.VoltageVyn) ?double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any).ToString() : "";
                item.VoltageVbn = !string.IsNullOrEmpty(item.VoltageVbn) ?double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any).ToString() : "";
                item.SignedPowerFactorRPhase = !string.IsNullOrEmpty(item.SignedPowerFactorRPhase) ?double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any).ToString() : "";
                item.SignedPowerFactorYPhase = !string.IsNullOrEmpty(item.SignedPowerFactorYPhase) ?double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any).ToString() : "";
                item.SignedPowerFactorBPhase = !string.IsNullOrEmpty(item.SignedPowerFactorBPhase) ?double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any).ToString() : "";
                item.CumulativeEnergykWhImport = !string.IsNullOrEmpty(item.CumulativeEnergykWhImport) ? double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any).ToString() : "";
                item.CumulativeTamperCount = item.CumulativeTamperCount;
                item.CumulativeEnergykWhExport = !string.IsNullOrEmpty(item.CumulativeEnergykWhExport) ?double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any).ToString() : "";
                item.CreatedOn = item.CreatedOn;
                item.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;
            }

            return gridData;
        }

        private async Task BindAllEventsSinglePhaseDto(List<AllEventsSinglePhaseDto> gridData)
        {
            try
            {
                AllEventsSinglePhase = new ObservableCollection<AllEventsSinglePhaseDto>();

                int index = 1;
                foreach (var item in gridData)
                {
                    AllEventsSinglePhase.Add(new AllEventsSinglePhaseDto
                    {
                        Sno = index,
                        RealTimeClock = item.RealTimeClock,
                        Event = item.Event,
                        Current = item.Current,
                        Voltage = item.Voltage,
                        PowerFactor = item.PowerFactor,
                        CumulativeEnergykWhExport = item.CumulativeEnergykWhExport,
                        CumulativeEnergykWhImport = item.CumulativeEnergykWhImport,
                        CumulativeTamperCount = item.CumulativeTamperCount,
                        CreatedOn = item.CreatedOn,
                        PowerFailureTime = item.PowerFailureTime,
                    });
                    index++;
                }

                AllEventsSingleGrid.ItemsSource = AllEventsSinglePhase;

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : ControlRelatedEvent : BindcontrolEventSinglePhaseDto : Exception ==>" + ex.Message);

            }

        }

        private async Task BindAllEventThreePhaseDto(List<AllEventsDTO> gridData)
        {
            try
            {
                AllEventsThreePhase = new ObservableCollection<AllEventsDTO>();
                
                int index = 1;

                foreach (var item in gridData)
                {
                    AllEventsThreePhase.Add(new AllEventsDTO
                    {
                        Sno = index,
                        RealTimeClockDateAndTime = item.RealTimeClockDateAndTime,
                        Event = item.Event,
                        CurrentIr = item.CurrentIr,
                        CurrentIy = item.CurrentIy,
                        CurrentIb = item.CurrentIb,
                        VoltageVrn = item.VoltageVrn,
                        VoltageVyn = item.VoltageVyn,
                        VoltageVbn = item.VoltageVbn,
                        SignedPowerFactorRPhase = item.SignedPowerFactorRPhase,
                        SignedPowerFactorYPhase = item.SignedPowerFactorYPhase,
                        SignedPowerFactorBPhase = item.SignedPowerFactorBPhase,
                        CumulativeEnergykWhImport = item.CumulativeEnergykWhImport,
                        CumulativeTamperCount = item.CumulativeTamperCount,
                        CumulativeEnergykWhExport = item.CumulativeEnergykWhExport,
                        CreatedOn = item.CreatedOn,
                        PowerFailureTime = item.PowerFailureTime,
                        GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber,
                    });
                    index++;
                }

                AllEventThreeGrid.ItemsSource = AllEventsThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : BindAllEventThreePhaseDto : Exception ==>" + ex.Message);

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
                AllEventThreeGrid.Visibility = Visibility.Collapsed;
                AllEventsSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                AllEventsSingleGrid.Visibility = Visibility.Collapsed;
                AllEventThreeGrid.Visibility = Visibility.Visible;
            }

            this.Dispatcher.Invoke(async () =>
            {
                await PopullateGrid();
            });

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
                List<AllEventsSinglePhaseDto> AllEventsSinglePhaseData = new();
                AllEventsSinglePhaseData = AllEventsSingleGrid.Items.OfType<AllEventsSinglePhaseDto>().ToList();
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.AllEventsSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(AllEventsSinglePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.AllEventsSinglePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.AllEventsSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(AllEventsSinglePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.AllEventsSinglePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<AllEventsDTO> AllEventsThreePhaseData = new();
                AllEventsThreePhaseData = AllEventThreeGrid.Items.OfType<AllEventsDTO>().ToList();

                string meterName = MeterType == Constants.ThreePhaseMeter ? Constants.ThreePhaseMeter : MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.AllEventsThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(AllEventsThreePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.AllEvents + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.AllEventsThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(AllEventsThreePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.AllEvents + meterName, MeterNumber);
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
                        var allEventThreePhaseList = AllEventThreeGrid.Items.OfType<AllEventsDTO>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventThreeGrid.ItemsSource = allEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var allEventsSinglePhaseList = AllEventsSingleGrid.Items.OfType<AllEventsSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventsSingleGrid.ItemsSource = allEventsSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "RealTimeClock")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var allEventThreePhaseList = AllEventThreeGrid.Items.OfType<AllEventsDTO>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventThreeGrid.ItemsSource = allEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var allEventsSinglePhaseList = AllEventsSingleGrid.Items.OfType<AllEventsSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventsSingleGrid.ItemsSource = allEventsSinglePhaseList;

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
                        var allEventThreePhaseList = AllEventThreeGrid.Items.OfType<AllEventsDTO>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventThreeGrid.ItemsSource = allEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var allEventsSinglePhaseList = AllEventsSingleGrid.Items.OfType<AllEventsSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventsSingleGrid.ItemsSource = allEventsSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "RealTimeClock")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var allEventThreePhaseList = AllEventThreeGrid.Items.OfType<AllEventsDTO>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventThreeGrid.ItemsSource = allEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var allEventsSinglePhaseList = AllEventsSingleGrid.Items.OfType<AllEventsSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        AllEventsSingleGrid.ItemsSource = allEventsSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }
    }
}
