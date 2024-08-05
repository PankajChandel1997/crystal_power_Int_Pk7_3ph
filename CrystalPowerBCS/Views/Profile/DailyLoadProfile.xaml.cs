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
    /// Interaction logic for DailyLoadProfile.xaml
    /// </summary>
    public partial class DailyLoadProfile : UserControl
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
        public List<DailyLoadProfileSinglePhaseDto> DailyLoadProfileSinglePhase;
        public DailyLoadProfileSinglePhaseCommand DailyLoadProfileSinglePhaseCommand;

        public List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhase;
        public List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhaseGraph;
        public DailyLoadProfileThreePhaseCommand DailyLoadProfileThreePhaseCommand;

        public List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCT;
        public List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCTGraph;
        public DailyLoadProfileThreePhaseCTCommand DailyLoadProfileThreePhaseCTCommand;

        public string MeterType;
        public string MeterNumber = "00000000";
        public int pageSize = 10;
        public ErrorHelper _errorHelper;
        public string currentItem;
        public List<string> fatchedDates = new List<string>();
        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public ListSortDirection SortDirection = ListSortDirection.Descending;
        public DailyLoadProfile()
        {
            InitializeComponent();

            //If Meter is single Phase Disply Single Phase Grid else three phase grid
            DailyLoadProfileSinglePhaseGrid.Visibility = Visibility.Visible;

            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;

            downarrow.Visibility = Visibility.Visible;
            uparrow.Visibility = Visibility.Collapsed;

            Filters.Visibility = Visibility.Collapsed;
            _errorHelper = new ErrorHelper();

            DailyLoadProfileSinglePhase = new List<DailyLoadProfileSinglePhaseDto>();
            DailyLoadProfileThreePhase = new List<DailyLoadProfileThreePhaseDto>();

        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                Grid_Parent1.Height = height - 217;
                DailyLoadProfileSinglePhaseGrid.MaxHeight = height - 217;
                DailyLoadProfileThreePhaseGrid.MaxHeight = height - 217;
                DailyLoadProfileThreePhaseCTGrid.MaxHeight = height - 217;
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

            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate, endDate, null, pageSize); ;
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
            string[] meterData = MeterPhaseType.Text.Split("&&");
            if(meterData.Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                    await Filter(startDate, endDate, null, pageSize); ;
                });
            }
            
        }

        private async Task Filter(string startDate, string endDate, string fatchDate, int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {

                DailyLoadProfileSinglePhaseCommand = new DailyLoadProfileSinglePhaseCommand();

                var gridData = await DailyLoadProfileSinglePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

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

                //var gridDataModel = await DailyLoadProfileSinglePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                DailyLoadProfileSinglePhaseGrid.ItemsSource = gridData;
                DailyLoadProfileSinglePhase = gridData;

                if (FatchDateFilter.SelectedIndex < 0)
                {
                    FatchDateFilter.SelectedIndex = 0;
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                DailyLoadProfileThreePhaseCommand = new DailyLoadProfileThreePhaseCommand();

                var gridData = await DailyLoadProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

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

                //var gridDataModel = await DailyLoadProfileThreePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                DailyLoadProfileThreePhaseGrid.ItemsSource = gridData;
                DailyLoadProfileThreePhase = gridData;

                DailyLoadProfileThreePhaseGraph = DailyLoadProfileThreePhase
                    .GroupBy(item => item.RealTimeClock)
                    .Select(group => group.FirstOrDefault())
                    .Select(item => new DailyLoadProfileThreePhaseDto
                    {
                        Number = item.Number,
                        CreatedOn = item.CreatedOn,
                        MeterNo = item.MeterNo,
                        RealTimeClock = item.RealTimeClock,
                        CumulativeEnergykWhImport = item.CumulativeEnergykWhImport,
                        CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport,
                        CumulativeEnergykWhExport = item.CumulativeEnergykWhExport,
                        CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport
                    })
                    .ToList();

                if (FatchDateFilter.SelectedIndex < 0)
                {
                    FatchDateFilter.SelectedIndex = 0;
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                DailyLoadProfileThreePhaseCTCommand = new DailyLoadProfileThreePhaseCTCommand();

                var gridData = await DailyLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

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
                    FatchDateFilter.SelectedIndex = 0;
                }

                var gridDataModel = await DailyLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                DailyLoadProfileThreePhaseCTGrid.ItemsSource = gridData;
                DailyLoadProfileThreePhaseCT = gridData;

                DailyLoadProfileThreePhaseCTGraph = DailyLoadProfileThreePhaseCT
                    .GroupBy(item => item.RealTimeClock)
                    .Select(group => group.FirstOrDefault())
                    .Select(item => new DailyLoadProfileThreePhaseCTDto
                    {
                        Number = item.Number,
                        CreatedOn = item.CreatedOn,
                        MeterNo = item.MeterNo,
                        RealTimeClock = item.RealTimeClock,
                        CumulativeEnergykWhImport = item.CumulativeEnergykWhImport,
                        CumulativeEnergykVAhImport = item.CumulativeEnergykVAhImport,
                        CumulativeEnergykWhExport = item.CumulativeEnergykWhExport,
                        CumulativeEnergykVAhExport = item.CumulativeEnergykVAhExport,
                        CumulativeEnergykVArhQ1 = item.CumulativeEnergykVArhQ1,
                        CumulativeEnergykVArhQ2 = item.CumulativeEnergykVArhQ2,
                        CumulativeEnergykVArhQ3 = item.CumulativeEnergykVArhQ3,
                        CumulativeEnergykVArhQ4 = item.CumulativeEnergykVArhQ4,
                    })
                    .ToList();
            }

            return;
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                DailyLoadProfileSinglePhaseCommand = new DailyLoadProfileSinglePhaseCommand();

                var gridData = await DailyLoadProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                DailyLoadProfileSinglePhaseGrid.ItemsSource = gridData;
                DailyLoadProfileSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                DailyLoadProfileThreePhaseCommand = new DailyLoadProfileThreePhaseCommand();

                var gridData = await DailyLoadProfileThreePhaseCommand.GetAll(pageSize, MeterNumber);

                DailyLoadProfileThreePhaseGrid.ItemsSource = gridData;
                DailyLoadProfileThreePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                DailyLoadProfileThreePhaseCTCommand = new DailyLoadProfileThreePhaseCTCommand();

                var gridData = await DailyLoadProfileThreePhaseCTCommand.GetAll(pageSize, MeterNumber);

                DailyLoadProfileThreePhaseCTGrid.ItemsSource = gridData;
                DailyLoadProfileThreePhaseCT = gridData;

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
                DailyLoadProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileSinglePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImport);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if(MeterType == Constants.ThreePhaseMeter)
            {
                DailyLoadProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImport);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                DailyLoadProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseCTGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImport);

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
                    string FileName = Constants.DailyLoadProfileSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(DailyLoadProfileSinglePhase);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.DailyLoadProfileSinglePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.DailyLoadProfileSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(DailyLoadProfileSinglePhase);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.DailyLoadProfileSinglePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.DailyLoadProfileThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(DailyLoadProfileThreePhase);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.DailyLoadProfileThreePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.DailyLoadProfileThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(DailyLoadProfileThreePhase);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.DailyLoadProfileThreePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                string meterName = MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.DailyLoadProfileThreePhaseCT + MeterType + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(DailyLoadProfileThreePhaseCT);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.DailyLoadProfile + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.DailyLoadProfileThreePhaseCT + MeterType + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(DailyLoadProfileThreePhaseCT);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.DailyLoadProfile + meterName, MeterNumber);
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
                    if (DailyLoadProfileSinglePhase == null || DailyLoadProfileSinglePhase.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in DailyLoadProfileSinglePhase)
                    {
                        if (DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date != DateTime.MinValue)
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
                                values.Add(Math.Round(double.Parse(item.CumulativeEnergyKVAhImport, System.Globalization.NumberStyles.Any), 2));
                            }
                        }
                    }
                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    if (DailyLoadProfileThreePhaseGraph == null || DailyLoadProfileThreePhaseGraph.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in DailyLoadProfileThreePhaseGraph)
                    {
                        if (DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date != DateTime.MinValue)
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
                            else if (currentItem ==Constants.CumulativeEnergykVAhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                                values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem ==  Constants.CumulativeEnergykVAhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
                                values.Add(Math.Round(double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any), 2));
                            }
                        }
                    }
                }

                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (DailyLoadProfileThreePhaseCTGraph == null || DailyLoadProfileThreePhaseCTGraph.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in DailyLoadProfileThreePhaseCTGraph)
                    {
                        if (DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date != DateTime.MinValue)
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
                            Fill = null,
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
            string GraphName = Constants.DailyLoadProfile;
            if (MeterType == Constants.SinglePhaseMeter)
            {
                GraphName = selectedItem + "_" + Constants.DailyLoadProfileSinglePhase;
            }
            else
            {
                GraphName = selectedItem + "_" + Constants.DailyLoadProfileThreePhase + "_"+ selectedItem;
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
                        var dailyLoadProfileThreePhaseList = DailyLoadProfileThreePhaseGrid.Items.OfType<DailyLoadProfileThreePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseGrid.ItemsSource = dailyLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var dailyLoadProfileThreePhaseCTList = DailyLoadProfileThreePhaseCTGrid.Items.OfType<DailyLoadProfileThreePhaseCTDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseCTGrid.ItemsSource = dailyLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var dailyLoadProfileSinglePhaseList = DailyLoadProfileSinglePhaseGrid.Items.OfType<DailyLoadProfileSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileSinglePhaseGrid.ItemsSource = dailyLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "RealTimeClock")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var dailyLoadProfileThreePhaseList = DailyLoadProfileThreePhaseGrid.Items.OfType<DailyLoadProfileThreePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseGrid.ItemsSource = dailyLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var dailyLoadProfileThreePhaseCTList = DailyLoadProfileThreePhaseCTGrid.Items.OfType<DailyLoadProfileThreePhaseCTDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseCTGrid.ItemsSource = dailyLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var dailyLoadProfileSinglePhaseList = DailyLoadProfileSinglePhaseGrid.Items.OfType<DailyLoadProfileSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileSinglePhaseGrid.ItemsSource = dailyLoadProfileSinglePhaseList;

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
                        var dailyLoadProfileThreePhaseList = DailyLoadProfileThreePhaseGrid.Items.OfType<DailyLoadProfileThreePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseGrid.ItemsSource = dailyLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var dailyLoadProfileThreePhaseCTList = DailyLoadProfileThreePhaseCTGrid.Items.OfType<DailyLoadProfileThreePhaseCTDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseCTGrid.ItemsSource = dailyLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var dailyLoadProfileSinglePhaseList = DailyLoadProfileSinglePhaseGrid.Items.OfType<DailyLoadProfileSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileSinglePhaseGrid.ItemsSource = dailyLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == "RealTimeClockDateAndTime" || column == "RealTimeClock")
                {
                    if (MeterType.Contains("ThreePhase"))
                    {
                        var dailyLoadProfileThreePhaseList = DailyLoadProfileThreePhaseGrid.Items.OfType<DailyLoadProfileThreePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseGrid.ItemsSource = dailyLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == "ThreePhaseLTCT" || MeterType == "ThreePhaseHTCT")
                    {
                        var dailyLoadProfileThreePhaseCTList = DailyLoadProfileThreePhaseCTGrid.Items.OfType<DailyLoadProfileThreePhaseCTDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileThreePhaseCTGrid.ItemsSource = dailyLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var dailyLoadProfileSinglePhaseList = DailyLoadProfileSinglePhaseGrid.Items.OfType<DailyLoadProfileSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        DailyLoadProfileSinglePhaseGrid.ItemsSource = dailyLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }

    }
}