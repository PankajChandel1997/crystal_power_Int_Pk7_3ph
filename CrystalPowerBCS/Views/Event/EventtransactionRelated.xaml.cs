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
    /// Interaction logic for EventtransactionRelated.xaml
    /// </summary>
    public partial class EventtransactionRelated : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<TransactionEventDto> TransactionEvent;
        public TransactionEventCommand TransactionEventCommand;
        public List<TransactionEventSinglePhaseDto> TransactionEventSinglePhase;
        public TransactionEventSinglePhaseCommand TransactionEventSinglePhaseCommand;
        public int pageSize = 10;
        public string MeterType;
        public string MeterNumber = "00000000";
        public ErrorHelper _errorHelper;
        public ListSortDirection SortDirection = ListSortDirection.Descending;

        public EventtransactionRelated()
        {
            InitializeComponent();

            TransactionEventSingleGrid.Visibility = Visibility.Visible;
            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;
            _errorHelper = new ErrorHelper();
            //TransactionEvent = new ObservableCollection<TransactionEventDto>();
        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                TransactionEventSingleGrid.MaxHeight = height - 217;
                TransactionEventThreeGrid.MaxHeight = height - 217;
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
            try
            {
                if (MeterType == Constants.SinglePhaseMeter)
                {
                    TransactionEventSingleGrid.Items.Clear();

                    TransactionEventSinglePhaseCommand = new TransactionEventSinglePhaseCommand();

                    var gridData = await TransactionEventSinglePhaseCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                    TransactionEventSingleGrid.ItemsSource = gridData;
                    TransactionEventSinglePhase = gridData;
                }
                else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    TransactionEventCommand = new TransactionEventCommand();

                    var gridData = await TransactionEventCommand.Filter(startDate, endDate, pageSize, MeterNumber);

                    TransactionEventThreeGrid.ItemsSource = gridData;
                    TransactionEvent = gridData;
                }
            }
            catch(Exception ex) 
            { 
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                TransactionEventSingleGrid.Items.Clear();

                TransactionEventSinglePhaseCommand = new TransactionEventSinglePhaseCommand();

                var gridData = await TransactionEventSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                TransactionEventSingleGrid.ItemsSource = gridData;
                TransactionEventSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                TransactionEventCommand = new TransactionEventCommand();

                var gridData = await TransactionEventCommand.GetAll(pageSize, MeterNumber);

                TransactionEventThreeGrid.ItemsSource = gridData;
                TransactionEvent = gridData;
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
                TransactionEventThreeGrid.Visibility = Visibility.Collapsed;
                TransactionEventSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                TransactionEventSingleGrid.Visibility = Visibility.Collapsed;
                TransactionEventThreeGrid.Visibility = Visibility.Visible;
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
                List<TransactionEventSinglePhaseDto> TransactionEventSinglePhaseData = new();
                TransactionEventSinglePhaseData = TransactionEventSingleGrid.Items.OfType<TransactionEventSinglePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.TransactionEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(TransactionEventSinglePhaseData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.TransactionEventSinglePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.TransactionEventSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(TransactionEventSinglePhaseData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.TransactionEventSinglePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter || MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<TransactionEventDto> TransactionEventData = new();
                TransactionEventData = TransactionEventThreeGrid.Items.OfType<TransactionEventDto>().ToList();

                string meterName = MeterType == Constants.ThreePhaseMeter ? Constants.ThreePhaseMeter : MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.TransactionEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(TransactionEventData);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.TransactionEvent + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.TransactionEventThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(TransactionEventData);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.TransactionEvent + meterName, MeterNumber);
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
                        var transactionEventThreePhaseList = TransactionEventThreeGrid.Items.OfType<TransactionEventDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventThreeGrid.ItemsSource = transactionEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var transactionEventSinglePhaseList = TransactionEventSingleGrid.Items.OfType<TransactionEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventSingleGrid.ItemsSource = transactionEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var transactionEventThreePhaseList = TransactionEventThreeGrid.Items.OfType<TransactionEventDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventThreeGrid.ItemsSource = transactionEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var transactionEventSinglePhaseList = TransactionEventSingleGrid.Items.OfType<TransactionEventSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventSingleGrid.ItemsSource = transactionEventSinglePhaseList;

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
                        var transactionEventThreePhaseList = TransactionEventThreeGrid.Items.OfType<TransactionEventDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventThreeGrid.ItemsSource = transactionEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var transactionEventSinglePhaseList = TransactionEventSingleGrid.Items.OfType<TransactionEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventSingleGrid.ItemsSource = transactionEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var transactionEventThreePhaseList = TransactionEventThreeGrid.Items.OfType<TransactionEventDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventThreeGrid.ItemsSource = transactionEventThreePhaseList;

                        e.Handled = true;
                    }
                    else
                    {
                        var transactionEventSinglePhaseList = TransactionEventSingleGrid.Items.OfType<TransactionEventSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        TransactionEventSingleGrid.ItemsSource = transactionEventSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }
    }
}
