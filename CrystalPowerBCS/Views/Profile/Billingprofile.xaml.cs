using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs.EventDTOs;
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
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
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
    /// Interaction logic for Billingprofile.xaml
    /// </summary>
    public partial class Billingprofile : UserControl
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
        public List<BillingProfileThreePhaseDto> BillingProfileThreePhaseGraph;

        public List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCT;
        public BillingProfileThreePhaseCTCommand BillingProfileThreePhaseCTCommand;
        public List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCTGraph;

        public string MeterType = Constants.SinglePhaseMeter;
        public string MeterNumber = "00000000";
        public int pageSize = 10;
        public string currentItem;
        public List<string> fatchedDates = new List<string>();
        public ListSortDirection SortDirection = ListSortDirection.Descending;

        public ErrorHelper _errorHelper;
        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }
        public Billingprofile()
        {
            InitializeComponent();

            //If Meter is single Phase Disply Single Phase Grid else three phase grid
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
            BillingProfileThreePhase = new List<BillingProfileThreePhaseDto>();

        }

        private void ShowLoader()
        {
            Spinner.IsLoading = true;
        }

        private void hideLoader()
        {
            Spinner.IsLoading = false;
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
                ExportDataGrid.Visibility = Visibility.Collapsed;
                ExportChartGrid.Visibility = Visibility.Visible;
                FatchDateFilter.Visibility = Visibility.Collapsed;

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
                ExportDataGrid.Visibility = Visibility.Visible;
                ExportChartGrid.Visibility = Visibility.Collapsed;
                FatchDateFilter.Visibility = Visibility.Visible;

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

        private async Task Filter(string startDate, string endDate, string fetchDate, int pageSize)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.CreatedOn).Select(x => x.CreatedOn).ToList();
                    dateList = dateList.OrderByDescending(d => d).ToList();
                    foreach (var item in dateList)
                    {
                        if (!FatchDateFilter.Items.Contains(item.ToString()))
                        {
                            FatchDateFilter.Items.Add(item.ToString());
                        }
                    }
                    //FatchDateFilter.SelectedIndex = 0;
                }

                //var gridDataModel = await BillingProfileSinglePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                BillingProfileSinglePhaseGrid.ItemsSource = gridData;
                BillingProfileSinglePhase = gridData;

                if (FatchDateFilter.SelectedIndex < 0)
                {
                    FatchDateFilter.SelectedIndex = 0;
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();

                var gridData = await BillingProfileThreePhaseCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.CreatedOn).Select(x => x.CreatedOn).ToList();
                    dateList = dateList.OrderByDescending(d => d).ToList();
                    foreach (var item in dateList)
                    {
                        if (!FatchDateFilter.Items.Contains(item.ToString()))
                        {
                            FatchDateFilter.Items.Add(item.ToString());
                        }
                    }
                }

                //var gridDataModel = await BillingProfileThreePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                BillingProfileThreePhaseGrid.ItemsSource = gridData;
                BillingProfileThreePhase = gridData;

                BillingProfileThreePhaseGraph = BillingProfileThreePhase
                    .GroupBy(item => item.RealTimeClock)
                    .Select(group => group.FirstOrDefault())
                    .Select(item => new BillingProfileThreePhaseDto
                    {
                        Number = item.Number,
                        CreatedOn = item.CreatedOn,
                        MeterNo = item.MeterNo,
                        RealTimeClock = item.RealTimeClock,
                        SystemPowerFactorImport = item.SystemPowerFactorImport,
                        CumulativeEnergykWh = item.CumulativeEnergykWh,
                        CumulativeEnergykWhTZ1 = item.CumulativeEnergykWhTZ1,
                        CumulativeEnergykWhTZ2 = item.CumulativeEnergykWhTZ2,
                        CumulativeEnergykWhTZ3 = item.CumulativeEnergykWhTZ3,
                        CumulativeEnergykWhTZ4 = item.CumulativeEnergykWhTZ4,
                        CumulativeEnergykWhTZ5 = item.CumulativeEnergykWhTZ5,
                        CumulativeEnergykWhTZ6 = item.CumulativeEnergykWhTZ6,
                        CumulativeEnergykWhTZ7 = item.CumulativeEnergykWhTZ7,
                        CumulativeEnergykWhTZ8 = item.CumulativeEnergykWhTZ8,
                        CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport,
                        CumulativeEnergykVAhTZ1 = item.CumulativeEnergykVAhTZ1,
                        CumulativeEnergykVAhTZ2 = item.CumulativeEnergykVAhTZ2,
                        CumulativeEnergykVAhTZ3 = item.CumulativeEnergykVAhTZ3,
                        CumulativeEnergykVAhTZ4 = item.CumulativeEnergykVAhTZ4,
                        CumulativeEnergykVAhTZ5 = item.CumulativeEnergykVAhTZ5,
                        CumulativeEnergykVAhTZ6 = item.CumulativeEnergykVAhTZ6,
                        CumulativeEnergykVAhTZ7 = item.CumulativeEnergykVAhTZ7,
                        CumulativeEnergykVAhTZ8 = item.CumulativeEnergykVAhTZ8,
                        MaximumDemandkW = item.MaximumDemandkW,
                        MaximumDemandkWDateTime = item.MaximumDemandkWDateTime,
                        MaximumDemandkWForTZ1 = item.MaximumDemandkWForTZ1,
                        MaximumDemandkWForTZ1DateTime = item.MaximumDemandkWForTZ1DateTime,
                        MaximumDemandkWForTZ2 = item.MaximumDemandkWForTZ2,
                        MaximumDemandkWForTZ2DateTime = item.MaximumDemandkWForTZ2DateTime,
                        MaximumDemandkWForTZ3 = item.MaximumDemandkWForTZ3,
                        MaximumDemandkWForTZ3DateTime = item.MaximumDemandkWForTZ3DateTime,
                        MaximumDemandkWForTZ4 = item.MaximumDemandkWForTZ4,
                        MaximumDemandkWForTZ4DateTime = item.MaximumDemandkWForTZ4DateTime,
                        MaximumDemandkWForTZ5 = item.MaximumDemandkWForTZ5,
                        MaximumDemandkWForTZ5DateTime = item.MaximumDemandkWForTZ5DateTime,
                        MaximumDemandkWForTZ6 = item.MaximumDemandkWForTZ6,
                        MaximumDemandkWForTZ6DateTime = item.MaximumDemandkWForTZ6DateTime,
                        MaximumDemandkWForTZ7 = item.MaximumDemandkWForTZ7,
                        MaximumDemandkWForTZ7DateTime = item.MaximumDemandkWForTZ7DateTime,
                        MaximumDemandkWForTZ8 = item.MaximumDemandkWForTZ8,
                        MaximumDemandkWForTZ8DateTime = item.MaximumDemandkWForTZ8DateTime,
                        MaximumDemandkVA = item.MaximumDemandkVA,
                        MaximumDemandkVADateTime = item.MaximumDemandkVADateTime,
                        MaximumDemandkVAForTZ1 = item.MaximumDemandkVAForTZ1,
                        MaximumDemandkVAForTZ1DateTime = item.MaximumDemandkVAForTZ1DateTime,
                        MaximumDemandkVAForTZ2 = item.MaximumDemandkVAForTZ2,
                        MaximumDemandkVAForTZ2DateTime = item.MaximumDemandkVAForTZ2DateTime,
                        MaximumDemandkVAForTZ3 = item.MaximumDemandkVAForTZ3,
                        MaximumDemandkVAForTZ3DateTime = item.MaximumDemandkVAForTZ3DateTime,
                        MaximumDemandkVAForTZ4 = item.MaximumDemandkVAForTZ4,
                        MaximumDemandkVAForTZ4DateTime = item.MaximumDemandkVAForTZ4DateTime,
                        MaximumDemandkVAForTZ5 = item.MaximumDemandkVAForTZ5,
                        MaximumDemandkVAForTZ5DateTime = item.MaximumDemandkVAForTZ5DateTime,
                        MaximumDemandkVAForTZ6 = item.MaximumDemandkVAForTZ6,
                        MaximumDemandkVAForTZ6DateTime = item.MaximumDemandkVAForTZ6DateTime,
                        MaximumDemandkVAForTZ7 = item.MaximumDemandkVAForTZ7,
                        MaximumDemandkVAForTZ7DateTime = item.MaximumDemandkVAForTZ7DateTime,
                        MaximumDemandkVAForTZ8 = item.MaximumDemandkVAForTZ8,
                        MaximumDemandkVAForTZ8DateTime = item.MaximumDemandkVAForTZ8DateTime,
                        BillingPowerONdurationInMinutesDBP = item.BillingPowerONdurationInMinutesDBP,
                        CumulativeEnergykWhExport = item.CumulativeEnergykWhExport,
                        CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport,
                        CumulativeEnergykVArhQ1 = item.CumulativeEnergykVArhQ1,
                        CumulativeEnergykVArhQ2 = item.CumulativeEnergykVArhQ2,
                        CumulativeEnergykVArhQ3 = item.CumulativeEnergykVArhQ3,
                        CumulativeEnergykVArhQ4 = item.CumulativeEnergykVArhQ4,
                        TamperCount = item.TamperCount,
                        CumulativeEnergykWhImportConsumption = item.CumulativeEnergykWhImportConsumption,
                        CumulativeEnergykWhExportConsumption = item.CumulativeEnergykWhExportConsumption,
                        CumulativeEnergykVAhImportConsumption = item.CumulativeEnergykVAhImportConsumption,
                        CumulativeEnergykVAhExportConsumption = item.CumulativeEnergykVAhExportConsumption
                    })
                    .ToList();

                if(FatchDateFilter.SelectedIndex < 0)
                {
                    FatchDateFilter.SelectedIndex = 0;
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BillingProfileThreePhaseCTCommand = new BillingProfileThreePhaseCTCommand();
                var gridData = await BillingProfileThreePhaseCTCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.CreatedOn).Select(x => x.CreatedOn).ToList();
                    dateList = dateList.OrderByDescending(d => d).ToList();
                    foreach (var item in dateList)
                    {
                        if (!FatchDateFilter.Items.Contains(item.ToString()))
                        {
                            FatchDateFilter.Items.Add(item.ToString());
                        }
                    }
                    //FatchDateFilter.SelectedIndex = 0;
                }

                //var gridDataModel = await BillingProfileThreePhaseCTCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                BillingProfileThreePhaseCTGrid.ItemsSource = gridData;
                BillingProfileThreePhaseCT = gridData;

                BillingProfileThreePhaseCTGraph = BillingProfileThreePhaseCT
                    .GroupBy(item => item.RealTimeClock)
                    .Select(group => group.FirstOrDefault())
                    .Select(item => new BillingProfileThreePhaseCTDto
                    {
                        Number = item.Number,
                        CreatedOn = item.CreatedOn,
                        MeterNo = item.MeterNo,
                        BillingDate = item.BillingDate,
                        AveragePFForBillingPeriod = item.AveragePFForBillingPeriod,
                        RealTimeClock = item.RealTimeClock,
                        CumulativeEnergykWh = item.CumulativeEnergykWh,
                        CumulativeEnergykWhTZ1 = item.CumulativeEnergykWhTZ1,
                        CumulativeEnergykWhTZ2 = item.CumulativeEnergykWhTZ2,
                        CumulativeEnergykWhTZ3 = item.CumulativeEnergykWhTZ3,
                        CumulativeEnergykWhTZ4 = item.CumulativeEnergykWhTZ4,
                        CumulativeEnergykWhTZ5 = item.CumulativeEnergykWhTZ5,
                        CumulativeEnergykWhTZ6 = item.CumulativeEnergykWhTZ6,
                        CumulativeEnergykWhTZ7 = item.CumulativeEnergykWhTZ7,
                        CumulativeEnergykWhTZ8 = item.CumulativeEnergykWhTZ8,
                        CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport,
                        CumulativeEnergykVAhTZ1 = item.CumulativeEnergykVAhTZ1,
                        CumulativeEnergykVAhTZ2 = item.CumulativeEnergykVAhTZ2,
                        CumulativeEnergykVAhTZ3 = item.CumulativeEnergykVAhTZ3,
                        CumulativeEnergykVAhTZ4 = item.CumulativeEnergykVAhTZ4,
                        CumulativeEnergykVAhTZ5 = item.CumulativeEnergykVAhTZ5,
                        CumulativeEnergykVAhTZ6 = item.CumulativeEnergykVAhTZ6,
                        CumulativeEnergykVAhTZ7 = item.CumulativeEnergykVAhTZ7,
                        CumulativeEnergykVAhTZ8 = item.CumulativeEnergykVAhTZ8,
                        MaximumDemandkW = item.MaximumDemandkW,
                        MaximumDemandkWDateTime = item.MaximumDemandkWDateTime,
                        MaximumDemandkWForTZ1 = item.MaximumDemandkWForTZ1,
                        MaximumDemandkWForTZ1DateTime = item.MaximumDemandkWForTZ1DateTime,
                        MaximumDemandkWForTZ2 = item.MaximumDemandkWForTZ2,
                        MaximumDemandkWForTZ2DateTime = item.MaximumDemandkWForTZ2DateTime,
                        MaximumDemandkWForTZ3 = item.MaximumDemandkWForTZ3,
                        MaximumDemandkWForTZ3DateTime = item.MaximumDemandkWForTZ3DateTime,
                        MaximumDemandkWForTZ4 = item.MaximumDemandkWForTZ4,
                        MaximumDemandkWForTZ4DateTime = item.MaximumDemandkWForTZ4DateTime,
                        MaximumDemandkWForTZ5 = item.MaximumDemandkWForTZ5,
                        MaximumDemandkWForTZ5DateTime = item.MaximumDemandkWForTZ5DateTime,
                        MaximumDemandkWForTZ6 = item.MaximumDemandkWForTZ6,
                        MaximumDemandkWForTZ6DateTime = item.MaximumDemandkWForTZ6DateTime,
                        MaximumDemandkWForTZ7 = item.MaximumDemandkWForTZ7,
                        MaximumDemandkWForTZ7DateTime = item.MaximumDemandkWForTZ7DateTime,
                        MaximumDemandkWForTZ8 = item.MaximumDemandkWForTZ8,
                        MaximumDemandkWForTZ8DateTime = item.MaximumDemandkWForTZ8DateTime,
                        MaximumDemandkVA = item.MaximumDemandkVA,
                        MaximumDemandkVADateTime = item.MaximumDemandkVADateTime,
                        MaximumDemandkVAForTZ1 = item.MaximumDemandkVAForTZ1,
                        MaximumDemandkVAForTZ1DateTime = item.MaximumDemandkVAForTZ1DateTime,
                        MaximumDemandkVAForTZ2 = item.MaximumDemandkVAForTZ2,
                        MaximumDemandkVAForTZ2DateTime = item.MaximumDemandkVAForTZ2DateTime,
                        MaximumDemandkVAForTZ3 = item.MaximumDemandkVAForTZ3,
                        MaximumDemandkVAForTZ3DateTime = item.MaximumDemandkVAForTZ3DateTime,
                        MaximumDemandkVAForTZ4 = item.MaximumDemandkVAForTZ4,
                        MaximumDemandkVAForTZ4DateTime = item.MaximumDemandkVAForTZ4DateTime,
                        MaximumDemandkVAForTZ5 = item.MaximumDemandkVAForTZ5,
                        MaximumDemandkVAForTZ5DateTime = item.MaximumDemandkVAForTZ5DateTime,
                        MaximumDemandkVAForTZ6 = item.MaximumDemandkVAForTZ6,
                        MaximumDemandkVAForTZ6DateTime = item.MaximumDemandkVAForTZ6DateTime,
                        MaximumDemandkVAForTZ7 = item.MaximumDemandkVAForTZ7,
                        MaximumDemandkVAForTZ7DateTime = item.MaximumDemandkVAForTZ7DateTime,
                        MaximumDemandkVAForTZ8 = item.MaximumDemandkVAForTZ8,
                        MaximumDemandkVAForTZ8DateTime = item.MaximumDemandkVAForTZ8DateTime,
                        BillingPowerONdurationInMinutesDBP = item.BillingPowerONdurationInMinutesDBP,
                        CumulativeEnergykWhExport = item.CumulativeEnergykWhExport,
                        CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport,
                        CumulativeEnergykVArhQ1 = item.CumulativeEnergykVArhQ1,
                        CumulativeEnergykVArhQ2 = item.CumulativeEnergykVArhQ2,
                        CumulativeEnergykVArhQ3 = item.CumulativeEnergykVArhQ3,
                        CumulativeEnergykVArhQ4 = item.CumulativeEnergykVArhQ4,
                        //TamperCount = item.TamperCount,
                        CumulativeEnergykWhImportConsumption = item.CumulativeEnergykWhImportConsumption,
                        CumulativeEnergykWhExportConsumption = item.CumulativeEnergykWhExportConsumption,
                        CumulativeEnergykVAhImportConsumption = item.CumulativeEnergykVAhImportConsumption,
                        CumulativeEnergykVAhExportConsumption = item.CumulativeEnergykVAhExportConsumption
                    })
                    .ToList();

                if (FatchDateFilter.SelectedIndex < 0)
                {
                    FatchDateFilter.SelectedIndex = 0;
                }
            }
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                #region Bind dates to DropDown list
                //fatchedDates = gridData.DistinctBy(x => x.CreatedOn).Select(d => d.CreatedOn).Take(4).ToList();
                //foreach (var fatchedDate in fatchedDates)
                //{
                //    FatchDateFilter.Items.Add(fatchedDate);
                //}
                #endregion

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
  
        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
                FatchDateFilter.Items.Clear();
                FatchDateFilter.Items.Refresh();
            });
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BillingProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhImport);
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
                graphDataFilter.Items.Add( Constants.MaximumDemandkW);
                graphDataFilter.Items.Add( Constants.MaximumDemandkV);
                graphDataFilter.Items.Add( Constants.AveragePowerFactor);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseGrid.Visibility = Visibility.Visible;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhImport);
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
                graphDataFilter.Items.Add( Constants.MaximumDemandkW);
                graphDataFilter.Items.Add( Constants.MaximumDemandkV);
                graphDataFilter.Items.Add( Constants.SystemPowerFactorImport);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BillingProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BillingProfileThreePhaseCTGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhImport);
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
                graphDataFilter.Items.Add( Constants.MaximumDemandkW);
                graphDataFilter.Items.Add( Constants.MaximumDemandkV);
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
                    string FileName =  Constants.BillingProfileSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(BillingProfileSinglePhase, null, null, FileName, Constants.BillingProfileSinglePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName =  Constants.BillingProfileSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(BillingProfileSinglePhase, null, null, FileName, Constants.BillingProfileSinglePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.BillingProfileThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(null, BillingProfileThreePhase, null, FileName, Constants.BillingProfileThreePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.BillingProfileThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(null, BillingProfileThreePhase, null, FileName, Constants.BillingProfileThreePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                string meterName = MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.BillingProfile + MeterType + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingExcel(null, null, BillingProfileThreePhaseCT, FileName, Constants.BillingProfile + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.BillingProfile + MeterType + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateBillingPdf(null, null, BillingProfileThreePhaseCT, FileName, Constants.BillingProfile + meterName, MeterNumber);
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
                        if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
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
                        else if (currentItem ==  Constants.MaximumDemandkW)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.MaximumDemandkW, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.MaximumDemandkV)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.MaximumDemandkVA, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.AveragePowerFactor)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.AveragePowerFactor, System.Globalization.NumberStyles.Any), 2));
                        }
                    }
                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    if (BillingProfileThreePhaseGraph == null || BillingProfileThreePhaseGraph.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in BillingProfileThreePhaseGraph)
                    {
                        if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWh, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
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
                        else if (currentItem == Constants.CumulativeEnergykVArhQ1)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ2)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ3)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ4)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.MaximumDemandkW)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.MaximumDemandkW, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.MaximumDemandkV)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.MaximumDemandkVA, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.SystemPowerFactorImport)
                        {
                            labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.SystemPowerFactorImport, System.Globalization.NumberStyles.Any), 2));
                        }
                    }
                }
                //rtc==billing date
                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (BillingProfileThreePhaseCTGraph == null || BillingProfileThreePhaseCTGraph.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in BillingProfileThreePhaseCTGraph)
                    {
                        if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykWh, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykVAhImport)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.CumulativeEnergykWhTZ1)
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
                        else if (currentItem == Constants.CumulativeEnergykVArhQ1)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ1, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ2)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ2, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ3)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ3, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ4)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.CumulativeEnergykVArhQ4, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.MaximumDemandkW)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.MaximumDemandkW, System.Globalization.NumberStyles.Any), 2));
                        }
                        else if (currentItem ==  Constants.MaximumDemandkV)
                        {
                            labels.Add(DateTime.ParseExact(item.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                            values.Add(Math.Round(double.Parse(item.MaximumDemandkVA, System.Globalization.NumberStyles.Any), 2));
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

            string GraphName = Constants.BillingProfile;
            if (MeterType == Constants.SinglePhaseMeter)
            {
                GraphName = selectedItem + "_" +  Constants.BillingProfileSinglePhase;
            }
            else
            {
                GraphName = selectedItem + "_" + Constants.BillingProfileThreePhase;
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

        private async void FilterDateChange(object sender, SelectionChangedEventArgs e)
        {
            string selectedDate = FatchDateFilter.SelectedValue?.ToString();

            await Filter(null, null, selectedDate, int.MaxValue);
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
                        var billingProfileThreePhaseList = BillingProfileThreePhaseGrid.Items.OfType<BillingProfileThreePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseGrid.ItemsSource = billingProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var billingProfileThreePhaseCTList = BillingProfileThreePhaseCTGrid.Items.OfType<BillingProfileThreePhaseCTDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseCTGrid.ItemsSource = billingProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var billingProfileSinglePhaseList = BillingProfileSinglePhaseGrid.Items.OfType<BillingProfileSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileSinglePhaseGrid.ItemsSource = billingProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "RealTimeClock")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var billingProfileThreePhaseList = BillingProfileThreePhaseGrid.Items.OfType<BillingProfileThreePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseGrid.ItemsSource = billingProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var billingProfileThreePhaseCTList = BillingProfileThreePhaseCTGrid.Items.OfType<BillingProfileThreePhaseCTDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseCTGrid.ItemsSource = billingProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var billingProfileSinglePhaseList = BillingProfileSinglePhaseGrid.Items.OfType<BillingProfileSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileSinglePhaseGrid.ItemsSource = billingProfileSinglePhaseList;

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
                        var billingProfileThreePhaseList = BillingProfileThreePhaseGrid.Items.OfType<BillingProfileThreePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseGrid.ItemsSource = billingProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var billingProfileThreePhaseCTList = BillingProfileThreePhaseCTGrid.Items.OfType<BillingProfileThreePhaseCTDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseCTGrid.ItemsSource = billingProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var billingProfileSinglePhaseList = BillingProfileSinglePhaseGrid.Items.OfType<BillingProfileSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileSinglePhaseGrid.ItemsSource = billingProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "RealTimeClock")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var billingProfileThreePhaseList = BillingProfileThreePhaseGrid.Items.OfType<BillingProfileThreePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseGrid.ItemsSource = billingProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var billingProfileThreePhaseCTList = BillingProfileThreePhaseCTGrid.Items.OfType<BillingProfileThreePhaseCTDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileThreePhaseCTGrid.ItemsSource = billingProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var billingProfileSinglePhaseList = BillingProfileSinglePhaseGrid.Items.OfType<BillingProfileSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BillingProfileSinglePhaseGrid.ItemsSource = billingProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }

    }
}
