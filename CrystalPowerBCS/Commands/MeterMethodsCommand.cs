using CrystalPowerBCS.Helpers;
using CrystalPowerBCS.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities;
using Domain.Entities.SinglePhaseEntities;
using Domain.Entities.ThreePhaseCTEntities;
using Domain.Entities.ThreePhaseEntities;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Serial;
using Infrastructure.API;
using Infrastructure.API.EventAPIs;
using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.API.EventAPIThreePhaseCT;
using Infrastructure.DTOs;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.API.EventAPIs.EventAPIsSinglePhase;

namespace CrystalPowerBCS.Commands
{
    public class MeterMethodsCommand
    {
        private readonly InstantaneousProfileSinglePhaseService _instantaneousProfileSinglePhaseService;
        private readonly InstantaneousProfileThreePhaseService _instantaneousProfileThreePhaseService;
        private readonly InstantaneousProfileThreePhaseCTService _instantaneousProfileThreePhaseCTService;

        private readonly BlockLoadProfileSinglePhaseService _blockLoadProfileSinglePhaseService;
        private readonly BlockLoadProfileThreePhaseService _blockLoadProfileThreePhaseService;
        private readonly BlockLoadProfileThreePhaseCTService _blockLoadProfileThreePhaseCTService;

        private readonly BillingProfileSinglePhaseService _billingProfileSinglePhaseService;
        private readonly BillingProfileThreePhaseService _billingProfileThreePhaseService;
        private readonly BillingProfileThreePhaseCTService _billingProfileThreePhaseCTService;

        private readonly DailyLoadProfileSinglePhaseService _dailyLoadSinglePhaseService;
        private readonly DailyLoadProfileThreePhaseService _dailyLoadThreePhaseService;
        private readonly DailyLoadProfileThreePhaseCTService _dailyLoadThreePhaseCTService;

        private readonly ControlEventService _controlEventService;
        private readonly ControlEventSinglePhaseService _controlEventSinglePhaseService;

        private readonly NonRolloverEventService _nonRolloverEventService;
        private readonly NonRolloverEventSinglePhaseService _nonRolloverEventSinglePhaseService;

        private readonly OtherEventService _otherEventService;
        private readonly OtherEventSinglePhaseService _otherEventSinglePhaseService;

        private readonly TransactionEventService _transactionEventService;
        private readonly TransactionEventSinglePhaseService _transactionEventSinglePhaseService;

        private readonly DIEventService _dIEventService;
        private readonly DIEventSinglePhaseService _dIEventSinglePhaseService;

        private readonly PowerRelatedEventService _powerRelatedEventService;
        private readonly PowerRelatedEventSinglePhaseService _powerRelatedEventSinglePhaseService;

        private readonly CurrentRelatedEventService _currentRelatedEventService;
        private readonly CurrentRelatedEventSinglePhaseService _currentRelatedEventSinglePhaseService;

        private readonly VoltageRelatedEventService _voltageRelatedEventService;
        private readonly VoltageRelatedEventSinglePhaseService _voltageRelatedEventSinglePhaseService;

        private readonly SelfDiagnosticService _selfDiagnosticService;

        private readonly MeterService _meterService;

        private readonly ESWSinglePhaseService _eswSinglePhaseService;
        private readonly ESWThreePhaseService _eswThreePhaseSerice;

        public MeterDto meterDetails;
        public ErrorHelper _errorHelper;
        public DateValidatorHelper _validateDateHelper;
        public ConsumerDetailsDto ConsumerDetailsDto;

        public bool IsInit = false;
        public GXDLMSSettings Settings { get; set; }
        public Tester t;
        public MeterConnectionViewModel ConnectionViewModel;
        GXSerial serial;
        InterfaceType InterfaceTypeisWrapper = InterfaceType.HDLC;
        GXDLMSSecureClient secureClient;
        int logicalname = Convert.ToInt32(InterfaceType.HDLC);

        private System.Timers.Timer keepAliveTimer;
        public MeterMethodsCommand(MeterConnectionViewModel connectionViewModel)
        {
            _instantaneousProfileSinglePhaseService = new InstantaneousProfileSinglePhaseService();
            _instantaneousProfileThreePhaseService = new InstantaneousProfileThreePhaseService();
            _instantaneousProfileThreePhaseCTService = new InstantaneousProfileThreePhaseCTService();

            _blockLoadProfileSinglePhaseService = new BlockLoadProfileSinglePhaseService();
            _blockLoadProfileThreePhaseService = new BlockLoadProfileThreePhaseService();
            _blockLoadProfileThreePhaseCTService = new BlockLoadProfileThreePhaseCTService();

            _billingProfileSinglePhaseService = new BillingProfileSinglePhaseService();
            _billingProfileThreePhaseService = new BillingProfileThreePhaseService();
            _billingProfileThreePhaseCTService = new BillingProfileThreePhaseCTService();

            _dailyLoadSinglePhaseService = new DailyLoadProfileSinglePhaseService();
            _dailyLoadThreePhaseService = new DailyLoadProfileThreePhaseService();
            _dailyLoadThreePhaseCTService = new DailyLoadProfileThreePhaseCTService();


            _controlEventService = new ControlEventService();
            _nonRolloverEventService = new NonRolloverEventService();
            _otherEventService = new OtherEventService();
            _transactionEventService = new TransactionEventService();
            _powerRelatedEventService = new PowerRelatedEventService();
            _currentRelatedEventService = new CurrentRelatedEventService();
            _voltageRelatedEventService = new VoltageRelatedEventService();
            _voltageRelatedEventSinglePhaseService = new VoltageRelatedEventSinglePhaseService();
            _dIEventService = new DIEventService();
            _selfDiagnosticService = new SelfDiagnosticService();

            _controlEventSinglePhaseService = new ControlEventSinglePhaseService();
            _nonRolloverEventSinglePhaseService = new NonRolloverEventSinglePhaseService();
            _transactionEventSinglePhaseService = new TransactionEventSinglePhaseService();
            _powerRelatedEventSinglePhaseService = new PowerRelatedEventSinglePhaseService();
            _currentRelatedEventSinglePhaseService = new CurrentRelatedEventSinglePhaseService();
            _otherEventSinglePhaseService = new OtherEventSinglePhaseService();
            _dIEventSinglePhaseService = new DIEventSinglePhaseService();

            _meterService = new MeterService();

            _eswSinglePhaseService = new ESWSinglePhaseService();
            _eswThreePhaseSerice = new ESWThreePhaseService();
            meterDetails = new MeterDto();

            _errorHelper = new ErrorHelper();
            _validateDateHelper = new DateValidatorHelper();

            ConnectionViewModel = connectionViewModel;
            serial = new GXSerial(connectionViewModel.Port);
            t = new Tester(serial, InterfaceTypeisWrapper, Convert.ToInt32(ConnectionViewModel.DeviceId));

            secureClient = new GXDLMSSecureClient(true, 0x30, GXDLMSClient.GetServerAddress(logicalname, ConnectionViewModel.DeviceId), Authentication.High, MeterKeys.Active_Password, InterfaceTypeisWrapper, Security.AuthenticationEncryption, ASCIIEncoding.ASCII.GetBytes("GRX12345"), ASCIIEncoding.ASCII.GetBytes(MeterKeys.Active_Encryption), ASCIIEncoding.ASCII.GetBytes(MeterKeys.Active_Authentication));
        }

        public async Task<string> GetMeterTime(ConsumerDetailsDto consumerDetails)
        {
            ConsumerDetailsDto = consumerDetails;
            try
            {
                string MeterTime;
                if (ConnectionViewModel.SerialConnected == 1)
                {
                    byte[] time = null;
                    ushort year = 0, tempyear = 0;
                    ushort coverOpen = 0;

                    t.MediaOpen();
                    t.Initialize();
                    IsInit = true;
                    time = t.ReadClock2();
                    coverOpen = t.coveropen();
                    t.HDLC_Disconnect();
                    t.MediaClose();
                    if (time != null)
                    {
                        MeterTime = time[5].ToString("D2");
                        MeterTime += ":";
                        MeterTime += time[6].ToString("D2");
                        MeterTime += ":";
                        MeterTime += time[7].ToString("D2");
                        MeterTime += " ";

                        MeterTime += time[3].ToString("D2");
                        MeterTime += "/";
                        MeterTime += time[2].ToString("D2");
                        MeterTime += "/";
                        ushort.TryParse(time[0].ToString(), out tempyear);
                        ushort.TryParse(time[1].ToString(), out year);
                        tempyear = (ushort)(tempyear * 256);
                        year = (ushort)(tempyear + year);
                        MeterTime += year.ToString("D4");

                        return MeterTime;
                    }
                }
                else
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, Constants.Probleminopeningserialport, NotificationType.Error, CloseOnClick: true);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Access to the path") && ex.Message.Contains("is denied"))
                { 
                    NotificationManager notificationManager = new NotificationManager();
                    notificationManager.Show(Constants.Notification, Constants.AccessToPortDenied, NotificationType.Error, CloseOnClick: true);
                }
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : Get Meter Time : Exception ==>" + ex.Message);
                throw;
            }
            finally
            {
                t.MediaClose();
            }
            return null;
        }

        public string GetSerialNum_Click(ConsumerDetailsDto consumerDetails)
        {
            ConsumerDetailsDto = consumerDetails;
            string SerialNo = null;
            if (ConnectionViewModel.SerialConnected == 1)
            {
                t.MediaOpen();
                t.Initialize();
                IsInit = true;
                SerialNo = t.GetSerialNo();
                t.HDLC_Disconnect();
                t.MediaClose();
            }
            else
            {
            }

            return SerialNo;
        }

