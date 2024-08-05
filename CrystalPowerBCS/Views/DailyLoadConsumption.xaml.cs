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
    /// Interaction logic for DailyLoadConsumption.xaml
    /// </summary>
    public partial class DailyLoadConsumption : UserControl
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
        public DailyLoadProfileThreePhaseCommand DailyLoadProfileThreePhaseCommand;

        public List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCT;
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
        public DailyLoadConsumption()
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
            
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            string[] meterData = MeterPhaseType.Text.Split("&&");
            if (meterData.Length > 1)
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

                await BindDailyLoadProfileSinglePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                DailyLoadProfileThreePhaseCommand = new DailyLoadProfileThreePhaseCommand();

                var gridData = await DailyLoadProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

                await BindDailyLoadProfileThreePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                DailyLoadProfileThreePhaseCTCommand = new DailyLoadProfileThreePhaseCTCommand();

                var gridData = await DailyLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

                await BindDailyLoadProfileThreePhaseCTDto(gridData);

            }

            return;
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                DailyLoadProfileSinglePhaseCommand = new DailyLoadProfileSinglePhaseCommand();

                var gridData = await DailyLoadProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);
                await BindDailyLoadProfileSinglePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                DailyLoadProfileThreePhaseCommand = new DailyLoadProfileThreePhaseCommand();

                var gridData = await DailyLoadProfileThreePhaseCommand.GetAll(pageSize, MeterNumber);
                await BindDailyLoadProfileThreePhaseDto(gridData);
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                DailyLoadProfileThreePhaseCTCommand = new DailyLoadProfileThreePhaseCTCommand();

                var gridData = await DailyLoadProfileThreePhaseCTCommand.GetAll(pageSize, MeterNumber);
                await BindDailyLoadProfileThreePhaseCTDto(gridData);

            }
        }

        public async Task BindDailyLoadProfileSinglePhaseDto(List<DailyLoadProfileSinglePhaseDto> gridData)
        {
            try
            {
                if (gridData.Count == 0)
                {
                    return;
                }

                DailyLoadProfileSinglePhase = new List<DailyLoadProfileSinglePhaseDto>();

                var sortedData = gridData.OrderBy(y => System.DateTime.Parse(y.RealTimeClock)).ToList();

                DailyLoadProfileSinglePhaseDto previousRecord = null;

                for (int i = 0; i < sortedData.Count; i++) // Start from index 1 to skip the first record
                {
                    var currentRecord = sortedData[i];

                    if (previousRecord == null && i != 0)
                    {
                        previousRecord = sortedData[i - 1];
                    }
                    if (previousRecord != null)
                    {
                        DateTime previousDate = DateTime.Parse(previousRecord.RealTimeClock);
                        DateTime currentDate = DateTime.Parse(currentRecord.RealTimeClock);

                        // Perform calculations for the current record
                        DailyLoadProfileSinglePhaseDto dailyLoadDTO = new DailyLoadProfileSinglePhaseDto();
                        dailyLoadDTO.Number = i;
                        dailyLoadDTO.CreatedOn = currentRecord.CreatedOn;
                        dailyLoadDTO.RealTimeClock = currentRecord.RealTimeClock;
                        dailyLoadDTO.CumulativeEnergyKVAhImport = Math.Round((Double.Parse(currentRecord.CumulativeEnergyKVAhImport) - Double.Parse(previousRecord.CumulativeEnergyKVAhImport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykVAhExport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykVAhExport) - Double.Parse(previousRecord.CumulativeEnergykVAhExport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykWhImport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykWhImport) - Double.Parse(previousRecord.CumulativeEnergykWhImport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykWhExport = Math.Round(Double.Parse(currentRecord.CumulativeEnergykWhExport) - Double.Parse(previousRecord.CumulativeEnergykWhExport), 2).ToString();

                        DailyLoadProfileSinglePhase.Add(dailyLoadDTO);
                        previousRecord = currentRecord;
                    }
                }

                DailyLoadProfileSinglePhaseGrid.ItemsSource = DailyLoadProfileSinglePhase;
            }


            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine(Constants.Anerroroccurred + ex.Message);
            }
        }

        public async Task BindDailyLoadProfileThreePhaseDto(List<DailyLoadProfileThreePhaseDto> gridData)
        {
            try
            {
                if (gridData.Count == 0)
                {
                    return;
                }

                DailyLoadProfileThreePhase = new List<DailyLoadProfileThreePhaseDto>();

                var sortedData = gridData.OrderBy(y => System.DateTime.Parse(y.RealTimeClock)).ToList();

                DailyLoadProfileThreePhaseDto previousRecord = null;
                
                for (int i = 0; i < sortedData.Count; i++) // Start from index 1 to skip the first record
                {
                    var currentRecord = sortedData[i];

                    if (previousRecord == null && i!=0)
                    {
                        previousRecord = sortedData[i-1];
                    }
                    if (previousRecord != null) 
                    {
                        DateTime previousDate = DateTime.Parse(previousRecord.RealTimeClock);
                        DateTime currentDate = DateTime.Parse(currentRecord.RealTimeClock);

                        // Perform calculations for the current record
                        DailyLoadProfileThreePhaseDto dailyLoadDTO = new DailyLoadProfileThreePhaseDto();
                        dailyLoadDTO.Number = i;
                        dailyLoadDTO.CreatedOn = currentRecord.CreatedOn;
                        dailyLoadDTO.RealTimeClock = currentRecord.RealTimeClock;
                        dailyLoadDTO.CumulativeEnergykVAhImport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykVAhImport) - Double.Parse(previousRecord.CumulativeEnergykVAhImport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykVAhExport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykVAhExport) - Double.Parse(previousRecord.CumulativeEnergykVAhExport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykWhImport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykWhImport) - Double.Parse(previousRecord.CumulativeEnergykWhImport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykWhExport = Math.Round(Double.Parse(currentRecord.CumulativeEnergykWhExport) - Double.Parse(previousRecord.CumulativeEnergykWhExport), 2).ToString();

                        DailyLoadProfileThreePhase.Add(dailyLoadDTO);
                        previousRecord = currentRecord;
                    }
                    else
                    {
                        DailyLoadProfileThreePhase.Add(sortedData[i]);
                    }             
                    
                }

                DailyLoadProfileThreePhaseGrid.ItemsSource = DailyLoadProfileThreePhase;
            }


            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine(Constants.Anerroroccurred + ex.Message);
            }
        }
        public async Task BindDailyLoadProfileThreePhaseCTDto(List<DailyLoadProfileThreePhaseCTDto> gridData)
        {
            try
            {
                if (gridData.Count == 0)
                {
                    return;
                }

                DailyLoadProfileThreePhaseCT = new List<DailyLoadProfileThreePhaseCTDto>();

                var sortedData = gridData.OrderBy(y => System.DateTime.Parse(y.RealTimeClock)).ToList();

                DailyLoadProfileThreePhaseCTDto previousRecord = null;

                for (int i = 0; i < sortedData.Count; i++) // Start from index 1 to skip the first record
                {
                    var currentRecord = sortedData[i];

                    if (previousRecord == null && i != 0)
                    {
                        previousRecord = sortedData[i - 1];
                    }
                    if (previousRecord != null)
                    {
                        DateTime previousDate = DateTime.Parse(previousRecord.RealTimeClock);
                        DateTime currentDate = DateTime.Parse(currentRecord.RealTimeClock);

                        // Perform calculations for the current record
                        DailyLoadProfileThreePhaseCTDto dailyLoadDTO = new DailyLoadProfileThreePhaseCTDto();
                        dailyLoadDTO.Number = i;
                        dailyLoadDTO.CreatedOn = currentRecord.CreatedOn;
                        dailyLoadDTO.RealTimeClock = currentRecord.RealTimeClock;
                        dailyLoadDTO.CumulativeEnergykVAhImport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykVAhImport) - Double.Parse(previousRecord.CumulativeEnergykVAhImport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykVAhExport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykVAhExport) - Double.Parse(previousRecord.CumulativeEnergykVAhExport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykWhImport = Math.Round((Double.Parse(currentRecord.CumulativeEnergykWhImport) - Double.Parse(previousRecord.CumulativeEnergykWhImport)), 2).ToString();
                        dailyLoadDTO.CumulativeEnergykWhExport = Math.Round(Double.Parse(currentRecord.CumulativeEnergykWhExport) - Double.Parse(previousRecord.CumulativeEnergykWhExport), 2).ToString();

                        DailyLoadProfileThreePhaseCT.Add(dailyLoadDTO);
                        previousRecord = currentRecord;
                    }
                }

                DailyLoadProfileThreePhaseCTGrid.ItemsSource = DailyLoadProfileThreePhaseCT;
            }


            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine(Constants.Anerroroccurred + ex.Message);
            }
        }
        #region Binding data to Observable


        #endregion
        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });
            if (MeterType == Constants.SinglePhaseMeter)
            {
                DailyLoadProfileThreePhaseGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileSinglePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhImport);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                DailyLoadProfileSinglePhaseGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseCTGrid.Visibility = Visibility.Collapsed;
                DailyLoadProfileThreePhaseGrid.Visibility = Visibility.Visible;
                graphDataFilter.Items.Clear();

                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhImport);

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
                graphDataFilter.Items.Add( Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add( Constants.CumulativeEnergykVAhImport);

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
                    if (DailyLoadProfileThreePhase == null || DailyLoadProfileThreePhase.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in DailyLoadProfileThreePhase)
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

                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (DailyLoadProfileThreePhaseCT == null || DailyLoadProfileThreePhaseCT.Count <= 0)
                    {
                        return;
                    }

                    foreach (var item in DailyLoadProfileThreePhaseCT)
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
                GraphName = selectedItem + "_" + Constants.BillingProfileSinglePhase;
            }
            else
            {
                GraphName = selectedItem + "_" + Constants.BillingProfileThreePhase + "_" + selectedItem;
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
