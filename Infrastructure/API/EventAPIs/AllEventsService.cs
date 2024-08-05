using Domain.Entities;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.API.EventAPIs
{
    public class AllEventsService
    {
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;

        public AllEventsService()
        {
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }


        public async Task<List<AllEventsDTO>> GetAll(string meterNumber)
        {
            try
            {
                var meter = await _contextFactory.CreateDbContext().Meter.FirstOrDefaultAsync(x => x.MeterNo == meterNumber);
                List<AllEventsDTO> allEventsData = new List<AllEventsDTO>();

                var controlEvent = await ControlEvent(meterNumber);
                if (controlEvent.Count > 0)
                {
                    foreach (var item in controlEvent)
                    {
                        AllEventsDTO data = new AllEventsDTO();
                        data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.CreatedOn = item.CreatedOn;
                        data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                        allEventsData.Add(data);
                    }
                }

                var nonRollOverEvent = await NonRolloverEvent(meterNumber);
                if (nonRollOverEvent.Count > 0)
                {
                    foreach (var item in nonRollOverEvent)
                    {
                        AllEventsDTO data = new AllEventsDTO();
                        data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var powerRelatedEvent = await PowerRelatedEvent(meterNumber);
                if (powerRelatedEvent.Count > 0)
                {
                    for (int i = 0; i < powerRelatedEvent.Count; i++)
                    {
                        AllEventsDTO data = new AllEventsDTO();
                        data.RealTimeClockDateAndTime = powerRelatedEvent[i].RealTimeClockDateAndTime;
                        data.Event = powerRelatedEvent[i].EventCode;
                        data.GenericEventLogSequenceNumber = powerRelatedEvent[i].GenericEventLogSequenceNumber;
                        data.CreatedOn = powerRelatedEvent[i].CreatedOn;

                        var time = i == 0 ? "" : (powerRelatedEvent[i].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Restoration).ToString()) && powerRelatedEvent[i - 1].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Occurrence).ToString())) ? ((DateTime.ParseExact(powerRelatedEvent[i].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.ParseExact(powerRelatedEvent[i - 1].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).TotalMinutes).ToString() : "";
                        if (!string.IsNullOrEmpty(time))
                        {
                            double totalMinutes = double.Parse(time);

                            int hours = (int)(totalMinutes / 60);
                            int minutes = (int)(totalMinutes % 60);
                            int seconds = (int)((totalMinutes * 60) % 60);

                            data.PowerFailureTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                        }

                        allEventsData.Add(data);
                    }
                }

                var transactionEvent = await TransactionEvent(meterNumber);
                if (transactionEvent.Count > 0)
                {
                    foreach (var item in transactionEvent)
                    {
                        AllEventsDTO data = new AllEventsDTO();
                        data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var currentRelatedEvent = await CurrentRelatedEvent(meterNumber);
                if (currentRelatedEvent.Count > 0)
                {
                    if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                    {
                        foreach (var item in currentRelatedEvent)
                        {
                            AllEventsDTO data = new AllEventsDTO();
                            data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                            data.Event = item.EventCode;
                            data.CurrentIr = (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIy = (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIb = (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVrn = (double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVyn = (double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVbn = (double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorRPhase = (double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorYPhase = (double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorBPhase = (double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.CumulativeEnergykWhImport = (double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CumulativeTamperCount = item.CumulativeTamperCount;
                            data.CumulativeEnergykWhExport = (double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CreatedOn = item.CreatedOn;
                            data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                            allEventsData.Add(data);
                        }
                    }
                    else
                    {
                        foreach (var item in currentRelatedEvent)
                        {
                            AllEventsDTO data = new AllEventsDTO();
                            data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                            data.Event = item.EventCode;
                            data.CurrentIr = (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIy = (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIb = (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVrn = (double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVyn = (double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVbn = (double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorRPhase = (double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorYPhase = (double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorBPhase = (double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.CumulativeEnergykWhImport = (double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CumulativeTamperCount = item.CumulativeTamperCount;
                            data.CumulativeEnergykWhExport = (double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CreatedOn = item.CreatedOn;
                            data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                            allEventsData.Add(data);
                        }
                    }

                }

                var voltageRelatedEvent = await VoltageRelatedEvent(meterNumber);
                if (voltageRelatedEvent.Count > 0)
                {
                    if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                    {
                        foreach (var item in voltageRelatedEvent)
                        {
                            AllEventsDTO data = new AllEventsDTO();
                            data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                            data.Event = item.EventCode;
                            data.CurrentIr = (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIy = (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIb = (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVrn = (double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVyn = (double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVbn = (double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorRPhase = (double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorYPhase = (double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorBPhase = (double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CumulativeEnergykWhImport = (double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CumulativeTamperCount = item.CumulativeTamperCount;
                            data.CumulativeEnergykWhExport = (double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CreatedOn = item.CreatedOn;
                            data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                            allEventsData.Add(data);
                        }
                    }
                    else
                    {
                        foreach (var item in voltageRelatedEvent)
                        {
                            AllEventsDTO data = new AllEventsDTO();
                            data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                            data.Event = item.EventCode;
                            data.CurrentIr = (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any)).ToString();
                            data.CurrentIy = (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any)).ToString();
                            data.CurrentIb = (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any)).ToString();
                            data.VoltageVrn = (double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString();
                            data.VoltageVyn = (double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString();
                            data.VoltageVbn = (double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorRPhase = (double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorYPhase = (double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorBPhase = (double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.CumulativeEnergykWhImport = (double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CumulativeTamperCount = item.CumulativeTamperCount;
                            data.CumulativeEnergykWhExport = (double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CreatedOn = item.CreatedOn;
                            data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                            allEventsData.Add(data);
                        }
                    }
                }

                var otherEvent = await OtherEvent(meterNumber);
                if (otherEvent.Count > 0)
                {
                    if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                    {
                        foreach (var item in otherEvent)
                        {
                            AllEventsDTO data = new AllEventsDTO();
                            data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                            data.Event = item.EventCode;
                            data.CurrentIr = (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any) / 100) == 0 ? "0" : (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIy = (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any) / 100) == 0 ? "0" : (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIb = (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any) / 100) == 0 ? "0" : (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVrn = (double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVyn = (double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVbn = (double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorRPhase = (double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorYPhase = (double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorBPhase = (double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.CumulativeEnergykWhImport = (double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CumulativeTamperCount = item.CumulativeTamperCount;
                            data.CumulativeEnergykWhExport = (double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CreatedOn = item.CreatedOn;
                            data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                            allEventsData.Add(data);
                        }
                    }
                    else
                    {
                        foreach (var item in otherEvent)
                        {
                            AllEventsDTO data = new AllEventsDTO();
                            data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                            data.Event = item.EventCode;
                            data.CurrentIr = (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any) / 1000) == 0 ? "0" : (double.Parse(item.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIy = (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any) / 1000) == 0 ? "0" : (double.Parse(item.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.CurrentIb = (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any) / 1000) == 0 ? "0" : (double.Parse(item.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVrn = (double.Parse(item.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVyn = (double.Parse(item.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.VoltageVbn = (double.Parse(item.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                            data.SignedPowerFactorRPhase = (double.Parse(item.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorYPhase = (double.Parse(item.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.SignedPowerFactorBPhase = (double.Parse(item.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                            data.CumulativeEnergykWhImport = (double.Parse(item.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CumulativeTamperCount = item.CumulativeTamperCount;
                            data.CumulativeEnergykWhExport = (double.Parse(item.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                            data.CreatedOn = item.CreatedOn;
                            data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                            allEventsData.Add(data);
                        }
                    }
                }

                var DIEvent = await DIEventThreePhase(meterNumber);
                if (DIEvent.Count > 0)
                {
                    foreach (var item in DIEvent)
                    {
                        AllEventsDTO data = new AllEventsDTO();

                        data.RealTimeClockDateAndTime = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.GenericEventLogSequenceNumber = item.GenericEventLogSequenceNumber;

                        allEventsData.Add(data);
                    }
                }

                return allEventsData;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : GetAllEvents : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ControlEvent>> ControlEvent(string meterNumber)
        {
            try
            {
                List<ControlEvent> controlEventThreePhase = new List<ControlEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from ControlEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    controlEventThreePhase = await context.Set<ControlEvent>().FromSqlRaw(query).ToListAsync();
                }

                return controlEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : ControlEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<NonRolloverEvent>> NonRolloverEvent(string meterNumber)
        {
            try
            {
                List<NonRolloverEvent> nonRolloverEventThreePhase = new List<NonRolloverEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from NonRolloverEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    nonRolloverEventThreePhase = await context.Set<NonRolloverEvent>().FromSqlRaw(query).ToListAsync();
                }

                return nonRolloverEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : NonRolloverEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<PowerRelatedEvent>> PowerRelatedEvent(string meterNumber)
        {
            try
            {
                List<PowerRelatedEvent> powerRelatedEventThreePhase = new List<PowerRelatedEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from PowerRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    powerRelatedEventThreePhase = await context.Set<PowerRelatedEvent>().FromSqlRaw(query).ToListAsync();

                    if (powerRelatedEventThreePhase.Any())
                    {
                        powerRelatedEventThreePhase = powerRelatedEventThreePhase.OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
                    }
                }

                return powerRelatedEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : PowerRelatedEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<TransactionEvent>> TransactionEvent(string meterNumber)
        {
            try
            {
                List<TransactionEvent> transactionEventThreePhase = new List<TransactionEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from TransactionEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    transactionEventThreePhase = await context.Set<TransactionEvent>().FromSqlRaw(query).ToListAsync();
                }

                return transactionEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : TransactionEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<CurrentRelatedEvent>> CurrentRelatedEvent(string meterNumber)
        {
            try
            {
                List<CurrentRelatedEvent> currentRelatedEventThreePhase = new List<CurrentRelatedEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from CurrentRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    currentRelatedEventThreePhase = await context.Set<CurrentRelatedEvent>().FromSqlRaw(query).ToListAsync();
                }

                return currentRelatedEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : CurrentRelatedEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<VoltageRelatedEvent>> VoltageRelatedEvent(string meterNumber)
        {
            try
            {
                List<VoltageRelatedEvent> voltageRelatedEventThreePhase = new List<VoltageRelatedEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from VoltageRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    voltageRelatedEventThreePhase = await context.Set<VoltageRelatedEvent>().FromSqlRaw(query).ToListAsync();
                }

                return voltageRelatedEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : VoltageRelatedEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<OtherEvent>> OtherEvent(string meterNumber)
        {
            try
            {
                List<OtherEvent> otherEventThreePhase = new List<OtherEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from OtherEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    otherEventThreePhase = await context.Set<OtherEvent>().FromSqlRaw(query).ToListAsync();
                }

                return otherEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : OtherEvent : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DIEvent>> DIEventThreePhase(string meterNumber)
        {
            try
            {
                List<DIEvent> dIEventThreePhase = new List<DIEvent>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from DIEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    dIEventThreePhase = await context.Set<DIEvent>().FromSqlRaw(query).ToListAsync();
                }

                return dIEventThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEventsThreePhase : dIEventThreePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}