using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class AllEventsSinglePhaseService
    {
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public AllEventsSinglePhaseService()
        {
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();           
        }
      

        public async Task<List<AllEventsSinglePhaseDto>> GetAll(string meterNumber)
        {
            try
            {
                List<AllEventsSinglePhaseDto> allEventsData = new List<AllEventsSinglePhaseDto>();

                var controlEvent = await ControlEventSinglePhase(meterNumber);
                if(controlEvent.Count > 0)
                {
                    foreach(var item in controlEvent)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var nonRollOverEvent = await NonRolloverEventSinglePhase(meterNumber);
                if (nonRollOverEvent.Count > 0)
                {
                    foreach (var item in nonRollOverEvent)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var powerRelatedEvent = await PowerRelatedEventSinglePhase(meterNumber);
                if (powerRelatedEvent.Count > 0)
                {
                    for (int i = 0; i < powerRelatedEvent.Count; i++)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = powerRelatedEvent[i].RealTimeClockDateAndTime;
                        data.Event = powerRelatedEvent[i].EventCode;
                        data.CreatedOn = powerRelatedEvent[i].CreatedOn;

                        var time = i == 0 ? "" : (powerRelatedEvent[i].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Restoration).ToString()) && powerRelatedEvent[i - 1].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Occurrence).ToString())) ?  ((DateTime.ParseExact(powerRelatedEvent[i].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.ParseExact(powerRelatedEvent[i - 1].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).TotalMinutes).ToString() : "";
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

                var transactionEvent = await TransactionEventSinglePhase(meterNumber);
                if (transactionEvent.Count > 0)
                {
                    foreach (var item in transactionEvent)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = item.RealTimeClockDateAndTime;
                        data.Event = item.EventCode;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var currentRelatedEvent = await CurrentRelatedEventSinglePhase(meterNumber);
                if (currentRelatedEvent.Count > 0)
                {
                    foreach (var item in currentRelatedEvent)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = item.DateAndTimeOfEvent;
                        data.Event = item.EventCode;
                        data.Current = item.Current;
                        data.Voltage =  (double.Parse(item.Voltage, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                        data.PowerFactor =  (double.Parse(item.PowerFactor, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                        data.CumulativeEnergykWhImport = StringExtensions.CheckNullable(item.CumulativeEnergykWhImport);
                        data.CumulativeEnergykWhExport = StringExtensions.CheckNullable(item.CumulativeEnergykWhExport);
                        data.CumulativeTamperCount = item.CumulativeTamperCount;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var voltageRelatedEvent = await VoltageRelatedEventSinglePhase(meterNumber);
                if (voltageRelatedEvent.Count > 0)
                {
                    foreach (var item in voltageRelatedEvent)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = item.DateandTimeofEvent;
                        data.Event = item.EventCode;
                        data.Current = StringExtensions.CheckNullableOnly(item.Current);
                        data.Voltage = StringExtensions.CheckNullableOnly(item.Voltage);
                        data.PowerFactor = StringExtensions.CheckNullableOnly(item.PowerFactor);
                        data.CumulativeEnergykWhImport = StringExtensions.CheckNullable(item.CumulativeEnergykWhImport);
                        data.CumulativeEnergykWhExport = StringExtensions.CheckNullable(item.CumulativeEnergykWhExport);
                        data.CumulativeTamperCount = item.CumulativeTamperCount;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }

                var otherEvent = await OtherEventSinglePhase(meterNumber);
                if (otherEvent.Count > 0)
                {
                    foreach (var item in otherEvent)
                    {
                        AllEventsSinglePhaseDto data = new AllEventsSinglePhaseDto();
                        data.RealTimeClock = item.DateandTimeofEvent;
                        data.Event = item.EventCode;
                        data.Current = StringExtensions.CheckNullableOnly(item.Current);
                        data.Voltage = StringExtensions.CheckNullableOnly(item.Voltage);
                        data.PowerFactor = StringExtensions.CheckNullableOnly(item.PowerFactor);
                        data.CumulativeEnergykWhImport = StringExtensions.CheckNullable(item.CumulativeEnergykWhImport);
                        data.CumulativeEnergykWhExport = StringExtensions.CheckNullable(item.CumulativeEnergykWhExport);
                        data.CumulativeTamperCount = item.CumulativeTamperCount;
                        data.CreatedOn = item.CreatedOn;

                        allEventsData.Add(data);
                    }
                }



                return allEventsData;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : GetAllEvents : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ControlEventSinglePhase>> ControlEventSinglePhase(string meterNumber)
        {
            try
            {
                List<ControlEventSinglePhase> controlEventSinglePhase = new List<ControlEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from ControlEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    controlEventSinglePhase = await context.Set<ControlEventSinglePhase>().FromSqlRaw(query).ToListAsync();
                }

                return controlEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : ControlEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<NonRolloverEventSinglePhase>> NonRolloverEventSinglePhase(string meterNumber)
        {
            try
            {
                List<NonRolloverEventSinglePhase> nonRolloverEventSinglePhase = new List<NonRolloverEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from NonRolloverEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    nonRolloverEventSinglePhase = await context.Set<NonRolloverEventSinglePhase>().FromSqlRaw(query).ToListAsync();
                }

                return nonRolloverEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : NonRolloverEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<PowerRelatedEventSinglePhase>> PowerRelatedEventSinglePhase(string meterNumber)
        {
            try
            {
                List<PowerRelatedEventSinglePhase> powerRelatedEventSinglePhase = new List<PowerRelatedEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from PowerRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    powerRelatedEventSinglePhase = await context.Set<PowerRelatedEventSinglePhase>().FromSqlRaw(query).ToListAsync();

                    if (powerRelatedEventSinglePhase.Any())
                    {
                        powerRelatedEventSinglePhase = powerRelatedEventSinglePhase.OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
                    }                  
                }

                return powerRelatedEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : PowerRelatedEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<TransactionEventSinglePhase>> TransactionEventSinglePhase(string meterNumber)
        {
            try
            {
                List<TransactionEventSinglePhase> transactionEventSinglePhase = new List<TransactionEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from TransactionEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    transactionEventSinglePhase = await context.Set<TransactionEventSinglePhase>().FromSqlRaw(query).ToListAsync();
                }

                return transactionEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : TransactionEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<CurrentRelatedEventSinglePhase>> CurrentRelatedEventSinglePhase(string meterNumber)
        {
            try
            {
                List<CurrentRelatedEventSinglePhase> currentRelatedEventSinglePhase = new List<CurrentRelatedEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from CurrentRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    currentRelatedEventSinglePhase = await context.Set<CurrentRelatedEventSinglePhase>().FromSqlRaw(query).ToListAsync();
                }

                return currentRelatedEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + ": AllEvents : CurrentRelatedEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<VoltageRelatedEventSinglePhase>> VoltageRelatedEventSinglePhase(string meterNumber)
        {
            try
            {
                List<VoltageRelatedEventSinglePhase> voltageRelatedEventSinglePhase = new List<VoltageRelatedEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from VoltageRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    voltageRelatedEventSinglePhase = await context.Set<VoltageRelatedEventSinglePhase>().FromSqlRaw(query).ToListAsync();
                }

                return voltageRelatedEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : VoltageRelatedEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<OtherEventSinglePhase>> OtherEventSinglePhase(string meterNumber)
        {
            try
            {
                List<OtherEventSinglePhase> otherEventSinglePhase = new List<OtherEventSinglePhase>();

                using (ApplicationDBContext context = _contextFactory.CreateDbContext())
                {
                    string query = "select * from OtherEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    otherEventSinglePhase = await context.Set<OtherEventSinglePhase>().FromSqlRaw(query).ToListAsync();
                }

                return otherEventSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(DateTime.UtcNow + " : AllEvents : OtherEventSinglePhase : Exception ==>" + ex.Message);
                throw new Exception(ex.Message);
            }
        }


    }
}