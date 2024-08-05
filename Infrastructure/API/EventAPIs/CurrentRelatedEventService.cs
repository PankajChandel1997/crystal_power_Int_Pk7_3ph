using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIs
{
    public class CurrentRelatedEventService
    {
        private readonly IDataService<CurrentRelatedEvent> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public CurrentRelatedEventService()
        {
            _dataService = new GenericDataService<CurrentRelatedEvent>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<CurrentRelatedEvent> currentRelatedEventModel)
        {
            try
            {
                await Delete(currentRelatedEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(currentRelatedEventModel);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> Delete(string meterNumber)
        {
            try
            {
                using (ApplicationDBContext db = _contextFactory.CreateDbContext())
                {
                    string query = "select * from CurrentRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<CurrentRelatedEvent>().RemoveRange(res);
                            await db.SaveChangesAsync();
                        }
                };
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<CurrentRelatedEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from CurrentRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var currentRelatedEventDto = new List<CurrentRelatedEventDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    currentRelatedEventDto = await ParseDataToDTO(response);
                }
                else
                {
                    currentRelatedEventDto = await ParseDataToDTOUMD(response);
                }

                return currentRelatedEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CurrentRelatedEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from CurrentRelatedEvent where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var currentRelatedEventDto = new List<CurrentRelatedEventDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion ) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    currentRelatedEventDto = await ParseDataToDTO(response);
                }
                else
                {
                    currentRelatedEventDto = await ParseDataToDTOUMD(response);
                }

                return currentRelatedEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<CurrentRelatedEvent>> ParseDataToClass(List<CurrentRelatedEventDto> currentRelatedEventDtoList)
        {
            List<CurrentRelatedEvent> currentRelatedEventList = new List<CurrentRelatedEvent>();

            foreach (var currentRelatedEventDto in currentRelatedEventDtoList)
            {
                CurrentRelatedEvent currentRelatedEvent = new CurrentRelatedEvent();

                currentRelatedEvent.MeterNo = currentRelatedEventDto.MeterNo;
                currentRelatedEvent.CreatedOn = currentRelatedEventDto.CreatedOn;
                currentRelatedEvent.RealTimeClockDateAndTime = currentRelatedEventDto.RealTimeClockDateAndTime;
                currentRelatedEvent.EventCode = currentRelatedEventDto.Event;
                currentRelatedEvent.CurrentIr = currentRelatedEventDto.CurrentIr;
                currentRelatedEvent.CurrentIy = currentRelatedEventDto.CurrentIy;
                currentRelatedEvent.CurrentIb = currentRelatedEventDto.CurrentIb;
                currentRelatedEvent.VoltageVrn = currentRelatedEventDto.VoltageVrn;
                currentRelatedEvent.VoltageVyn = currentRelatedEventDto.VoltageVyn;
                currentRelatedEvent.VoltageVbn = currentRelatedEventDto.VoltageVbn;
                currentRelatedEvent.SignedPowerFactorRPhase = currentRelatedEventDto.SignedPowerFactorRPhase;
                currentRelatedEvent.SignedPowerFactorYPhase = currentRelatedEventDto.SignedPowerFactorYPhase;
                currentRelatedEvent.SignedPowerFactorBPhase = currentRelatedEventDto.SignedPowerFactorBPhase;
                currentRelatedEvent.CumulativeEnergykWhImport = currentRelatedEventDto.CumulativeEnergykWhImport;
                currentRelatedEvent.CumulativeTamperCount = currentRelatedEventDto.CumulativeTamperCount;
                currentRelatedEvent.CumulativeEnergykWhExport = currentRelatedEventDto.CumulativeEnergykWhExport;

                currentRelatedEventList.Add(currentRelatedEvent);
            }

            return currentRelatedEventList;
        }

        private async Task<List<CurrentRelatedEventDto>> ParseDataToDTO(List<CurrentRelatedEvent> currentRelatedEventList)
        {
            int index = 1;
            List<CurrentRelatedEventDto> currentRelatedEventDtoList = new List<CurrentRelatedEventDto>();
            foreach (var currentRelatedEvent in currentRelatedEventList)
            {
                CurrentRelatedEventDto currentRelatedEventDto = new CurrentRelatedEventDto();

                currentRelatedEventDto.Number = index;
                currentRelatedEventDto.RealTimeClockDateAndTime = currentRelatedEvent.RealTimeClockDateAndTime;
                currentRelatedEventDto.Event = currentRelatedEvent.EventCode;
                currentRelatedEventDto.CurrentIr = (double.Parse(currentRelatedEvent.CurrentIr, System.Globalization.NumberStyles.Any) /100).ToString().CustomTrucate();
                currentRelatedEventDto.CurrentIy = (double.Parse(currentRelatedEvent.CurrentIy, System.Globalization.NumberStyles.Any) / 100).ToString().CustomTrucate();
                currentRelatedEventDto.CurrentIb = (double.Parse(currentRelatedEvent.CurrentIb, System.Globalization.NumberStyles.Any) / 100).ToString().CustomTrucate();
                currentRelatedEventDto.VoltageVrn = (double.Parse(currentRelatedEvent.VoltageVrn,System.Globalization.NumberStyles.Any)/10).ToString().CustomTrucate();
                currentRelatedEventDto.VoltageVyn = (double.Parse(currentRelatedEvent.VoltageVyn, System.Globalization.NumberStyles.Any) / 10).ToString().CustomTrucate();
                currentRelatedEventDto.VoltageVbn = (double.Parse(currentRelatedEvent.VoltageVbn, System.Globalization.NumberStyles.Any) / 10).ToString().CustomTrucate();
                currentRelatedEventDto.SignedPowerFactorRPhase = (double.Parse(currentRelatedEvent.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any) / 100).ToString().CustomTrucate();
                currentRelatedEventDto.SignedPowerFactorYPhase = (double.Parse(currentRelatedEvent.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any) / 100).ToString().CustomTrucate();
                currentRelatedEventDto.SignedPowerFactorBPhase = (double.Parse(currentRelatedEvent.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any) / 100).ToString().CustomTrucate();
                currentRelatedEventDto.CumulativeEnergykWhImport = (double.Parse(currentRelatedEvent.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any)/100).ToString().CustomTrucate();
                currentRelatedEventDto.CumulativeTamperCount = currentRelatedEvent.CumulativeTamperCount;
                currentRelatedEventDto.CumulativeEnergykWhExport = (double.Parse(currentRelatedEvent.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 100).ToString().CustomTrucate();
                currentRelatedEventDto.CreatedOn = currentRelatedEvent.CreatedOn;
                currentRelatedEventDto.GenericEventLogSequenceNumber = currentRelatedEvent.GenericEventLogSequenceNumber;

                currentRelatedEventDtoList.Add(currentRelatedEventDto);
                index++;
            }

            return currentRelatedEventDtoList;
        }

        private async Task<List<CurrentRelatedEventDto>> ParseDataToDTOUMD(List<CurrentRelatedEvent> currentRelatedEventList)
        {
            int index = 1;
            List<CurrentRelatedEventDto> currentRelatedEventDtoList = new List<CurrentRelatedEventDto>();
            foreach (var currentRelatedEvent in currentRelatedEventList)
            {
                CurrentRelatedEventDto currentRelatedEventDto = new CurrentRelatedEventDto();

                currentRelatedEventDto.Number = index;
                currentRelatedEventDto.RealTimeClockDateAndTime = currentRelatedEvent.RealTimeClockDateAndTime;
                currentRelatedEventDto.Event = currentRelatedEvent.EventCode;
                currentRelatedEventDto.CurrentIr = (double.Parse(currentRelatedEvent.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                currentRelatedEventDto.CurrentIy = (double.Parse(currentRelatedEvent.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                currentRelatedEventDto.CurrentIb = (double.Parse(currentRelatedEvent.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                currentRelatedEventDto.VoltageVrn =(double.Parse(currentRelatedEvent.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                currentRelatedEventDto.VoltageVyn = (double.Parse(currentRelatedEvent.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                currentRelatedEventDto.VoltageVbn = (double.Parse(currentRelatedEvent.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                currentRelatedEventDto.SignedPowerFactorRPhase = (double.Parse(currentRelatedEvent.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                currentRelatedEventDto.SignedPowerFactorYPhase = (double.Parse(currentRelatedEvent.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                currentRelatedEventDto.SignedPowerFactorBPhase = (double.Parse(currentRelatedEvent.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                currentRelatedEventDto.CumulativeEnergykWhImport = (double.Parse(currentRelatedEvent.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                currentRelatedEventDto.CumulativeTamperCount = currentRelatedEvent.CumulativeTamperCount;
                currentRelatedEventDto.CumulativeEnergykWhExport = (double.Parse(currentRelatedEvent.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                currentRelatedEventDto.CreatedOn = currentRelatedEvent.CreatedOn;
                currentRelatedEventDto.GenericEventLogSequenceNumber = currentRelatedEvent.GenericEventLogSequenceNumber;
                //currentRelatedEventDto.NeutralCurrent = double.Parse(currentRelatedEvent.NuetralCurrent).ToString().CustomTrucate();

                currentRelatedEventDtoList.Add(currentRelatedEventDto);
                index++;
            }

            return currentRelatedEventDtoList;
        }
    }
}