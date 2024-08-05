using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIs
{
    public class NonRolloverEventService
    {
        private readonly IDataService<NonRolloverEvent> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public NonRolloverEventService()
        {
            _dataService = new GenericDataService<NonRolloverEvent>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<NonRolloverEvent> nonRolloverEventModel)
        {
            try
            {
                await Delete(nonRolloverEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(nonRolloverEventModel);
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
                    string query = "select * from NonRolloverEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<NonRolloverEvent>().RemoveRange(res);
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

        public async Task<List<NonRolloverEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from NonRolloverEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<NonRolloverEventDto> nonRolloverEvent = await ParseDataToDTO(response);

                return nonRolloverEvent;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<NonRolloverEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from NonRolloverEvent where MeterNo = '" + meterNumber + "'";

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

                List<NonRolloverEventDto> nonRolloverEventDto = await ParseDataToDTO(response);

                return nonRolloverEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<NonRolloverEvent>> ParseDataToClass(List<NonRolloverEventDto> nonRolloverEventDtoList)
        {
            List<NonRolloverEvent> nonRolloverEventList = new List<NonRolloverEvent>();

            foreach (var nonRolloverEventDto in nonRolloverEventDtoList)
            {
                NonRolloverEvent nonRolloverEvent = new NonRolloverEvent();

                nonRolloverEvent.MeterNo = nonRolloverEventDto.MeterNo;
                nonRolloverEvent.CreatedOn = nonRolloverEventDto.CreatedOn;
                nonRolloverEvent.RealTimeClockDateAndTime = nonRolloverEventDto.RealTimeClockDateAndTime;
                nonRolloverEvent.EventCode = nonRolloverEventDto.Event;
                nonRolloverEvent.GenericEventLogSequenceNumber = nonRolloverEventDto.GenericEventLogSequenceNumber;

                nonRolloverEventList.Add(nonRolloverEvent);
            }

            return nonRolloverEventList;
        }

        private async Task<List<NonRolloverEventDto>> ParseDataToDTO(List<NonRolloverEvent> nonRolloverEventList)
        {
            int index = 1;
            List<NonRolloverEventDto> nonRolloverEventDtoList = new List<NonRolloverEventDto>();
            foreach (var nonRolloverEvent in nonRolloverEventList)
            {
                NonRolloverEventDto nonRolloverEventDto = new NonRolloverEventDto();

                nonRolloverEventDto.Number = index;
                nonRolloverEventDto.RealTimeClockDateAndTime = nonRolloverEvent.RealTimeClockDateAndTime;
                nonRolloverEventDto.Event = nonRolloverEvent.EventCode;
                nonRolloverEventDto.CreatedOn = nonRolloverEvent.CreatedOn;
                nonRolloverEventDto.MeterNo = nonRolloverEvent.MeterNo;
                nonRolloverEventDto.GenericEventLogSequenceNumber = nonRolloverEvent.GenericEventLogSequenceNumber;

                nonRolloverEventDtoList.Add(nonRolloverEventDto);
                index++;
            }

            return nonRolloverEventDtoList;
        }
    }
}