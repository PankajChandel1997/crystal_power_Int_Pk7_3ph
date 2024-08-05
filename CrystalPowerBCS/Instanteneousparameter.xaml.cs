using CrystalPowerBCS.Helpers;
using Infrastructure.Helpers;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Spire.Xls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Drawing.Imaging;
using OxyPlot.Annotations;
using CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions;
using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions;
using System.Linq;
using SkiaSharp;
using Gurux.DLMS.Objects;
using System.Threading;
using OxyPlot.Wpf;
using static CrystalPowerBCS.Helpers.ExportHelper;
using System.IO;
using Notification.Wpf;
using System.Diagnostics;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for Instanteneousparameter.xaml
    /// </summary>
    public partial class Instanteneousparameter : UserControl
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
        public InstantaneousProfileSinglePhaseDto currentRow;
        public List<InstantaneousProfileSinglePhaseDto> InstantaneousProfileSingle;
        public IPSinglePhaseCommand IPSinglePhaseCommand;
        public List<InstantaneousProfileThreePhaseDto> InstantaneousProfileThree;
        public IPThreePhaseCommand IPThreePhaseCommand;
        public List<InstantaneousProfileThreePhaseCTDto> InstantaneousProfileThreeCT;
        public IPThreePhaseCTCommand IPThreePhaseCTCommand;
        //public readonly CurrentMeterTypeViewModel currentMeterTypeViewModel;
        public string MeterType;
        public string MeterNumber;
        public int pageSize = 10;
        public ErrorHelper _errorHelper;
        public string filterByField = "";
        public string currentItem;

        public ISeries[] Series { get; set; }
        public LiveChartsCore.SkiaSharpView.Axis[] XAxes { get; set; }
        public Instanteneousparameter()
        {

            InitializeComponent();

            _errorHelper = new ErrorHelper();


            //If Meter is single Phase Disply Single Phase Grid else three phase grid
            InstantaneousProfileSingleGrid.Visibility = Visibility.Visible;
            //InstantaneousProfileThreeGrid.Visibility = Visibility.Collapsed;

            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;

            downarrow.Visibility = Visibility.Visible;
            uparrow.Visibility = Visibility.Collapsed;

            Filters.Visibility = Visibility.Collapsed;

        }
        private void ShowLoader()
        {
            Spinner.IsLoading = true;
            InstantaneousProfileSingleGrid.Opacity = 0.7;
            InstantaneousProfileSingleGrid.IsEnabled = false;
        }

        private void hideLoader()
        {
            InstantaneousProfileSingleGrid.Opacity = 1;
            Spinner.IsLoading = false;
            InstantaneousProfileSingleGrid.IsEnabled = true;
        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                Grid_Parent1.Height = height - 217;
                InstantaneousProfileSingleGrid.MaxHeight = height - 217;
                InstantaneousProfileThreeGrid.MaxHeight = height - 217;
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

                IpTitle.Text = Constants.VectorGraph;
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
                FatchDateFilter.Visibility = Visibility.Collapsed;
                this.Dispatcher.Invoke(async () =>
                {
                    FilterGraph(null, null);
                }).Wait();
            }
            else
            {
                IpTitle.Text = Constants.Instanteneousparameter;
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
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate, endDate, null,pageSize); ;
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
                await Filter(startDate, endDate, null,pageSize); ;
            });
        }

        [Serializable]
        public class User
        {
            public int age { get; set; }
            public string name { get; set; }
        }

        private void DataGrid_CopyingRowClipboardContent(object sender, MouseButtonEventArgs e)
        {
            var data = ((FrameworkElement)e.OriginalSource).DataContext as InstantaneousProfileSinglePhaseDto;
            currentRow = data;
        }

        private void copyRow_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(currentRow.MeterNo + "  " + currentRow.Number + "  " + currentRow.Voltage + "  " + currentRow.CumulativeenergykVAhExport);
        }

        private async Task Filter(string startDate, string endDate, string fetchDate,int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ThreePhaseGraph.Visibility = Visibility.Hidden;
                SinglePhaseGraph.Visibility = Visibility.Visible;
                IPSinglePhaseCommand = new IPSinglePhaseCommand();

                var gridData = await IPSinglePhaseCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.Realtimeclock).Select(x => x.Realtimeclock).ToList();
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

                var gridDataModel = await IPSinglePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                SinglePhaseVectorGraph(MeterNumber, gridDataModel[0].SignedPowerFactor, gridDataModel[0].Voltage, gridDataModel[0].PhaseCurrent);
                InstantaneousProfileSingleGrid.ItemsSource = gridDataModel;
                InstantaneousProfileSingle = gridDataModel;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                ThreePhaseGraph.Visibility = Visibility.Visible;
                SinglePhaseGraph.Visibility = Visibility.Hidden;
                IPThreePhaseCommand = new IPThreePhaseCommand();

                var gridData = await IPThreePhaseCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.RealTimeClockDateAndTime).Select(x => x.RealTimeClockDateAndTime).ToList();
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

                var gridDataModel = await IPThreePhaseCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                ThreePhaseVectorGraph(gridDataModel[0].MeterNo, gridDataModel[0].SignedPowerFactorRPhase, gridDataModel[0].SignedPowerFactorBPhase, gridDataModel[0].SignedPowerFactorYPhase, gridDataModel[0].VoltageR, gridDataModel[0].VoltageY, gridDataModel[0].VoltageB, gridDataModel[0].CurrentR, gridDataModel[0].CurrentY, gridDataModel[0].CurrentB);
                InstantaneousProfileThreeGrid.ItemsSource = gridDataModel;
                InstantaneousProfileThree = gridDataModel;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                ThreePhaseGraph.Visibility = Visibility.Visible;
                SinglePhaseGraph.Visibility = Visibility.Hidden;
                IPThreePhaseCTCommand = new IPThreePhaseCTCommand();
                var gridData = await IPThreePhaseCTCommand.Filter(startDate, endDate, fetchDate, pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.RealTimeClockDateAndTime).Select(x => x.RealTimeClockDateAndTime).ToList();
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

                var gridDataModel = await IPThreePhaseCTCommand.Filter(startDate, endDate, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                ThreePhaseVectorGraph(gridDataModel[0].MeterNo, gridDataModel[0].SignedPowerFactorRPhase, gridDataModel[0].SignedPowerFactorBPhase, gridDataModel[0].SignedPowerFactorYPhase, gridDataModel[0].VoltageR, gridDataModel[0].VoltageY, gridDataModel[0].VoltageB, gridDataModel[0].CurrentR, gridDataModel[0].CurrentY, gridDataModel[0].CurrentB);
                InstantaneousProfileThreeCTGrid.ItemsSource = gridDataModel;
                InstantaneousProfileThreeCT = gridDataModel;
            }
        }

        private async Task PopullateIPGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ThreePhaseGraph.Visibility = Visibility.Hidden;
                SinglePhaseGraph.Visibility = Visibility.Visible;
                IPSinglePhaseCommand = new IPSinglePhaseCommand();
                var gridData = await IPSinglePhaseCommand.GetAll(pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.Realtimeclock).Select(x => x.Realtimeclock).ToList();
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

                var gridDataModel = await IPSinglePhaseCommand.Filter(null, null, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                SinglePhaseVectorGraph(MeterNumber, gridDataModel[0].SignedPowerFactor, gridDataModel[0].Voltage, gridDataModel[0].PhaseCurrent);
                InstantaneousProfileSingleGrid.ItemsSource = gridDataModel;
                InstantaneousProfileSingle = gridDataModel;
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                ThreePhaseGraph.Visibility = Visibility.Visible;
                SinglePhaseGraph.Visibility = Visibility.Hidden;
                IPThreePhaseCommand = new IPThreePhaseCommand();
                var gridData = await IPThreePhaseCommand.GetAll(pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.RealTimeClockDateAndTime).Select(x => x.RealTimeClockDateAndTime).ToList();
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

                var gridDataModel = await IPThreePhaseCommand.Filter(null, null, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                ThreePhaseVectorGraph(gridDataModel[0].MeterNo, gridDataModel[0].SignedPowerFactorRPhase, gridDataModel[0].SignedPowerFactorBPhase, gridDataModel[0].SignedPowerFactorYPhase, gridDataModel[0].VoltageR, gridDataModel[0].VoltageY, gridDataModel[0].VoltageB, gridDataModel[0].CurrentR, gridDataModel[0].CurrentY, gridDataModel[0].CurrentB);
                InstantaneousProfileThreeGrid.ItemsSource = gridDataModel;
                InstantaneousProfileThree = gridDataModel;
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                ThreePhaseGraph.Visibility = Visibility.Visible;
                SinglePhaseGraph.Visibility = Visibility.Hidden;
                IPThreePhaseCTCommand = new IPThreePhaseCTCommand();
                var gridData = await IPThreePhaseCTCommand.GetAll(pageSize, MeterNumber);

                if (gridData.Any() && FatchDateFilter.Items.Count <= 0)
                {
                    var dateList = gridData.DistinctBy(d => d.RealTimeClockDateAndTime).Select(x => x.RealTimeClockDateAndTime).ToList();
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

                var gridDataModel = await IPThreePhaseCTCommand.Filter(null, null, FatchDateFilter?.SelectedValue?.ToString(), pageSize, MeterNumber);

                ThreePhaseVectorGraph(gridDataModel[0].MeterNo, gridDataModel[0].SignedPowerFactorRPhase, gridDataModel[0].SignedPowerFactorYPhase, gridDataModel[0].SignedPowerFactorBPhase, gridDataModel[0].VoltageR, gridDataModel[0].VoltageY, gridDataModel[0].VoltageB, gridDataModel[0].CurrentR, gridDataModel[0].CurrentY, gridDataModel[0].CurrentB);
                InstantaneousProfileThreeCTGrid.ItemsSource = gridDataModel;
                InstantaneousProfileThreeCT = gridDataModel;
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
                InstantaneousProfileThreeGrid.Visibility = Visibility.Collapsed;
                InstantaneousProfileThreeCTGrid.Visibility = Visibility.Collapsed;
                InstantaneousProfileSingleGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Add(Constants.PhaseCurrent);
                graphDataFilter.Items.Add(Constants.Voltage);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykWhImport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhExport);
                graphDataFilter.Items.Add(Constants.CumulativeEnergykVAhImport);
                graphDataFilter.SelectedIndex = 0;
                graphFilter.SelectedIndex = 0;

            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                InstantaneousProfileSingleGrid.Visibility = Visibility.Collapsed;
                InstantaneousProfileThreeCTGrid.Visibility = Visibility.Collapsed;
                InstantaneousProfileThreeGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Add(Constants.VoltageY);
                graphDataFilter.Items.Add(Constants.VoltageB);
                graphDataFilter.Items.Add(Constants.VoltageR);
                graphDataFilter.Items.Add(Constants.CurrentB);
                graphDataFilter.Items.Add(Constants.CurrentY);
                graphDataFilter.Items.Add(Constants.CurrentR);
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
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                InstantaneousProfileSingleGrid.Visibility = Visibility.Collapsed;
                InstantaneousProfileThreeGrid.Visibility = Visibility.Collapsed;
                InstantaneousProfileThreeCTGrid.Visibility = Visibility.Visible;

                graphDataFilter.Items.Add(Constants.VoltageY);
                graphDataFilter.Items.Add(Constants.VoltageB);
                graphDataFilter.Items.Add(Constants.VoltageR);
                graphDataFilter.Items.Add(Constants.CurrentB);
                graphDataFilter.Items.Add(Constants.CurrentY);
                graphDataFilter.Items.Add(Constants.CurrentR);
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

        private void ExportData(object sender, SelectionChangedEventArgs e)
        {

            var exportType = ((System.Windows.Controls.ContentControl)cbExport.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;

            if (MeterType == Constants.SinglePhaseMeter)
            {
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.InstantaneousProfileSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(InstantaneousProfileSingle);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.InstantaneousProfileSinglePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.InstantaneousProfileSinglePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(InstantaneousProfileSingle);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.InstantaneousProfileSinglePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.InstantaneousProfileThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(InstantaneousProfileThree);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.InstantaneousProfileThreePhase, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.InstantaneousProfileThreePhase + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(InstantaneousProfileThree);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.InstantaneousProfileThreePhase, MeterNumber);
                }
            }
            else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
            {
                string meterName = MeterType == Constants.ThreePhaseLTCT ? Constants.ThreePhaseLTCT : Constants.ThreePhaseHTCT;

                var data = InstantaneousProfileThreeCT.Select(x => new
                {
                    x.Number,
                    x.CreatedOn,
                    x.MeterNo,
                    x.RealTimeClockDateAndTime,
                    x.CurrentR,
                    x.CurrentY,
                    x.CurrentB,
                    x.VoltageR,
                    x.VoltageY,
                    x.VoltageB,
                    x.SignedPowerFactorRPhase,
                    x.SignedPowerFactorYPhase,
                    x.SignedPowerFactorBPhase,
                    x.ThreePhasePowerFactoPF,
                    x.FrequencyHz,
                    x.ApparentPowerKVA,
                    x.SignedActivePowerkW,
                    x.SignedReactivePowerkvar,
                    x.CumulativeEnergykWhImport,
                    x.CumulativeEnergykWhExport,
                    x.CumulativeEnergykVAhImport,
                    x.CumulativeEnergykVAhExport,
                    x.CumulativeEnergykVArhQ1,
                    x.CumulativeEnergykVArhQ2,
                    x.CumulativeEnergykVArhQ3,
                    x.CumulativeEnergykVArhQ4,
                    x.NumberOfPowerFailures,
                    x.CumulativePowerOFFDurationInMin,
                    x.CumulativeTamperCount,
                    x.BillingPeriodCounter,
                    x.CumulativeProgrammingCount,
                    x.BillingDateImportMode,
                    MDkW = x.MaximumDemandkW,
                    MDkWDateTime = x.MaximumDemandkWDateTime,
                    MDkVA = x.MaximumDemandkVA,
                    MDkVADateTime = x.MaximumDemandkVADateTime,
                    x.CumulativeEnergyWhImport,
                    x.CumulativeEnergyWhExport,
                    x.CumulativeEnergyVAhImport,
                    x.CumulativeEnergyVAhExport
                }).ToList();

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.InstantaneousProfile + MeterType + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(data);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.InstantaneousProfile + meterName + "", MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.InstantaneousProfile + MeterType + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(data);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.InstantaneousProfile + meterName + "", MeterNumber);
                }
            }

            cbExport.SelectedIndex = 0;
        }

        private async void FilterDateChange(object sender, SelectionChangedEventArgs e)
        {
            string selectedDate = FatchDateFilter.SelectedValue?.ToString();

            await Filter(null, null, selectedDate, int.MaxValue);
        }

        private void FilterGraph(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentItem = graphDataFilter.SelectedValue?.ToString();

                string currentGraph = ((System.Windows.Controls.ContentControl)graphFilter.SelectedValue).Content as string;

                if (currentItem == null || MeterType == null)
                {
                    return;
                }

                var dates = new ObservableCollection<DateTimePoint>();

                if (MeterType == Constants.SinglePhaseMeter)
                {
                    if (InstantaneousProfileSingle == null || InstantaneousProfileSingle.Count <= 0)
                    {
                        return;
                    }
                    foreach (var item in InstantaneousProfileSingle)
                    {
                        if (currentItem == Constants.PhaseCurrent)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.PhaseCurrent, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.NeutralCurrent)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.NeutralCurrent, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.Voltage)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.Voltage, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.CumulativeenergykVAhExport, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhImport)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.CumulativeenergykVAhimport, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.CumulativeenergykWhExport, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhImport)
                        {
                            dates.Add(new(DateTime.Parse(item.Realtimeclock).Date, double.Parse(item.CumulativeenergykWhimport, System.Globalization.NumberStyles.Any)));
                        }
                    }
                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    if (InstantaneousProfileThree == null || InstantaneousProfileThree.Count <= 0)
                    {
                        return;
                    }
                    foreach (var item in InstantaneousProfileThree)
                    {
                        if (currentItem == Constants.VoltageY)
                        {
                            dates.Add(new(DateTime.Parse(item.VoltageY).Date, double.Parse(item.VoltageY, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.VoltageB)
                        {
                            dates.Add(new(DateTime.Parse(item.VoltageB).Date, double.Parse(item.VoltageB, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.VoltageR)
                        {
                            dates.Add(new(DateTime.Parse(item.VoltageR).Date, double.Parse(item.VoltageR, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CurrentB)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CurrentB, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CurrentR)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CurrentR, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CurrentY)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CurrentY, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhExport)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykWhImport)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any)));
                        }

                        else if (currentItem == Constants.CumulativeEnergykVAhExport)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVAhImport)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykVAhImport, System.Globalization.NumberStyles.Any)));
                        }

                        else if (currentItem == Constants.CumulativeEnergykVArhQ1)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykVArhQ1, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ2)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykVArhQ2, System.Globalization.NumberStyles.Any)));
                        }

                        else if (currentItem == Constants.CumulativeEnergykVArhQ3)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykVArhQ3, System.Globalization.NumberStyles.Any)));
                        }
                        else if (currentItem == Constants.CumulativeEnergykVArhQ4)
                        {
                            dates.Add(new(DateTime.Parse(item.RealTimeClockDateAndTime).Date, double.Parse(item.CumulativeEnergykVArhQ4, System.Globalization.NumberStyles.Any)));
                        }
                    }
                }
                else
                {
                    return;
                }


                Series = new ISeries[]
                {
                 new PieSeries<double> { Values = new double[] { 2 } },
                new PieSeries<double> { Values = new double[] { 4 } },
                new PieSeries<double> { Values = new double[] { 1 } },
                new PieSeries<double> { Values = new double[] { 4 } },
                new PieSeries<double> { Values = new double[] { 3 } }
                };

                XAxes = new LiveChartsCore.SkiaSharpView.Axis[]
                {
                new LiveChartsCore.SkiaSharpView.Axis
                {
                    LabelsRotation = 15,
                }
                };

                CartesianChart ch = new CartesianChart();
                ch.Series = Series;

                ch.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.None;
                MainGraphView.Children.Add(ch);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void GlobalDateChange(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await Filter(SelectedDate.Text, SelectedDate.Text, null,int.MaxValue);
            });
        }

        // single phase vector graph starts
        private void SinglePhaseVectorGraph(string GraphMeterNumber,string PF, string Voltage, string Current)
        {
            double pf = Double.Parse(PF);
            double voltage = Double.Parse(Voltage);
            double current = Double.Parse(Current);

            var model = new PlotModel { Title = "" };
            model.Background = OxyColors.White;
            // Calculate angles from power factor using inverse cosine function
            double angle = pf<0?Math.Acos(pf) * (180.0 / Math.PI): 360-Math.Acos(pf) * (180.0 / Math.PI);

            // Calculate coordinates for the red line endpoint
            double lineX = 4 * Math.Cos(angle * Math.PI / 180.0);
            double lineY = 4 * Math.Sin(angle * Math.PI / 180.0);

            double FixedRedLineX = 4 * Math.Cos(0 * Math.PI / 180.0);
            double FixedRedLineY = 4 * Math.Sin(0 * Math.PI / 180.0);
            var FixedRedLineSeries = new LineSeries
            {
                Title = "Red",
                Color = OxyColors.Black,
                StrokeThickness = 1.0
            };
            FixedRedLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
            FixedRedLineSeries.Points.Add(new DataPoint(FixedRedLineX, FixedRedLineY)); // Line endpoint
            var FixedRedArrowAnnotation = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(FixedRedLineX, FixedRedLineY),
                Color = OxyColors.Red,
                HeadLength = 5,
                HeadWidth = 3
            };
            var FixedRedTextAnnotation = new TextAnnotation
            {
                Text = "V",
                TextPosition = new DataPoint(FixedRedLineX, FixedRedLineY),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Black,
                StrokeThickness = 0
            };

            model.Annotations.Add(FixedRedArrowAnnotation);
            model.Annotations.Add(FixedRedTextAnnotation);
            model.Series.Add(FixedRedLineSeries);

            var RedPFTextAnnotation = new TextAnnotation
            {
                Text = $"PF = {pf}",
                TextPosition = new DataPoint(4, 4.5),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Black,
                StrokeThickness = 0
            };
            var RedCurrentTextAnnotation = new TextAnnotation
            {
                Text = $"I = {current}" + "A",
                TextPosition = new DataPoint(4, 4),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Black,
                StrokeThickness = 0
            };
            var RedVoltageTextAnnotation = new TextAnnotation
            {
                Text = $"V = {voltage}" + "V",
                TextPosition = new DataPoint(4, 3.5),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Black,
                StrokeThickness = 0
            };
            var MeterNumberTextAnnotation = new TextAnnotation
            {
                Text = $"Meter Number = {GraphMeterNumber}",
                TextPosition = new DataPoint(-4.5, 0.3),
                TextRotation = -90,
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Black,
                StrokeThickness = 0,
                FontSize = 14
            };
            model.Annotations.Add(MeterNumberTextAnnotation);
            model.Annotations.Add(RedCurrentTextAnnotation);
            model.Annotations.Add(RedPFTextAnnotation);
            model.Annotations.Add(RedVoltageTextAnnotation);
            // Create a LineSeries for the red line
            var redLineSeries = new LineSeries
            {
                Title = "Red Line",
                Color = OxyColors.Red
            };

            redLineSeries.Points.Add(new DataPoint(0, 0));
            redLineSeries.Points.Add(new DataPoint(lineX, lineY)); 

            // Create an ArrowAnnotation for the triangle at the end of the line
            var arrowAnnotation = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(lineX, lineY),
                Color = OxyColors.Red,
                HeadLength = 5,
                HeadWidth = 3
            };

            // Create a TextAnnotation for the angle at the end of the line
            var angleTextAnnotation = new TextAnnotation
            {
                Text = (pf<0?angle:360-angle).ToString("F1") + "°",
                TextPosition = new DataPoint(0.5, 0.5),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Top,
                TextColor = OxyColors.Red,
                StrokeThickness = 0
            };

            // Create a TextAnnotation for the current at the end of the line
            var currentTextAnnotation = new TextAnnotation
            {
                Text = pf >= 0 && pf != 1 && pf !=-1? "I Lag" : pf != 1 && pf < 0 && pf != -1 ? "I Lead" : "I",
                TextPosition = new DataPoint(lineX, lineY),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Red,
                StrokeThickness = 0
            };

            // Create a TextAnnotation for the current at the end of the line

            // Create X and Y axes with fixed range of [-5, 5]
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -5,
                Maximum = 5,
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -5,
                Maximum = 5,
            };

            // Create X and Y axis annotations for the origin
            var xAxisOrigin = new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = 0,
                MinimumY = -5,
                MaximumY = 5,
                Color = OxyColors.Black,
                LineStyle = LineStyle.Solid
            };

            var yAxisOrigin = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Y = 0,
                MinimumX = -5,
                MaximumX = 5,
                Color = OxyColors.Black,
                LineStyle = LineStyle.Solid
            };
            if(voltage != 0 && current !=0)
            {
                model.Annotations.Add(arrowAnnotation);
                model.Annotations.Add(angleTextAnnotation);
                model.Annotations.Add(currentTextAnnotation);
                model.Annotations.Add(xAxisOrigin);
                model.Annotations.Add(yAxisOrigin);
                model.Series.Add(redLineSeries);
            }
            
            // Set the axes and the plot model to the PlotView
            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);
            PhasorPlotView.Model = model;
            PhasorPlotView.Controller = new PlotController();
            PhasorPlotView.Controller.UnbindAll();
        }

        //3 Phase Vector Graph
        private void ThreePhaseVectorGraph(string GraphMeterNumber, string PF1, string PF2, string PF3, string Voltage1, string Voltage2, string Voltage3, string Current1, string Current2, string Current3)
        {
            var model = new PlotModel { Title = "" };
            model.Background = OxyColors.White;

            var pf1 = Double.Parse(PF1);
            var voltage1 = Double.Parse(Voltage1);
            var current1 = Double.Parse(Current1);

            var pf2 = Double.Parse(PF2);
            var voltage2 = Double.Parse(Voltage2);
            var current2 = Double.Parse(Current2);

            var pf3 = Double.Parse(PF3);
            var voltage3 = Double.Parse(Voltage3);
            var current3 = Double.Parse(Current3);


            //red line
            if (voltage1 != 0)
            {
                double FixedRedLineX = 4 * Math.Cos(0 * Math.PI / 180.0);
                double FixedRedLineY = 4 * Math.Sin(0 * Math.PI / 180.0);
                var FixedRedLineSeries = new LineSeries
                {
                    Title = "Red",
                    Color = OxyColors.Black,
                    StrokeThickness = 1.0
                };
                FixedRedLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
                FixedRedLineSeries.Points.Add(new DataPoint(FixedRedLineX, FixedRedLineY)); // Line endpoint
                var FixedRedArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(FixedRedLineX, FixedRedLineY),
                    Color = OxyColors.Red,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var FixedRedTextAnnotation = new TextAnnotation
                {
                    Text = "Vr",
                    TextPosition = new DataPoint(FixedRedLineX, FixedRedLineY),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextColor = OxyColors.Black,
                    StrokeThickness = 0
                };

                model.Annotations.Add(FixedRedArrowAnnotation);
                model.Annotations.Add(FixedRedTextAnnotation);
                model.Series.Add(FixedRedLineSeries);

            }
            else
            {
                double FixedRedLineX = 4 * Math.Cos(0 * Math.PI / 180.0);
                double FixedRedLineY = 4 * Math.Sin(0 * Math.PI / 180.0);
                var FixedRedLineSeries = new LineSeries
                {
                    Title = "Red",
                    Color = OxyColors.Transparent,
                    StrokeThickness = 1.0
                };
                FixedRedLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
                FixedRedLineSeries.Points.Add(new DataPoint(FixedRedLineX, FixedRedLineY)); // Line endpoint
                var FixedRedArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(FixedRedLineX, FixedRedLineY),
                    Color = OxyColors.Transparent,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var FixedRedTextAnnotation = new TextAnnotation
                {
                    Text = "Vr",
                    TextPosition = new DataPoint(FixedRedLineX, FixedRedLineY),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextColor = OxyColors.Transparent,
                    StrokeThickness = 0
                };

                model.Annotations.Add(FixedRedArrowAnnotation);
                model.Annotations.Add(FixedRedTextAnnotation);
                model.Series.Add(FixedRedLineSeries);
            }
            var RedPFTextAnnotation = new TextAnnotation
            {
                Text = $"PF = {pf1}",
                TextPosition = new DataPoint(4, 3),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Red,
                StrokeThickness = 0
            };
            var RedCurrentTextAnnotation = new TextAnnotation
            {
                Text = $"I = {current1}" + "A",
                TextPosition = new DataPoint(5, 3),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Red,
                StrokeThickness = 0
            };
            var RedVoltageTextAnnotation = new TextAnnotation
            {
                Text = $"V = {voltage1}" + "V",
                TextPosition = new DataPoint(6, 3),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Red,
                StrokeThickness = 0
            };
            model.Annotations.Add(RedCurrentTextAnnotation);
            model.Annotations.Add(RedPFTextAnnotation);
            model.Annotations.Add(RedVoltageTextAnnotation);
            if (current1 != 0 && voltage1 !=0)
            {
                double angleRed = pf1 < 0 ? (Math.Acos(pf1) * (180.0 / Math.PI)) : (360 - (Math.Acos(pf1) * (180.0 / Math.PI)));
                double RedLineX = 4 * Math.Cos(angleRed * Math.PI / 180.0);
                double RedLineY = 4 * Math.Sin(angleRed * Math.PI / 180.0);
                double RedLineAnglePointX = pf1 >= 0 ? (0.7 * Math.Cos((angleRed + ((360 - angleRed) / 2)) * Math.PI / 180.0)) : (0.5 * Math.Cos(angleRed / 2 * Math.PI / 180.0));
                double RedLineAnglePointY = pf1 >= 0 ? (0.7 * Math.Sin((angleRed + ((360 - angleRed) / 2)) * Math.PI / 180.0)) : (0.5 * Math.Sin(angleRed / 2 * Math.PI / 180.0));
                var RedLeadLag = pf1 >= 0 && pf1!=1 && pf1!=-1? "Ir Lag" : pf1!=1 && pf1<0 && pf1 != -1 ? "Ir Lead":"Ir";
                double magnitude1 = Math.Sqrt(RedLineX * RedLineX + RedLineY * RedLineY);
                RedLineX = RedLineX - (RedLineX / magnitude1); // Decrease x by 1 to move one point previous
                RedLineY = RedLineY - (RedLineY / magnitude1);
                var redLineSeries = new LineSeries
                {
                    Title = "Red",
                    Color = OxyColors.Red,
                    StrokeThickness = 1.0
                };
                redLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
                redLineSeries.Points.Add(new DataPoint(RedLineX, RedLineY)); // Line endpoint
                var RedArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(RedLineX, RedLineY),
                    Color = OxyColors.Red,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var RedLeadLagTextAnnotation = new TextAnnotation
                {
                    Text = RedLeadLag,
                    TextPosition = new DataPoint(RedLineX+0.3, RedLineY),
                    TextColor = OxyColors.Red,
                    StrokeThickness = 0
                };
                var RedAngleTextAnnotation = new TextAnnotation
                {
                    Text = (pf1 >= 0 ? 360 - angleRed : angleRed).ToString("F1") + "°",
                    TextPosition = new DataPoint(RedLineAnglePointX, RedLineAnglePointY),
                    TextColor = OxyColors.Red,
                    StrokeThickness = 0
                };
                model.Annotations.Add(RedAngleTextAnnotation);
                model.Annotations.Add(RedLeadLagTextAnnotation);
                model.Series.Add(redLineSeries);
                model.Annotations.Add(RedArrowAnnotation);

            }

            //yellow line
            if (voltage2 != 0)
            {
                double FixedYellowLineX = 4 * Math.Cos(120.0 * Math.PI / 180.0);
                double FixedYellowLineY = 4 * Math.Sin(120.0 * Math.PI / 180.0);
                var FixedYellowLineSeries = new LineSeries
                {
                    Title = "Yellow",
                    Color = OxyColor.Parse("##bfa119"),
                    StrokeThickness = 1.0
                };
                FixedYellowLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
                FixedYellowLineSeries.Points.Add(new DataPoint(FixedYellowLineX, FixedYellowLineY)); // Line endpoint
                var FixedYellowArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(FixedYellowLineX, FixedYellowLineY),
                    Color = OxyColor.Parse("##bfa119"),
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var FixedYellowTextAnnotation = new TextAnnotation
                {
                    Text = "Vy",
                    TextPosition = new DataPoint(FixedYellowLineX - 0.3, FixedYellowLineY - 0.3),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextColor = OxyColors.Black,
                    StrokeThickness = 0
                };
                model.Series.Add(FixedYellowLineSeries);
                model.Annotations.Add(FixedYellowArrowAnnotation);
                model.Annotations.Add(FixedYellowTextAnnotation);

            }
            else
            {
                double FixedYellowLineX = 4 * Math.Cos(120.0 * Math.PI / 180.0);
                double FixedYellowLineY = 4 * Math.Sin(120.0 * Math.PI / 180.0);
                var FixedYellowLineSeries = new LineSeries
                {
                    Title = "Yellow",
                    Color = OxyColors.Transparent,
                    StrokeThickness = 1.0
                };
                FixedYellowLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
                FixedYellowLineSeries.Points.Add(new DataPoint(FixedYellowLineX, FixedYellowLineY)); // Line endpoint
                var FixedYellowArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(FixedYellowLineX, FixedYellowLineY),
                    Color = OxyColors.Transparent,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var FixedYellowTextAnnotation = new TextAnnotation
                {
                    Text = "Vy",
                    TextPosition = new DataPoint(FixedYellowLineX - 0.3, FixedYellowLineY - 0.3),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextColor = OxyColors.Transparent,
                    StrokeThickness = 0
                };
                model.Series.Add(FixedYellowLineSeries);
                model.Annotations.Add(FixedYellowArrowAnnotation);
                model.Annotations.Add(FixedYellowTextAnnotation);
            }
            var YellowPFTextAnnotation = new TextAnnotation
            {
                Text = $"PF = {pf2}",
                TextPosition = new DataPoint(4, 2.7),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColor.Parse("##bfa119"),
                StrokeThickness = 0
            };
            var YellowCurrentTextAnnotation = new TextAnnotation
            {
                Text = $"I = {current2}" + "A",
                TextPosition = new DataPoint(5, 2.7),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColor.Parse("##bfa119"),
                StrokeThickness = 0
            };
            var YellowVoltageTextAnnotation = new TextAnnotation
            {
                Text = $"V = {voltage2}" + "V",
                TextPosition = new DataPoint(6, 2.7),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColor.Parse("##bfa119"),
                StrokeThickness = 0
            };
            model.Annotations.Add(YellowPFTextAnnotation);
            model.Annotations.Add(YellowCurrentTextAnnotation);
            model.Annotations.Add(YellowVoltageTextAnnotation);

            if (current2 != 0 && voltage2 !=0)
            {
                double angleYellow = pf3 < 0 ? ((Math.Acos(pf2) * (180.0 / Math.PI)) + 120) : (120 - (Math.Acos(pf2) * (180.0 / Math.PI)));
                double YellowLineX = 4 * Math.Cos(angleYellow * Math.PI / 180.0);
                double YellowLineY = 4 * Math.Sin(angleYellow * Math.PI / 180.0);
                double YellowLineAnglePointX = 0.5 * Math.Cos(((angleYellow - 120) / 2 + 120) * Math.PI / 180.0);
                double YellowLineAnglePointY = 0.5 * Math.Sin(((angleYellow - 120) / 2 + 120) * Math.PI / 180.0);
                var YellowLeadLag = pf2 >= 0 && pf2 != 1 && pf2 !=-1? "Iy Lag" : pf2 != 1 && pf2 < 0 && pf2 != -1 ? "Iy Lead" : "Iy";
                double magnitude3 = Math.Sqrt(YellowLineX * YellowLineX + YellowLineY * YellowLineY);
                YellowLineX = YellowLineX - (YellowLineX / magnitude3); // Decrease x by 1 to move one point previous
                YellowLineY = YellowLineY - (YellowLineY / magnitude3);
                var yellowLineSeries = new LineSeries
                {
                    Title = "Yellow",
                    Color = OxyColor.Parse("##bfa119"),
                    StrokeThickness = 1.0
                };
                yellowLineSeries.Points.Add(new DataPoint(0, 0)); // Origin
                yellowLineSeries.Points.Add(new DataPoint(YellowLineX, YellowLineY)); // Line endpoint

                var YellowArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(YellowLineX, YellowLineY),
                    Color = OxyColor.Parse("##bfa119"),
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var YellowLeadLagTextAnnotation = new TextAnnotation
                {
                    Text = YellowLeadLag,
                    TextPosition = new DataPoint(YellowLineX, YellowLineY),
                    TextColor = OxyColor.Parse("##bfa119"),
                    StrokeThickness = 0
                };
                var YellowAngleTextAnnotation = new TextAnnotation
                {
                    Text = ((angleYellow - 120) < 0 ? (angleYellow - 120) * -1 : (angleYellow - 120)).ToString("F1") + "°",
                    TextPosition = new DataPoint(YellowLineAnglePointX, YellowLineAnglePointY),
                    TextColor = OxyColor.Parse("##bfa119"),
                    StrokeThickness = 0
                };
                model.Annotations.Add(YellowAngleTextAnnotation);
                model.Annotations.Add(YellowLeadLagTextAnnotation);
                model.Series.Add(yellowLineSeries);
                model.Annotations.Add(YellowArrowAnnotation);
            }

            //Blue Line
            if (voltage3 != 0)
            {
                double FixedBlueLineX = 4 * Math.Cos(240.0 * Math.PI / 180.0);
                double FixedBlueLineY = 4 * Math.Sin(240.0 * Math.PI / 180.0);
                var FixedBlueLineSeries = new LineSeries
                {
                    Title = "Blue",
                    Color = OxyColors.Blue,
                    StrokeThickness = 1.0
                };
                FixedBlueLineSeries.Points.Add(new DataPoint(0, 0));
                FixedBlueLineSeries.Points.Add(new DataPoint(FixedBlueLineX, FixedBlueLineY));
                var FixedBlueArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(FixedBlueLineX, FixedBlueLineY),
                    Color = OxyColors.Blue,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var FixedBlueTextAnnotation = new TextAnnotation
                {
                    Text = "Vb",
                    TextPosition = new DataPoint(FixedBlueLineX + 0.2, FixedBlueLineY),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextColor = OxyColors.Black,
                    StrokeThickness = 0
                };
                model.Series.Add(FixedBlueLineSeries);
                model.Annotations.Add(FixedBlueArrowAnnotation);
                model.Annotations.Add(FixedBlueTextAnnotation);
            }
            else
            {
                double FixedBlueLineX = 4 * Math.Cos(240.0 * Math.PI / 180.0);
                double FixedBlueLineY = 4 * Math.Sin(240.0 * Math.PI / 180.0);
                var FixedBlueLineSeries = new LineSeries
                {
                    Title = "Blue",
                    Color = OxyColors.Transparent,
                    StrokeThickness = 1.0
                };
                FixedBlueLineSeries.Points.Add(new DataPoint(0, 0));
                FixedBlueLineSeries.Points.Add(new DataPoint(FixedBlueLineX, FixedBlueLineY));
                var FixedBlueArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(FixedBlueLineX, FixedBlueLineY),
                    Color = OxyColors.Transparent,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var FixedBlueTextAnnotation = new TextAnnotation
                {
                    Text = "Vb",
                    TextPosition = new DataPoint(FixedBlueLineX + 0.2, FixedBlueLineY),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    TextColor = OxyColors.Transparent,
                    StrokeThickness = 0
                };
                model.Series.Add(FixedBlueLineSeries);
                model.Annotations.Add(FixedBlueArrowAnnotation);
                model.Annotations.Add(FixedBlueTextAnnotation);
            }
            var BluePFTextAnnotation = new TextAnnotation
            {
                Text = $"PF = {pf3}",
                TextPosition = new DataPoint(4, 2.4),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Blue,
                StrokeThickness = 0
            };
            var BlueCurrentTextAnnotation = new TextAnnotation
            {
                Text = $"I = {current3}" + "A",
                TextPosition = new DataPoint(5, 2.4),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Blue,
                StrokeThickness = 0
            };
            var BlueVoltageTextAnnotation = new TextAnnotation
            {
                Text = $"V = {voltage3}" + "V",
                TextPosition = new DataPoint(6, 2.4),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Blue,
                StrokeThickness = 0
            };
            model.Annotations.Add(BlueCurrentTextAnnotation);
            model.Annotations.Add(BluePFTextAnnotation);
            model.Annotations.Add(BlueVoltageTextAnnotation);

            if (current3 != 0 && voltage3 !=0)
            {
                double angleBlue = pf2 < 0 ? ((Math.Acos(pf3) * (180.0 / Math.PI)) + 240) : (240 - (Math.Acos(pf3) * (180.0 / Math.PI)));
                double BlueLineX = 4 * Math.Cos(angleBlue * Math.PI / 180.0);
                double BlueLineY = 4 * Math.Sin(angleBlue * Math.PI / 180.0);
                double BlueLineAnglePointX = 0.5 * Math.Cos(((angleBlue - 240) / 2 + 240) * Math.PI / 180.0);
                double BlueLineAnglePointY = 0.5 * Math.Sin(((angleBlue - 240) / 2 + 240) * Math.PI / 180.0);

                var BlueLeadLag = pf3 >= 0 && pf3 != 1 && pf3 !=-1 ? "Ib Lag" : pf3 != 1 && pf3 < 0 && pf3 != -1 ? "Ib Lead" : "Ib";

                //calculating one prevoius point of coordinates
                double magnitude2 = Math.Sqrt(BlueLineX * BlueLineX + BlueLineY * BlueLineY);
                BlueLineX = BlueLineX - (BlueLineX / magnitude2); // Decrease x by 1 to move one point previous
                BlueLineY = BlueLineY - (BlueLineY / magnitude2);

                var blueLineSeries = new LineSeries
                {
                    Title = "Blue",
                    Color = OxyColors.Blue,
                    StrokeThickness = 1.0
                };
                blueLineSeries.Points.Add(new DataPoint(0, 0));
                blueLineSeries.Points.Add(new DataPoint(BlueLineX, BlueLineY));
                var BlueArrowAnnotation = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(BlueLineX, BlueLineY),
                    Color = OxyColors.Blue,
                    HeadLength = 5,
                    HeadWidth = 3
                };
                var BlueLeadLagTextAnnotation = new TextAnnotation
                {
                    Text = BlueLeadLag,
                    TextPosition = new DataPoint(BlueLineX, BlueLineY),
                    TextColor = OxyColors.Blue,
                    StrokeThickness = 0
                };
                var BlueAngleTextAnnotation = new TextAnnotation
                {
                    Text = ((angleBlue - 240) < 0 ? (angleBlue - 240) * -1 : (angleBlue - 240)).ToString("F1") + "°",
                    TextPosition = new DataPoint(BlueLineAnglePointX, BlueLineAnglePointY),
                    TextColor = OxyColors.Blue,
                    StrokeThickness = 0
                };
                model.Annotations.Add(BlueLeadLagTextAnnotation);
                model.Series.Add(blueLineSeries);
                model.Annotations.Add(BlueAngleTextAnnotation);
                model.Annotations.Add(BlueArrowAnnotation);
            }

            //Meter Number Text
            var MeterNumberTextAnnotation = new TextAnnotation
            {
                Text = $"Meter Number = {GraphMeterNumber}",
                TextPosition = new DataPoint(-6.5, 0.3),
                TextRotation = -90,
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextColor = OxyColors.Black,
                StrokeThickness = 0,
                FontSize = 14
            };
            //axis
            if (voltage1 == 0 && voltage2 == 0 && voltage3==0)
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -7,
                    Maximum = 7,
                };

                var yAxis = new LinearAxis
                {
                    Minimum = -4,
                    Maximum = 4,
                    Position = AxisPosition.Left,
                };

                var xAxisOrigin = new LineAnnotation
                {
                    Type = LineAnnotationType.Vertical,
                    X = 0,
                    MinimumY = -7,
                    MaximumY = 7,
                    Color = OxyColors.Black,
                    LineStyle = LineStyle.Solid
                };

                var yAxisOrigin = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = 0,
                    MinimumX = -7,
                    MaximumX = 7,
                    Color = OxyColors.Black,
                    LineStyle = LineStyle.Solid
                };
                var circleAnnotation = new EllipseAnnotation
                {
                    X = 0,
                    Y = 0,
                    Width = 0.5,
                    Height = 0.8,
                    Stroke = OxyColors.Black,
                    StrokeThickness = 1,
                    Fill = OxyColors.Transparent
                };
                model.Annotations.Add(circleAnnotation);
                model.Axes.Add(xAxis);
                model.Axes.Add(yAxis);
                model.Annotations.Add(xAxisOrigin);
                model.Annotations.Add(yAxisOrigin);
            }
            else
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -7,
                    Maximum = 7,
                };

                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                };

                var xAxisOrigin = new LineAnnotation
                {
                    Type = LineAnnotationType.Vertical,
                    X = 0,
                    MinimumY = -7,
                    MaximumY = 7,
                    Color = OxyColors.Black,
                    LineStyle = LineStyle.Solid
                };

                var yAxisOrigin = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = 0,
                    MinimumX = -7,
                    MaximumX = 7,
                    Color = OxyColors.Black,
                    LineStyle = LineStyle.Solid
                };
                var circleAnnotation = new EllipseAnnotation
                {
                    X = 0,
                    Y = 0,
                    Width = 0.5,
                    Height = 0.5,
                    Stroke = OxyColors.Black,
                    StrokeThickness = 1,
                    Fill = OxyColors.Transparent
                };
                model.Annotations.Add(circleAnnotation);
                model.Axes.Add(xAxis);
                model.Axes.Add(yAxis);
                model.Annotations.Add(xAxisOrigin);
                model.Annotations.Add(yAxisOrigin);
            }

            model.Annotations.Add(MeterNumberTextAnnotation);
            plotView.Model = model;
            plotView.Controller = new PlotController();
            plotView.Controller.UnbindAll();
        }
        //end vector graph

        //export chart as image
        public void ExportImage(object sender, SelectionChangedEventArgs e)
        {
            if (IsGraph == true)
            {
                var exporter = new PngExporter { Width = 1920, Height = 800 };
                string fileName = Constants.VectorGraph + Guid.NewGuid().ToString() + ".jpg";

                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                string folderPath = Path.Combine(downloadsPath, "CrystalPowerBCSExport");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                // Choose the correct PlotView based on the visible StackPanel
                if (SinglePhaseGraph.Visibility == Visibility.Visible)
                {
                    exporter.ExportToFile(PhasorPlotView.Model, filePath);
                }
                else if (ThreePhaseGraph.Visibility == Visibility.Visible)
                {
                    exporter.ExportToFile(plotView.Model, filePath);
                }

                NotificationManager notificationManager = new NotificationManager();
                notificationManager.Show("Exported Successfully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);
            }
        }
    }
}

