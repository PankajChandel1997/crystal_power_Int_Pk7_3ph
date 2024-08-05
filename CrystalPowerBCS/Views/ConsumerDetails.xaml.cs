using CrystalPowerBCS.DbFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for ConsumerDetails.xaml
    /// </summary>
    public partial class ConsumerDetails : UserControl
    {
        public string MeterType;
        public string MeterNumber = "";

        public MeterCommand MeterCommand;
        public MeterDto meterData;
        public ConsumerDetails()
        {
            InitializeComponent();

            MeterCommand = new MeterCommand();
        }

        private async Task BindCustomerDetails()
        {
            meterData = await MeterCommand.GetMeterByMeterNo(MeterNumber);
            if(meterData != null)
            {
                ConsumerNo.Text = meterData.ConsumerNo;
                ConsumerName.Text = meterData.ConsumerName;
                ConsumerAddress.Text = meterData.ConsumerAddress;
            }
        }

        private async void BindMeterTypeAndNumber(object sender, TextChangedEventArgs e)
        {
            await this.Dispatcher.Invoke(async () =>
            {
                await BindMeterTypeAndNumber();
            });

            await BindCustomerDetails();
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

        private async void ConsumerUpdate(object sender, RoutedEventArgs e)
        {
            meterData.ConsumerNo = ConsumerNo.Text;
            meterData.ConsumerName = ConsumerName.Text;
            meterData.ConsumerAddress = ConsumerAddress.Text;

            bool res = await MeterCommand.UpdateMeterByMeterNo(meterData);
            if (res)
            {
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, Constants.consumerdetailsupdatedsuccessfully , NotificationType.Success, CloseOnClick: true);
            }
            else
            {
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, Constants.unabletoupdateconsumerdetailspleasetryagain, NotificationType.Error, CloseOnClick: true);
            }
        }

        private void ConsumerCancel(object sender, RoutedEventArgs e)
        {
            ConsumerNo.Text = meterData.ConsumerNo;
            ConsumerName.Text = meterData.ConsumerName;
            ConsumerAddress.Text = meterData.ConsumerAddress;
        }
    }
}
