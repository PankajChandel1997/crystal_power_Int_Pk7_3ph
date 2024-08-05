using CrystalPowerBCS.DbFunctions;
using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions;
using CrystalPowerBCS.Helpers;
using DocumentFormat.OpenXml.Wordprocessing;
using Infrastructure.DTOs;
using Infrastructure.DTOs.EventDTOs;
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
using System.util;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for BlockLoadProfile.xaml
    /// </summary>
    public partial class BlockLoadProfile : UserControl
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
        public List<BlockLoadProfileSinglePhaseDto> BlockLoadProfileSinglePhase;
        public List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhase;
        public List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhaseGraph;
        public BlockLoadProfileSinglePhaseCommand BlockLoadProfileSinglePhaseCommand;
        public List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCT;
        public List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCTGraph;
        public BlockLoadProfileThreePhaseCTCommand BlockLoadProfileThreePhaseCTCommand;
        public BlockLoadProfileThreePhaseCommand BlockLoadProfileThreePhaseCommand;

        public string MeterType;
        public string MeterNumber;
        public int pageSize = 10;
        public ErrorHelper _errorHelper;
        public string currentItem;
        public List<string> fatchedDates = new List<string>();

        public ISeries[] Series { get; set; }

        public Axis[] YAxes { get; set; }
        public Axis[] XAxes { get; set; }

        public ListSortDirection SortDirection = ListSortDirection.Descending;

        public BlockLoadProfile()
        {
            InitializeComponent();
            //If Meter is single Phase Disply Single Phase Grid else three phase grid
            BlockLoadProfileSinglePhaseGrid.Visibility = Visibility.Visible;
            ClearFilter.Visibility = Visibility.Collapsed;
            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;

            downarrow.Visibility = Visibility.Visible;
            uparrow.Visibility = Visibility.Collapsed;

            Filters.Visibility = Visibility.Collapsed;

            BlockLoadProfileSinglePhase = new List<BlockLoadProfileSinglePhaseDto>();

            BlockLoadProfileThreePhase = new List<BlockLoadProfileThreePhaseDto>();
            _errorHelper = new ErrorHelper();

            var actualH = System.Windows.SystemParameters.PrimaryScreenHeight;

            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });
        }

        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                Grid_Parent1.Height = height - 217;
                BlockLoadProfileSinglePhaseGrid.MaxHeight = height - 217;
                BlockLoadProfileThreePhaseGrid.MaxHeight = height - 217;
                BlockLoadProfileThreePhaseCTGrid.MaxHeight = height - 217;
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
                FatchDateFilter.Visibility = Visibility.Collapsed;
                graphFilter.Visibility = Visibility.Visible;
                GraphDatePicker.Visibility = Visibility.Visible;
                ExportDataGrid.Visibility = Visibility.Collapsed;
                ExportChartGrid.Visibility = Visibility.Visible;
                ClearFilter.Visibility = Visibility.Visible;

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
                FatchDateFilter.Visibility = Visibility.Visible;
                graphFilter.Visibility = Visibility.Collapsed;
                GraphDatePicker.Visibility = Visibility.Collapsed;
                ClearFilter.Visibility = Visibility.Collapsed;
                ExportDataGrid.Visibility = Visibility.Visible;
                ExportChartGrid.Visibility = Visibility.Collapsed;
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
            //  string fatchDate = FatchDateFilter.SelectedValue != null ? FatchDateFilter.SelectedValue?.ToString() : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != Constants.All ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            if (MeterPhaseType.Text.Split("&&").Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                    await Filter(startDate, endDate, null, pageSize);
                });
            }

        }

        private async Task Filter(string startDate, string endDate, string fetchDate, int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BlockLoadProfileSinglePhaseCommand = new BlockLoadProfileSinglePhaseCommand();
              
                var gridData = await BlockLoadProfileSinglePhaseCommand.Filter(null, null, null, pageSize, MeterNumber);

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

                var gridDataModel = await BlockLoadProfileSinglePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                BlockLoadProfileSinglePhaseGrid.ItemsSource = gridDataModel;
                BlockLoadProfileSinglePhase = gridDataModel;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BlockLoadProfileThreePhaseCommand = new BlockLoadProfileThreePhaseCommand();

                var gridData = await BlockLoadProfileThreePhaseCommand.Filter(null, null, null, pageSize, MeterNumber);

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


                var gridDataModel = await BlockLoadProfileThreePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                BlockLoadProfileThreePhaseGrid.ItemsSource = gridDataModel;
                BlockLoadProfileThreePhase = gridDataModel;
                BlockLoadProfileThreePhaseGraph = BlockLoadProfileThreePhase
                    .GroupBy(item => item.RealTimeClock)
                    .Select(group => group.FirstOrDefault())
                    .Select(item => new BlockLoadProfileThreePhaseDto
                    {
                        Number = item.Number,
                        CreatedOn = item.CreatedOn,
                        MeterNo = item.MeterNo,
                        RealTimeClock = item.RealTimeClock,
                        CurrentR = item.CurrentR,
                        CurrentY = item.CurrentY,
                        CurrentB = item.CurrentB,
                        VoltageR = item.VoltageR,
                        VoltageY = item.VoltageY,
                        VoltageB = item.VoltageB,
                        PowerFactorRPhase = item.PowerFactorRPhase,
                        PowerFactorYPhase = item.PowerFactorYPhase,
                        PowerFactorBPhase = item.PowerFactorBPhase,
                        BlockEnergykWhImport = item.BlockEnergykWhImport,
                        BlockEnergykVAhImport = item.BlockEnergykVAhImport,
                        BlockEnergykWhExport = item.BlockEnergykWhExport,
                        BlockEnergykVAhExport = item.BlockEnergykVAhExport
                    })
                    .ToList();
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BlockLoadProfileThreePhaseCTCommand = new BlockLoadProfileThreePhaseCTCommand();
                
                var gridData = await BlockLoadProfileThreePhaseCTCommand.Filter(null, null, null, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.CreatedOn).Select(x => x.CreatedOn).ToList();
                    dateList = dateList.OrderByDescending(d => d).ToList();
                    foreach (var item in dateList)
                    {
                        if(!FatchDateFilter.Items.Contains(item.ToString()))
                        {
                            FatchDateFilter.Items.Add(item.ToString());
                        }                       
                    }
                    FatchDateFilter.SelectedIndex = 0;
                }
                


                var gridDataModel = await BlockLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                BlockLoadProfileThreePhaseCTGrid.ItemsSource = gridDataModel;
                BlockLoadProfileThreePhaseCT = gridData;

                BlockLoadProfileThreePhaseCTGraph = BlockLoadProfileThreePhaseCT
                    .GroupBy(item => item.RealTimeClock)
                    .Select(group => group.FirstOrDefault())
                    .Select(item => new BlockLoadProfileThreePhaseCTDto
                    {
                        Number = item.Number,
                        CreatedOn = item.CreatedOn,
                        MeterNo = item.MeterNo,
                        RealTimeClock = item.RealTimeClock,
                        CurrentR = item.CurrentR,
                        CurrentY = item.CurrentY,
                        CurrentB = item.CurrentB,
                        VoltageR = item.VoltageR,
                        VoltageY = item.VoltageY,
                        VoltageB = item.VoltageB,

                        BlockEnergykWhImport = item.BlockEnergykWhImport,
                        BlockEnergykVAhImport = item.BlockEnergykVAhImport,
                        BlockEnergykWhExport = item.BlockEnergykWhExport,
                        BlockEnergykVAhExport = item.BlockEnergykVAhExport,
                        BlockEnergykVArhQ1 = item.BlockEnergykVArhQ1,
                        BlockEnergykVArhQ2 = item.BlockEnergykVArhQ2,
                        BlockEnergykVArhQ3 = item.BlockEnergykVArhQ3,
                        BlockEnergykVArhQ4 = item.BlockEnergykVArhQ4,
                        MeterHealthIndicator = item.MeterHealthIndicator
                    })
                    .ToList();

            }         
            return;
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                BlockLoadProfileSinglePhaseCommand = new BlockLoadProfileSinglePhaseCommand();

                var gridData = await BlockLoadProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                BlockLoadProfileSinglePhaseGrid.ItemsSource = gridData;
                BlockLoadProfileSinglePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BlockLoadProfileThreePhaseCommand = new BlockLoadProfileThreePhaseCommand();

                var gridData = await BlockLoadProfileThreePhaseCommand.GetAll(pageSize, MeterNumber);
                BlockLoadProfileThreePhaseGrid.ItemsSource = gridData;
                BlockLoadProfileThreePhase = gridData;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BlockLoadProfileThreePhaseCTCommand = new BlockLoadProfileThreePhaseCTCommand();

                var gridData = await BlockLoadProfileThreePhaseCTCommand.GetAll(pageSize, MeterNumber);
                BlockLoadProfileThreePhaseCTGrid.ItemsSource = gridData;
                BlockLoadProfileThreePhaseCT = gridData;
            }
            return;
        }

        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
                FatchDateFilter.Items.Clear();
                FatchDateFilter.Items.Refresh();
            });
            FatchDateFilter.Items.Clear();

            if (MeterType == Constants.SinglePhaseMeter)
            {
                BlockLoadProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BlockLoadProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                BlockLoadProfileSinglePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                //graphDataFilter.Items.Add("Neutral Current");
                graphDataFilter.Items.Add(Constants.PhaseCurrent);
                graphDataFilter.Items.Add(Constants.AverageVoltage);
                graphDataFilter.Items.Add(Constants.BlockEnergykWhExport);
                graphDataFilter.Items.Add(Constants.BlockEnergykWhImport);
                graphDataFilter.Items.Add(Constants.BlockEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.BlockEnergykVAhImport);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                BlockLoadProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BlockLoadProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                BlockLoadProfileThreePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CurrentR);
                graphDataFilter.Items.Add(Constants.CurrentY);
                graphDataFilter.Items.Add(Constants.CurrentB);
                graphDataFilter.Items.Add(Constants.VoltageR);
                graphDataFilter.Items.Add(Constants.VoltageY);
                graphDataFilter.Items.Add(Constants.VoltageB);
                graphDataFilter.Items.Add(Constants.BlockEnergykWhExport);
                graphDataFilter.Items.Add(Constants.BlockEnergykWhImport);
                graphDataFilter.Items.Add(Constants.BlockEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.BlockEnergykVAhImport);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                BlockLoadProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                BlockLoadProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                BlockLoadProfileThreePhaseCTGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CurrentR);
                graphDataFilter.Items.Add(Constants.CurrentY);
                graphDataFilter.Items.Add(Constants.CurrentB);
                graphDataFilter.Items.Add(Constants.VoltageR);
                graphDataFilter.Items.Add(Constants.VoltageY);
                graphDataFilter.Items.Add(Constants.VoltageB);
                graphDataFilter.Items.Add(Constants.BlockEnergykWhExport);
                graphDataFilter.Items.Add(Constants.BlockEnergykWhImport);
                graphDataFilter.Items.Add(Constants.BlockEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.BlockEnergykVAhImport);

                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }


            if (MeterPhaseType.Text.Split("&&").Length > 1)
            {
                this.Dispatcher.Invoke(async () =>
                {
                    FilterByPageSize(null, null);
                });

                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
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
               // List<BlockLoadProfileSinglePhaseDto> BlockLoadProfileSinglePhaseData = new();
                BlockLoadProfileSinglePhase = BlockLoadProfileSinglePhaseGrid.Items.OfType<BlockLoadProfileSinglePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = "BlockLoadProfileSinglePhase" + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(BlockLoadProfileSinglePhase);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, "BlockLoad Profile Single Phase Meter", MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = "BlockLoadProfileSinglePhase" + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(BlockLoadProfileSinglePhase);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, "BlockLoad Profile Single Phase Meter", MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
               // List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhaseData = new();
                BlockLoadProfileThreePhase = BlockLoadProfileThreePhaseGrid.Items.OfType<BlockLoadProfileThreePhaseDto>().ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.BlockLoadProfileThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToBlockThreePhaseDataTable(BlockLoadProfileThreePhase);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.BlockLoadProfileThreePhaseMeter, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.BlockLoadProfileThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToBlockThreePhaseDataTable(BlockLoadProfileThreePhase);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.BlockLoadProfileThreePhaseMeter, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {               
              //  List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCTData = new();
                BlockLoadProfileThreePhaseCT = BlockLoadProfileThreePhaseCTGrid.Items.OfType<BlockLoadProfileThreePhaseCTDto>().ToList();

                string meterName = MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.BlockLoadProfileThreePhase + MeterType + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(BlockLoadProfileThreePhaseCT);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.BlockLoadProfile + meterName, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.BlockLoadProfileThreePhase + MeterType + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(BlockLoadProfileThreePhaseCT);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.BlockLoadProfile + meterName, MeterNumber);
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
                    BlockLoadProfileSinglePhase = BlockLoadProfileSinglePhaseGrid.Items.OfType<BlockLoadProfileSinglePhaseDto>().ToList();
                    if (BlockLoadProfileSinglePhase == null || BlockLoadProfileSinglePhase.Count <= 0)
                    {
                        labels.Clear();
                        values.Clear();
                        MainGraphView.Children.Clear();
                        YAxes = new Axis[]
                        {
                            new Axis
                            {
                                Name = "No Data",
                                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                                Labeler = (value) => Math.Round((double)value,2).ToString(),
                            }
                        };
                        CartesianChart ch1 = new CartesianChart();
                        ch1.YAxes = YAxes;
                        MainGraphView.Children.Add(ch1);
                        return;
                    }

                    foreach (var item in BlockLoadProfileSinglePhase)
                    {
                        if (DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date != DateTime.MinValue)
                        {
                            if (currentItem == Constants.NeutralCurrent)
                            {
                                //labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                //values.Add(Math.Round(double.Parse(item.NeutralCurrent, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.PhaseCurrent)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.PhaseCurrent, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.AverageVoltage)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.AverageVoltage, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykWhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykWhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykWhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykWhImport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykVAhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykVAhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykVAh, System.Globalization.NumberStyles.Any), 2));
                            }
                        }
                    }
                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    BlockLoadProfileThreePhase = BlockLoadProfileThreePhaseGrid.Items.OfType<BlockLoadProfileThreePhaseDto>().ToList();
                    if (BlockLoadProfileThreePhaseGraph == null || BlockLoadProfileThreePhaseGraph.Count <= 0)
                    {
                        labels.Clear();
                        values.Clear();
                        MainGraphView.Children.Clear();
                        YAxes = new Axis[]
                        {
                            new Axis
                            {
                                Name = "No Data",
                                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                                Labeler = (value) => Math.Round((double)value,2).ToString(),
                            }
                        };
                        CartesianChart ch1 = new CartesianChart();
                        ch1.YAxes = YAxes;
                        MainGraphView.Children.Add(ch1);
                        return;
                    }

                    foreach (var item in BlockLoadProfileThreePhaseGraph)
                    {
                        if (DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date != DateTime.MinValue)
                        {
                            if (currentItem == Constants.CurrentR)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.CurrentR, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.CurrentY)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.CurrentY, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.CurrentB)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.CurrentB, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.VoltageR)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.VoltageR, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.VoltageY)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.VoltageY, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.VoltageB)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.VoltageB, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykWhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykWhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykWhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykWhImport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykVAhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykVAhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykVAhImport, System.Globalization.NumberStyles.Any), 2));
                            }
                        }
                    }
                }

                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                   // List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCTData = new();
                    BlockLoadProfileThreePhaseCTGraph = BlockLoadProfileThreePhaseCTGrid.Items.OfType<BlockLoadProfileThreePhaseCTDto>().ToList();

                    if (BlockLoadProfileThreePhaseCTGraph == null || BlockLoadProfileThreePhaseCTGraph.Count <= 0)
                    {
                        labels.Clear();
                        values.Clear();
                        MainGraphView.Children.Clear();
                        YAxes = new Axis[]
                        {
                            new Axis
                            {
                                Name = "No Data",
                                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                                Labeler = (value) => Math.Round((double)value,2).ToString(),
                            }
                        };
                        CartesianChart ch1 = new CartesianChart();
                        ch1.YAxes = YAxes;
                        MainGraphView.Children.Add(ch1);
                        return;
                    }

                    foreach (var item in BlockLoadProfileThreePhaseCTGraph)
                    {
                        if (DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date != DateTime.MinValue)
                        {
                            if (currentItem == Constants.CurrentR)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.CurrentR, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.CurrentY)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.CurrentY, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.CurrentB)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.CurrentB, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.VoltageR)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.VoltageR, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.VoltageY)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.VoltageY, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.VoltageB)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.VoltageB, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykWhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykWhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykWhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykWhImport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykVAhExport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykVAhExport, System.Globalization.NumberStyles.Any), 2));
                            }
                            else if (currentItem == Constants.BlockEnergykVAhImport)
                            {
                                labels.Add(DateTime.ParseExact(item.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("HH:mm"));
                                values.Add(Math.Round(double.Parse(item.BlockEnergykVAhImport, System.Globalization.NumberStyles.Any), 2));
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
                        DataLabelsSize = 12,
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
                        DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        DataLabelsFormatter = point => $"{point.PrimaryValue} {toolTip}",
                        DataLabelsPosition = DataLabelsPosition.Top,
                        Values = values.ToArray(),
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
                    MinStep = 1,
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

        private async void BlockLoadGraphDateChange(object sender, SelectionChangedEventArgs e)
        {
            var currentFilterDate = GraphDatePicker.SelectedDate != null ? GraphDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;

            if (currentFilterDate != null)
            {
                await Filter(currentFilterDate, currentFilterDate, null, int.MaxValue);

                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
            }
            else
            {
                await Filter(currentFilterDate, currentFilterDate, null, int.MaxValue);
            }

        }
        private void DownloadGraph(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = graphDataFilter != null ? graphDataFilter.SelectedItem.ToString() : null;
            var exportType = ((System.Windows.Controls.ContentControl)cbExportChart.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;
            string GraphName = Constants.BlockLoadProfile;
            if (MeterType == Constants.SinglePhaseMeter)
            {
                GraphName = selectedItem + "_" + Constants.BlockLoadProfileSinglePhase;
            }
            else
            {
                GraphName = selectedItem + "_" + Constants.BlockLoadProfileThreePhase;
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

        public void ClearDateFilter(object sender, RoutedEventArgs e)
        {
            BlockLoadProfileSinglePhase = null;
            BlockLoadProfileThreePhase = null;
            BlockLoadProfileThreePhaseCT = null;
            FilterGraph(null, null);
            GraphDatePicker.SelectedDate = null;
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
                    if (MeterType.Equals(Constants.ThreePhaseMeter))
                    {
                        var blockLoadProfileThreePhaseList = BlockLoadProfileThreePhaseGrid.Items.OfType<BlockLoadProfileThreePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseGrid.ItemsSource = blockLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                    {
                        var blockLoadProfileThreePhaseCTList = BlockLoadProfileThreePhaseCTGrid.Items.OfType<BlockLoadProfileThreePhaseCTDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseCTGrid.ItemsSource = blockLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var blockLoadProfileSinglePhaseList = BlockLoadProfileSinglePhaseGrid.Items.OfType<BlockLoadProfileSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileSinglePhaseGrid.ItemsSource = blockLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == Constants.RealTimeClockDateAndTime || column == Constants.RealTimeClock)
                {
                    if (MeterType.Equals(Constants.ThreePhaseMeter))
                    {
                        var blockLoadProfileThreePhaseList = BlockLoadProfileThreePhaseGrid.Items.OfType<BlockLoadProfileThreePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseGrid.ItemsSource = blockLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                    {
                        var blockLoadProfileThreePhaseCTList = BlockLoadProfileThreePhaseCTGrid.Items.OfType<BlockLoadProfileThreePhaseCTDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseCTGrid.ItemsSource = blockLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var blockLoadProfileSinglePhaseList = BlockLoadProfileSinglePhaseGrid.Items.OfType<BlockLoadProfileSinglePhaseDto>().OrderBy(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileSinglePhaseGrid.ItemsSource = blockLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
            else
            {
                SortDirection = ListSortDirection.Ascending;
                if (column == "CreatedOn")
                {
                    if (MeterType.Equals(Constants.ThreePhaseMeter))
                    {
                        var blockLoadProfileThreePhaseList = BlockLoadProfileThreePhaseGrid.Items.OfType<BlockLoadProfileThreePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseGrid.ItemsSource = blockLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                    {
                        var blockLoadProfileThreePhaseCTList = BlockLoadProfileThreePhaseCTGrid.Items.OfType<BlockLoadProfileThreePhaseCTDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseCTGrid.ItemsSource = blockLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var blockLoadProfileSinglePhaseList = BlockLoadProfileSinglePhaseGrid.Items.OfType<BlockLoadProfileSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileSinglePhaseGrid.ItemsSource = blockLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
                else if (column == Constants.RealTimeClockDateAndTime || column == Constants.RealTimeClock)
                {
                    if (MeterType.Equals(Constants.ThreePhaseMeter))
                    {
                        var blockLoadProfileThreePhaseList = BlockLoadProfileThreePhaseGrid.Items.OfType<BlockLoadProfileThreePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseGrid.ItemsSource = blockLoadProfileThreePhaseList;

                        e.Handled = true;
                    }
                    else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                    {
                        var blockLoadProfileThreePhaseCTList = BlockLoadProfileThreePhaseCTGrid.Items.OfType<BlockLoadProfileThreePhaseCTDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileThreePhaseCTGrid.ItemsSource = blockLoadProfileThreePhaseCTList;

                        e.Handled = true;
                    }
                    else
                    {
                        var blockLoadProfileSinglePhaseList = BlockLoadProfileSinglePhaseGrid.Items.OfType<BlockLoadProfileSinglePhaseDto>().OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

                        BlockLoadProfileSinglePhaseGrid.ItemsSource = blockLoadProfileSinglePhaseList;

                        e.Handled = true;
                    }
                }
            }
        }
    }
}