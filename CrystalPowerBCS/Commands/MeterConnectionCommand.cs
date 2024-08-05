using CrystalPowerBCS.Helpers;
using CrystalPowerBCS.ViewModels;
using Gurux.DLMS.Enums;
using Gurux.Serial;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace CrystalPowerBCS.Commands
{
    public class MeterConnectionCommand
    {
        bool IsWrapper = false;
        MeterConnectionViewModel connectionModel = new MeterConnectionViewModel();
        public Tester t;
        GXSerial serial = new GXSerial();
        InterfaceType InterfaceTypeisWrapper = InterfaceType.HDLC;

        public MeterConnectionCommand()
        {
            t = new Tester(serial, InterfaceTypeisWrapper, Convert.ToInt32(connectionModel.DeviceId));
        }

        public async Task<MeterConnectionViewModel> ConnectMeter(string port)
        {
            try
            {
                if (!string.IsNullOrEmpty(port))
                {
                    connectionModel.Port = port;
                    await OpenPort_Click();
                    if (connectionModel.SerialConnected == 1)
                    {
                        try
                        {
                            await Interface_update();
                        }
                        catch(Exception)
                        {
                            NotificationManager notificationManager = new NotificationManager();

                            notificationManager.Show(Constants.Notification, Constants.Probleminopeningserialport, NotificationType.Error, CloseOnClick: true);

                            connectionModel.SerialConnected = 0;

                            throw;
                        }
                    } 
                    connectionModel.IsConnected = true;
                    return connectionModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<List<string>> GetComPorts()
        {
            try
            {
                var portList = await RefreshPorts_Click();
                return portList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MeterConnectionViewModel> DisconnectMeter()
        {
            try
            {
                if(connectionModel.SerialConnected == 1)
                {
                    await ClosePort_Click();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return connectionModel;
        }

        private async Task<List<string>> RefreshPorts_Click()
        {
            try
            {
                List<string> PortsList = new List<string>();
                foreach (string str in SerialPort.GetPortNames())
                {
                    try
                    {
                        PortsList.Add(str);
                    }
                    catch (Exception)
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.UnabletoOpenPort, NotificationType.Error, CloseOnClick: true);
                        throw;
                    }
                }
                return PortsList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async System.Threading.Tasks.Task OpenPort_Click()
        {
            if (connectionModel.SerialConnected == 0)
            {
                if (!string.IsNullOrEmpty(connectionModel.Port))
                {
                    connectionModel.SerialConnected = 1;
                }
                else
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, Constants.UnabletoOpenPort, NotificationType.Error, CloseOnClick: true);
                }
            }
            return;
        }

        private async System.Threading.Tasks.Task ClosePort_Click()
        {
            try
            {
                if (connectionModel.IsInit == true)
                    t.HDLC_Disconnect();
                t.MediaClose();
                connectionModel.IsInit = false;

                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        private async System.Threading.Tasks.Task Interface_update()
        {
            try
            {
                if (IsWrapper == true)
                    connectionModel.InterfaceTypeisWrapper = InterfaceType.WRAPPER;
                else
                    connectionModel.InterfaceTypeisWrapper = InterfaceType.HDLC;
                t.update_interface(connectionModel.InterfaceTypeisWrapper, Convert.ToUInt16(connectionModel.DeviceId));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
