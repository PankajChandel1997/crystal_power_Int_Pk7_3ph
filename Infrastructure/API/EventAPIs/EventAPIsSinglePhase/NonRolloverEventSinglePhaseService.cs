using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class NonRolloverEventSinglePhaseService
    {
        private readonly IDataService<NonRolloverEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public NonRolloverEventSinglePhaseService()
        {
            _dataService = new GenericDataService<NonRolloverEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<NonRolloverEventSinglePhase> nonRolloverEventModel)
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
                    string query = "select * from NonRolloverEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<NonRolloverEventSinglePhase>().RemoveRange(res);
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

        public async Task<List<NonRolloverEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from NonRolloverEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<NonRolloverEventSinglePhaseDto> nonRolloverEvent = await ParseDataToDTO(response);

                return nonRolloverEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<NonRolloverEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from NonRolloverEventSinglePhase where MeterNo = '" + meterNumber + "'";

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

                List<NonRolloverEventSinglePhaseDto> nonRolloverEvent = await ParseDataToDTO(response);

                return nonRolloverEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<NonRolloverEventSinglePhase>> ParseDataToClass(List<NonRolloverEventSinglePhaseDto> nonRolloverEventDtoList)
        //{
        //    List<NonRolloverEventSinglePhase> nonRolloverEventList = new List<NonRolloverEventSinglePhase>();

        //    foreach (var nonRolloverEventDto in nonRolloverEventDtoList)
        //    {
        //        NonRolloverEventSinglePhase nonRolloverEvent = new NonRolloverEventSinglePhase();

        //        nonRolloverEvent.MeterNo = nonRolloverEventDto.MeterNo;
        //        nonRolloverEvent.CreatedOn = nonRolloverEventDto.CreatedOn;
        //        nonRolloverEvent.RealTimeClockDateAndTime = nonRolloverEventDto.RealTimeClockDateAndTime;
        //        nonRolloverEvent.EventCode = nonRolloverEventDto.Event;

        //        nonRolloverEventList.Add(nonRolloverEvent);
        //    }

        //    return nonRolloverEventList;
        //}

        private async Task<List<NonRolloverEventSinglePhaseDto>> ParseDataToDTO(List<NonRolloverEventSinglePhase> nonRolloverEventList)
        {
            int index = 1;
            List<NonRolloverEventSinglePhaseDto> nonRolloverEventDtoList = new List<NonRolloverEventSinglePhaseDto>();
            foreach (var nonRolloverEvent in nonRolloverEventList)
            {
                NonRolloverEventSinglePhaseDto nonRolloverEventDto = new NonRolloverEventSinglePhaseDto();

                nonRolloverEventDto.Number = index;
                nonRolloverEventDto.RealTimeClockDateAndTime = nonRolloverEvent.RealTimeClockDateAndTime;
                nonRolloverEventDto.Event = nonRolloverEvent.EventCode;
                nonRolloverEventDto.CreatedOn = nonRolloverEvent.CreatedOn;

                nonRolloverEventDtoList.Add(nonRolloverEventDto);
                index++;
            }

            return nonRolloverEventDtoList;
        }
    }
}
