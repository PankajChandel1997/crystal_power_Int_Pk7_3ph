using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for Consumption.xaml
    /// </summary>
    public partial class Consumption : UserControl
    {
        private int _numValue = 0;
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<BillingProfileSinglePhaseDto> BillingProfileSinglePhase;
        public BillingProfileSinglePhaseCommand BillingProfileSinglePhaseCommand;

        public List<BillingProfileThreePhaseDto> BillingProfileThreePhase;
        public BillingProfileThreePhaseCommand BillingProfileThreePhaseCommand;

        public List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCT;
        public BillingProfileThreePhaseCTCommand BillingProfileThreePhaseCTCommand;
        public string MeterType;
        public string MeterNumber = "00000000";
        public int pageSize = 10;
        public string currentItem;
        public List<string> fatchedDates = new List<string>();

        public ErrorHelper _errorHelper;
        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }
        public Consumption()
        {
            InitializeComponent();
            BillingProfileSinglePhaseGrid.Visibility = Visibility.Visible;

            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;

            downarrow.Visibility = Visibility.Visible;
            uparrow.Visibility = Visibility.Collapsed;

            Filters.Visibility = Visibility.Collapsed;
            _errorHelper = new ErrorHelper();


            BillingProfileSinglePhase = new List<BillingProfileSinglePhaseDto>();


        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                Grid_Parent1.Height = height - 217;
                BillingProfileSinglePhaseGrid.MaxHeight = height - 217;
                BillingProfileThreePhaseGrid.MaxHeight = height - 217;
                BillingProfileThreePhaseCTGrid.MaxHeight = height - 217;
                MainGraphView.Height = height - 217;
            }
            catch (Exception ex)
            {
                return;
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ChangeView(object sender, RoutedEventArgs e)
        {
            IsGraph = !IsGraph;
            if (IsGraph == true)
            {
                TableView.Visibility = Visibility.Collapsed;
                listView.Visibility = Visibility.Collapsed;
                listViewDisabled.Visibility = Visibility.Visible;

                Paginator.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Visible;
                graphView.Visibility = Visibility.Visible;
                graphView.IsHitTestVisible = false;
                graphViewDisabled.Visibility = Visibility.Collapsed;
                graphDataFilter.Visibility = Visibility.Visible;
                graphFilter.Visibility = Visibility.Visible;
                ExportChartGrid.Visibility = Visibility.Visible;
                FatchDateFilter.Visibility = Visibility.Visible;

                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
            }
            else
            {
                TableView.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Visible;
                listView.IsHitTestVisible = false;
                listViewDisabled.Visibility = Visibility.Collapsed;
                Paginator.Visibility = Visibility.Visible;
                GraphView.Visibility = Visibility.Collapsed;
                graphView.Visibility = Visibility.Collapsed;
                graphViewDisabled.Visibility = Visibility.Visible;
                graphDataFilter.Visibility = Visibility.Collapsed;
                graphFilter.Visibility = Visibility.Collapsed;
                ExportChartGrid.Visibility = Visibility.Collapsed;
                FatchDateFilter.Visibility = Visibility.Collapsed;

                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
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
            

        private void CmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void CmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void TxtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }

        private void TxtNum_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (txtNum1 == null)
            {
                return;
            }

            if (!int.TryParse(txtNum1.Text, out _numValue))
                txtNum1.Text = _numValue.ToString();
        }

        private void ShowHideFilter(object sender, MouseButtonEventArgs e)
        {
            IsFilterEnabled = !IsFilterEnabled;
            if (IsFilterEnabled)
            {
                downarrow.Visibility = Visibility.Collapsed;
                uparrow.Visibility = Visibility.Visible;

                Filters.Visibility = Visibility.Visible;

            }
            else
            {
                downarrow.Visibility = Visibility.Visible;
                uparrow.Visibility = Visibility.Collapsed;

                Filters.Visibility = Visibility.Collapsed;

            }
        }

        private async void FilterByDate(object sender, RoutedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string fatchDate = FatchDateFilter.SelectedValue != null ? FatchDateFilter.SelectedValue.ToString() : null;

            pageSize = gridPageSize != null && gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize != null && gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate, endDate, fatchDate, pageSize);
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
            string fatchDate = FatchDateFilter.SelectedValue != null ? FatchDateFilter.SelectedValue.ToString() : null;

            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, fatchDate, pageSize);
            });
        }

        private async Task Filter(string startDate, string endDate, string fatchDate, int pageSize)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                if(gridData != null && gridData.Count > 0)
                    await BindBillingProfileSinglePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();

                var gridData = await BillingProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                if (gridData != null && gridData.Count > 0)
                    await BindBillingProfileThreePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();

                var gridData = await BillingProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                if (gridData != null && gridData.Count > 0)
                    await BindBillingProfileThreePhaseCTDto(gridData);
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {

                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);
                FatchDateFilter.Items.Clear();
                fatchedDates = gridData.DistinctBy(x => System.DateTime.Parse(x.RealTimeClock).Year).Select(d => d.RealTimeClock).Take(4).ToList();
                foreach (var fatchedDate in fatchedDates)
                {
                    FatchDateFilter.Items.Add(System.DateTime.Parse(fatchedDate).Year.ToString());
                }

                await BindBillingProfileSinglePhaseDto(gridData);


                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();


            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {

                BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();

                var gridData = await BillingProfileThreePhaseCommand.GetAll(pageSize, MeterNumber);
                FatchDateFilter.Items.Clear();
                fatchedDates = gridData.DistinctBy(x => System.DateTime.Parse(x.RealTimeClock).Year).Select(d => d.RealTimeClock).Take(4).ToList();
                foreach (var fatchedDate in fatchedDates)
                {
                    FatchDateFilter.Items.Add(System.DateTime.Parse(fatchedDate).Year.ToString());
                }

                await BindBillingProfileThreePhaseDto(gridData);

                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {

                BillingProfileThreePhaseCTCommand = new BillingProfileThreePhaseCTCommand();

                var gridData = await BillingProfileThreePhaseCTCommand.GetAll(pageSize, MeterNumber);
                FatchDateFilter.Items.Clear();
                fatchedDates = gridData.DistinctBy(x => System.DateTime.Parse(x.BillingDate).Year).Select(d => d.BillingDate).Take(4).ToList();
                foreach (var fatchedDate in fatchedDates)
                {
                    FatchDateFilter.Items.Add(System.DateTime.Parse(fatchedDate).Year.ToString());
                }

                await BindBillingProfileThreePhaseCTDto(gridData);

                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
            }
        }

        private async Task BindBillingProfileSinglePhaseDto(List<BillingProfileSinglePhaseDto> gridData)
        {
            try
            {
                if(gridData.Count == 0)
                {
                    return;
                }
                BillingProfileSinglePhase = new List<BillingProfileSinglePhaseDto>();
                int index = 1;
                List<BillingProfileSinglePhaseDto> filteredDateList = new List<BillingProfileSinglePhaseDto>();

                string dateFormat = "dd-MM-yyyy HH:mm:ss";

                var list = gridData.OrderBy(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).ToList();

                string oldestDate = gridData.OrderBy(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).FirstOrDefault().RealTimeClock;
                string newestDate = gridData.OrderByDescending(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).FirstOrDefault().RealTimeClock;

                int monthsApart = 12 * (System.DateTime.Parse(newestDate).Year - System.DateTime.Parse(oldestDate).Year) + (System.DateTime.Parse(newestDate).Month - System.DateTime.Parse(oldestDate).Month);

                List<System.DateTime> datesList = new List<System.DateTime>();

                for (int i = 0; i <= monthsApart; i++)
                {
                    System.DateTime dateTime = System.DateTime.ParseExact(oldestDate, dateFormat, CultureInfo.InvariantCulture).AddMonths(i);

                    datesList.Add(dateTime);
                }

                foreach(var item in datesList)
                {
                    var newData = gridData.Where(x => System.DateTime.ParseExact(x.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Month == item.Month && System.DateTime.ParseExact(x.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Year == item.Year).OrderBy(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).FirstOrDefault();
                    if (newData != null)
                    {
                        newData.Number = filteredDateList.Count + 1;

                        filteredDateList.Add(newData);
                    }
                }

                foreach (var item in filteredDateList)
                {
                    BillingProfileSinglePhaseDto billingprofile = new BillingProfileSinglePhaseDto();
                    billingprofile.Number = index;
                    billingprofile.RealTimeClock = item.RealTimeClock;
                    billingprofile.CreatedOn = item.CreatedOn;
                    billingprofile.AveragePowerFactor = item.AveragePowerFactor.ToString();
                    billingprofile.CumulativeEnergykWhImport = item.CumulativeEnergykWhImport.ToString();
                    billingprofile.CumulativeEnergykWhTZ1 = item.CumulativeEnergykWhTZ1.ToString();
                    billingprofile.CumulativeEnergykWhTZ2 = item.CumulativeEnergykWhTZ2.ToString();
                    billingprofile.CumulativeEnergykWhTZ3 = item.CumulativeEnergykWhTZ3.ToString();
                    billingprofile.CumulativeEnergykWhTZ4 = item.CumulativeEnergykWhTZ4.ToString();
                    billingprofile.CumulativeEnergykWhTZ5 = item.CumulativeEnergykWhTZ5.ToString();
                    billingprofile.CumulativeEnergykWhTZ6 = item.CumulativeEnergykWhTZ6.ToString();
                    billingprofile.CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport.ToString();
                    billingprofile.CumulativeEnergykVAhTZ1 = item.CumulativeEnergykVAhTZ1.ToString();
                    billingprofile.CumulativeEnergykVAhTZ2 = item.CumulativeEnergykVAhTZ2.ToString();
                    billingprofile.CumulativeEnergykVAhTZ3 = item.CumulativeEnergykVAhTZ3.ToString();
                    billingprofile.CumulativeEnergykVAhTZ4 = item.CumulativeEnergykVAhTZ4.ToString();
                    billingprofile.CumulativeEnergykVAhTZ5 = item.CumulativeEnergykVAhTZ5.ToString();
                    billingprofile.CumulativeEnergykVAhTZ6 = item.CumulativeEnergykVAhTZ6;
                    billingprofile.MaximumDemandkW = item.MaximumDemandkW.ToString();
                    billingprofile.MaximumDemandkWDateTime = item.MaximumDemandkWDateTime;
                    billingprofile.MaximumDemandkVA = item.MaximumDemandkVA.ToString();
                    billingprofile.MaximumDemandkVADateTime = item.MaximumDemandkVADateTime;
                    billingprofile.BillingPowerONdurationinMinutes = item.BillingPowerONdurationinMinutes;
                    billingprofile.CumulativeEnergykWhExport = item.CumulativeEnergykWhExport.ToString();
                    billingprofile.CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport.ToString();

                    var lastMonthReading = index == 1 ? null : filteredDateList.FirstOrDefault(x => x.Number == (index - 1));

                    billingprofile.CumulativeEnergykWhImportConsumption =  lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykWhImport) - double.Parse(lastMonthReading.CumulativeEnergykWhImport)), 2).ToString();

                    billingprofile.CumulativeEnergykWhExportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykWhExport) - double.Parse(lastMonthReading.CumulativeEnergykWhExport)), 2).ToString();

                    billingprofile.CumulativeEnergykVAhImportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykVAhImport) - double.Parse(lastMonthReading.CumulativeEnergykVAhImport)), 2).ToString();

                    billingprofile.CumulativeEnergykVAhExportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykVAhExport) - double.Parse(lastMonthReading.CumulativeEnergykVAhExport)), 2).ToString();

                    //Updating old Entry
                    var previousMonthEntry = BillingProfileSinglePhase.FirstOrDefault(x => x.Number == (index - 1));
                    if (previousMonthEntry != null)
                    {
                        previousMonthEntry.CumulativeEnergykWhImportConsumption = billingprofile.CumulativeEnergykWhImportConsumption;
                        previousMonthEntry.CumulativeEnergykWhExportConsumption = billingprofile.CumulativeEnergykWhExportConsumption;
                        previousMonthEntry.CumulativeEnergykVAhImportConsumption = billingprofile.CumulativeEnergykVAhImportConsumption;
                        previousMonthEntry.CumulativeEnergykVAhExportConsumption = billingprofile.CumulativeEnergykVAhExportConsumption;
                    }
                    if(index == filteredDateList.Count)
                    {
                        billingprofile.CumulativeEnergykWhImportConsumption = "0";
                        billingprofile.CumulativeEnergykWhExportConsumption = "0";
                        billingprofile.CumulativeEnergykVAhImportConsumption = "0";
                        billingprofile.CumulativeEnergykVAhExportConsumption = "0";
                    }
                    BillingProfileSinglePhase.Add(billingprofile);

                    index++;
                }

                BillingProfileSinglePhaseGrid.ItemsSource = BillingProfileSinglePhase;

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(System.DateTime.UtcNow + " : Billingprofile : BillingProfileSinglePhaseDto : Exception ==>" + ex.Message);
            }
        }

        private async Task BindBillingProfileThreePhaseDto(List<BillingProfileThreePhaseDto> gridData)
        {
            try
            {
                if (gridData.Count == 0)
                {
                    return;
                }
                BillingProfileThreePhase = new List<BillingProfileThreePhaseDto>();
                int index = 1;
                List<BillingProfileThreePhaseDto> filteredDateList = new List<BillingProfileThreePhaseDto>();

                var list = gridData.OrderBy(y => System.DateTime.Parse(y.RealTimeClock)).ToList();

                string oldestDate = gridData.OrderBy(y => System.DateTime.Parse(y.RealTimeClock)).FirstOrDefault().RealTimeClock;
                string newestDate = gridData.OrderByDescending(y => System.DateTime.Parse(y.RealTimeClock)).FirstOrDefault().RealTimeClock;

                int monthsApart = 12 * (System.DateTime.Parse(newestDate).Year - System.DateTime.Parse(oldestDate).Year) + (System.DateTime.Parse(newestDate).Month - System.DateTime.Parse(oldestDate).Month);

                List<System.DateTime> datesList = new List<System.DateTime>();

                for (int i = 0; i <= monthsApart; i++)
                {
                    System.DateTime dateTime = System.DateTime.Parse(oldestDate).AddMonths(i);

                    datesList.Add(dateTime);
                }

                foreach (var item in datesList)
                {
                    var newData = gridData.Where(x => System.DateTime.Parse(x.RealTimeClock).Month == item.Month && System.DateTime.Parse(x.RealTimeClock).Year == item.Year).OrderBy(y => System.DateTime.Parse(y.RealTimeClock)).FirstOrDefault();
                    if (newData != null)
                    {
                        newData.Number = filteredDateList.Count+1;

                        filteredDateList.Add(newData);
                    }
                }

                foreach (var item in filteredDateList)
                {
                    var lastMonthReading = index == 1 ? null : filteredDateList.FirstOrDefault(x => x.Number == (index - 1));

                    if (lastMonthReading == null)
                    {
                        item.CumulativeEnergykWhImportConsumption = item.CumulativeEnergykWh;

                        item.CumulativeEnergykWhExportConsumption = item.CumulativeEnergykWhExport;

                        item.CumulativeEnergykVAhImportConsumption = item.CumulativeEnergykVAhImport;

                        item.CumulativeEnergykVAhExportConsumption = item.CumulativeEnergykVAhExport;

                        BillingProfileThreePhase.Add(item);
                    }
                    else
                    {
                        BillingProfileThreePhaseDto billingDTO = new BillingProfileThreePhaseDto();

                        billingDTO.Number = index;
                        billingDTO.CreatedOn = item.CreatedOn;
                        billingDTO.RealTimeClock = item.RealTimeClock;
                        billingDTO.SystemPowerFactorImport = item.SystemPowerFactorImport;
                        billingDTO.CumulativeEnergykWh = item.CumulativeEnergykWh;
                        billingDTO.CumulativeEnergykWhTZ1 = item.CumulativeEnergykWhTZ1;
                        billingDTO.CumulativeEnergykWhTZ2 = item.CumulativeEnergykWhTZ2;
                        billingDTO.CumulativeEnergykWhTZ3 = item.CumulativeEnergykWhTZ3;
                        billingDTO.CumulativeEnergykWhTZ4 = item.CumulativeEnergykWhTZ4;
                        billingDTO.CumulativeEnergykWhTZ5 = item.CumulativeEnergykWhTZ5;
                        billingDTO.CumulativeEnergykWhTZ6 = item.CumulativeEnergykWhTZ6;
                        billingDTO.CumulativeEnergykWhTZ7 = item.CumulativeEnergykWhTZ7;
                        billingDTO.CumulativeEnergykWhTZ8 = item.CumulativeEnergykWhTZ8;
                        billingDTO.CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport;
                        billingDTO.CumulativeEnergykVAhTZ1 = item.CumulativeEnergykVAhTZ1;
                        billingDTO.CumulativeEnergykVAhTZ2 = item.CumulativeEnergykVAhTZ2;
                        billingDTO.CumulativeEnergykVAhTZ3 = item.CumulativeEnergykVAhTZ3;
                        billingDTO.CumulativeEnergykVAhTZ4 = item.CumulativeEnergykVAhTZ4;
                        billingDTO.CumulativeEnergykVAhTZ5 = item.CumulativeEnergykVAhTZ5;
                        billingDTO.CumulativeEnergykVAhTZ6 = item.CumulativeEnergykVAhTZ6;
                        billingDTO.CumulativeEnergykVAhTZ7 = item.CumulativeEnergykVAhTZ7;
                        billingDTO.CumulativeEnergykVAhTZ8 = item.CumulativeEnergykVAhTZ8;
                        billingDTO.MaximumDemandkW = item.MaximumDemandkW;
                        billingDTO.MaximumDemandkWDateTime = item.MaximumDemandkWDateTime;
                        billingDTO.MaximumDemandkWForTZ1 = item.MaximumDemandkWForTZ1;
                        billingDTO.MaximumDemandkWForTZ1DateTime = item.MaximumDemandkWForTZ1DateTime;
                        billingDTO.MaximumDemandkWForTZ2 = item.MaximumDemandkWForTZ2;
                        billingDTO.MaximumDemandkWForTZ2DateTime = item.MaximumDemandkWForTZ2DateTime;
                        billingDTO.MaximumDemandkWForTZ3 = item.MaximumDemandkWForTZ3;
                        billingDTO.MaximumDemandkWForTZ3DateTime = item.MaximumDemandkWForTZ3DateTime;
                        billingDTO.MaximumDemandkWForTZ4 = item.MaximumDemandkWForTZ4;
                        billingDTO.MaximumDemandkWForTZ4DateTime = item.MaximumDemandkWForTZ4DateTime;
                        billingDTO.MaximumDemandkWForTZ5 = item.MaximumDemandkWForTZ5;
                        billingDTO.MaximumDemandkWForTZ5DateTime = item.MaximumDemandkWForTZ5DateTime;
                        billingDTO.MaximumDemandkWForTZ6 = item.MaximumDemandkWForTZ6;
                        billingDTO.MaximumDemandkWForTZ6DateTime = item.MaximumDemandkWForTZ6DateTime;
                        billingDTO.MaximumDemandkWForTZ7 = item.MaximumDemandkWForTZ7;
                        billingDTO.MaximumDemandkWForTZ7DateTime = item.MaximumDemandkWForTZ7DateTime;
                        billingDTO.MaximumDemandkWForTZ8 = item.MaximumDemandkWForTZ8;
                        billingDTO.MaximumDemandkWForTZ8DateTime = item.MaximumDemandkWForTZ8DateTime;
                        billingDTO.MaximumDemandkVA = item.MaximumDemandkVA;
                        billingDTO.MaximumDemandkVADateTime = item.MaximumDemandkVADateTime;
                        billingDTO.MaximumDemandkVAForTZ1 = item.MaximumDemandkVAForTZ1;
                        billingDTO.MaximumDemandkVAForTZ1DateTime = item.MaximumDemandkVAForTZ1DateTime;
                        billingDTO.MaximumDemandkVAForTZ2 = item.MaximumDemandkVAForTZ2;
                        billingDTO.MaximumDemandkVAForTZ2DateTime = item.MaximumDemandkVAForTZ2DateTime;
                        billingDTO.MaximumDemandkVAForTZ3 = item.MaximumDemandkVAForTZ3;
                        billingDTO.MaximumDemandkVAForTZ3DateTime = item.MaximumDemandkVAForTZ3DateTime;
                        billingDTO.MaximumDemandkVAForTZ4 = item.MaximumDemandkVAForTZ4;
                        billingDTO.MaximumDemandkVAForTZ4DateTime = item.MaximumDemandkVAForTZ4DateTime;
                        billingDTO.MaximumDemandkVAForTZ5 = item.MaximumDemandkVAForTZ5;
                        billingDTO.MaximumDemandkVAForTZ5DateTime = item.MaximumDemandkVAForTZ5DateTime;
                        billingDTO.MaximumDemandkVAForTZ6 = item.MaximumDemandkVAForTZ6;
                        billingDTO.MaximumDemandkVAForTZ6DateTime = item.MaximumDemandkVAForTZ6DateTime;
                        billingDTO.MaximumDemandkVAForTZ7 = item.MaximumDemandkVAForTZ7;
                        billingDTO.MaximumDemandkVAForTZ7DateTime = item.MaximumDemandkVAForTZ7DateTime;
                        billingDTO.MaximumDemandkVAForTZ8 = item.MaximumDemandkVAForTZ8;
                        billingDTO.MaximumDemandkVAForTZ8DateTime = item.MaximumDemandkVAForTZ8DateTime;
                        billingDTO.BillingPowerONdurationInMinutesDBP = item.BillingPowerONdurationInMinutesDBP;
                        billingDTO.CumulativeEnergykWhExport = item.CumulativeEnergykWhExport;
                        billingDTO.CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport;
                        billingDTO.CumulativeEnergykVArhQ1 = item.CumulativeEnergykVArhQ1;
                        billingDTO.CumulativeEnergykVArhQ2 = item.CumulativeEnergykVArhQ2;
                        billingDTO.CumulativeEnergykVArhQ3 = item.CumulativeEnergykVArhQ3;
                        billingDTO.CumulativeEnergykVArhQ4 = item.CumulativeEnergykVArhQ4;
                        billingDTO.TamperCount = item.TamperCount;

                        billingDTO.CumulativeEnergykWhImportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykWh) - double.Parse(lastMonthReading.CumulativeEnergykWh)), 2).ToString();

                        billingDTO.CumulativeEnergykWhExportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykWhExport) - double.Parse(lastMonthReading.CumulativeEnergykWhExport)), 2).ToString();

                        billingDTO.CumulativeEnergykVAhImportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykVAhImport) - double.Parse(lastMonthReading.CumulativeEnergykVAhImport)), 2).ToString();

                        billingDTO.CumulativeEnergykVAhExportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykVAhExport) - double.Parse(lastMonthReading.CumulativeEnergykVAhExport)), 2).ToString();

                        BillingProfileThreePhase.Add(billingDTO);
                    }

                    index++;
                }

                BillingProfileThreePhaseGrid.ItemsSource = BillingProfileThreePhase;

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(System.DateTime.UtcNow + " : Billingprofile : BillingProfileThreePhaseDto : Exception ==>" + ex.Message);
            }
        }

        //three phase ltct htct
        private async Task BindBillingProfileThreePhaseCTDto(List<BillingProfileThreePhaseCTDto> gridData)
        {
            try
            {
                if (gridData.Count == 0)
                {
                    return;
                }
                BillingProfileThreePhaseCT = new List<BillingProfileThreePhaseCTDto>();
                int index = 1;
                List<BillingProfileThreePhaseCTDto> filteredDateList = new List<BillingProfileThreePhaseCTDto>();

                var list = gridData.OrderBy(y => System.DateTime.Parse(y.BillingDate)).ToList();

                string oldestDate = gridData.OrderBy(y => System.DateTime.Parse(y.BillingDate)).FirstOrDefault().BillingDate;
                string newestDate = gridData.OrderByDescending(y => System.DateTime.Parse(y.BillingDate)).FirstOrDefault().BillingDate;

                int monthsApart = 12 * (System.DateTime.Parse(newestDate).Year - System.DateTime.Parse(oldestDate).Year) + (System.DateTime.Parse(newestDate).Month - System.DateTime.Parse(oldestDate).Month);

                List<System.DateTime> datesList = new List<System.DateTime>();

                for (int i = 0; i <= monthsApart; i++)
                {
                    System.DateTime dateTime = System.DateTime.Parse(oldestDate).AddMonths(i);

                    datesList.Add(dateTime);
                }

                foreach (var item in datesList)
                {
                    var newData = gridData.Where(x => System.DateTime.Parse(x.BillingDate).Month == item.Month && System.DateTime.Parse(x.BillingDate).Year == item.Year).OrderBy(y => System.DateTime.Parse(y.BillingDate)).FirstOrDefault();
                    if (newData != null)
                    {
                        newData.Number = filteredDateList.Count + 1;

                        filteredDateList.Add(newData);
                    }
                }

                foreach (var item in filteredDateList)
                {
                    BillingProfileThreePhaseCTDto billingDTO = new BillingProfileThreePhaseCTDto();

                    billingDTO.Number = index;
                    billingDTO.CreatedOn = item.CreatedOn;
                    billingDTO.BillingDate = item.BillingDate;
                    //billingDTO.SystemPowerFactorImport = item.SystemPowerFactorImport;
                    billingDTO.CumulativeEnergykWh = item.CumulativeEnergykWh;
                    billingDTO.CumulativeEnergykWhTZ1 = item.CumulativeEnergykWhTZ1;
                    billingDTO.CumulativeEnergykWhTZ2 = item.CumulativeEnergykWhTZ2;
                    billingDTO.CumulativeEnergykWhTZ3 = item.CumulativeEnergykWhTZ3;
                    billingDTO.CumulativeEnergykWhTZ4 = item.CumulativeEnergykWhTZ4;
                    billingDTO.CumulativeEnergykWhTZ5 = item.CumulativeEnergykWhTZ5;
                    billingDTO.CumulativeEnergykWhTZ6 = item.CumulativeEnergykWhTZ6;
                    billingDTO.CumulativeEnergykWhTZ7 = item.CumulativeEnergykWhTZ7;
                    billingDTO.CumulativeEnergykWhTZ8 = item.CumulativeEnergykWhTZ8;
                    billingDTO.CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport;
                    billingDTO.CumulativeEnergykVAhTZ1 = item.CumulativeEnergykVAhTZ1;
                    billingDTO.CumulativeEnergykVAhTZ2 = item.CumulativeEnergykVAhTZ2;
                    billingDTO.CumulativeEnergykVAhTZ3 = item.CumulativeEnergykVAhTZ3;
                    billingDTO.CumulativeEnergykVAhTZ4 = item.CumulativeEnergykVAhTZ4;
                    billingDTO.CumulativeEnergykVAhTZ5 = item.CumulativeEnergykVAhTZ5;
                    billingDTO.CumulativeEnergykVAhTZ6 = item.CumulativeEnergykVAhTZ6;
                    billingDTO.CumulativeEnergykVAhTZ7 = item.CumulativeEnergykVAhTZ7;
                    billingDTO.CumulativeEnergykVAhTZ8 = item.CumulativeEnergykVAhTZ8;
                    billingDTO.MaximumDemandkW = item.MaximumDemandkW;
                    billingDTO.MaximumDemandkWDateTime = item.MaximumDemandkWDateTime;
                    billingDTO.MaximumDemandkWForTZ1 = item.MaximumDemandkWForTZ1;
                    billingDTO.MaximumDemandkWForTZ1DateTime = item.MaximumDemandkWForTZ1DateTime;
                    billingDTO.MaximumDemandkWForTZ2 = item.MaximumDemandkWForTZ2;
                    billingDTO.MaximumDemandkWForTZ2DateTime = item.MaximumDemandkWForTZ2DateTime;
                    billingDTO.MaximumDemandkWForTZ3 = item.MaximumDemandkWForTZ3;
                    billingDTO.MaximumDemandkWForTZ3DateTime = item.MaximumDemandkWForTZ3DateTime;
                    billingDTO.MaximumDemandkWForTZ4 = item.MaximumDemandkWForTZ4;
                    billingDTO.MaximumDemandkWForTZ4DateTime = item.MaximumDemandkWForTZ4DateTime;
                    billingDTO.MaximumDemandkWForTZ5 = item.MaximumDemandkWForTZ5;
                    billingDTO.MaximumDemandkWForTZ5DateTime = item.MaximumDemandkWForTZ5DateTime;
                    billingDTO.MaximumDemandkWForTZ6 = item.MaximumDemandkWForTZ6;
                    billingDTO.MaximumDemandkWForTZ6DateTime = item.MaximumDemandkWForTZ6DateTime;
                    billingDTO.MaximumDemandkWForTZ7 = item.MaximumDemandkWForTZ7;
                    billingDTO.MaximumDemandkWForTZ7DateTime = item.MaximumDemandkWForTZ7DateTime;
                    billingDTO.MaximumDemandkWForTZ8 = item.MaximumDemandkWForTZ8;
                    billingDTO.MaximumDemandkWForTZ8DateTime = item.MaximumDemandkWForTZ8DateTime;
                    billingDTO.MaximumDemandkVA = item.MaximumDemandkVA;
                    billingDTO.MaximumDemandkVADateTime = item.MaximumDemandkVADateTime;
                    billingDTO.MaximumDemandkVAForTZ1 = item.MaximumDemandkVAForTZ1;
                    billingDTO.MaximumDemandkVAForTZ1DateTime = item.MaximumDemandkVAForTZ1DateTime;
                    billingDTO.MaximumDemandkVAForTZ2 = item.MaximumDemandkVAForTZ2;
                    billingDTO.MaximumDemandkVAForTZ2DateTime = item.MaximumDemandkVAForTZ2DateTime;
                    billingDTO.MaximumDemandkVAForTZ3 = item.MaximumDemandkVAForTZ3;
                    billingDTO.MaximumDemandkVAForTZ3DateTime = item.MaximumDemandkVAForTZ3DateTime;
                    billingDTO.MaximumDemandkVAForTZ4 = item.MaximumDemandkVAForTZ4;
                    billingDTO.MaximumDemandkVAForTZ4DateTime = item.MaximumDemandkVAForTZ4DateTime;
                    billingDTO.MaximumDemandkVAForTZ5 = item.MaximumDemandkVAForTZ5;
                    billingDTO.MaximumDemandkVAForTZ5DateTime = item.MaximumDemandkVAForTZ5DateTime;
                    billingDTO.MaximumDemandkVAForTZ6 = item.MaximumDemandkVAForTZ6;
                    billingDTO.MaximumDemandkVAForTZ6DateTime = item.MaximumDemandkVAForTZ6DateTime;
                    billingDTO.MaximumDemandkVAForTZ7 = item.MaximumDemandkVAForTZ7;
                    billingDTO.MaximumDemandkVAForTZ7DateTime = item.MaximumDemandkVAForTZ7DateTime;
                    billingDTO.MaximumDemandkVAForTZ8 = item.MaximumDemandkVAForTZ8;
                    billingDTO.MaximumDemandkVAForTZ8DateTime = item.MaximumDemandkVAForTZ8DateTime;
                    billingDTO.BillingPowerONdurationInMinutesDBP = item.BillingPowerONdurationInMinutesDBP;
                    billingDTO.CumulativeEnergykWhExport = item.CumulativeEnergykWhExport;
                    billingDTO.CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport;
                    billingDTO.CumulativeEnergykVArhQ1 = item.CumulativeEnergykVArhQ1;
                    billingDTO.CumulativeEnergykVArhQ2 = item.CumulativeEnergykVArhQ2;
                    billingDTO.CumulativeEnergykVArhQ3 = item.CumulativeEnergykVArhQ3;
                    billingDTO.CumulativeEnergykVArhQ4 = item.CumulativeEnergykVArhQ4;
                    //billingDTO.TamperCount = item.TamperCount;

                    var lastMonthReading = index == 1 ? null : filteredDateList.FirstOrDefault(x => x.Number == (index - 1));

                    billingDTO.CumulativeEnergykWhImportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykWh) - double.Parse(lastMonthReading.CumulativeEnergykWh)), 2).ToString();

                    billingDTO.CumulativeEnergykWhExportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykWhExport) - double.Parse(lastMonthReading.CumulativeEnergykWhExport)), 2).ToString();

                    billingDTO.CumulativeEnergykVAhImportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykVAhImport) - double.Parse(lastMonthReading.CumulativeEnergykVAhImport)), 2).ToString();

                    billingDTO.CumulativeEnergykVAhExportConsumption = lastMonthReading == null ? "0" : Math.Round((double.Parse(item.CumulativeEnergykVAhExport) - double.Parse(lastMonthReading.CumulativeEnergykVAhExport)), 2).ToString();

                    //Updating old Entry
                    var previousMonthEntry = BillingProfileThreePhaseCT.FirstOrDefault(x => x.Number == (index - 1));
                    if (previousMonthEntry != null)
                    {
                        previousMonthEntry.CumulativeEnergykWhImportConsumption = billingDTO.CumulativeEnergykWhImportConsumption;
                        previousMonthEntry.CumulativeEnergykWhExportConsumption = billingDTO.CumulativeEnergykWhExportConsumption;
                        previousMonthEntry.CumulativeEnergykVAhImportConsumption = billingDTO.CumulativeEnergykVAhImportConsumption;
                        previousMonthEntry.CumulativeEnergykVAhExportConsumption = billingDTO.CumulativeEnergykVAhExportConsumption;
                    }

                    if (index == filteredDateList.Count)
                    {
                        billingDTO.CumulativeEnergykWhImportConsumption = "0";
                        billingDTO.CumulativeEnergykWhExportConsumption = "0";
                        billingDTO.CumulativeEnergykVAhImportConsumption = "0";
                        billingDTO.CumulativeEnergykVAhExportConsumption = "0";
                    }

                    BillingProfileThreePhaseCT.Add(billingDTO);

                    index++;
                }

                BillingProfileThreePhaseCTGrid.ItemsSource = BillingProfileThreePhaseCT;

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(System.DateTime.UtcNow + " : Billingprofile : BillingProfileThreePhaseDto : Exception ==>" + ex.Message);
            }
        }

        private void CreateImageFromCartesianControl()
        {
            // you can take any chart in the UI, and build an image from it 
            var chartControl = (CartesianChart)FindName(Constants.CartesianChart);
            var skChart = new SKCartesianChart(chartControl) { Width = 900, Height = 600, };
            skChart.SaveImage("CartesianImageFromControl.png");
        }
        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BillingProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Clear();
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExportConsumption);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if(MeterType==Constants.ThreePhaseMeter)
            {
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Clear();
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExportConsumption);


                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Clear();
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImportConsumption);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExportConsumption);


                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
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
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ConsumptionSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(BillingProfileSinglePhase, null,null, FileName, Constants.ConsumptionSinglePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ConsumptionSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(BillingProfileSinglePhase, null,null, FileName, Constants.ConsumptionSinglePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ConsumptionThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(null, BillingProfileThreePhase,null, FileName, Constants.ConsumptionThreePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ConsumptionThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(null, BillingProfileThreePhase,null, FileName, Constants.ConsumptionThreePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                string meterName = MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ConsumptionThreePhaseCT + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(null,null, BillingProfileThreePhaseCT, FileName, Constants.ConsumptionThreePhaseCT + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ConsumptionThreePhaseCT + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(null,null, BillingProfileThreePhaseCT, FileName, Constants.ConsumptionThreePhaseCT + meterName, MeterNumber);
                }
            }
            cbExport.SelectedIndex = 0;
        }

        private void FilterGraph(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                string toolTip = string.Empty;
                currentItem = graphDataFilter.SelectedValue?.ToString();

                string currentGraph = ((System.Windows.Controls.ContentControl)graphFilter.SelectedValue).Content as string;

                if (currentItem == null || MeterType == null)
                {
                    return;
                }

                List<string> labels = new List<string>();
                List<double> values = new List<double>();

                if (MeterType == Constants.SinglePhaseMeter)
                {
                    if (BillingProfileSinglePhase == null || BillingProfileSinglePhase.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in BillingProfileSinglePhase)
                    {
                        if (currentItem ==  Constants.CumulativeEnergykWhImportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhImportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhExportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhExportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhImportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhImportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==Constants. CumulativeEnergykVAhExportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhExportConsumption, System.Globalization.NumberStyles.Any));
                        }

                    }
                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    if (BillingProfileThreePhase == null || BillingProfileThreePhase.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in BillingProfileThreePhase)
                    {
                        if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWh)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWh, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhExport, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ1, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ2, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ3, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ4, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ5, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ6, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ7, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ8, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ1, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ2, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ3, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ4, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ5, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ6, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ7, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ8, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ1, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ2, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ3, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ4, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhImportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhImportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhExportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhExportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhImportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhImportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==Constants. CumulativeEnergykVAhExportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhExportConsumption, System.Globalization.NumberStyles.Any));
                        }
                    }
                }
                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (BillingProfileThreePhaseCT == null || BillingProfileThreePhaseCT.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in BillingProfileThreePhaseCT)
                    {
                        if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWh)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWh, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhExport, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ1, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ2, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ3, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ4, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ5, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ6, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ7, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhTZ8, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ1, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ2, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ3, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ4, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ5, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ6, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ7, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhTZ8, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ1)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ1, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ2)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ2, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ3)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ3, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ4)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVArhQ4, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhImportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhImportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhExportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykWhExportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhImportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhImportConsumption, System.Globalization.NumberStyles.Any));
                        }
                        else if (currentItem ==Constants. CumulativeEnergykVAhExportConsumption)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/yyyy"));
                            values.Add(double.Parse(item.CumulativeEnergykVAhExportConsumption, System.Globalization.NumberStyles.Any));
                        }
                    }
                }
                else
                {
                    return;
                }
                if (currentGraph == Constants.LineGraph)
                {
                    Series = new ISeries[]
                    {
                    new LineSeries<double>
                    {
                        TooltipLabelFormatter =
                            chartPoint => $"{chartPoint.PrimaryValue} {toolTip}",
                        Values = values.ToArray(),
                        LineSmoothness = 0.35,
                        DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        DataLabelsFormatter = point => $"{point.PrimaryValue} {toolTip}",
                        DataLabelsPosition = DataLabelsPosition.Top,
                        DataLabelsSize = 12

                    }
                    };

                }

                else if (currentGraph == Constants.BarGraph)
                {
                    Series = new ISeries[]
                   {
                    new ColumnSeries<double>
                        {
                            TooltipLabelFormatter =
                                chartPoint => $"{chartPoint.PrimaryValue} {toolTip}",
                            Values = values.ToArray(),
                            DataLabelsSize = 10,
                            DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                            DataLabelsFormatter = point => $"{point.PrimaryValue} {toolTip}",
                            DataLabelsPosition = DataLabelsPosition.Top,
                            MaxBarWidth = 20,
                        }
                   };
                }


              

                YAxes = new Axis[]
                {
                new Axis
                {
                    Labeler = (value) => Math.Round((double)value,2).ToString(),
                    Name = currentItem +" (" + MeterNumber + ")",
                    NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                }
                };

                XAxes = new Axis[]
                {
                new Axis
                {
                    Labels = labels.ToArray()
                }
                };

                MainGraphView.Children.Clear();

                CartesianChart ch = new CartesianChart();
                ch.Name = Constants.CartesianChart;
                ch.Series = Series;
                ch.XAxes = XAxes;
                ch.YAxes = YAxes;
                ch.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X;
                ch.Margin = new Thickness(30, 30, 30, 30);
                MainGraphView.Children.Add(ch);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void DownloadGraph(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = graphDataFilter != null ? graphDataFilter.SelectedItem.ToString() : null;
            var exportType = ((System.Windows.Controls.ContentControl)cbExportChart.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;

            string GraphName = Constants.Consumption;
            if (MeterType == Constants.SinglePhaseMeter)
            {
                GraphName = selectedItem + "_" + Constants.ConsumptionSinglePhase;
            }
            else
            {
                GraphName = selectedItem + "_" + Constants.ConsumptionThreePhase;
            }

            if (exportType == Constants.ExportasJPG)
            {
                var control = (Grid)FindName(Constants.MainGraphView);
                var chartControl = (CartesianChart)control.Children[0];
                ExportHelper.GenerateGraphPng(chartControl, GraphName, "jpg", MeterNumber);

            }
            else if (exportType == Constants.ExportasPNG)
            {
                var control = (Grid)FindName(Constants.MainGraphView);
                var chartControl = (CartesianChart)control.Children[0];
                ExportHelper.GenerateGraphPng(chartControl, GraphName, "png", MeterNumber);
            }
        }

        private void FilterDateChange(object sender, SelectionChangedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string fatchDate = FatchDateFilter.SelectedValue != null ? FatchDateFilter.SelectedValue.ToString() : null;

            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, fatchDate, int.MaxValue);
            });

            this.Dispatcher.Invoke(async () =>
            {
                FilterGraph(null, null);
            }).Wait();
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
    }
}
