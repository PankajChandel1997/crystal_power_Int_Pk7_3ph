


using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;
using System.Linq;

namespace Infrastructure.API.EventAPIs
{
    public class PowerRelatedEventService
    {
        private readonly IDataService<PowerRelatedEvent> _dataService; 
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public PowerRelatedEventService()
        {
            _dataService = new GenericDataService<PowerRelatedEvent>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<PowerRelatedEvent> powerRelatedEventModel)
        {
            try
            {
                await Delete(powerRelatedEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(powerRelatedEventModel);
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
                    string query = "select * from PowerRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<PowerRelatedEvent>().RemoveRange(res);
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
        public async Task<List<PowerRelatedEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from PowerRelatedEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<PowerRelatedEventDto> powerRelatedEvent = await ParseDataToDTO(response);

                return powerRelatedEvent;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PowerRelatedEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from PowerRelatedEvent where MeterNo = '" + meterNumber + "' order by RealTimeClockDateAndTime desc";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Take(pageSize).ToList();
                }
                else
                {
                    response = response.OrderBy(x => DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Take(pageSize).ToList();
                }

                List<PowerRelatedEventDto> powerRelatedEventDto = await ParseDataToDTO(response);

                return powerRelatedEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<PowerRelatedEvent>> ParseDataToClass(List<PowerRelatedEventDto> powerRelatedEventDtoList)
        {
            List<PowerRelatedEvent> powerRelatedEventList = new List<PowerRelatedEvent>();

            foreach (var powerRelatedEventDto in powerRelatedEventDtoList)
            {
                PowerRelatedEvent powerRelatedEvent = new PowerRelatedEvent();

                powerRelatedEvent.MeterNo = powerRelatedEventDto.MeterNo;
                powerRelatedEvent.CreatedOn = powerRelatedEventDto.CreatedOn;
                powerRelatedEvent.RealTimeClockDateAndTime = powerRelatedEventDto.RealTimeClockDateAndTime;
                powerRelatedEvent.EventCode = powerRelatedEventDto.Event;
                powerRelatedEvent.GenericEventLogSequenceNumber = powerRelatedEventDto.GenericEventLogSequenceNumber;

                powerRelatedEventList.Add(powerRelatedEvent);
            }

            return powerRelatedEventList;
        }

        private async Task<List<PowerRelatedEventDto>> ParseDataToDTO(List<PowerRelatedEvent> powerRelatedEventList)
        {
            int index = 1;
            List<PowerRelatedEventDto> powerRelatedEventDtoList = new List<PowerRelatedEventDto>();
            
            #region Skip

            //foreach (var powerRelatedEvent in powerRelatedEventList)
            //{
            //    PowerRelatedEventDto powerRelatedEventDto = new PowerRelatedEventDto();

            //    powerRelatedEventDto.Number = index;
            //    powerRelatedEventDto.RealTimeClockDateAndTime = powerRelatedEvent.RealTimeClockDateAndTime;
            //    powerRelatedEventDto.Event = powerRelatedEvent.EventCode;
            //    powerRelatedEventDto.CreatedOn = powerRelatedEvent.CreatedOn;

            //    powerRelatedEventDtoList.Add(powerRelatedEventDto);
            //    index++;
            //}

            #endregion

            for (int i = 0; i < powerRelatedEventList.Count(); i++)
            {
                PowerRelatedEventDto powerRelatedEventDto = new PowerRelatedEventDto();

                powerRelatedEventDto.Number = index;
                powerRelatedEventDto.RealTimeClockDateAndTime = powerRelatedEventList[i].RealTimeClockDateAndTime;
                powerRelatedEventDto.Event = powerRelatedEventList[i].EventCode;
                powerRelatedEventDto.CreatedOn = powerRelatedEventList[i].CreatedOn;
                powerRelatedEventDto.GenericEventLogSequenceNumber = powerRelatedEventList[i].GenericEventLogSequenceNumber;

                var time = i == 0 ? "" : (powerRelatedEventList[i].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Restoration).ToString()) && powerRelatedEventList[i-1].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Occurrence).ToString())) ? Math.Round( (DateTime.ParseExact(powerRelatedEventList[i].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.ParseExact(powerRelatedEventList[i - 1].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).TotalMinutes ,2).ToString() : "";                           
                if(!string.IsNullOrEmpty(time))
                {
                    double totalMinutes = double.Parse(time);

                    int hours = (int)(totalMinutes / 60);
                    int minutes = (int)(totalMinutes % 60);
                    int seconds = (int)((totalMinutes * 60) % 60);

                    powerRelatedEventDto.PowerFailureTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                }

                powerRelatedEventDtoList.Add(powerRelatedEventDto);
                index++;
            }

            return powerRelatedEventDtoList;
        }
    }
}