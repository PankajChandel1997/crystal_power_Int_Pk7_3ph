using CrystalPowerBCS.DbFunctions;
using CrystalPowerBCS.DbFunctions.EventFunctions;
using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
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

        public class SingleVM
        {
            public string Current { get; set; }
            public string Voltage { get; set; }
        }

        public bool IsGraph;
        public bool IsFilterEnabled;
       
        public IPSinglePhaseCommand IPSinglePhaseCommand;
        public IPThreePhaseCommand IPThreePhaseCommand;
        public IPThreePhaseCTCommand IPThreePhaseCTCommand;

        public BillingProfileSinglePhaseCommand BillingProfileSinglePhaseCommand;
        public BillingProfileThreePhaseCommand BillingProfileThreePhaseCommand;
        public BillingProfileThreePhaseCTCommand BillingProfileThreePhaseCTCommand;
        public AllEventsSinglePhaseCommand AllEventsSinglePhaseReportCommand;

        public BlockLoadProfileSinglePhaseCommand BlockLoadProfileSinglePhaseCommand;
        public BlockLoadProfileThreePhaseCommand BlockLoadProfileThreePhaseCommand;
        public BlockLoadProfileThreePhaseCTCommand BlockLoadProfileThreePhaseCTCommand;
        public AllEventsCommand AllEventsThreePhaseReportCommand;

        public DailyLoadProfileSinglePhaseCommand DailyLoadProfileSinglePhaseCommand;
        public DailyLoadProfileThreePhaseCommand DailyLoadProfileThreePhaseCommand;
        public DailyLoadProfileThreePhaseCTCommand DailyLoadProfileThreePhaseCTCommand;

        public SelfDiagnosticCommand SelfDiagnosticCommand;


        public string MeterType;
        public string MeterNumber;
        public int pageSize = 10;
        public ErrorHelper _errorHelper;
        public string filterByField = "";
        public string currentItem;
        public string dateFormat = "dd-MM-yyyy HH:mm:ss";

        public List<string> _reports = new List<string>();

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Reports()
        {
            InitializeComponent();

            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;
            downarrow.Visibility = Visibility.Visible;
            uparrow.Visibility = Visibility.Collapsed;
            Filters.Visibility = Visibility.Collapsed;
        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                Grid_Parent1.Height = height - 217;
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
                graphFilter.Visibility = Visibility.Visible;
                graphDataFilter.Visibility = Visibility.Visible;
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
                graphFilter.Visibility = Visibility.Hidden;
                graphDataFilter.Visibility = Visibility.Hidden;
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

        private void cbNumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string startDate = FilterDatePicker != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, null, pageSize); ;
            });
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
        private void FilterByPageSize(object sender, SelectionChangedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, null, pageSize); ;
            });
        }

        [Serializable]
        public class User
        {
            public int age { get; set; }
            public string name { get; set; }
        }

        private async Task Filter(string startDate, string endDate,string fetchDate, int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                IPSinglePhaseCommand = new IPSinglePhaseCommand();

                var gridData = await IPSinglePhaseCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                IPThreePhaseCommand = new IPThreePhaseCommand();

                var gridData = await IPThreePhaseCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);
            }
        }

        private async Task PopullateIPGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                IPSinglePhaseCommand = new IPSinglePhaseCommand();

                var gridData = await IPSinglePhaseCommand.GetAll(pageSize, MeterNumber);
       
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                IPThreePhaseCommand = new IPThreePhaseCommand();

                var gridData = await IPThreePhaseCommand.GetAll(pageSize, MeterNumber);
            }
        }

        private async Task GetReportData(bool isBilling, bool isBlockLoad, bool isDailyLoad)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {


                BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();

                var gridData = await BillingProfileSinglePhaseCommand.GetAll(pageSize, MeterNumber);
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                IPThreePhaseCommand = new IPThreePhaseCommand();

                var gridData = await IPThreePhaseCommand.GetAll(pageSize, MeterNumber);
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
                graphDataFilter.Items.Add(Constants.PhaseCurrent);
                graphDataFilter.Items.Add(Constants.Voltage);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhImport);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;
            }
            else
            {
                graphDataFilter.Items.Add(Constants.VoltageY);
                graphDataFilter.Items.Add(Constants.VoltageB);
                graphDataFilter.Items.Add(Constants.VoltageR);
                graphDataFilter.Items.Add(Constants.CurrentB);
                graphDataFilter.Items.Add(Constants.CurrentR);
                graphDataFilter.Items.Add(Constants.CurrentY);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVArhQ1);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVArhQ2);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVArhQ3);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVArhQ4);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;

            }

            this.Dispatcher.Invoke(async () =>
            {
                await PopullateIPGrid();
            });

        }

        private async Task BindMeterTypeAndNumber()
        {
            string[] meterData = MeterPhaseType.Text.Split("&&");
            MeterType = meterData[0].ToString();
            MeterNumber = meterData[1].ToString();
        }

        private async void ExportData(object sender, SelectionChangedEventArgs e)
        {
            

            var exportType = ((System.Windows.Controls.ContentControl)cbExport.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;

            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            string fatchDate = null;

            bool IsBockLoad = chkBlockLoad.IsChecked.Value;
            bool IsBillingProfile = chkBillingProfile.IsChecked.Value;
            bool IsDailyLoadProfile = chkDailyLoadProfile.IsChecked.Value;
            bool isIp = chkIp.IsChecked.Value;
            bool isAllEvents = chkAllEvents.IsChecked.Value;
            bool isSelfDiagnosis = chkSelfDiagnosis.IsChecked.Value;

            if (MeterType == Constants.SinglePhaseMeter)
            {
                List<InstantaneousProfileSinglePhaseDto> InstantaneousProfileSinglePhaseReport = new();
                List<BillingProfileSinglePhaseDto> BillingProfileSinglePhaseReport = new();
                List<BlockLoadProfileSinglePhaseDto> BlockLoadProfileReport = new();
                List<DailyLoadProfileSinglePhaseDto> DailyLoadProfileSinglePhaseReport = new();
                List<AllEventsSinglePhaseDto> AllEventsSinglePhaseReport = new();
                List<SelfDiagnosticDto> selfDiagnosticsReport = new();

                if (isIp)
                {
                    IPSinglePhaseCommand = new IPSinglePhaseCommand();
                    InstantaneousProfileSinglePhaseReport = await IPSinglePhaseCommand.Filter(startDate, endDate, null, pageSize, MeterNumber);
                }
                if (IsBillingProfile)
                {
                    BillingProfileSinglePhaseCommand = new BillingProfileSinglePhaseCommand();
                    BillingProfileSinglePhaseReport = await BillingProfileSinglePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);

                    DateTime latestDate = BillingProfileSinglePhaseReport.Max(d => DateTime.ParseExact(d.CreatedOn, dateFormat, CultureInfo.InvariantCulture));

                    BillingProfileSinglePhaseReport = BillingProfileSinglePhaseReport.Where(d => DateTime.ParseExact(d.CreatedOn, dateFormat, CultureInfo.InvariantCulture) == latestDate).ToList();
                }

                if (IsBockLoad)
                {
                    BlockLoadProfileSinglePhaseCommand = new BlockLoadProfileSinglePhaseCommand();
                    BlockLoadProfileReport = await BlockLoadProfileSinglePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber); 
                }
                if (IsDailyLoadProfile)
                {
                    DailyLoadProfileSinglePhaseCommand = new DailyLoadProfileSinglePhaseCommand();
                    DailyLoadProfileSinglePhaseReport = await DailyLoadProfileSinglePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                }
                if (isAllEvents)
                {
                    AllEventsSinglePhaseReportCommand = new AllEventsSinglePhaseCommand();
                    AllEventsSinglePhaseReport = await AllEventsSinglePhaseReportCommand.GetAll(MeterNumber);
                }
                
                if (isSelfDiagnosis)
                {
                    SelfDiagnosticCommand = new SelfDiagnosticCommand();
                    selfDiagnosticsReport = await SelfDiagnosticCommand.GetAll(int.MaxValue,MeterNumber);
                }

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.AllReportsSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateReportsExcel(InstantaneousProfileSinglePhaseReport, BillingProfileSinglePhaseReport, BlockLoadProfileReport, DailyLoadProfileSinglePhaseReport,null,null,null,null, null, null, null, null, AllEventsSinglePhaseReport,null, selfDiagnosticsReport, FileName, Constants.Report, MeterNumber, MeterType);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.AllReportsSinglePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateReportsPdf(InstantaneousProfileSinglePhaseReport,BillingProfileSinglePhaseReport, BlockLoadProfileReport, DailyLoadProfileSinglePhaseReport, null, null, null,null,null,null,null,null, AllEventsSinglePhaseReport, null, selfDiagnosticsReport, FileName, Constants.Report, MeterNumber, MeterType);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                List<InstantaneousProfileThreePhaseDto> InstantaneousProfileThreePhaseReport = new();
                List<BillingProfileThreePhaseDto> BillingProfileThreePhaseReport = new();
                List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhaseReport = new();
                List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhaseReport = new();
                List<AllEventsDTO> AllEventsThreePhaseReport = new();
                List<SelfDiagnosticDto> selfDiagnosticsReport = new();

                if (isIp)
                {
                    IPThreePhaseCommand = new IPThreePhaseCommand();
                    InstantaneousProfileThreePhaseReport = await IPThreePhaseCommand.Filter(startDate, endDate, null, pageSize, MeterNumber);
                }
                if (IsBillingProfile)
                {
                    BillingProfileThreePhaseCommand = new BillingProfileThreePhaseCommand();
                    BillingProfileThreePhaseReport = await BillingProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber); 
                }

                if (IsBockLoad)
                {
                    BlockLoadProfileThreePhaseCommand = new BlockLoadProfileThreePhaseCommand();
                    BlockLoadProfileThreePhaseReport = await BlockLoadProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber); 
                }
                if (IsDailyLoadProfile)
                {
                    DailyLoadProfileThreePhaseCommand = new DailyLoadProfileThreePhaseCommand();
                    DailyLoadProfileThreePhaseReport = await DailyLoadProfileThreePhaseCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                }
                if (isAllEvents)
                {
                    AllEventsThreePhaseReportCommand = new AllEventsCommand();
                    AllEventsThreePhaseReport = await AllEventsThreePhaseReportCommand.GetAll(MeterNumber);
                }

                if (isSelfDiagnosis)
                {
                    SelfDiagnosticCommand = new SelfDiagnosticCommand();
                    selfDiagnosticsReport = await SelfDiagnosticCommand.GetAll(int.MaxValue, MeterNumber);
                }

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.AllReportsThreePhase + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateReportsExcel(null, null, null, null, InstantaneousProfileThreePhaseReport, BillingProfileThreePhaseReport, BlockLoadProfileThreePhaseReport, DailyLoadProfileThreePhaseReport,null,null,null,null, null,AllEventsThreePhaseReport, selfDiagnosticsReport, FileName, Constants.Report, MeterNumber, MeterType);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.AllReportsThreePhase + Guid.NewGuid().ToString();
                   
                    bool res = ExportHelper.GenerateReportsPdf(null,null,null,null,InstantaneousProfileThreePhaseReport,BillingProfileThreePhaseReport, BlockLoadProfileThreePhaseReport, DailyLoadProfileThreePhaseReport,null,null,null,null, null, AllEventsThreePhaseReport, selfDiagnosticsReport,FileName, Constants.Report, MeterNumber, MeterType);
                }
            }

            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                List<InstantaneousProfileThreePhaseCTDto> InstantaneousProfileThreePhaseCTReport = new();
                List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCTReport = new();
                List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCTReport = new();
                List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCTReport = new();
                List<AllEventsDTO> AllEventsThreePhaseCTReport = new();
                List<SelfDiagnosticDto> selfDiagnosticsReport = new();
                if (isIp)
                {
                    IPThreePhaseCTCommand = new IPThreePhaseCTCommand();
                    InstantaneousProfileThreePhaseCTReport = await IPThreePhaseCTCommand.Filter(startDate, endDate, null, pageSize, MeterNumber);
                }
                if (IsBillingProfile)
                {
                    BillingProfileThreePhaseCTCommand = new BillingProfileThreePhaseCTCommand();
                    BillingProfileThreePhaseCTReport = await BillingProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                }

                if (IsBockLoad)
                {
                    BlockLoadProfileThreePhaseCTCommand = new BlockLoadProfileThreePhaseCTCommand();
                    BlockLoadProfileThreePhaseCTReport = await BlockLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                }
                if (IsDailyLoadProfile)
                {
                    DailyLoadProfileThreePhaseCTCommand = new DailyLoadProfileThreePhaseCTCommand();
                    DailyLoadProfileThreePhaseCTReport = await DailyLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                }
                if (isAllEvents)
                {
                    AllEventsThreePhaseReportCommand = new AllEventsCommand();
                    AllEventsThreePhaseCTReport = await AllEventsThreePhaseReportCommand.GetAll(MeterNumber);
                }
                if (IsDailyLoadProfile)
                {
                    DailyLoadProfileThreePhaseCTCommand = new DailyLoadProfileThreePhaseCTCommand();
                    DailyLoadProfileThreePhaseCTReport = await DailyLoadProfileThreePhaseCTCommand.Filter(startDate, endDate, fatchDate, pageSize, MeterNumber);
                }

                if (isSelfDiagnosis)
                {
                    SelfDiagnosticCommand = new SelfDiagnosticCommand();
                    selfDiagnosticsReport = await SelfDiagnosticCommand.GetAll(int.MaxValue, MeterNumber);
                }

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.AllReportsThreePhaseCT + Guid.NewGuid().ToString();
                    bool res = ExportHelper.GenerateReportsExcel(null, null, null, null, null, null, null, null, InstantaneousProfileThreePhaseCTReport, BillingProfileThreePhaseCTReport, BlockLoadProfileThreePhaseCTReport, DailyLoadProfileThreePhaseCTReport,null, AllEventsThreePhaseCTReport, selfDiagnosticsReport, FileName, Constants.Report, MeterNumber, MeterType);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.AllReportsThreePhaseCT + Guid.NewGuid().ToString();

                    bool res = ExportHelper.GenerateReportsPdf(null, null, null, null, null, null, null, null, InstantaneousProfileThreePhaseCTReport, BillingProfileThreePhaseCTReport, BlockLoadProfileThreePhaseCTReport, DailyLoadProfileThreePhaseCTReport, null, AllEventsThreePhaseCTReport, selfDiagnosticsReport,FileName, Constants.Report, MeterNumber, MeterType);
                }
            }

            cbExport.SelectedIndex = 0;
        }

        private void FilterGraph(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newVal = (cbAllFeatures.IsChecked == true);
            chkIp.IsChecked = newVal;
            chkBlockLoad.IsChecked = newVal;
            chkBillingProfile.IsChecked = newVal;
            chkDailyLoadProfile.IsChecked = newVal;
            chkAllEvents.IsChecked = newVal;
            chkSelfDiagnosis.IsChecked = newVal;
        }
        private void cbFeature_CheckedChanged(object sender, RoutedEventArgs e)
        {
            cbAllFeatures.IsChecked = null;
            if ((chkBlockLoad.IsChecked == true) && (chkBillingProfile.IsChecked == true) && (chkDailyLoadProfile.IsChecked == true) && (chkIp.IsChecked == true) && (chkAllEvents.IsChecked == true) && (chkSelfDiagnosis.IsChecked == true))
                cbAllFeatures.IsChecked = true;
            if ((chkBlockLoad.IsChecked == false) && (chkBillingProfile.IsChecked == false) && (chkDailyLoadProfile.IsChecked == false) && (chkIp.IsChecked == false) && (chkAllEvents.IsChecked == false) && (chkSelfDiagnosis.IsChecked == false))
                cbAllFeatures.IsChecked = false;
        }
    }
}