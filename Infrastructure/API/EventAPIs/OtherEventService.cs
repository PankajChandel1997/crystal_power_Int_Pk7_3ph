using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIs
{
    public class OtherEventService
    {
        private readonly IDataService<OtherEvent> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public OtherEventService()
        {
            _dataService = new GenericDataService<OtherEvent>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<OtherEvent> otherEventModel)
        {
            try
            {
                await Delete(otherEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(otherEventModel);
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
                    string query = "select * from OtherEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<OtherEvent>().RemoveRange(res);
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
        public async Task<List<OtherEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from OtherEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var otherEventDto = new List<OtherEventDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    otherEventDto = await ParseDataToDTO(response);
                }
                else
                {
                    otherEventDto = await ParseDataToDTOUMD(response);
                }

                return otherEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OtherEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from OtherEvent where MeterNo = '" + meterNumber + "' order by RealTimeClockDateAndTime desc";

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

                var otherEventDto = new List<OtherEventDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion) && meter.MeterType != Domain.Enums.MeterType.ThreePhaseLTCT && meter.MeterType != Domain.Enums.MeterType.ThreePhaseHTCT)
                {
                    otherEventDto = await ParseDataToDTO(response);
                }
                else
                {
                    otherEventDto = await ParseDataToDTOUMD(response);
                }

                return otherEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<OtherEvent>> ParseDataToClass(List<OtherEventDto> otherEventDtoList)
        {
            List<OtherEvent> otherEventList = new List<OtherEvent>();

            foreach (var otherEventDto in otherEventDtoList)
            {
                OtherEvent otherEvent = new OtherEvent();

                otherEvent.MeterNo = otherEventDto.MeterNo;
                otherEvent.CreatedOn = otherEventDto.CreatedOn;
                otherEvent.RealTimeClockDateAndTime = otherEventDto.RealTimeClockDateAndTime;
                otherEvent.EventCode = otherEventDto.Event;
                otherEvent.CurrentIr = otherEventDto.CurrentIr;
                otherEvent.CurrentIy = otherEventDto.CurrentIy;
                otherEvent.CurrentIb = otherEventDto.CurrentIb;
                otherEvent.VoltageVrn = otherEventDto.VoltageVrn;
                otherEvent.VoltageVyn = otherEventDto.VoltageVyn;
                otherEvent.VoltageVbn = otherEventDto.VoltageVbn;
                otherEvent.SignedPowerFactorRPhase = otherEventDto.SignedPowerFactorRPhase;
                otherEvent.SignedPowerFactorYPhase = otherEventDto.SignedPowerFactorYPhase;
                otherEvent.SignedPowerFactorBPhase = otherEventDto.SignedPowerFactorBPhase;
                otherEvent.CumulativeEnergykWhImport = otherEventDto.CumulativeEnergykWhImport;
                otherEvent.CumulativeTamperCount = otherEventDto.CumulativeTamperCount;
                otherEvent.CumulativeEnergykWhExport = otherEventDto.CumulativeEnergykWhExport;

                otherEventList.Add(otherEvent);
            }

            return otherEventList;
        }

        private async Task<List<OtherEventDto>> ParseDataToDTO(List<OtherEvent> otherEventList)
        {
            int index = 1;
            List<OtherEventDto> otherEventDtoList = new List<OtherEventDto>();
            foreach (var otherEvent in otherEventList)
            {
                OtherEventDto otherEventDto = new OtherEventDto();

                otherEventDto.Number = index;
                otherEventDto.MeterNo = otherEvent.MeterNo;
                otherEventDto.CreatedOn = otherEvent.CreatedOn;
                otherEventDto.RealTimeClockDateAndTime = otherEvent.RealTimeClockDateAndTime;
                otherEventDto.Event = otherEvent.EventCode;
                otherEventDto.CurrentIr = (double.Parse(otherEvent.CurrentIr,System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.CurrentIy = (double.Parse(otherEvent.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.CurrentIb = (double.Parse(otherEvent.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.VoltageVrn = (double.Parse(otherEvent.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.VoltageVyn = (double.Parse(otherEvent.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.VoltageVbn = (double.Parse(otherEvent.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.SignedPowerFactorRPhase = (double.Parse(otherEvent.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.SignedPowerFactorYPhase = (double.Parse(otherEvent.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.SignedPowerFactorBPhase = (double.Parse(otherEvent.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.CumulativeEnergykWhImport = (double.Parse(otherEvent.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.CumulativeTamperCount = otherEvent.CumulativeTamperCount;
                otherEventDto.CumulativeEnergykWhExport = (double.Parse(otherEvent.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.GenericEventLogSequenceNumber = otherEvent.GenericEventLogSequenceNumber;

                otherEventDtoList.Add(otherEventDto);
                index++;
            }

            return otherEventDtoList;
        }

        private async Task<List<OtherEventDto>> ParseDataToDTOUMD(List<OtherEvent> otherEventList)
        {
            int index = 1;
            List<OtherEventDto> otherEventDtoList = new List<OtherEventDto>();
            foreach (var otherEvent in otherEventList)
            {
                OtherEventDto otherEventDto = new OtherEventDto();

                otherEventDto.Number = index;
                otherEventDto.MeterNo = otherEvent.MeterNo;
                otherEventDto.CreatedOn = otherEvent.CreatedOn;
                otherEventDto.RealTimeClockDateAndTime = otherEvent.RealTimeClockDateAndTime;
                otherEventDto.Event = otherEvent.EventCode;
                otherEventDto.CurrentIr = (double.Parse(otherEvent.CurrentIr, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.CurrentIy = (double.Parse(otherEvent.CurrentIy, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.CurrentIb = (double.Parse(otherEvent.CurrentIb, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.VoltageVrn = (double.Parse(otherEvent.VoltageVrn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.VoltageVyn = (double.Parse(otherEvent.VoltageVyn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.VoltageVbn = (double.Parse(otherEvent.VoltageVbn, System.Globalization.NumberStyles.Any)).ToString().CustomTrucate();
                otherEventDto.SignedPowerFactorRPhase = (double.Parse(otherEvent.SignedPowerFactorRPhase, System.Globalization.NumberStyles.Any)).ToString();
                otherEventDto.SignedPowerFactorYPhase = (double.Parse(otherEvent.SignedPowerFactorYPhase, System.Globalization.NumberStyles.Any)).ToString();
                otherEventDto.SignedPowerFactorBPhase =(double.Parse(otherEvent.SignedPowerFactorBPhase, System.Globalization.NumberStyles.Any)).ToString();
                otherEventDto.CumulativeEnergykWhImport =(double.Parse(otherEvent.CumulativeEnergykWhImport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                otherEventDto.CumulativeTamperCount = otherEvent.CumulativeTamperCount;
                otherEventDto.CumulativeEnergykWhExport = (double.Parse(otherEvent.CumulativeEnergykWhExport, System.Globalization.NumberStyles.Any) / 1000).ToString().CustomTrucate();
                otherEventDto.GenericEventLogSequenceNumber = otherEvent.GenericEventLogSequenceNumber;

                otherEventDtoList.Add(otherEventDto);
                index++;
            }

            return otherEventDtoList;
        }
    }
}