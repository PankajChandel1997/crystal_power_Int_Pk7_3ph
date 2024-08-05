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
    /// Interaction logic for EventNonRollOver.xaml
    /// </summary>
    public partial class EventNonRollOver : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<NonRolloverEventDto> NonRolloverEvent;
        public NonRolloverEventCommand NonRolloverEventCommand;
        public List<NonRolloverEventSinglePhaseDto> NonRolloverEventSinglePhase;
        public NonRolloverEventSinglePhaseCommand NonRolloverEventSinglePhaseCommand;
        public int pageSize = 10;
        public string MeterType = Constants.SinglePhaseMeter;
        public string MeterNumber = "00000000";
        public ErrorHelper _errorHelper;
        public ListSortDirection SortDirection = ListSortDirection.Descending;

        public EventNonRollOver()
        {
            InitializeComponent();

            EventNonRolloverSingleGrid.Visibility = Visibility.Visible;
            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;
            _errorHelper = new ErrorHelper();

            //NonRolloverEvent = new ObservableCollection<NonRolloverEventDto>();

        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                EventNonRolloverSingleGrid.MaxHeight = height - 217;
                EventNonRolloverThreeGrid.MaxHeight = height - 217;
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
                EventNonRolloverSingleGrid.Items.Clear();

                NonRolloverEventSinglePhaseCommand = new NonRolloverEventSinglePhaseCommand();

                var gridData = await NonRolloverEventSinglePhaseCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                EventNonRolloverSingleGrid.ItemsSource = gridData;
                NonRolloverEventSinglePhase = gridData;
                //await BindEventNonRollOverSinglePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                NonRolloverEventCommand = new NonRolloverEventCommand();

                var gridData = await NonRolloverEventCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                EventNonRolloverThreeGrid.ItemsSource = gridData;
                NonRolloverEvent = gridData;
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                EventNonRolloverSingleGrid.Items.Clear();

                NonRolloverEventSinglePhaseCommand = new NonRolloverEventSinglePhaseCommand();

                var gridData = await NonRolloverEventSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                EventNonRolloverSingleGrid.ItemsSource = gridData;
                NonRolloverEventSinglePhase = gridData;
                //await BindEventNonRollOverSinglePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                NonRolloverEventCommand = new NonRolloverEventCommand();

                var gridData = await NonRolloverEventCommand.GetAll(pageSize, MeterNumber);

                EventNonRolloverThreeGrid.ItemsSource = gridData;
                NonRolloverEvent = gridData;
                //await BindEventNonRollOverThreePhaseDto(gridData);
            }
        }
        #region Binding Data to observabe Collection
        //private async Task BindEventNonRollOverSinglePhaseDto(List<NonRolloverEventSinglePhaseDto> gridData)
        //{
        //    try
        //    {
        //        NonRolloverEventSinglePhase = new ObservableCollection<NonRolloverEventSinglePhaseDto>();

        //        int index = 1;
        //        foreach (var item in gridData)
        //        {
        //            NonRolloverEventSinglePhase.Add(new NonRolloverEventSinglePhaseDto
        //            {
        //                Number = index,
        //                CreatedOn = item.CreatedOn,
        //                Event = item.Event,
        //                RealTimeClockDateAndTime = item.RealTimeClockDateAndTime
        //            });
        //            index++;
        //        }

        //        EventNonRolloverSingleGrid.ItemsSource = NonRolloverEventSinglePhase;
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorHelper.WriteLog(DateTime.UtcNow + " :  EventNonRollover : BindEventNonRollOverSinglePhaseDto : Exception ==>" + ex.Message);

        //    }

        //}

        //private async Task BindEventNonRollOverThreePhaseDto(List<NonRolloverEventDto> gridData)
        //{
        //    try
        //    {
        //        NonRolloverEvent = new ObservableCollection<NonRolloverEventDto>();

        //        int index = 1;
        //        foreach (var item in gridData)
        //        {
        //            NonRolloverEvent.Add(new NonRolloverEventDto
        //            {
        //                Number = index,
        //                CreatedOn = item.CreatedOn,
        //                RealTimeClockDateAndTime = item.RealTimeClockDateAndTime,
        //                Event = item.Event
        //            });
        //            index++;
        //        }

        //        EventNonRolloverThreeGrid.ItemsSource = NonRolloverEvent;
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorHelper.WriteLog(DateTime.UtcNow + " :  EventNonRollover : BindEventNonRollOverThreePhaseDto : Exception ==>" + ex.Message);


        //    }

        //}
        #endregion

        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });
            if (MeterType == Constants.SinglePhaseMeter)
            {
                EventNonRolloverThreeGrid.Visibility = Visibility.Collapsed;
                EventNonRolloverSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                EventNonRolloverSingleGrid.Visibility = Visibility.Collapsed;
                EventNonRolloverThreeGrid.Visibility = Visibility.Visible;
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
                List<NonRolloverEventSinglePhaseDto> NonRolloverEventSinglePhaseData = new();
                NonRolloverEventSinglePhaseData = EventNonRolloverSingleGrid.Items.OfType<NonRolloverEventSinglePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.NonRolloverEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(NonRolloverEventSinglePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.NonRolloverEventSinglePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.NonRolloverEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(NonRolloverEventSinglePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.NonRolloverEventSinglePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<NonRolloverEventDto> NonRolloverEventData = new();
                NonRolloverEventData = EventNonRolloverThreeGrid.Items.OfType<NonRolloverEventDto>().ToList();

                string meterName = MeterType == Constants.ThreePhaseMeter ? Constants.ThreePhaseMeter : MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.NonRolloverEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(NonRolloverEventData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.NonRolloverEventThreePhaseMeter + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.NonRolloverEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(NonRolloverEventData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.NonRolloverEventThreePhaseMeter + meterName, MeterNumber);
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
                        var eventNonRolloverThreePhaseList = EventNonRolloverThreeGrid.Items.OfType<NonRolloverEventDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverThreeGrid.ItemsSource = eventNonRolloverThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var eventNonRolloverSinglePhaseList = EventNonRolloverSingleGrid.Items.OfType<NonRolloverEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverSingleGrid.ItemsSource = eventNonRolloverSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "DateAndTimeOfEvent")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var eventNonRolloverThreePhaseList = EventNonRolloverThreeGrid.Items.OfType<NonRolloverEventDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverThreeGrid.ItemsSource = eventNonRolloverThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var eventNonRolloverSinglePhaseList = EventNonRolloverSingleGrid.Items.OfType<NonRolloverEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverSingleGrid.ItemsSource = eventNonRolloverSinglePhaseList;

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
                        var eventNonRolloverThreePhaseList = EventNonRolloverThreeGrid.Items.OfType<NonRolloverEventDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverThreeGrid.ItemsSource = eventNonRolloverThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var eventNonRolloverSinglePhaseList = EventNonRolloverSingleGrid.Items.OfType<NonRolloverEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverSingleGrid.ItemsSource = eventNonRolloverSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "DateAndTimeOfEvent")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var eventNonRolloverThreePhaseList = EventNonRolloverThreeGrid.Items.OfType<NonRolloverEventDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverThreeGrid.ItemsSource = eventNonRolloverThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var eventNonRolloverSinglePhaseList = EventNonRolloverSingleGrid.Items.OfType<NonRolloverEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        EventNonRolloverSingleGrid.ItemsSource = eventNonRolloverSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }
    }
}