        public async Task<bool> SetBillingDate(DateTime dt)
        {
            try
            {
                var billingDate = await System.Threading.Tasks.Task.Run(() => t.SetBillingDate(dt));

                return true;
            }
            catch(Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : SetBillingDate : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<bool> MDResetSet()
        {
            try
            {
                return await System.Threading.Tasks.Task.Run(() => t.SendMDResetCommand());
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : MDResetSet : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async System.Threading.Tasks.Task SendConnectCommand()
        {
            try
            {
                if (meterDetails == null || meterDetails.MeterType == Domain.Enums.MeterType.NotAvaliabile)
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        meterDetails = await System.Threading.Tasks.Task.Run(() => ReadMeterLoadDetails());
                    }, System.Windows.Threading.DispatcherPriority.Send);
                }

                if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT)
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, "Access Denied For CT Meters", NotificationType.Error, CloseOnClick: true);
                }
                else
                {
                    bool res = await System.Threading.Tasks.Task.Run(() => t.SendConnectCommand());
                    if (res)
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, "Command Executed SuccessFully", NotificationType.Information, CloseOnClick: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : SendConnectCommand : Exception ==>" + ex.Message);
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, "Something went wrong please try again", NotificationType.Information, CloseOnClick: true);
            }
        }

        public async System.Threading.Tasks.Task SendDisconnectCommand()
        {
            try
            {
                if (meterDetails == null || meterDetails.MeterType == Domain.Enums.MeterType.NotAvaliabile)
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        meterDetails = await System.Threading.Tasks.Task.Run(() => ReadMeterLoadDetails());
                    }, System.Windows.Threading.DispatcherPriority.Send);
                }

                if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT)
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, "Access Denied For CT Meters", NotificationType.Error, CloseOnClick: true);
                }
                else
                {
                    bool res = await System.Threading.Tasks.Task.Run(() => t.SendDisconnectCommand());
                    if (res)
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, "Command Executed SuccessFully", NotificationType.Information, CloseOnClick: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : SendDisconnectCommand : Exception ==>" + ex.Message);

                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, "Something went wrong please try again", NotificationType.Information, CloseOnClick: true);
            }
        }

        public async Task<bool> ReadAllMeterData()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();
                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    var resIP = await System.Threading.Tasks.Task.Run(() => ReadIPSinglePhase());
                    var resBlock = await System.Threading.Tasks.Task.Run(() => ReadBlockLoadProfileSinglePhase(null, null));
                    var resBilling = await System.Threading.Tasks.Task.Run(() => ReadBillingProfileSinglePhase(null, null));
                    var resDaily = await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileSinglePhase(null, null));
                    var resEvent = await System.Threading.Tasks.Task.Run(() => ReadEventOnly(null, null));
                    var resSelf = await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resIP ? Environment.NewLine + Constants.IPDownloadedSuccessFully : Environment.NewLine + Constants.IPDownloadedFailed;
                        Message = resBilling ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = resBlock ? Message + Environment.NewLine + Constants.BlockLoadProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BlockLoadProfileDownloadFailed;
                        Message = resDaily ? Message + Environment.NewLine + Constants.DailyProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.DailyProfileDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    var resIP = await System.Threading.Tasks.Task.Run(() => ReadIPThreePhase());
                    var resBlock = await System.Threading.Tasks.Task.Run(() => ReadBlockLoadProfileThreePhase(null, null));
                    var resBilling = await System.Threading.Tasks.Task.Run(() => ReadBillingProfileThreePhase(null, null));
                    var resDaily = await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileThreePhase(null, null));
                    var resEvent = await System.Threading.Tasks.Task.Run(() => ReadEventOnly(null, null));
                    var resSelf = await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resIP ? Environment.NewLine + Constants.IPDownloadedSuccessFully : Environment.NewLine + Constants.IPDownloadedFailed;
                        Message = resBilling ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = resBlock ? Message + Environment.NewLine + Constants.BlockLoadProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BlockLoadProfileDownloadFailed;
                        Message = resDaily ? Message + Environment.NewLine + Constants.DailyProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.DailyProfileDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    var resIP = await System.Threading.Tasks.Task.Run(() => ReadIPThreePhaseCT());
                    var resBlock = await System.Threading.Tasks.Task.Run(() => ReadBlockLoadProfileThreePhaseCT(null, null));
                    var resBilling = await System.Threading.Tasks.Task.Run(() => ReadBillingProfileThreePhaseCT(null, null));
                    var resDaily = await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileThreePhaseCT(null, null));
                    var resEvent = await System.Threading.Tasks.Task.Run(() => ReadEventOnly(null, null));
                    var resSelf = await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resIP ? Environment.NewLine + Constants.IPDownloadedSuccessFully : Environment.NewLine + Constants.IPDownloadedFailed;
                        Message = resBilling ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = resBlock ? Message + Environment.NewLine + Constants.BlockLoadProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BlockLoadProfileDownloadFailed;
                        Message = resDaily ? Message + Environment.NewLine + Constants.DailyProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.DailyProfileDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }

                else
                {
                    NotificationManager notificationManager = new NotificationManager();

                    notificationManager.Show(Constants.Notification, Constants.SomethingWentWrongUnabletoreadMeterData, NotificationType.Error, CloseOnClick: true);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : Read All Meter Data : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async System.Threading.Tasks.Task OpenConnection()
        {
            try
            {
                await System.Threading.Tasks.Task.Run(() => t.OpenConnection());
                StartKeepAliveTimer();
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
            }
        }

        public async System.Threading.Tasks.Task CloseConnection()
        {
            try
            {
                await System.Threading.Tasks.Task.Run(() => t.CloseConnection());
                //StopKeepAliveTimer();
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
            }
        }

        public async System.Threading.Tasks.Task GetMeterDetails()
        {
            try
            {
                if (meterDetails == null)
                {
                    meterDetails = new MeterDto();
                    meterDetails.MeterType = Domain.Enums.MeterType.NotAvaliabile;
                }

                if (meterDetails.MeterType == Domain.Enums.MeterType.NotAvaliabile)
                {
                    StopKeepAliveTimer();
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        meterDetails = await System.Threading.Tasks.Task.Run(() => ReadMeterLoadDetails());
                    }, System.Windows.Threading.DispatcherPriority.Send);
                    StartKeepAliveTimer();
                }

                return;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : Read Meter Details : Exception ==>" + ex.Message);
            }
        }

        public async Task<bool> ReadIPOnly()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadIPSinglePhase());
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadIPThreePhase());
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadIPThreePhaseCT());
                }
                else if(meterDetails.MeterType == Domain.Enums.MeterType.NotAvaliabile)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadIPOnly : Exception ==>" + ex.Message);

                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, "Something Went Wrong ex=====> " + ex.Message, NotificationType.Error, CloseOnClick: true);
                return false;
            }
        }

        public async Task<bool> ReadBillingProfileOnly(DateTime? startDate, DateTime? endDate)
        {
            try
            {

                await System.Threading.Tasks.Task.Run(() => GetMeterDetails());

                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadBillingProfileSinglePhase(startDate, endDate));
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadBillingProfileThreePhase(startDate, endDate));
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadBillingProfileThreePhaseCT(startDate, endDate));
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadBillingProfileOnly : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<bool> ReadBlockLoadProfileOnly(DateTime? startDate, DateTime? endDate)
        {
            try
            {


                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadBlockLoadProfileSinglePhase(startDate, endDate));
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadBlockLoadProfileThreePhase(startDate, endDate));
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadBlockLoadProfileThreePhaseCT(null, null));
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadBlockLoadProfileOnly : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<bool> ReadDailyLoadProfile(DateTime? startDate, DateTime? endDate)
        {
            try
            {

                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileSinglePhase(startDate, endDate));
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileThreePhase(startDate, endDate));
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileThreePhaseCT(startDate, endDate));
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDailyLoadProfile : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<bool> ReadSelfDiagnosticOnly()
        {
            try
            {

                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDailyLoadProfile : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<bool> ReadNamePlateOnly()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadNamePlate : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<bool> ReadAllWithoutLoadProfile()
        {
            try
            {

                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    bool resIP = await System.Threading.Tasks.Task.Run(() => ReadIPSinglePhase());
                    bool resBilling = await System.Threading.Tasks.Task.Run(() => ReadBillingProfileSinglePhase(null, null));
                    bool eventData = await System.Threading.Tasks.Task.Run(() => ReadEventOnly(null, null));
                    var resDaily = await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileSinglePhase(null, null));
                    var resSelf = await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resIP ? Environment.NewLine + Constants.IPDownloadedSuccessFully : Environment.NewLine + Constants.IPDownloadedFailed;
                        Message = resBilling ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = eventData ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = resDaily ? Message + Environment.NewLine + Constants.DailyProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.DailyProfileDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    bool resIP = await System.Threading.Tasks.Task.Run(() => ReadIPThreePhase());
                    bool resBilling = await System.Threading.Tasks.Task.Run(() => ReadBillingProfileThreePhase(null, null));
                    bool eventData = await System.Threading.Tasks.Task.Run(() => ReadEventOnly(null, null));
                    var resDaily = await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileThreePhase(null, null));
                    var resSelf = await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resIP ? Environment.NewLine + Constants.IPDownloadedSuccessFully : Environment.NewLine + Constants.IPDownloadedFailed;
                        Message = resBilling ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = eventData ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = resDaily ? Message + Environment.NewLine + Constants.DailyProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.DailyProfileDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    bool resIP = await System.Threading.Tasks.Task.Run(() => ReadIPThreePhaseCT());
                    bool resBilling = await System.Threading.Tasks.Task.Run(() => ReadBillingProfileThreePhaseCT(null, null));
                    bool eventData = await System.Threading.Tasks.Task.Run(() => ReadEventOnly(null, null));
                    var resDaily = await System.Threading.Tasks.Task.Run(() => ReadDailyLoadProfileThreePhaseCT(null, null));
                    var resSelf = await System.Threading.Tasks.Task.Run(() => ReadSelfDiagnostic());

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resIP ? Environment.NewLine + Constants.IPDownloadedSuccessFully : Environment.NewLine + Constants.IPDownloadedFailed;
                        Message = resBilling ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = eventData ? Message + Environment.NewLine + Constants.BillingProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.BillingProfileDownloadFailed;
                        Message = resDaily ? Message + Environment.NewLine + Constants.DailyProfileDownloadedSuccessFully : Message + Environment.NewLine + Constants.DailyProfileDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadAllWithoutLoadProfile : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<bool> ReadEventOnly(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                System.Threading.Tasks.Task.Run(() => GetMeterDetails()).Wait();

                //Single Phase
                if (meterDetails.MeterType == Domain.Enums.MeterType.SinglePhase)
                {
                    bool resControlEvent = await System.Threading.Tasks.Task.Run(() => ReadControlEventSingle(startDate, endDate));
                    bool resNonRollOverEvent = await System.Threading.Tasks.Task.Run(() => ReadNonRolloverEventSingle(startDate, endDate));
                    bool resOtherEvent = await System.Threading.Tasks.Task.Run(() => ReadOtherEventSingle(startDate, endDate));
                    bool resTransactionEvent = await System.Threading.Tasks.Task.Run(() => ReadTransactionEventSingle(startDate, endDate));
                    bool resPowerRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadPowerRelatedEventSingle(startDate, endDate));
                    bool resCurrentReatedEvent = await System.Threading.Tasks.Task.Run(() => ReadCurrentRelatedEventSingle(startDate, endDate));
                    bool resVoltageRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadVoltageRelatedEventSingle(startDate, endDate));
                    bool dIRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadDIEventSingle(startDate, endDate));
                    keepAliveTimer.Start();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resControlEvent ? Environment.NewLine + Constants.ControlEventDownloadedSuccessFully : Environment.NewLine + Constants.ControlEventDownloadedFailed;
                        Message = resNonRollOverEvent ? Message + Environment.NewLine + Constants.NonRollOverEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.NonRollOverEventDownloadFailed;
                        Message = resOtherEvent ? Message + Environment.NewLine + Constants.OthersEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.OthersEventDownloadFailed;
                        Message = resTransactionEvent ? Message + Environment.NewLine + Constants.TransactionEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.TransactionEventDownloadFailed;
                        Message = resPowerRelatedEvent ? Message + Environment.NewLine + Constants.PowerRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.PowerRelatedEventDownloadFailed;
                        Message = resCurrentReatedEvent ? Message + Environment.NewLine + Constants.CurrentRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.CurrentRelatedEventDownloadFailed;
                        Message = resVoltageRelatedEvent ? Message + Environment.NewLine + Constants.VoltageRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.VoltageRelatedEventDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                }
                //Three Phase
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhase)
                {
                    bool resControlEvent = await System.Threading.Tasks.Task.Run(() => ReadControlEventThree(startDate, endDate));
                    bool resNonRollOverEvent = await System.Threading.Tasks.Task.Run(() => ReadNonRolloverEventThree(startDate, endDate));
                    bool resOtherEvent = await System.Threading.Tasks.Task.Run(() => ReadOtherEventThree(startDate, endDate));
                    bool resTransactionEvent = await System.Threading.Tasks.Task.Run(() => ReadTransactionEventThree(startDate, endDate));
                    bool resPowerRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadPowerRelatedEventThree(startDate, endDate));
                    bool resCurrentReatedEvent = await System.Threading.Tasks.Task.Run(() => ReadCurrentRelatedEventThree(startDate, endDate));
                    bool resVoltageRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadVoltageRelatedEventThree(startDate, endDate));
                    bool dIRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadDIThree(startDate, endDate));
                    keepAliveTimer.Start();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resControlEvent ? Environment.NewLine + Constants.ControlEventDownloadedSuccessFully : Environment.NewLine + Constants.ControlEventDownloadedFailed;
                        Message = resNonRollOverEvent ? Message + Environment.NewLine + Constants.NonRollOverEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.NonRollOverEventDownloadFailed;
                        Message = resOtherEvent ? Message + Environment.NewLine + Constants.OthersEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.OthersEventDownloadFailed;
                        Message = resTransactionEvent ? Message + Environment.NewLine + Constants.TransactionEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.TransactionEventDownloadFailed;
                        Message = resPowerRelatedEvent ? Message + Environment.NewLine + Constants.PowerRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.PowerRelatedEventDownloadFailed;
                        Message = resCurrentReatedEvent ? Message + Environment.NewLine + Constants.CurrentRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.CurrentRelatedEventDownloadFailed;
                        Message = resVoltageRelatedEvent ? Message + Environment.NewLine + Constants.VoltageRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.VoltageRelatedEventDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }

                //Three Phase LTCT HTCT
                else if (meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseLTCT || meterDetails.MeterType == Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    bool resNonRollOverEvent = await System.Threading.Tasks.Task.Run(() => ReadNonRolloverEventThree(startDate, endDate));
                    bool resOtherEvent = await System.Threading.Tasks.Task.Run(() => ReadOtherEventThree(startDate, endDate));
                    bool resTransactionEvent = await System.Threading.Tasks.Task.Run(() => ReadTransactionEventThree(startDate, endDate));
                    bool resPowerRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadPowerRelatedEventThree(startDate, endDate));
                    bool resCurrentReatedEvent = await System.Threading.Tasks.Task.Run(() => ReadCurrentRelatedEventThree(startDate, endDate));
                    bool resVoltageRelatedEvent = await System.Threading.Tasks.Task.Run(() => ReadVoltageRelatedEventThree(startDate, endDate));
                    bool resDIEvent = await System.Threading.Tasks.Task.Run(() => ReadDIThree(startDate, endDate));
                    keepAliveTimer.Start();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Message = resNonRollOverEvent ? Environment.NewLine + Constants.NonRollOverEventDownloadedSuccessFully : Environment.NewLine + Constants.NonRollOverEventDownloadFailed;
                        Message = resOtherEvent ? Message + Environment.NewLine + Constants.OthersEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.OthersEventDownloadFailed;
                        Message = resTransactionEvent ? Message + Environment.NewLine + Constants.TransactionEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.TransactionEventDownloadFailed;
                        Message = resPowerRelatedEvent ? Message + Environment.NewLine + Constants.PowerRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.PowerRelatedEventDownloadFailed;
                        Message = resCurrentReatedEvent ? Message + Environment.NewLine + Constants.CurrentRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.CurrentRelatedEventDownloadFailed;
                        Message = resVoltageRelatedEvent ? Message + Environment.NewLine + Constants.VoltageRelatedEventDownloadedSuccessFully : Message + Environment.NewLine + Constants.VoltageRelatedEventDownloadFailed;

                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Message, NotificationType.Information, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadEventOnly : Exception ==>" + ex.Message);

                return false;
            }
        }



        #region Add To Meter Log
        public async Task<bool> AddMeterFetchDataLog(string MeterNo)
        {
            try
            {
                MeterFetchDataLogDto meterFetchDataLogDto = new MeterFetchDataLogDto();

                meterFetchDataLogDto.MeterNo = meterDetails.MeterNumber;

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : AddMeterFetchDataLog : Exception ==>" + ex.Message);

                return false;
            }
        }
        #endregion

        #region Get Meter Phase Details

        public async Task<MeterDto> ReadMeterLoadDetails()
        {
            try
            {
                //StopKeepAliveTimer();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetNamePlate, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip);
                //StartKeepAliveTimer();
                if (res != null && res.Buffer.Count > 0)
                {
                    MeterDto meter = await ConvertToMeterModel(res);
                    if (meter != null)
                    {
                        if (meter.MeterType.ToString().Contains(Constants.ThreePhaseMeter))
                        {
                            //StopKeepAliveTimer();
                            GXDLMSData firmwareCheck = t.GetProfileFirmWareVersionTest("1.0.128.2.0.255");
                            if (firmwareCheck != null)
                            {
                                _errorHelper.WriteLog(DateTime.UtcNow + " : firmwareCheck Commands : firmwareCheck : Version ==>" + firmwareCheck.Value.ToString());
                                meter.ManSpecificFirmwareVersion = firmwareCheck.Value.ToString();
                            }
                            else
                            {
                                _errorHelper.WriteLog(DateTime.UtcNow + " : firmwareCheck Commands : Not Found ");
                            }
                            List<MeterDto> meterDtos = new List<MeterDto>();
                            meterDtos.Add(meter);

                            await _meterService.Add(meterDtos);
                        }
                        else
                        {
                            List<MeterDto> meterDtos = new List<MeterDto>();
                            meterDtos.Add(meter);

                            await _meterService.Add(meterDtos);
                        }
                    }
                    return meter;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<MeterDto> ConvertToMeterModel(GXDLMSProfileGeneric parsedDataList)
        {
            try
            {
                MeterDto meter = new MeterDto();

                for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                {
                    string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                    switch (key)
                    {

                        case OBISConstants.MeterNumber:
                            meter.MeterNumber = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.DeviceId:
                            meter.DeviceId = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.ManufacturerName:
                            meter.ManufacturerName = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.FirmwareVersion:
                            meter.FirmwareVersion = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.MeterType:
                            meter.MeterType = (Domain.Enums.MeterType)int.Parse(parsedDataList.Buffer[0][i].ToString());
                            break;

                        case OBISConstants.Category:
                            meter.Category = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.CurrentRating:
                            meter.CurrentRating = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.MeterYearManufacturer:
                            meter.MeterYearManufacturer = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            break;

                        case OBISConstants.CTRatio:
                            meter.CTRatio = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            if (string.IsNullOrEmpty(meter.CTRatio))
                                meter.CTRatio = "1";
                            break;

                        case OBISConstants.PTRatio:
                            meter.PTRatio = parsedDataList.Buffer[0][i].GetType().IsArray ? Encoding.UTF8.GetString((byte[])parsedDataList.Buffer[0][i]) : parsedDataList.Buffer[0][i].ToString();
                            if (string.IsNullOrEmpty(meter.PTRatio))
                                meter.PTRatio = "1";
                            break;
                    }

                }

                meter.ConsumerNo = ConsumerDetailsDto.ConsumerNo;
                meter.ConsumerAddress = ConsumerDetailsDto.ConsumerAddress;
                meter.ConsumerName = ConsumerDetailsDto.ConsumerName;

                return meter;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToMeterModel : Exception ==>" + ex.Message);

            }
            return null;
        }

        #endregion

        #region ip Methods

        public async Task<bool> ReadIPSinglePhase()
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetInstantProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<InstantaneousProfileSinglePhase> instantaneousProfile = await ConvertToIPSinglePhase(res);
                    if (instantaneousProfile != null)
                    {
                        await _instantaneousProfileSinglePhaseService.Add(instantaneousProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.IPDownloadedSuccessfullyCount + instantaneousProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.IPDownloadedFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadIPSinglePhase : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<InstantaneousProfileSinglePhase>> ConvertToIPSinglePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<InstantaneousProfileSinglePhase> instantaneousProfileSinglePhaseList = new List<InstantaneousProfileSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    InstantaneousProfileSinglePhase instantaneousProfileSinglePhase = new InstantaneousProfileSinglePhase();

                    instantaneousProfileSinglePhase.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                instantaneousProfileSinglePhase.Realtimeclock = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.Voltage:
                                instantaneousProfileSinglePhase.Voltage = item[i].ToString();
                                break;

                            case OBISConstants.PhaseCurrent:
                                instantaneousProfileSinglePhase.PhaseCurrent = item[i].ToString();
                                break;

                            case OBISConstants.NeutralCurrent:
                                instantaneousProfileSinglePhase.NeutralCurrent = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactor:
                                instantaneousProfileSinglePhase.SignedPowerFactor = item[i].ToString();
                                break;

                            case OBISConstants.FrequencyHz:
                                instantaneousProfileSinglePhase.FrequencyHz = item[i].ToString();
                                break;

                            case OBISConstants.ApparentPowerKVA:
                                instantaneousProfileSinglePhase.ApparentPowerKVA = item[i].ToString();
                                break;

                            case OBISConstants.ActivePowerkW:
                                instantaneousProfileSinglePhase.ActivePowerkW = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                instantaneousProfileSinglePhase.CumulativeenergykWhimport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                instantaneousProfileSinglePhase.CumulativeenergykVAhimport = item[i].ToString();
                                break;

                            case OBISConstants.MaximumDemandkWOrDT:
                                string date = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                if (date == null)
                                {
                                    instantaneousProfileSinglePhase.MaxumumDemandkW = item[i].ToString();
                                }
                                else
                                {
                                    instantaneousProfileSinglePhase.MaxumumDemandkWdateandtime = date;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVahOrDT:
                                string date1 = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                if (date1 == null)
                                {
                                    instantaneousProfileSinglePhase.MaxumumDemandkVA = item[i].ToString();
                                }
                                else
                                {
                                    instantaneousProfileSinglePhase.MaxumumDemandkVAdateandtime = date1;
                                }
                                break;

                            case OBISConstants.CumulativepowerONdurationinminute:
                                instantaneousProfileSinglePhase.CumulativepowerONdurationinminute = item[i].ToString();
                                break;

                            case OBISConstants.Cumulativetampercount:
                                instantaneousProfileSinglePhase.Cumulativetampercount = item[i].ToString();
                                break;

                            case OBISConstants.Cumulativebillingcount:
                                instantaneousProfileSinglePhase.Cumulativebillingcount = item[i].ToString();
                                break;

                            case OBISConstants.Cumulativeprogrammingcount:
                                instantaneousProfileSinglePhase.Cumulativeprogrammingcount = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                instantaneousProfileSinglePhase.CumulativeenergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                instantaneousProfileSinglePhase.CumulativeenergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.Loadlimitfunctionstatus:
                                instantaneousProfileSinglePhase.Loadlimitfunctionstatus = item[i].ToString();
                                break;

                            case OBISConstants.LoadlimitvalueinkW:
                                instantaneousProfileSinglePhase.LoadlimitvalueinkW = item[i].ToString();
                                break;
                        }
                    }
                    instantaneousProfileSinglePhaseList.Add(instantaneousProfileSinglePhase);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : InstantaneousProfileSinglePhaseDto : Exception ==>" + ex.Message);
            }
            return instantaneousProfileSinglePhaseList;
        }

        public async Task<bool> ReadIPThreePhase()
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetInstantProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<InstantaneousProfileThreePhase> instantaneousProfile = await ConvertToIPThreePhase(res);

                    if (instantaneousProfile != null)
                    {
                        await _instantaneousProfileThreePhaseService.Add(instantaneousProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.IPDownloadedSuccessfullyCount + instantaneousProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.IPDownloadedFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadIPThreePhase : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<InstantaneousProfileThreePhase>> ConvertToIPThreePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<InstantaneousProfileThreePhase> instantaneousProfileThreePhaseList = new List<InstantaneousProfileThreePhase>();
            try
            {

                foreach (var item in parsedDataList.Buffer)
                {
                    InstantaneousProfileThreePhase instantaneousProfileThreePhase = new InstantaneousProfileThreePhase();

                    instantaneousProfileThreePhase.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                instantaneousProfileThreePhase.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CurrentR:
                                instantaneousProfileThreePhase.CurrentR = item[i].ToString();
                                break;

                            case OBISConstants.CurrentY:
                                instantaneousProfileThreePhase.CurrentY = item[i].ToString();
                                break;

                            case OBISConstants.CurrentB:
                                instantaneousProfileThreePhase.CurrentB = item[i].ToString();
                                break;

                            case OBISConstants.VoltageR:
                                instantaneousProfileThreePhase.VoltageR = item[i].ToString();
                                break;

                            case OBISConstants.VoltageY:
                                instantaneousProfileThreePhase.VoltageY = item[i].ToString();
                                break;

                            case OBISConstants.VoltageB:
                                instantaneousProfileThreePhase.VoltageB = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorRPhase:
                                instantaneousProfileThreePhase.SignedPowerFactorRPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorYPhase:
                                instantaneousProfileThreePhase.SignedPowerFactorYPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorBPhase:
                                instantaneousProfileThreePhase.SignedPowerFactorBPhase = item[i].ToString();
                                break;

                            case OBISConstants.ThreePhasePowerFactoPF:
                                instantaneousProfileThreePhase.ThreePhasePowerFactoRF = item[i].ToString();
                                break;

                            case OBISConstants.FrequencyHz:
                                instantaneousProfileThreePhase.FrequencyHz = item[i].ToString();
                                break;

                            case OBISConstants.ApparentPowerKVA:
                                instantaneousProfileThreePhase.ApparentPowerKVA = item[i].ToString();
                                break;

                            case OBISConstants.SignedActivePowerkW:
                                instantaneousProfileThreePhase.SignedActivePowerkW = item[i].ToString();
                                break;

                            case OBISConstants.SignedReactivePowerkvar:
                                instantaneousProfileThreePhase.SignedReactivePowerkvar = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                instantaneousProfileThreePhase.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                instantaneousProfileThreePhase.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                instantaneousProfileThreePhase.CumulativeEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                instantaneousProfileThreePhase.CumulativeEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ1:
                                instantaneousProfileThreePhase.CumulativeEnergykVArhQ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ2:
                                instantaneousProfileThreePhase.CumulativeEnergykVArhQ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ3:
                                instantaneousProfileThreePhase.CumulativeEnergykVArhQ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ4:
                                instantaneousProfileThreePhase.CumulativeEnergykVArhQ4 = item[i].ToString();
                                break;

                            case OBISConstants.NumberOfPowerFailures:
                                instantaneousProfileThreePhase.NumberOfPowerFailures = item[i].ToString();
                                break;

                            case OBISConstants.CumulativePowerOFFDurationInMin:
                                instantaneousProfileThreePhase.CumulativePowerOFFDurationInMin = item[i].ToString();
                                break;

                            case OBISConstants.Cumulativetampercount:
                                instantaneousProfileThreePhase.CumulativeTamperCount = item[i].ToString();
                                break;

                            case OBISConstants.BillingPeriodCounter:
                                instantaneousProfileThreePhase.BillingPeriodCounter = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeProgrammingCount:
                                instantaneousProfileThreePhase.CumulativeProgrammingCount = item[i].ToString();
                                break;

                            case OBISConstants.BillingDateImportMode:
                                instantaneousProfileThreePhase.BillingDateImportMode = item[i].GetType().IsArray ? Functions.ToIndiaDateTimeAdd11(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDateAdd11(item[i].ToString());
                                break;

                            case OBISConstants.MaximumDemandkWOrDT:
                                string date = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date == null)
                                {
                                    instantaneousProfileThreePhase.MaximumDemandkW = item[i].ToString();
                                }
                                else
                                {
                                    instantaneousProfileThreePhase.MaximumDemandkWDateTime = date;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVahOrDT:
                                string maxdate = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());

                                if (maxdate == null)
                                {
                                    instantaneousProfileThreePhase.MaximumDemandkVA = item[i].ToString();
                                }
                                else
                                {
                                    instantaneousProfileThreePhase.MaximumDemandkVADateTime = maxdate;
                                }
                                break;

                            case OBISConstants.LoadLimitFunctionStatus:
                                instantaneousProfileThreePhase.LoadLimitFunctionStatus = item[i].ToString();
                                break;

                            case OBISConstants.LoadLimitThresholdkW:
                                instantaneousProfileThreePhase.LoadLimitThresholdkW = item[i].ToString();
                                break;
                        }

                    }

                    instantaneousProfileThreePhaseList.Add(instantaneousProfileThreePhase);

                }

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToIPThreePhase : Exception ==>" + ex.Message);
            }
            return instantaneousProfileThreePhaseList;
        }


        public async Task<bool> ReadIPThreePhaseCT()
        {
            try
            {
                StopKeepAliveTimer();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetInstantProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip);
                StartKeepAliveTimer();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<InstantaneousProfileThreePhaseCT> instantaneousProfile = await ConvertToIPThreePhaseCT(res);

                    if (instantaneousProfile != null)
                    {
                        await _instantaneousProfileThreePhaseCTService.Add(instantaneousProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.IPDownloadedSuccessfullyCount + instantaneousProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.IPDownloadedFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadIPThreePhase : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<InstantaneousProfileThreePhaseCT>> ConvertToIPThreePhaseCT(GXDLMSProfileGeneric parsedDataList)
        {
            List<InstantaneousProfileThreePhaseCT> instantaneousProfileThreePhaseCTList = new List<InstantaneousProfileThreePhaseCT>();
            try
            {

                foreach (var item in parsedDataList.Buffer)
                {
                    InstantaneousProfileThreePhaseCT instantaneousProfileThreePhaseCT = new InstantaneousProfileThreePhaseCT();

                    instantaneousProfileThreePhaseCT.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                instantaneousProfileThreePhaseCT.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CurrentR:
                                instantaneousProfileThreePhaseCT.CurrentR = item[i].ToString();
                                break;

                            case OBISConstants.CurrentY:
                                instantaneousProfileThreePhaseCT.CurrentY = item[i].ToString();
                                break;

                            case OBISConstants.CurrentB:
                                instantaneousProfileThreePhaseCT.CurrentB = item[i].ToString();
                                break;

                            case OBISConstants.VoltageR:
                                instantaneousProfileThreePhaseCT.VoltageR = item[i].ToString();
                                break;

                            case OBISConstants.VoltageY:
                                instantaneousProfileThreePhaseCT.VoltageY = item[i].ToString();
                                break;

                            case OBISConstants.VoltageB:
                                instantaneousProfileThreePhaseCT.VoltageB = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorRPhase:
                                instantaneousProfileThreePhaseCT.SignedPowerFactorRPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorYPhase:
                                instantaneousProfileThreePhaseCT.SignedPowerFactorYPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorBPhase:
                                instantaneousProfileThreePhaseCT.SignedPowerFactorBPhase = item[i].ToString();
                                break;

                            case OBISConstants.ThreePhasePowerFactoPF:
                                instantaneousProfileThreePhaseCT.ThreePhasePowerFactorPF = item[i].ToString();
                                break;

                            case OBISConstants.FrequencyHz:
                                instantaneousProfileThreePhaseCT.FrequencyHz = item[i].ToString();
                                break;

                            case OBISConstants.ApparentPowerKVA:
                                instantaneousProfileThreePhaseCT.ApparentPowerKVA = item[i].ToString();
                                break;

                            case OBISConstants.SignedActivePowerkW:
                                instantaneousProfileThreePhaseCT.SignedActivePowerkW = item[i].ToString();
                                break;

                            case OBISConstants.SignedReactivePowerkvar:
                                instantaneousProfileThreePhaseCT.SignedReactivePowerkvar = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ1:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ2:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ3:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ4:
                                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ4 = item[i].ToString();
                                break;

                            case OBISConstants.NumberOfPowerFailures:
                                instantaneousProfileThreePhaseCT.NumberOfPowerFailures = item[i].ToString();
                                break;

                            case OBISConstants.CumulativePowerOFFDurationInMin:
                                instantaneousProfileThreePhaseCT.CumulativePowerOFFDurationInMin = item[i].ToString();
                                break;

                            case OBISConstants.Cumulativetampercount:
                                instantaneousProfileThreePhaseCT.CumulativeTamperCount = item[i].ToString();
                                break;

                            case OBISConstants.BillingPeriodCounter:
                                instantaneousProfileThreePhaseCT.BillingPeriodCounter = item[i].ToString();
                                break;

                            case OBISConstants.Cumulativeprogrammingcount:
                                instantaneousProfileThreePhaseCT.CumulativeProgrammingCount = item[i].ToString();
                                break;

                            case OBISConstants.BillingDateImportMode:
                                instantaneousProfileThreePhaseCT.BillingDateImportMode = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.MaximumDemandkWOrDT:
                                string date = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date == null)
                                {
                                    instantaneousProfileThreePhaseCT.MaximumDemandkW = item[i].ToString();
                                }
                                else
                                {
                                    instantaneousProfileThreePhaseCT.MaximumDemandkWDateTime = date;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVahOrDT:
                                string maxdate = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());

                                if (maxdate == null)
                                {
                                    instantaneousProfileThreePhaseCT.MaximumDemandkVA = item[i].ToString();
                                }
                                else
                                {
                                    instantaneousProfileThreePhaseCT.MaximumDemandkVADateTime = maxdate;
                                }
                                break;
                        }

                    }

                    instantaneousProfileThreePhaseCTList.Add(instantaneousProfileThreePhaseCT);

                }

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToIPThreePhase : Exception ==>" + ex.Message);
            }
            return instantaneousProfileThreePhaseCTList;
        }

        #endregion

        #region Block Load Profile Methods

        public async Task<bool> ReadBlockLoadProfileSinglePhase(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetBlockLoadProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<BlockLoadProfileSinglePhase> blockLoadProfile = await ConvertToBlockLoadProfileSinglePhase(res);

                    if (blockLoadProfile != null)
                    {
                        await _blockLoadProfileSinglePhaseService.Add(blockLoadProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BlockloadProfileDownloadedSuccessfullyCount + blockLoadProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BlockLoadProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToMeterModel : Exception ==>" + ex.Message + " Stack Trace: " + ex.StackTrace);
                return false;
            }
        }

        public async Task<List<BlockLoadProfileSinglePhase>> ConvertToBlockLoadProfileSinglePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<BlockLoadProfileSinglePhase> blockLoadProfileSinglePhaseList = new List<BlockLoadProfileSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    
                    BlockLoadProfileSinglePhase blockLoadProfileSinglePhase = new BlockLoadProfileSinglePhase();
                    blockLoadProfileSinglePhase.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                blockLoadProfileSinglePhase.RealTimeClock = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.AverageVoltage:
                                blockLoadProfileSinglePhase.AverageVoltage = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykWhImport:
                                blockLoadProfileSinglePhase.BlockEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVAh:
                                blockLoadProfileSinglePhase.BlockEnergykVAh = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykWhExport:
                                blockLoadProfileSinglePhase.BlockEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVAhExport:
                                blockLoadProfileSinglePhase.BlockEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadSinglePhase:
                                blockLoadProfileSinglePhase.PhaseCurrent = item[i].ToString();
                                break;

                            case OBISConstants.BlockNeutralCurrent:
                                blockLoadProfileSinglePhase.NeutralCurrent = item[i].ToString();
                                break;


                            default :
                            break;
                        }
                    }
                    blockLoadProfileSinglePhaseList.Add(blockLoadProfileSinglePhase);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToBlockLoadProfileSinglePhase : Exception ==>" + ex.Message + " StackTrace : " + ex.StackTrace);
                return null;
            }
            return blockLoadProfileSinglePhaseList;
        }

        public async Task<bool> ReadBlockLoadProfileThreePhase(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetBlockLoadProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<BlockLoadProfileThreePhase> blockLoadProfile = await ConvertToBlockLoadProfileThreePhase(res);

                    if (blockLoadProfile != null)
                    {
                        await _blockLoadProfileThreePhaseService.Add(blockLoadProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BlockLoadProfileDownloadedSuccessFully + blockLoadProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;

                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification,Constants.BlockLoadProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception)
            {
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification,Constants.Disconnected, NotificationType.Error, CloseOnClick: true);
                return false;
            }
        }
        public async Task<List<BlockLoadProfileThreePhase>> ConvertToBlockLoadProfileThreePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<BlockLoadProfileThreePhase> blockLoadProfileThreePhaseList = new List<BlockLoadProfileThreePhase>();
            try
            {


                foreach (var item in parsedDataList.Buffer)
                {
                    BlockLoadProfileThreePhase blockLoadProfileThreePhase = new BlockLoadProfileThreePhase();


                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        blockLoadProfileThreePhase.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                blockLoadProfileThreePhase.RealTimeClock = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.BlockLoadThreePhaseCurrentR:
                                blockLoadProfileThreePhase.CurrentR = item[i].ToString().ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseCurrentY:

                                blockLoadProfileThreePhase.CurrentY = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseCurrentB:
                                blockLoadProfileThreePhase.CurrentB = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseVoltageR:
                                blockLoadProfileThreePhase.VoltageR = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseVoltageY:
                                blockLoadProfileThreePhase.VoltageY = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseVoltageB:
                                blockLoadProfileThreePhase.VoltageB = item[i].ToString();
                                break;

                            //case OBISConstants.PowerFactorRPhase:
                            //    blockLoadProfileThreePhase.PowerFactorRPhase = item[i].ToString();
                            //    break;

                            //case OBISConstants.PowerFactorYPhase:
                            //    blockLoadProfileThreePhase.PowerFactorYPhase = item[i].ToString();
                            //    break;

                            //case OBISConstants.PowerFactorBPhase:
                            //    blockLoadProfileThreePhase.PowerFactorBPhase = item[i].ToString();
                            //    break;

                            case OBISConstants.BlockEnergykWhImport:
                                blockLoadProfileThreePhase.BlockEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVAh:
                                blockLoadProfileThreePhase.BlockEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykWhExport:
                                blockLoadProfileThreePhase.BlockEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVAhExport:
                                blockLoadProfileThreePhase.BlockEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.MeterHealthIndicator:
                                blockLoadProfileThreePhase.MeterHealthIndicator = item[i].ToString();
                                break;

                            case OBISConstants.ImportAveragePF:
                                blockLoadProfileThreePhase.ImportAvgPF = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    blockLoadProfileThreePhaseList.Add(blockLoadProfileThreePhase);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : BlockLoadProfileThreePhaseDto : Exception ==>" + ex.Message);
                return null;
            }
            return blockLoadProfileThreePhaseList;
        }


        public async Task<bool> ReadBlockLoadProfileThreePhaseCT(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetBlockLoadProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : BlockLoadProfileThreePhaseCT : Data Count ==>" + res.Buffer.Count);

                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<BlockLoadProfileThreePhaseCT> blockLoadProfile = await ConvertToBlockLoadProfileThreePhaseCT(res);

                    if (blockLoadProfile != null)
                    {
                        await _blockLoadProfileThreePhaseCTService.Add(blockLoadProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BlockLoadProfileDownloadedSuccessFully, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;

                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BillingProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<BlockLoadProfileThreePhaseCT>> ConvertToBlockLoadProfileThreePhaseCT(GXDLMSProfileGeneric parsedDataList)
        {
            List<BlockLoadProfileThreePhaseCT> blockLoadProfileThreePhaseCTList = new List<BlockLoadProfileThreePhaseCT>();
            try
            {


                foreach (var item in parsedDataList.Buffer)
                {
                    BlockLoadProfileThreePhaseCT blockLoadProfileThreePhaseCT = new BlockLoadProfileThreePhaseCT();


                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        blockLoadProfileThreePhaseCT.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                blockLoadProfileThreePhaseCT.RealTimeClock = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.BlockLoadThreePhaseCurrentR:
                                blockLoadProfileThreePhaseCT.CurrentR = item[i].ToString().ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseCurrentY:

                                blockLoadProfileThreePhaseCT.CurrentY = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseCurrentB:
                                blockLoadProfileThreePhaseCT.CurrentB = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseVoltageR:
                                blockLoadProfileThreePhaseCT.VoltageR = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseVoltageY:
                                blockLoadProfileThreePhaseCT.VoltageY = item[i].ToString();
                                break;

                            case OBISConstants.BlockLoadThreePhaseVoltageB:
                                blockLoadProfileThreePhaseCT.VoltageB = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykWhImport:
                                blockLoadProfileThreePhaseCT.BlockEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVAh:
                                blockLoadProfileThreePhaseCT.BlockEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykWhExport:
                                blockLoadProfileThreePhaseCT.BlockEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVAhExport:
                                blockLoadProfileThreePhaseCT.BlockEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVArhQ1:
                                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ1 = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVArhQ2:
                                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ2 = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVArhQ3:
                                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ3 = item[i].ToString();
                                break;

                            case OBISConstants.BlockEnergykVArhQ4:
                                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ4 = item[i].ToString();
                                break;

                            case OBISConstants.MeterHealthIndicator:
                                blockLoadProfileThreePhaseCT.MeterHealthIndicator = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    blockLoadProfileThreePhaseCTList.Add(blockLoadProfileThreePhaseCT);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : BlockLoadProfileThreePhaseDto : Exception ==>" + ex.Message);
                return null;
            }
            return blockLoadProfileThreePhaseCTList;
        }

        #endregion

        #region Billing Profile Methods

        public async Task<bool> ReadBillingProfileSinglePhase(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetBillingProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, null, null);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<BillingProfileSinglePhase> billingProfile = await ConvertToBillingProfileSinglePhase(res);

                    if (billingProfile != null)
                    {
                        await _billingProfileSinglePhaseService.Add(billingProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BillingProfileDownloadedSuccessFully + billingProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BlockloadProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToBillingProfileSinglePhase : Exception ==>" + ex.Message + " Stack Trace: " + ex.StackTrace);
                return false;
            }
        }

        public async Task<List<BillingProfileSinglePhase>> ConvertToBillingProfileSinglePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<BillingProfileSinglePhase> billingProfileSinglePhaseList = new List<BillingProfileSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    BillingProfileSinglePhase billingProfileSinglePhase = new BillingProfileSinglePhase();

                    billingProfileSinglePhase.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        try
                        {
                            string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                            switch (key)
                            {
                                case OBISConstants.BillingRealTime:
                                    billingProfileSinglePhase.RealTimeClock = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                    break;

                                case OBISConstants.AveragePowerFactor:
                                    billingProfileSinglePhase.AveragePowerFactor = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.CumulativeenergykWhimport:
                                    billingProfileSinglePhase.CumulativeEnergykWhImport = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.CumulativeEnergykWhTZ1:
                                    billingProfileSinglePhase.CumulativeEnergykWhTZ1 = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.CumulativeEnergykWhTZ2:
                                    billingProfileSinglePhase.CumulativeEnergykWhTZ2 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykWhTZ3:
                                    billingProfileSinglePhase.CumulativeEnergykWhTZ3 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykWhTZ4:
                                    billingProfileSinglePhase.CumulativeEnergykWhTZ4 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykWhTZ5:
                                    billingProfileSinglePhase.CumulativeEnergykWhTZ5 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykWhTZ6:
                                    billingProfileSinglePhase.CumulativeEnergykWhTZ6 = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.CumulativeenergykVAhimport:
                                    billingProfileSinglePhase.CumulativeEnergykVAhImport = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykVAhTZ1:
                                    billingProfileSinglePhase.CumulativeEnergykVAhTZ1 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykVAhTZ2:
                                    billingProfileSinglePhase.CumulativeEnergykVAhTZ2 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykVAhTZ3:
                                    billingProfileSinglePhase.CumulativeEnergykVAhTZ3 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykVAhTZ4:
                                    billingProfileSinglePhase.CumulativeEnergykVAhTZ4 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykVAhTZ5:
                                    billingProfileSinglePhase.CumulativeEnergykVAhTZ5 = item[i] != null ? item[i].ToString() : "";
                                    break;


                                case OBISConstants.CumulativeEnergykVAhTZ6:
                                    billingProfileSinglePhase.CumulativeEnergykVAhTZ6 = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.MaximumDemandkWOrDT:
                                    string date = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                    if (date == null)
                                    {
                                        billingProfileSinglePhase.MaximumDemandkW = item[i] != null ? item[i].ToString() : "";
                                    }
                                    else
                                    {
                                        billingProfileSinglePhase.MaximumDemandkWDateTime = date;
                                    }
                                    break;

                                case OBISConstants.MaximumDemandkVahOrDT:
                                    string date1 = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                    if (date1 == null)
                                    {
                                        billingProfileSinglePhase.MaximumDemandkVA = item[i] != null ? item[i].ToString() : "";
                                    }
                                    else
                                    {
                                        billingProfileSinglePhase.MaximumDemandkVADateTime = date1;
                                    }
                                    break;

                                case OBISConstants.BillingPowerONdurationinMinutes:
                                    billingProfileSinglePhase.BillingPowerONdurationinMinutes = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.CumulativeenergykWhExport:
                                    billingProfileSinglePhase.CumulativeEnergykWhExport = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.CumulativeenergykVAhExport:
                                    billingProfileSinglePhase.CumulativeEnergykVAhExport = item[i] != null ? item[i].ToString() : "";
                                    break;

                                case OBISConstants.MaximumDemandkW:
                                    string date2 = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                    if (date2 == null)
                                    {
                                        billingProfileSinglePhase.MaximumDemandkWExport = item[i] != null ? item[i].ToString() : "";
                                    }
                                    else
                                    {
                                        billingProfileSinglePhase.MaximumDemandkWExportDateTime = date2;
                                    }
                                    break;

                                case OBISConstants.MaximumDemandkva:
                                    string date3 = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                    if (date3 == null)
                                    {
                                        billingProfileSinglePhase.MaximumDemandkvaExport = item[i] != null ? item[i].ToString() : "";
                                    }
                                    else
                                    {
                                        billingProfileSinglePhase.MaximumDemandkvaExportDateTime = date3;
                                    }
                                    break;

                                case OBISConstants.AveragePowerFactorExport:
                                    billingProfileSinglePhase.AveragePowerFactorExport = item[i] != null ? item[i].ToString() : "";
                                    break;

                                default:
                                    break;
                            }
                        }
                        catch(Exception ex)
                        {
                            _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : AT Conversion : Exception ==>" + ex.Message + " Stack Trace: " + ex.StackTrace);

                            continue;
                        }
                    }

                    billingProfileSinglePhaseList.Add(billingProfileSinglePhase);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToBillingProfileSinglePhase : Exception ==>" + ex.Message + " Stack Trace: " + ex.StackTrace);
                return null;
            }
            return billingProfileSinglePhaseList;
        }

        public async Task<bool> ReadBillingProfileThreePhase(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetBillingProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<BillingProfileThreePhase> billingProfile = await ConvertToBillingProfileThreePhase(res);

                    if (billingProfile != null)
                    {
                        await _billingProfileThreePhaseService.Add(billingProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BillingProfileDownloadSuccessfullyCount + billingProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification,Constants.BillingProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadBillingProfileThreePhase : Exception ==>" + ex.Message);


                return false;
            }
        }

        public async Task<List<BillingProfileThreePhase>> ConvertToBillingProfileThreePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<BillingProfileThreePhase> billingProfileThreePhaseList = new List<BillingProfileThreePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {

                    BillingProfileThreePhase billingProfileThreePhase = new BillingProfileThreePhase();

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        billingProfileThreePhase.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                            case OBISConstants.BillingRealTime:
                                billingProfileThreePhase.RealTimeClock = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.SystemPowerFactorImport:
                                billingProfileThreePhase.SystemPowerFactorImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                billingProfileThreePhase.CumulativeEnergykWh = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ1:
                                billingProfileThreePhase.CumulativeEnergykWhTZ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ2:
                                billingProfileThreePhase.CumulativeEnergykWhTZ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ3:
                                billingProfileThreePhase.CumulativeEnergykWhTZ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ4:
                                billingProfileThreePhase.CumulativeEnergykWhTZ4 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ5:
                                billingProfileThreePhase.CumulativeEnergykWhTZ5 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ6:
                                billingProfileThreePhase.CumulativeEnergykWhTZ6 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ7:
                                billingProfileThreePhase.CumulativeEnergykWhTZ7 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ8:
                                billingProfileThreePhase.CumulativeEnergykWhTZ8 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                billingProfileThreePhase.CumulativeEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ1:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ2:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ3:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ4:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ4 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ5:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ5 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ6:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ6 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ7:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ7 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ8:
                                billingProfileThreePhase.CumulativeEnergykVAhTZ8 = item[i].ToString();
                                break;

                            case OBISConstants.MaximumDemandkWOrDT:
                                string date = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkW = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWDateTime = date;
                                }
                                break;


                            case OBISConstants.MaximumDemandkWForTZ1OrDT:
                                string date1 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date1 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ1 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ1DateTime = date1;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ2OrDT:
                                string date2 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date2 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ2 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ2DateTime = date2;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ3OrDT:
                                string date3 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date3 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ3 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ3DateTime = date3;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ4OrDT:
                                string date4 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date4 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ4 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ4DateTime = date4;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ5OrDT:
                                string date5 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date5 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ5 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ5DateTime = date5;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ6OrDT:
                                string date6 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date6 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ6 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ6DateTime = date6;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ7OrDT:
                                string date7 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date7 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ7 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ7DateTime = date7;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ8OrDT:
                                string date8 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date8 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ8 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWForTZ8DateTime = date8;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVahOrDT:
                                string dateKvA = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateKvA == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVA = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVADateTime = dateKvA;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ1OrDT:
                                string dateZ1 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ1 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ1 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ1DateTime = dateZ1;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ2OrDT:
                                string dateZ2 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ2 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ2 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ2DateTime = dateZ2;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ3OrDT:
                                string dateZ3 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ3 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ3 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ3DateTime = dateZ3;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ4OrDT:
                                string dateZ4 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ4 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ4 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ4DateTime = dateZ4;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ5OrDT:
                                string dateZ5 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ5 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ5 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ5DateTime = dateZ5;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ6OrDT:
                                string dateZ6 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ6 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ6 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ6DateTime = dateZ6;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ7OrDT:
                                string dateZ7 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ7 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ7 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ7DateTime = dateZ7;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ8OrDT:
                                string dateZ8 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ8 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ8 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAForTZ8DateTime = dateZ8;
                                }
                                break;

                            case OBISConstants.BillingPowerONdurationInMinutesDBP:
                                billingProfileThreePhase.BillingPowerONdurationInMinutesDBP = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                billingProfileThreePhase.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                billingProfileThreePhase.CumulativeEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ1:
                                billingProfileThreePhase.CumulativeEnergykVArhQ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ2:
                                billingProfileThreePhase.CumulativeEnergykVArhQ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ3:
                                billingProfileThreePhase.CumulativeEnergykVArhQ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ4:
                                billingProfileThreePhase.CumulativeEnergykVArhQ4 = item[i].ToString();
                                break;

                            case OBISConstants.TamperCount:
                                billingProfileThreePhase.TamperCount = item[i].ToString();
                                break;

                            case OBISConstants.AveragePowerFactorExport:
                                billingProfileThreePhase.PowerFactorExport = item[i].ToString();
                                break;


                            case OBISConstants.MaximumDemandkW:
                                string dateZ18 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ18 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkWExport = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkWExportDateTime = dateZ18;
                                }
                                break;

                            case OBISConstants.MaximumDemandkva:
                                string dateZ19 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ19 == null)
                                {
                                    billingProfileThreePhase.MaximumDemandkVAExport = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhase.MaximumDemandkVAExportDateTime = dateZ19;
                                }
                                break;

                            default:
                                break;

                        }
                    }

                    billingProfileThreePhaseList.Add(billingProfileThreePhase);

                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToBillingProfileThreePhase : Exception ==>" + ex.Message);
                return null;
            }
            return billingProfileThreePhaseList;
        }

        public async Task<bool> ReadBillingProfileThreePhaseCT(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetBillingProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<BillingProfileThreePhaseCT> billingProfile = await ConvertToBillingProfileThreePhaseCT(res);

                    if (billingProfile != null)
                    {
                        await _billingProfileThreePhaseCTService.Add(billingProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BillingProfileDownloadSuccessfullyCount + billingProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.BillingProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadBillingProfileThreePhase : Exception ==>" + ex.Message);


                return false;
            }
        }

        public async Task<List<BillingProfileThreePhaseCT>> ConvertToBillingProfileThreePhaseCT(GXDLMSProfileGeneric parsedDataList)
        {
            List<BillingProfileThreePhaseCT> billingProfileThreePhaseCTList = new List<BillingProfileThreePhaseCT>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {

                    BillingProfileThreePhaseCT billingProfileThreePhaseCT = new BillingProfileThreePhaseCT();

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        billingProfileThreePhaseCT.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                         
                            case OBISConstants.AveragePFForBillingPeriod:
                                billingProfileThreePhaseCT.AveragePFForBillingPeriod = item[i].ToString();
                                break;
                            case OBISConstants.BillingDate:
                                billingProfileThreePhaseCT.BillingDate = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;
                            case OBISConstants.CumulativeenergykWhimport:
                                billingProfileThreePhaseCT.CumulativeEnergykWh = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ1:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ2:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ3:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ4:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ4 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ5:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ5 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ6:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ6 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ7:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ7 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykWhTZ8:
                                billingProfileThreePhaseCT.CumulativeEnergykWhTZ8 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ1:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ2:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ3:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ4:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ4 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ5:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ5 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ6:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ6 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ7:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ7 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVAhTZ8:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhTZ8 = item[i].ToString();
                                break;

                            case OBISConstants.MaximumDemandkWOrDT:
                                string date = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkW = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWDateTime = date;
                                }
                                break;


                            case OBISConstants.MaximumDemandkWForTZ1OrDT:
                                string date1 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date1 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ1 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ1DateTime = date1;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ2OrDT:
                                string date2 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date2 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ2 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ2DateTime = date2;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ3OrDT:
                                string date3 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date3 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ3 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ3DateTime = date3;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ4OrDT:
                                string date4 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date4 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ4 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ4DateTime = date4;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ5OrDT:
                                string date5 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date5 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ5 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ5DateTime = date5;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ6OrDT:
                                string date6 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date6 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ6 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ6DateTime = date6;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ7OrDT:
                                string date7 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date7 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ7 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ7DateTime = date7;
                                }
                                break;

                            case OBISConstants.MaximumDemandkWForTZ8OrDT:
                                string date8 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (date8 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ8 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkWForTZ8DateTime = date8;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVahOrDT:
                                string dateKvA = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateKvA == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVA = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVADateTime = dateKvA;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ1OrDT:
                                string dateZ1 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ1 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ1 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ1DateTime = dateZ1;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ2OrDT:
                                string dateZ2 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ2 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ2 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ2DateTime = dateZ2;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ3OrDT:
                                string dateZ3 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ3 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ3 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ3DateTime = dateZ3;
                                }
                                break;

                            case OBISConstants.MaximumDemandkVAForTZ4OrDT:
                                string dateZ4 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ4 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ4 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ4DateTime = dateZ4;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ5OrDT:
                                string dateZ5 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ5 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ5 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ5DateTime = dateZ5;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ6OrDT:
                                string dateZ6 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ6 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ6 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ6DateTime = dateZ6;
                                }
                                break;


                            case OBISConstants.MaximumDemandkVAForTZ7OrDT:
                                string dateZ7 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ7 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ7 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ7DateTime = dateZ7;
                                }
                                break;


                             case OBISConstants.MaximumDemandkVAForTZ8OrDT:
                                string dateZ8 = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                if (dateZ8 == null)
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ8 = item[i].ToString();
                                }
                                else
                                {
                                    billingProfileThreePhaseCT.MaximumDemandkVAForTZ8DateTime = dateZ8;
                                }
                                break;

                            case OBISConstants.BillingPowerONdurationInMinutesDBP:
                                billingProfileThreePhaseCT.BillingPowerONdurationInMinutesDBP = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                billingProfileThreePhaseCT.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                billingProfileThreePhaseCT.CumulativeEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ1:
                                billingProfileThreePhaseCT.CumulativeEnergykVArhQ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ2:
                                billingProfileThreePhaseCT.CumulativeEnergykVArhQ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ3:
                                billingProfileThreePhaseCT.CumulativeEnergykVArhQ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ4:
                                billingProfileThreePhaseCT.CumulativeEnergykVArhQ4 = item[i].ToString();
                                break;

                            case OBISConstants.TamperCount:
                                billingProfileThreePhaseCT.TamperCount = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    billingProfileThreePhaseCTList.Add(billingProfileThreePhaseCT);

                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToBillingProfileThreePhase : Exception ==>" + ex.Message);
                return null;
            }
            return billingProfileThreePhaseCTList;
        }

        #endregion

        #region Daily Load Profile
        public async Task<bool> ReadDailyLoadProfileSinglePhase(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetDailyLoadProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<DailyLoadProfileSinglePhase> dailyLoadProfile = await ConvertToDailyLoadProfileSinglePhase(res);

                    if (dailyLoadProfile != null && dailyLoadProfile.Count > 0)
                    {
                        await _dailyLoadSinglePhaseService.Add(dailyLoadProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.DailyLoadProfileDownloadedSuccessfullyCount + dailyLoadProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.DailyLoadProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDailyLoadProfileSinglePhase : Exception ==>" + ex.Message+" Stack Trace: "+ex.StackTrace);
                return false;
            }
        }

        public async Task<List<DailyLoadProfileSinglePhase>> ConvertToDailyLoadProfileSinglePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<DailyLoadProfileSinglePhase> dailyLoadSinglePhaseList = new List<DailyLoadProfileSinglePhase>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    DailyLoadProfileSinglePhase dailyLoadSinglePhase = new DailyLoadProfileSinglePhase();

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        dailyLoadSinglePhase.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                dailyLoadSinglePhase.RealTimeClock = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                dailyLoadSinglePhase.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                dailyLoadSinglePhase.CumulativeEnergyKVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                dailyLoadSinglePhase.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                dailyLoadSinglePhase.CumulativeEnergykVAhExport = item[i].ToString();
                                break;
                        }
                    }

                    dailyLoadSinglePhaseList.Add(dailyLoadSinglePhase);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return dailyLoadSinglePhaseList;
        }

        public async Task<bool> ReadDailyLoadProfileThreePhase(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetDailyLoadProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<DailyLoadProfileThreePhase> dailyLoadProfile = await ConvertToDailyLoadProfileThreePhase(res);

                    if (dailyLoadProfile != null && dailyLoadProfile.Count > 0)
                    {
                        await _dailyLoadThreePhaseService.Add(dailyLoadProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.DailyLoadProfileDownloadedSuccessfullyCount + dailyLoadProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.DailyLoadProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDailyLoadProfileThreePhase : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<List<DailyLoadProfileThreePhase>> ConvertToDailyLoadProfileThreePhase(GXDLMSProfileGeneric parsedDataList)
        {
            List<DailyLoadProfileThreePhase> dailyLoadThreePhaseList = new List<DailyLoadProfileThreePhase>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    DailyLoadProfileThreePhase dailyLoadThreePhase = new DailyLoadProfileThreePhase();
                    dailyLoadThreePhase.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        dailyLoadThreePhase.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                dailyLoadThreePhase.RealTimeClock = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                dailyLoadThreePhase.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                dailyLoadThreePhase.CumulativeEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                dailyLoadThreePhase.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                dailyLoadThreePhase.CumulativeEnergykVAhExport = item[i].ToString();
                                break;
                        }
                    }

                    dailyLoadThreePhaseList.Add(dailyLoadThreePhase);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToDailyLoadProfileThreePhase : Exception ==>" + ex.Message);
                return null;
            }
            return dailyLoadThreePhaseList;
        }


        public async Task<bool> ReadDailyLoadProfileThreePhaseCT(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetDailyLoadProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);
                keepAliveTimer.Start();

                if (res != null && res.Buffer.Count > 0)
                {
                    List<DailyLoadProfileThreePhaseCT> dailyLoadProfile = await ConvertToDailyLoadProfileThreePhaseCT(res);

                    if (dailyLoadProfile != null && dailyLoadProfile.Count > 0)
                    {
                        await _dailyLoadThreePhaseCTService.Add(dailyLoadProfile);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.DailyLoadProfileDownloadedSuccessfullyCount + dailyLoadProfile.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.DailyLoadProfileDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDailyLoadProfileThreePhase : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<List<DailyLoadProfileThreePhaseCT>> ConvertToDailyLoadProfileThreePhaseCT(GXDLMSProfileGeneric parsedDataList)
        {
            List<DailyLoadProfileThreePhaseCT> dailyLoadThreePhaseCTList = new List<DailyLoadProfileThreePhaseCT>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    DailyLoadProfileThreePhaseCT dailyLoadThreePhaseCT = new DailyLoadProfileThreePhaseCT();
                    dailyLoadThreePhaseCT.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();
                        dailyLoadThreePhaseCT.MeterNo = meterDetails.MeterNumber;

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                dailyLoadThreePhaseCT.RealTimeClock = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                dailyLoadThreePhaseCT.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhimport:
                                dailyLoadThreePhaseCT.CumulativeEnergykVAhImport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                dailyLoadThreePhaseCT.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykVAhExport:
                                dailyLoadThreePhaseCT.CumulativeEnergykVAhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ1:
                                dailyLoadThreePhaseCT.CumulativeEnergykVArhQ1 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ2:
                                dailyLoadThreePhaseCT.CumulativeEnergykVArhQ2 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ3:
                                dailyLoadThreePhaseCT.CumulativeEnergykVArhQ3 = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeEnergykVArhQ4:
                                dailyLoadThreePhaseCT.CumulativeEnergykVArhQ4 = item[i].ToString();
                                break;
                        }
                    }

                    dailyLoadThreePhaseCTList.Add(dailyLoadThreePhaseCT);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToDailyLoadProfileThreePhase : Exception ==>" + ex.Message);
                return null;
            }
            return dailyLoadThreePhaseCTList;
        }
        #endregion

        #region Event methods

        #region ControlEvent
        public async Task<bool> ReadControlEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetControlEvent, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<ControlEvent> controlEvent = await ConvertControlEventThree(res);

                    if (controlEvent != null && controlEvent.Count > 0)
                    {
                        await _controlEventService.Add(controlEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadControlEventThree : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<ControlEvent>> ConvertControlEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<ControlEvent> controlEventeList = new List<ControlEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    ControlEvent controlEvent = new ControlEvent();
                    controlEvent.MeterNo = meterDetails.MeterNumber;
                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                controlEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.EventCode:
                                controlEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.GenericEventLogSequenceNumberThreePhaseControl:
                                controlEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    controlEventeList.Add(controlEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertControlEventThree : Exception ==>" + ex.Message);

                return null;
            }
            return controlEventeList;
        }

        public async Task<bool> ReadControlEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetControlEvent, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<ControlEventSinglePhase> controlEvent = await ConvertControlEventSingle(res);

                    if (controlEvent != null && controlEvent.Count > 0)
                    {
                        await _controlEventSinglePhaseService.Add(controlEvent);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadControlEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<ControlEventSinglePhase>> ConvertControlEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<ControlEventSinglePhase> controlEventeList = new List<ControlEventSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    ControlEventSinglePhase controlEvent = new ControlEventSinglePhase();

                    controlEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                controlEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.EventCode:
                                controlEvent.EventCode = item[1].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[1].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }
                    controlEventeList.Add(controlEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertControlEventSingle : Exception ==>" + ex.Message);

                return null;
            }

            return controlEventeList;
        }
        #endregion

        #region NonRolloverEvent
        public async Task<bool> ReadNonRolloverEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetNonRollOverEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<NonRolloverEvent> nonRolloverEvent = await ConvertNonRolloverEventThree(res);

                    if (nonRolloverEvent != null && nonRolloverEvent.Count > 0)
                    {
                        await _nonRolloverEventService.Add(nonRolloverEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadNonRolloverEventThree : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<NonRolloverEvent>> ConvertNonRolloverEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<NonRolloverEvent> nonRolloverEventeList = new List<NonRolloverEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    NonRolloverEvent nonRolloverEvent = new NonRolloverEvent();
                    nonRolloverEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                nonRolloverEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.EventCode:
                                nonRolloverEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.NonRolloverEvent:
                                nonRolloverEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.GenericEventLogSequenceNumberThreePhaseNonRollOver:
                                nonRolloverEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    nonRolloverEventeList.Add(nonRolloverEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertNonRolloverEventThree : Exception ==>" + ex.Message);

                return null;
            }
            return nonRolloverEventeList;
        }

        public async Task<bool> ReadNonRolloverEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetNonRollOverEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<NonRolloverEventSinglePhase> nonRolloverEvent = await ConvertNonRolloverEventSingle(res);

                    if (nonRolloverEvent != null && nonRolloverEvent.Count > 0)
                    {
                        await _nonRolloverEventSinglePhaseService.Add(nonRolloverEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadNonRolloverEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<NonRolloverEventSinglePhase>> ConvertNonRolloverEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<NonRolloverEventSinglePhase> nonRolloverEventDto = new List<NonRolloverEventSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    NonRolloverEventSinglePhase nonRollOverEvent = new NonRolloverEventSinglePhase();

                    nonRollOverEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                nonRollOverEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.EventCode:
                                nonRollOverEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.NonRolloverEvent:
                                nonRollOverEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }

                    nonRolloverEventDto.Add(nonRollOverEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertNonRolloverEventSingle : Exception ==>" + ex.Message+" StackTrace : "+ ex.StackTrace);

                return null;
            }

            return nonRolloverEventDto;
        }
        #endregion

        #region OtherEvent
        public async Task<bool> ReadOtherEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetOtherEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<OtherEvent> otherEvent = await ConvertOtherEventThree(res);

                    if (otherEvent != null && otherEvent.Count > 0)
                    {
                        await _otherEventService.Add(otherEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadOtherEventThree : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<OtherEvent>> ConvertOtherEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<OtherEvent> otherEventeList = new List<OtherEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    OtherEvent otherEvent = new OtherEvent();

                    otherEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                otherEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.OtherEventCode:
                                otherEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.CurrentIr:
                                otherEvent.CurrentIr = item[i].ToString();
                                break;

                            case OBISConstants.CurrentIy:
                                otherEvent.CurrentIy = item[i].ToString();
                                break;

                            case OBISConstants.CurrentIb:
                                otherEvent.CurrentIb = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVrn:
                                otherEvent.VoltageVrn = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVyn:
                                otherEvent.VoltageVyn = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVbn: 
                                otherEvent.VoltageVbn = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorRPhase:
                                otherEvent.SignedPowerFactorRPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorYPhase:
                                otherEvent.SignedPowerFactorYPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorBPhase:
                                otherEvent.SignedPowerFactorBPhase = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                otherEvent.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.TamperCount:
                                otherEvent.CumulativeTamperCount = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                otherEvent.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.OtherEventESD:
                                otherEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.GenericEventLogSequenceNumberThreePhaseOther:
                                otherEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            case OBISConstants.NeutralCurrent:
                                otherEvent.NuetralCurrent = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    otherEventeList.Add(otherEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertOtherEventThree : Exception ==>" + ex.Message);
                return null;
            }
            return otherEventeList;
        }
        public async Task<bool> ReadOtherEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetOtherEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<OtherEventSinglePhase> otherEvent = await ConvertOtherEventSingle(res);

                    if (otherEvent != null && otherEvent.Count > 0)
                    {
                        await _otherEventSinglePhaseService.Add(otherEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadOtherEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }
        public async Task<List<OtherEventSinglePhase>> ConvertOtherEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<OtherEventSinglePhase> otherEventeList = new List<OtherEventSinglePhase>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    OtherEventSinglePhase otherEvent = new OtherEventSinglePhase();

                    otherEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                otherEvent.DateandTimeofEvent = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.OtherEventCode:
                                otherEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.Current:
                                otherEvent.Current = item[i].ToString();
                                break;


                            case OBISConstants.Voltage:
                                otherEvent.Voltage = item[i].ToString();
                                break;

                            case OBISConstants.PowerFactor:
                                otherEvent.PowerFactor = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                otherEvent.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.OtherRelatedEventsExport:
                                otherEvent.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.TamperCount:
                                otherEvent.CumulativeTamperCount = item[i].ToString();
                                break;

                            case OBISConstants.OtherEventESD:
                                otherEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }

                    otherEventeList.Add(otherEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : OtherEventSinglePhaseDTO : Exception ==>" + ex.Message);

                return null;
            }
            return otherEventeList;
        }
        #endregion

        #region TransactionEvent
        public async Task<bool> ReadTransactionEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetTransactionEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<TransactionEvent> transactionEvent = await ConvertTransactionEventThree(res);

                    if (transactionEvent != null && transactionEvent.Count > 0)
                    {
                        await _transactionEventService.Add(transactionEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadTransactionEventThree : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<TransactionEvent>> ConvertTransactionEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<TransactionEvent> transactionEventeList = new List<TransactionEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    TransactionEvent transactionEvent = new TransactionEvent();

                    transactionEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                transactionEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.TransactionEventCode:
                                transactionEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.TransactionEventLeadLag:
                                transactionEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;
                            case OBISConstants.GenericEventLogSequenceNumberThreePhaseTransaction:
                                transactionEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }

                    transactionEventeList.Add(transactionEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertTransactionEventThree : Exception ==>" + ex.Message);

                return null;
            }
            return transactionEventeList;
        }

        public async Task<bool> ReadTransactionEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetTransactionEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<TransactionEventSinglePhase> transactionEvent = await ConvertTransactionEventSingle(res);

                    if (transactionEvent != null && transactionEvent.Count > 0)
                    {
                        await _transactionEventSinglePhaseService.Add(transactionEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadTransactionEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<TransactionEventSinglePhase>> ConvertTransactionEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<TransactionEventSinglePhase> transactionEventDto = new List<TransactionEventSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    TransactionEventSinglePhase transactionEvent = new TransactionEventSinglePhase();

                    transactionEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                transactionEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.TransactionEventCode:
                                transactionEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.TransactionEventLeadLag:
                                transactionEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }
                    transactionEventDto.Add(transactionEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertTransactionEventSingle : Exception ==>" + ex.Message+" StackTrace : "+ ex.StackTrace);

            }

            return transactionEventDto;
        }
        #endregion

        #region PowerRelatedEvent
        public async Task<bool> ReadPowerRelatedEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetPowerEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<PowerRelatedEvent> powerRelatedEvent = await ConvertPowerRelatedEventThree(res);

                    if (powerRelatedEvent != null && powerRelatedEvent.Count > 0)
                    {
                        await _powerRelatedEventService.Add(powerRelatedEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadPowerRelatedEventThree : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<List<PowerRelatedEvent>> ConvertPowerRelatedEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<PowerRelatedEvent> powerRelatedEventeList = new List<PowerRelatedEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    PowerRelatedEvent powerRelatedEvent = new PowerRelatedEvent();

                    powerRelatedEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                powerRelatedEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.PowerRelatedEventCode:
                                powerRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;
                            case OBISConstants.GenericEventLogSequenceNumberThreePhasePower:
                                powerRelatedEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }
                    powerRelatedEventeList.Add(powerRelatedEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertPowerRelatedEventThree : Exception ==>" + ex.Message);
                return null;
            }
            return powerRelatedEventeList;
        }

        public async Task<bool> ReadPowerRelatedEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetPowerEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<PowerRelatedEventSinglePhase> powerRelatedEvent = await ConvertPowerRelatedEventSingle(res);

                    if (powerRelatedEvent != null && powerRelatedEvent.Count > 0)
                    {
                        await _powerRelatedEventSinglePhaseService.Add(powerRelatedEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadPowerRelatedEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<PowerRelatedEventSinglePhase>> ConvertPowerRelatedEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<PowerRelatedEventSinglePhase> powerRelatedEventDto = new List<PowerRelatedEventSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    PowerRelatedEventSinglePhase powerRelatedEvent = new PowerRelatedEventSinglePhase();

                    powerRelatedEvent.MeterNo = meterDetails.MeterNumber;
                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                powerRelatedEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.PowerRelatedEventCode:
                                powerRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }
                    powerRelatedEventDto.Add(powerRelatedEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertPowerRelatedEventSingle : Exception ==>" + ex.Message);

                return null;
            }

            return powerRelatedEventDto;
        }
        #endregion

        #region CurrentRelatedEvent 
        public async Task<bool> ReadCurrentRelatedEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetCurrentEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<CurrentRelatedEvent> currentRelatedEvent = await ConvertCurrentRelatedEventThree(res);

                    if (currentRelatedEvent != null && currentRelatedEvent.Count > 0)
                    {
                        await _currentRelatedEventService.Add(currentRelatedEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadCurrentRelatedEventThree : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<List<CurrentRelatedEvent>> ConvertCurrentRelatedEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<CurrentRelatedEvent> currentRelatedEventeList = new List<CurrentRelatedEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    CurrentRelatedEvent currentRelatedEvent = new CurrentRelatedEvent();

                    currentRelatedEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                currentRelatedEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CurrentRelatedEventCode:
                                currentRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.CurrentIr:
                                currentRelatedEvent.CurrentIr = item[i].ToString();
                                break;

                            case OBISConstants.CurrentIy:
                                currentRelatedEvent.CurrentIy = item[i].ToString();
                                break;

                            case OBISConstants.CurrentIb:
                                currentRelatedEvent.CurrentIb = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVrn:
                                currentRelatedEvent.VoltageVrn = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVyn:
                                currentRelatedEvent.VoltageVyn = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVbn:
                                currentRelatedEvent.VoltageVbn = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorRPhase:
                                currentRelatedEvent.SignedPowerFactorRPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorYPhase:
                                currentRelatedEvent.SignedPowerFactorYPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorBPhase:
                                currentRelatedEvent.SignedPowerFactorBPhase = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                currentRelatedEvent.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.TamperCount:
                                currentRelatedEvent.CumulativeTamperCount = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                currentRelatedEvent.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CurrentHighRelatedEventCode:
                                currentRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.GenericEventLogSequenceNumberThreePhaseCurrent:
                                currentRelatedEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            case OBISConstants.NeutralCurrent:
                                currentRelatedEvent.NuetralCurrent = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }
                    currentRelatedEventeList.Add(currentRelatedEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertCurrentRelatedEventThree : Exception ==>" + ex.Message);

                return null;
            }
            return currentRelatedEventeList;
        }

        public async Task<bool> ReadCurrentRelatedEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetCurrentEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<CurrentRelatedEventSinglePhase> currentRelatedEvent = await ConvertCurrentRelatedEventSingle(res);

                    if (currentRelatedEvent != null && currentRelatedEvent.Count > 0)
                    {
                        await _currentRelatedEventSinglePhaseService.Add(currentRelatedEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadCurrentRelatedEventSingle : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<List<CurrentRelatedEventSinglePhase>> ConvertCurrentRelatedEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<CurrentRelatedEventSinglePhase> currentRelatedEventList = new List<CurrentRelatedEventSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    CurrentRelatedEventSinglePhase currentRelatedEvent = new CurrentRelatedEventSinglePhase();

                    currentRelatedEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                currentRelatedEvent.DateAndTimeOfEvent = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.CurrentRelatedEventCode:
                                currentRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.Current:
                                currentRelatedEvent.Current = item[i] != null ? item[i].ToString() : "";
                                break;

                            case OBISConstants.Voltage:
                                currentRelatedEvent.Voltage = item[i] != null ? item[i].ToString() : "";
                                break;

                            case OBISConstants.PowerFactor:
                                currentRelatedEvent.PowerFactor = item[i] != null ? item[i].ToString() : "";
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                currentRelatedEvent.CumulativeEnergykWhExport = item[i] != null ? item[i].ToString() : "";
                                break;
                            
                            case OBISConstants.CumulativeenergykWhimport:
                                currentRelatedEvent.CumulativeEnergykWhImport = item[i] != null ? item[i].ToString() : "";
                                break;

                            case OBISConstants.TamperCount:
                                currentRelatedEvent.CumulativeTamperCount = item[i] != null ? item[i].ToString() : "";
                                break;

                            case OBISConstants.CurrentHighRelatedEventCode:
                                currentRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }
                    currentRelatedEventList.Add(currentRelatedEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadCurrentRelatedEventSingle : Exception ==>" + ex.Message);
                return null;
            }

            return currentRelatedEventList;
        }
        #endregion

        #region VoltageRelatedEvent
        public async Task<bool> ReadVoltageRelatedEventThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetVoltageEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<VoltageRelatedEvent> voltageRelatedEvent = await ConvertVoltageRelatedEventThree(res);

                    if (voltageRelatedEvent != null && voltageRelatedEvent.Count > 0)
                    {
                        await _voltageRelatedEventService.Add(voltageRelatedEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadCurrentRelatedEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<VoltageRelatedEvent>> ConvertVoltageRelatedEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<VoltageRelatedEvent> voltageRelatedEventeList = new List<VoltageRelatedEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    VoltageRelatedEvent voltageRelatedEvent = new VoltageRelatedEvent();

                    voltageRelatedEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                voltageRelatedEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.VoltageRelatedEventCode:
                                voltageRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.CurrentIr:
                                voltageRelatedEvent.CurrentIr = item[i].ToString();
                                break;

                            case OBISConstants.CurrentIy:
                                voltageRelatedEvent.CurrentIy = item[i].ToString();
                                break;

                            case OBISConstants.CurrentIb:
                                voltageRelatedEvent.CurrentIb = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVrn:
                                voltageRelatedEvent.VoltageVrn = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVyn:
                                voltageRelatedEvent.VoltageVyn = item[i].ToString();
                                break;

                            case OBISConstants.VoltageVbn:
                                voltageRelatedEvent.VoltageVbn = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorRPhase:
                                voltageRelatedEvent.SignedPowerFactorRPhase = item[i].ToString();
                                break;

                            case OBISConstants.SignedPowerFactorYPhase:
                                voltageRelatedEvent.SignedPowerFactorYPhase = item[i].ToString();
                                break;


                            case OBISConstants.SignedPowerFactorBPhase:
                                voltageRelatedEvent.SignedPowerFactorBPhase = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                voltageRelatedEvent.CumulativeEnergykWhImport = item[i].ToString();
                                break;

                            case OBISConstants.TamperCount:
                                voltageRelatedEvent.CumulativeTamperCount = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                voltageRelatedEvent.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.GenericEventLogSequenceNumberThreePhaseVoltage:
                                voltageRelatedEvent.GenericEventLogSequenceNumber = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }
                    voltageRelatedEventeList.Add(voltageRelatedEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadCurrentRelatedEventSingle : Exception ==>" + ex.Message);
                return null;
            }
            return voltageRelatedEventeList;
        }

        public async Task<bool> ReadVoltageRelatedEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetVoltageEventProfile, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<VoltageRelatedEventSinglePhase> voltageRelatedEvent = await ConvertVoltageRelatedEventSingle(res);

                    if (voltageRelatedEvent != null && voltageRelatedEvent.Count > 0)
                    {
                        await _voltageRelatedEventSinglePhaseService.Add(voltageRelatedEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadVoltageRelatedEventSingle : Exception ==>" + ex.Message);
                return false;
            }
        }
        public async Task<List<VoltageRelatedEventSinglePhase>> ConvertVoltageRelatedEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<VoltageRelatedEventSinglePhase> voltageRelatedEventeList = new List<VoltageRelatedEventSinglePhase>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    VoltageRelatedEventSinglePhase voltageRelatedEvent = new VoltageRelatedEventSinglePhase();

                    voltageRelatedEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                voltageRelatedEvent.DateandTimeofEvent = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.VoltageRelatedEventCode:
                                voltageRelatedEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.Current:
                                voltageRelatedEvent.Current = item[i].ToString();
                                break;

                            case OBISConstants.Voltage:
                                voltageRelatedEvent.Voltage = item[i].ToString();
                                break;

                            case OBISConstants.PowerFactor:
                                voltageRelatedEvent.PowerFactor = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhExport:
                                voltageRelatedEvent.CumulativeEnergykWhExport = item[i].ToString();
                                break;

                            case OBISConstants.CumulativeenergykWhimport:
                                voltageRelatedEvent.CumulativeEnergykWhImport = item[i] != null ? item[i].ToString() : "";
                                break;

                            case OBISConstants.TamperCount:
                                voltageRelatedEvent.CumulativeTamperCount = item[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }
                    voltageRelatedEventeList.Add(voltageRelatedEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadVoltageRelatedEventSingle : Exception ==>" + ex.Message+" StackTrace : "+ ex.StackTrace);
                return null;
            }
            return voltageRelatedEventeList;
        }

        #endregion

        #region DIEvent
        public async Task<bool> ReadDIThree(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.DI, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<DIEvent> dIEvent = await ConvertDIEventThree(res);

                    if (dIEvent != null && dIEvent.Count > 0)
                    {
                        await _dIEventService.Add(dIEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDIEventThree : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<DIEvent>> ConvertDIEventThree(GXDLMSProfileGeneric parsedDataList)
        {
            List<DIEvent> dIEventeList = new List<DIEvent>();

            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    DIEvent dIEvent = new DIEvent();

                    dIEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                dIEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.CheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.DIEventCode:
                                dIEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            case OBISConstants.GenericEventLogSequenceNumberDI:
                                dIEvent.GenericEventLogSequenceNumber = item[i] != null ? item[i].ToString(): "";
                                break;

                            default:
                                break;
                        }
                    }

                    dIEventeList.Add(dIEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertTransactionEventThree : Exception ==>" + ex.Message);

                return null;
            }
            return dIEventeList;
        }

        public async Task<bool> ReadDIEventSingle(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.DI, false);
                GXDLMSProfileGeneric res = t.GetProfileData(ip, startDate, endDate);

                if (res != null && res.Buffer.Count > 0)
                {
                    List<DIEventSinglePhase> dIEvent = await ConvertDIEventSingle(res);

                    if (dIEvent != null && dIEvent.Count > 0)
                    {
                        await _dIEventSinglePhaseService.Add(dIEvent);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadDIEventSingle : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<DIEventSinglePhase>> ConvertDIEventSingle(GXDLMSProfileGeneric parsedDataList)
        {
            List<DIEventSinglePhase> dIEventDto = new List<DIEventSinglePhase>();
            try
            {
                foreach (var item in parsedDataList.Buffer)
                {
                    DIEventSinglePhase dIEvent = new DIEventSinglePhase();

                    dIEvent.MeterNo = meterDetails.MeterNumber;

                    for (int i = 0; i < parsedDataList.CaptureObjects.Count; i++)
                    {
                        string key = parsedDataList.CaptureObjects[i].Key.Name.ToString();

                        switch (key)
                        {
                            case OBISConstants.Realtimeclock:
                                dIEvent.RealTimeClockDateAndTime = item[i].GetType().IsArray ? Functions.ToBillingIndiaDateTime(DateTime.Parse(Functions.ConvertByteArrayToDateTime((byte[])item[i]).ToString())) : Functions.BillingCheckIfValidDate(item[i].ToString());
                                break;

                            case OBISConstants.DIEventCode:
                                dIEvent.EventCode = item[i].ToString() + " (" + EnumHelper.GetDescription((EventCodeTypeEnum)Convert.ToInt32(item[i].ToString())) + ")";
                                break;

                            default:
                                break;
                        }
                    }
                    dIEventDto.Add(dIEvent);
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertDIEventSingle : Exception ==>" + ex.Message + " StackTrace : " + ex.StackTrace);

            }

            return dIEventDto;
        }
        #endregion

        #endregion

        #region SelfDiagnostic
        public async Task<bool> ReadSelfDiagnostic()
        {
            try
            {
                keepAliveTimer.Stop();
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetClockStatus, false);
                int res = t.ReadGDLMSClock(ip);
                keepAliveTimer.Start();

                List<Domain.Entities.SelfDiagnostic> selfDiagnostic = await ConvertSelfDiagnostic(res);

                if (selfDiagnostic != null && selfDiagnostic.Count > 0)
                {
                    await _selfDiagnosticService.Add(selfDiagnostic);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification,Constants.SelfDiagnosticDownloadedSuccessFullyCount + selfDiagnostic.Count, NotificationType.Success, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationManager notificationManager = new NotificationManager();

                        notificationManager.Show(Constants.Notification, Constants.SelfDiagnosticDownloadFailed, NotificationType.Error, CloseOnClick: true);

                    }, System.Windows.Threading.DispatcherPriority.Send);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadVoltageRelatedEventSingle : Exception ==>" + ex.Message);
                return false;
            }
        }
        public async Task<List<Domain.Entities.SelfDiagnostic>> ConvertSelfDiagnostic(int parsedDataList)
        {
            List<Domain.Entities.SelfDiagnostic> selfDiagnosticList = new List<Domain.Entities.SelfDiagnostic>();
            try
            {
                Domain.Entities.SelfDiagnostic selfDiagnostic = new Domain.Entities.SelfDiagnostic();
                selfDiagnostic.MeterNo = meterDetails.MeterNumber;
                selfDiagnostic.Status = parsedDataList.ToString();

                selfDiagnosticList.Add(selfDiagnostic);

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertSelfDiagnostic : Exception ==>" + ex.Message);

                return null;
            }
            return selfDiagnosticList;
        }


        #endregion

        #region ESW Methods

        public async Task<bool> ReadESWSinglePhase()
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetESWF, false);

                string res = t.ReadData(ip);

                if (!string.IsNullOrEmpty(res))
                {
                    List<ESWSinglePhaseDto> esw = await ConvertToESWSinglePhase(res);

                    if (esw != null)
                    {
                        await _eswSinglePhaseService.Add(esw);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadESWSinglePhase : Exception ==>" + ex.Message);

                return false;
            }
        }

        public async Task<List<ESWSinglePhaseDto>> ConvertToESWSinglePhase(string parsedDataList)
        {
            List<ESWSinglePhaseDto> eswSinglePhaseList = new List<ESWSinglePhaseDto>();
            try
            {
                var parsedData = parsedDataList.ToCharArray();

                ESWSinglePhaseDto eSWSinglePhaseDto = new ESWSinglePhaseDto();

                eSWSinglePhaseDto.MeterNo = meterDetails.MeterNumber;
                eSWSinglePhaseDto.OverVoltage = Convert.ToBoolean(Convert.ToByte(parsedData[3].ToString()));
                eSWSinglePhaseDto.LowVoltage = Convert.ToBoolean(Convert.ToByte(parsedData[4].ToString()));
                eSWSinglePhaseDto.OverCurrent = Convert.ToBoolean(Convert.ToByte(parsedData[11].ToString()));
                eSWSinglePhaseDto.VerylowPF = Convert.ToBoolean(Convert.ToByte(parsedData[12].ToString()));
                eSWSinglePhaseDto.EarthLoading = Convert.ToBoolean(Convert.ToByte(parsedData[51].ToString()));
                eSWSinglePhaseDto.InfluenceOfPermanetMagnetOorAcDc = Convert.ToBoolean(Convert.ToByte(parsedData[81].ToString()));
                eSWSinglePhaseDto.NeutralDisturbance = Convert.ToBoolean(Convert.ToByte(parsedData[82].ToString()));
                eSWSinglePhaseDto.MeterCoverOpen = Convert.ToBoolean(Convert.ToByte(parsedData[83].ToString()));
                eSWSinglePhaseDto.MeterLoadDisconnectConnected = Convert.ToBoolean(Convert.ToByte(parsedData[84].ToString()));
                eSWSinglePhaseDto.LastGasp = Convert.ToBoolean(Convert.ToByte(parsedData[85].ToString()));
                eSWSinglePhaseDto.FirstBreath = Convert.ToBoolean(Convert.ToByte(parsedData[86].ToString()));
                eSWSinglePhaseDto.IncrementInBillingCounterMRI = Convert.ToBoolean(Convert.ToByte(parsedData[87].ToString()));

                eswSinglePhaseList.Add(eSWSinglePhaseDto);

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToESWSinglePhase : Exception ==>" + ex.Message);


            }
            return eswSinglePhaseList;
        }

        public async Task<bool> ReadESWThreePhase()
        {
            try
            {
                string ip = Functions.GetOBISCodeByCommandType((int)CommandTypeEnum.GetESWF, false);

                string res = t.ReadData(ip);

                if (!string.IsNullOrEmpty(res))
                {
                    List<ESWThreePhaseDto> esw = await ConvertToESWThreePhase(res);

                    if (esw != null)
                    {
                        await _eswThreePhaseSerice.Add(esw);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ReadESWSinglePhase : Exception ==>" + ex.Message);
                return false;
            }
        }

        public async Task<List<ESWThreePhaseDto>> ConvertToESWThreePhase(string parsedDataList)
        {
            List<ESWThreePhaseDto> eswThreePhaseList = new List<ESWThreePhaseDto>();
            try
            {
                var parsedData = parsedDataList.ToCharArray();

                ESWThreePhaseDto eSWThreePhaseDto = new ESWThreePhaseDto();

                eSWThreePhaseDto.MeterNo = meterDetails.MeterNumber;
                eSWThreePhaseDto.RPhaseVoltageMissing = Convert.ToBoolean(Convert.ToByte(parsedData[0].ToString()));
                eSWThreePhaseDto.YPhaseVoltageMissing = Convert.ToBoolean(Convert.ToByte(parsedData[1].ToString()));
                eSWThreePhaseDto.BPhaseVoltageMissing = Convert.ToBoolean(Convert.ToByte(parsedData[2].ToString()));
                eSWThreePhaseDto.OverVoltage = Convert.ToBoolean(Convert.ToByte(parsedData[3].ToString()));
                eSWThreePhaseDto.LowVoltage = Convert.ToBoolean(Convert.ToByte(parsedData[4].ToString()));
                eSWThreePhaseDto.VoltagUnbalance = Convert.ToBoolean(Convert.ToByte(parsedData[5].ToString()));
                eSWThreePhaseDto.RPhaseCurrentReverse = Convert.ToBoolean(Convert.ToByte(parsedData[6].ToString()));
                eSWThreePhaseDto.YPhaseCurrentReverse = Convert.ToBoolean(Convert.ToByte(parsedData[7].ToString()));
                eSWThreePhaseDto.BPhaseCurrentReverse = Convert.ToBoolean(Convert.ToByte(parsedData[8].ToString()));
                eSWThreePhaseDto.CurrentUnbalance = Convert.ToBoolean(Convert.ToByte(parsedData[9].ToString()));
                eSWThreePhaseDto.CurrentBypass = Convert.ToBoolean(Convert.ToByte(parsedData[10].ToString()));
                eSWThreePhaseDto.OverCurrent = Convert.ToBoolean(Convert.ToByte(parsedData[11].ToString()));
                eSWThreePhaseDto.VerylowPF = Convert.ToBoolean(Convert.ToByte(parsedData[12].ToString()));
                eSWThreePhaseDto.InfluenceOfPermanetMagnetOorAcDc = Convert.ToBoolean(Convert.ToByte(parsedData[81].ToString()));
                eSWThreePhaseDto.NeutralDisturbance = Convert.ToBoolean(Convert.ToByte(parsedData[82].ToString()));
                eSWThreePhaseDto.MeterCoverOpen = Convert.ToBoolean(Convert.ToByte(parsedData[83].ToString()));
                eSWThreePhaseDto.MeterLoadDisconnectConnected = Convert.ToBoolean(Convert.ToByte(parsedData[84].ToString()));
                eSWThreePhaseDto.LastGasp = Convert.ToBoolean(Convert.ToByte(parsedData[85].ToString()));
                eSWThreePhaseDto.FirstBreath = Convert.ToBoolean(Convert.ToByte(parsedData[86].ToString()));
                eSWThreePhaseDto.IncrementInBillingCounterMRI = Convert.ToBoolean(Convert.ToByte(parsedData[87].ToString()));

                eswThreePhaseList.Add(eSWThreePhaseDto);

            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : Meter Method Commands : ConvertToESWThreePhase : Exception ==>" + ex.Message);
            }
            return eswThreePhaseList;
        }

        #endregion

        private void StartKeepAliveTimer()
        {
            if (keepAliveTimer == null)
            {
                keepAliveTimer = new System.Timers.Timer();
                keepAliveTimer.Interval = 15000;
                keepAliveTimer.Elapsed += async (sender, e) => await t.SendKeepAlivePacket();
                keepAliveTimer.AutoReset = true;
            }

            keepAliveTimer.Start();
        }

        private void StopKeepAliveTimer()
        {
            keepAliveTimer.Stop();
        }

    }
}
