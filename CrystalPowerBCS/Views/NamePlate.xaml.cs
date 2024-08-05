using CrystalPowerBCS.DbFunctions;
using CrystalPowerBCS.Helpers;
using Domain.Entities;
using Domain.Entities.SinglePhaseEntities;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for NamePlate.xaml
    /// </summary>
    public partial class NamePlate : UserControl
    {
        public string MeterType;
        public string MeterNumber = "";

        public MeterCommand MeterCommand;
        public MeterDto meterData;
        public NamePlate()
        {
            InitializeComponent();

            MeterCommand = new MeterCommand();
        }

        private async void BindMeterTypeAndNumber(object sender, TextChangedEventArgs e)
        {
            await this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });

            await BindNamePlateDetails();
        }

        private async Task BindNamePlateDetails()
        {
            meterData = await MeterCommand.GetMeterByMeterNo(MeterNumber);
            if (meterData != null)
            {
                MeterNoValue.Text = "Meter No : " + meterData.MeterNumber;
                DeviceIdValue.Text = "Device Id : " + meterData.DeviceId;
                ManufactureNameValue.Text = "Manufacture Name : " + meterData.ManufacturerName;
                FirmwareVersionValue.Text = "Firmware Version : " + meterData.FirmwareVersion;
                MeterTypePhaseValue.Text = "Meter Type : " + meterData.MeterType.ToString();
                CategoryValue.Text = "Category : " + meterData.Category;
                CurrentRatingValue.Text = "Current Rating : " + meterData.CurrentRating;
                CTRatioValue.Text = "CT Ratio : " + meterData.CTRatio;
                PTRatioValue.Text = "PT Ratio : " + meterData.PTRatio;
            }
        }

        private async Task BindMeterTypeAndNumber()
        {
            if (!string.IsNullOrEmpty(MeterPhaseType.Text))
            {
                string[] meterData = MeterPhaseType.Text.Split("&&");
                MeterType = meterData[0].ToString();
                MeterNumber = meterData[1].ToString();
            }
        }

        private void OnResize(object sender, TextChangedEventArgs e)
        {
            try
            {
                double height = Convert.ToDouble(CurrentHeight.Text.ToString());
                Grid_Parent.Height = height - 217;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void ExportData(object sender, SelectionChangedEventArgs e)
        {
            List<MeterDto> NamePlate = new List<MeterDto>();
            NamePlate.Add(meterData);

            var exportType = ((System.Windows.Controls.ContentControl)cbExport.SelectedValue).Content as string;
            if (exportType == null || exportType == Constants.Export)
                return;
            
            if (exportType == Constants.ExportasExcel)
            {
                string FileName = Constants.NamePlate + Guid.NewGuid().ToString();
                DataTable dt = ExportHelper.ToDataTable(NamePlate);
                bool res = ExportHelper.GenerateExcel(dt, FileName, Constants.NamePlate, MeterNumber);
            }
            else if (exportType == Constants.ExportasPdf)
            {
                string FileName = Constants.NamePlate + Guid.NewGuid().ToString();
                DataTable dt = ExportHelper.ToDataTable(NamePlate);
                bool res = ExportHelper.GeneratePdf(dt, FileName, Constants.NamePlate, MeterNumber);
            }
        }
    }
}
