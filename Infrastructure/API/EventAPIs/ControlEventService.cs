using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIs
{
    public class ControlEventService
    {
        private readonly IDataService<ControlEvent> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public ControlEventService()
        {
            _dataService = new GenericDataService<ControlEvent>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<ControlEvent> controlEventModel)
        {
            try
            {
                await Delete(controlEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(controlEventModel);
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
                    string query = "select * from ControlEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<ControlEvent>().RemoveRange(res);
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
        public async Task<List<ControlEventDto>> GetAll(int pageSize,string meterNumber)
        {
            try
            {
                string query = "select * from ControlEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<ControlEventDto> controlEvent = await ParseDataToDTO(response);

                return controlEvent;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ControlEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from ControlEvent where MeterNo = '" + meterNumber + "'";

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

                List<ControlEventDto> controlEventDto = await ParseDataToDTO(response);

                return controlEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<ControlEvent>> ParseDataToClass(List<ControlEventDto> controlEventDtoList)
        {
            List<ControlEvent> controlEventList = new List<ControlEvent>();

            foreach (var controlEventDto in controlEventDtoList)
            {
                ControlEvent controlEvent = new ControlEvent();

                controlEvent.MeterNo = controlEventDto.MeterNo;
                controlEvent.CreatedOn = controlEventDto.CreatedOn;
                controlEvent.RealTimeClockDateAndTime = controlEventDto.RealTimeClockDateAndTime;
                controlEvent.EventCode = controlEventDto.Event;
                controlEvent.GenericEventLogSequenceNumber = controlEventDto.GenericEventLogSequenceNumber;

                controlEventList.Add(controlEvent);
            }

            return controlEventList;
        }

        private async Task<List<ControlEventDto>> ParseDataToDTO(List<ControlEvent> controlEventList)
        {
            int index = 1;
            List<ControlEventDto> controlEventDtoList = new List<ControlEventDto>();
            foreach (var controlEvent in controlEventList)
            {
                ControlEventDto controlEventDto = new ControlEventDto();

                controlEventDto.Number =  index;
                controlEventDto.RealTimeClockDateAndTime =  controlEvent.RealTimeClockDateAndTime;
                controlEventDto.Event =  controlEvent.EventCode;
                controlEventDto.CreatedOn =  controlEvent.CreatedOn;
                controlEventDto.GenericEventLogSequenceNumber =  controlEvent.GenericEventLogSequenceNumber;

                controlEventDtoList.Add(controlEventDto);
                index++;
            }

            return controlEventDtoList;
        }
    }
}