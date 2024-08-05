using CrystalPowerBCS.DbFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for SelfDiagnostic.xaml
    /// </summary>
    public partial class SelfDiagnostic : UserControl
    {
        public bool IsGraph;
        public bool IsFilterEnabled;
        public List<SelfDiagnosticDto> SelfDiagnosticDto;
        public SelfDiagnosticCommand SelfDiagnosticCommand;
        public int pageSize = 10;
        public string MeterType = Constants.SinglePhaseMeter;
        public string MeterNumber = "00000000";
        public ErrorHelper _errorHelper;
        public SelfDiagnostic()
        {
            InitializeComponent();

            SelfDiagnosticGrid.Visibility = Visibility.Visible;
            _errorHelper = new ErrorHelper();
            SelfDiagnosticDto = new List<SelfDiagnosticDto>();
        }
        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
                SelfDiagnosticGrid.MaxHeight = height - 217;
            }
            catch (Exception ex)
            {
                return;
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

                await Filter(startDate, endDate, pageSize); ;
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
                await Filter(startDate, endDate, pageSize); ;
            });
        }

        private async Task Filter(string startDate, string endDate, int page)
        {
            SelfDiagnosticCommand = new SelfDiagnosticCommand();

            var gridData = await SelfDiagnosticCommand.Filter(startDate, endDate, pageSize, MeterNumber);

            SelfDiagnosticGrid.ItemsSource = gridData;
            SelfDiagnosticDto = gridData;
        }

        private async Task PopullateGrid()
        {
            SelfDiagnosticCommand = new SelfDiagnosticCommand();

            var gridData = await SelfDiagnosticCommand.GetAll(pageSize, MeterNumber);

            SelfDiagnosticGrid.ItemsSource = gridData;
            SelfDiagnosticDto = gridData;
        }

        private async Task BindMeterTypeAndNumber()
        {
            string[] meterData = MeterPhaseType.Text.Split("&&");
            MeterType = meterData[0].ToString();
            MeterNumber = meterData[1].ToString();
        }
        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });

            this.Dispatcher.Invoke(async () =>
            {
                await PopullateGrid();
            });
        }

        private void ExportData(object sender, SelectionChangedEventArgs e)
        {

            var exportType = ((System.Windows.Controls.ContentControl)cbExport.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;

                if (exportType == Constants.ExportasExcel)
                {
                    string FileName = Constants.SelfDiagnostic + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(SelfDiagnosticDto);
                    bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.SelfDiagnostic, MeterNumber);
                }
                else if (exportType == Constants.ExportasPdf)
                {
                    string FileName = Constants.SelfDiagnostic + Guid.NewGuid().ToString();
                    DataTable dt = ExportHelper.ToDataTable(SelfDiagnosticDto);
                    bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.SelfDiagnostic, MeterNumber);
                }
          
            cbExport.SelectedIndex = 0;
        }
    }
}