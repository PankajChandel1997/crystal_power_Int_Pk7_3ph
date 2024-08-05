using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for TDO.xaml
    /// </summary>
    public partial class TOD : UserControl
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
        public string MeterType = Constants.SinglePhaseMeter;
        public string MeterNumber = "00000000";
        public int pageSize = 10;
        public string currentItem;
        public List<string> fatchedDates = new List<string>();

        public ErrorHelper _errorHelper;
        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }
        public TOD()
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

            pageSize = gridPageSize != null && gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize != null && gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate, endDate, null, pageSize);
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
                    await Filter(startDate, endDate, null, pageSize);
                });
            }
        }

        private async Task Filter(string startDate, string endDate, string fatchDate, int pageSize)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

                BillingProfileSinglePhaseGrid.ItemsSource = gridData;
                BillingProfileSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();

                var gridData = await BillingProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

                BillingProfileThreePhaseGrid.ItemsSource = gridData;
                BillingProfileThreePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BillingProfileThreePhaseCTCommand = new BillingProfileThreePhaseCTCommand();
                var gridData = await BillingProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

                BillingProfileThreePhaseCTGrid.ItemsSource = gridData;
                BillingProfileThreePhaseCT = gridData;
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {

                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                BillingProfileSinglePhaseGrid.ItemsSource = gridData;
                BillingProfileSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {

                BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();

                var gridData = await BillingProfileThreePhaseCommand.GetAll(pageSize, MeterNumber);

                BillingProfileThreePhaseGrid.ItemsSource = gridData;
                BillingProfileThreePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {

                BillingProfileThreePhaseCTCommand = new BillingProfileThreePhaseCTCommand();
                var gridData = await BillingProfileThreePhaseCTCommand.GetAll(pageSize, MeterNumber);

                BillingProfileThreePhaseCTGrid.ItemsSource = gridData;
                BillingProfileThreePhaseCT = gridData;
            }
        }

        private async Task BindBillingProfileSinglePhaseDto(List<BillingProfileSinglePhaseDto> gridData)
        {
            try
            {
                if (gridData.Count == 0)
                {
                    return;
                }
                BillingProfileSinglePhase = new List<BillingProfileSinglePhaseDto>();
                int index = 1;
                List<BillingProfileSinglePhaseDto> filteredDateList = new List<BillingProfileSinglePhaseDto>();

                string dateFormat = "dd-MM-yyyy HH:mm:ss";

                var list = gridData.OrderBy(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).ToList();
               
                filteredDateList = list;
                var count = 0;
                foreach (var item in filteredDateList)
                {
                    BillingProfileSinglePhaseDto billingDTO = new BillingProfileSinglePhaseDto();
                    if (count > 0)
                    {
                        billingDTO.CreatedOn = filteredDateList[count - 1].CreatedOn;
                        billingDTO.RealTimeClock = filteredDateList[count].RealTimeClock;
                        billingDTO.CumulativeEnergykWhTZ1 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykWhTZ1) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykWhTZ1), 2).ToString();
                        billingDTO.CumulativeEnergykWhTZ2 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykWhTZ2) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykWhTZ2), 2).ToString(); ;
                        billingDTO.CumulativeEnergykWhTZ3 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykWhTZ3) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykWhTZ3), 2).ToString(); ;
                        billingDTO.CumulativeEnergykWhTZ4 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykWhTZ4) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykWhTZ4), 2).ToString(); ;
                        billingDTO.CumulativeEnergykWhTZ5 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykWhTZ5) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykWhTZ5), 2).ToString();
                        billingDTO.CumulativeEnergykWhTZ6 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykWhTZ6) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykWhTZ6), 2).ToString(); ;

                        billingDTO.CumulativeEnergykVAhTZ1 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykVAhTZ1) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykVAhTZ1), 2).ToString();
                        billingDTO.CumulativeEnergykVAhTZ2 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykVAhTZ2) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykVAhTZ2), 2).ToString();
                        billingDTO.CumulativeEnergykVAhTZ3 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykVAhTZ3) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykVAhTZ3), 2).ToString();
                        billingDTO.CumulativeEnergykVAhTZ4 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykVAhTZ4) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykVAhTZ4), 2).ToString();
                        billingDTO.CumulativeEnergykVAhTZ5 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykVAhTZ5) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykVAhTZ5), 2).ToString();
                        billingDTO.CumulativeEnergykVAhTZ6 = Math.Round(Double.Parse(filteredDateList[count].CumulativeEnergykVAhTZ6) - Double.Parse(filteredDateList[count - 1].CumulativeEnergykVAhTZ6), 2).ToString();
                        BillingProfileSinglePhase.Add(billingDTO);
                    }
                    else
                    {
                        billingDTO = filteredDateList[count];
                        BillingProfileSinglePhase.Add(billingDTO);
                    }
                    count++;
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
                        newData.Number = filteredDateList.Count + 1;

                        filteredDateList.Add(newData);
                    }
                }
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;
                filteredDateList = filteredDateList.Where(date =>
                {
                    DateTime parsedDate = DateTime.ParseExact(date.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    return !(parsedDate.Month == currentMonth && parsedDate.Year == currentYear);
                }).ToList();

                if (filteredDateList != null && filteredDateList.Count > 1)
                {
                    var startYear = DateTime.Parse(filteredDateList[0].RealTimeClock).Year;
                    var startMonth = DateTime.Parse(filteredDateList[0].RealTimeClock).Month;
                    var endYear = DateTime.Parse(filteredDateList[filteredDateList.Count - 1].RealTimeClock).Year;
                    var endMonth = DateTime.Parse(filteredDateList[filteredDateList.Count - 1].RealTimeClock).Month;

                    for (int i = 0; i < filteredDateList.Count; i++)
                    {
                        BillingProfileThreePhaseDto billingDTO = new BillingProfileThreePhaseDto();
                        if (i > 0)
                        {
                            billingDTO.CreatedOn = filteredDateList[i].CreatedOn;
                            billingDTO.RealTimeClock = filteredDateList[i].RealTimeClock;
                            billingDTO.CumulativeEnergykWhTZ1 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ1) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ1), 2).ToString();
                            billingDTO.CumulativeEnergykWhTZ2 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ2) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ2), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ3 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ3) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ3), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ4 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ4) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ4), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ5 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ5) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ5), 2).ToString();
                            billingDTO.CumulativeEnergykWhTZ6 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ6) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ6), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ7 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ7) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ7), 2).ToString();
                            billingDTO.CumulativeEnergykWhTZ8 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ8) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ8), 2).ToString();

                            billingDTO.CumulativeEnergykVAhTZ1 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ1) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ1), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ2 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ2) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ2), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ3 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ3) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ3), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ4 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ4) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ4), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ5 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ5) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ5), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ6 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ6) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ6), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ7 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ7) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ7), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ8 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ8) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ8), 2).ToString();

                            billingDTO.MaximumDemandkWForTZ1 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ1) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ1), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ2 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ2) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ2), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ3 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ3) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ3), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ4 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ4) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ4), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ5 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ5) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ5), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ6 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ6) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ6), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ7 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ7) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ7), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ8 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ8) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ8), 2).ToString();


                            billingDTO.MaximumDemandkVAForTZ1 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ1) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ1), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ2 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ2) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ2), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ3 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ3) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ3), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ4 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ4) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ4), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ5 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ5) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ5), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ6 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ6) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ6), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ7 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ7) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ7), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ8 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ8) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ8), 2).ToString();

                            BillingProfileThreePhase.Add(billingDTO);
                        }
                        else
                        {
                            billingDTO = filteredDateList[i];
                            BillingProfileThreePhase.Add(billingDTO);
                        }
                    }
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

                if (filteredDateList != null && filteredDateList.Count > 1)
                {
                    var startYear = DateTime.Parse(filteredDateList[0].BillingDate).Year;
                    var startMonth = DateTime.Parse(filteredDateList[0].BillingDate).Month;
                    var endYear = DateTime.Parse(filteredDateList[filteredDateList.Count - 1].BillingDate).Year;
                    var endMonth = DateTime.Parse(filteredDateList[filteredDateList.Count - 1].BillingDate).Month;

                    for (int i = 0; i < filteredDateList.Count; i++)
                    {
                        BillingProfileThreePhaseCTDto billingDTO = new BillingProfileThreePhaseCTDto();
                        if (i > 0)
                        {
                            billingDTO.CreatedOn = filteredDateList[i - 1].CreatedOn;
                            billingDTO.BillingDate = filteredDateList[i - 1].BillingDate;
                            billingDTO.CumulativeEnergykWhTZ1 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ1) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ1), 2).ToString();
                            billingDTO.CumulativeEnergykWhTZ2 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ2) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ2), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ3 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ3) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ3), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ4 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ4) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ4), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ5 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ5) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ5), 2).ToString();
                            billingDTO.CumulativeEnergykWhTZ6 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ6) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ6), 2).ToString(); ;
                            billingDTO.CumulativeEnergykWhTZ7 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ7) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ7), 2).ToString();
                            billingDTO.CumulativeEnergykWhTZ8 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykWhTZ8) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykWhTZ8), 2).ToString();

                            billingDTO.CumulativeEnergykVAhTZ1 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ1) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ1), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ2 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ2) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ2), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ3 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ3) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ3), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ4 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ4) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ4), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ5 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ5) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ5), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ6 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ6) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ6), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ7 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ7) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ7), 2).ToString();
                            billingDTO.CumulativeEnergykVAhTZ8 = Math.Round(Double.Parse(filteredDateList[i].CumulativeEnergykVAhTZ8) - Double.Parse(filteredDateList[i - 1].CumulativeEnergykVAhTZ8), 2).ToString();

                            billingDTO.MaximumDemandkWForTZ1 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ1) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ1), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ2 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ2) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ2), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ3 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ3) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ3), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ4 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ4) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ4), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ5 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ5) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ5), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ6 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ6) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ6), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ7 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ7) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ7), 2).ToString();
                            billingDTO.MaximumDemandkWForTZ8 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkWForTZ8) - Double.Parse(filteredDateList[i - 1].MaximumDemandkWForTZ8), 2).ToString();


                            billingDTO.MaximumDemandkVAForTZ1 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ1) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ1), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ2 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ2) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ2), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ3 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ3) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ3), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ4 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ4) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ4), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ5 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ5) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ5), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ6 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ6) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ6), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ7 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ7) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ7), 2).ToString();
                            billingDTO.MaximumDemandkVAForTZ8 = Math.Round(Double.Parse(filteredDateList[i].MaximumDemandkVAForTZ8) - Double.Parse(filteredDateList[i - 1].MaximumDemandkVAForTZ8), 2).ToString();

                            BillingProfileThreePhaseCT.Add(billingDTO);
                        }
                        else
                        {
                            billingDTO = filteredDateList[i];
                            BillingProfileThreePhaseCT.Add(billingDTO);
                        }
                    }
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

                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ1);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ2);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ3);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ4);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ5);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ6);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ1);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ2);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ3);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ4);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ5);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ6);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseGrid.Visibility = Visibility.Visible;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ1);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ2);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ3);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ4);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ5);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ6);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ7);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ8);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ1);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ2);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ3);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ4);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ5);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ6);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ7);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ8);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ1);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ2);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ3);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ4);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ5);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ6);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ7);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhTZ8);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ1);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ2);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ3);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ4);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ5);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ6);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ7);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhTZ8);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
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
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ConsumptionSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(BillingProfileSinglePhase, null, null, FileName, Constants.ConsumptionSinglePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ConsumptionSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(BillingProfileSinglePhase, null, null, FileName, Constants.ConsumptionSinglePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ConsumptionThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(null, BillingProfileThreePhase, null, FileName, Constants.ConsumptionThreePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ConsumptionThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(null, BillingProfileThreePhase, null, FileName, Constants.ConsumptionThreePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                string meterName = MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.ConsumptionThreePhaseCT + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(null, null, BillingProfileThreePhaseCT, FileName, Constants.ConsumptionThreePhaseCT + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.ConsumptionThreePhaseCT + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(null, null, BillingProfileThreePhaseCT, FileName, Constants.ConsumptionThreePhaseCT + meterName, MeterNumber);
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
                        if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ5, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ6, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ5, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ6, System.Globalization.NumberStyles.Any), 2));
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
                        if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ5, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ6, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ7, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ8, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ5, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ6, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ7, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ8, System.Globalization.NumberStyles.Any), 2));
                        }
                    }
                }
                //rtc==billing date
                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (BillingProfileThreePhaseCT == null || BillingProfileThreePhaseCT.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in BillingProfileThreePhaseCT)
                    {
                        if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ5, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ6, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ7, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhTZ8, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ1)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ2)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ3)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ4)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ5)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ5, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ6)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ6, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ7)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ7, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhTZ8)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhTZ8, System.Globalization.NumberStyles.Any), 2));
                        }
                    }
                }
                else
                {
                    return;
                }

                //Graph Binnding
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
                        DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        DataLabelsFormatter = point => $"{point.PrimaryValue} {toolTip}",
                        DataLabelsPosition = DataLabelsPosition.Top,
                        DataLabelsSize = 12

                    }
                   };
                }
                YAxes = new Axis[]
                {
                new Axis
                {
                    Name = currentItem+" (" + MeterNumber + ")",
                    NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                    Labeler = (value) => Math.Round((double)value,2).ToString(),
                }
                };

                XAxes = new Axis[]
                {
                new Axis
                {
                    Labels = labels.ToArray(),
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
