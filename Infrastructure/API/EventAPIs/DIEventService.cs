using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.API.EventAPIs
{
    public class DIEventService
    {
        private readonly IDataService<DIEvent> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;

        public DIEventService()
        {
            _dataService = new GenericDataService<DIEvent>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<DIEvent> dIEventModel)
        {
            try
            {
                await Delete(dIEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(dIEventModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> Delete(string meterNumber)
        {
            try
            {
                using (ApplicationDBContext db = _contextFactory.CreateDbContext())
                {
                    string query = "select * from DIEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<DIEvent>().RemoveRange(res);
                    }
                    await db.SaveChangesAsync();
                };
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<DIEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DIEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<DIEventDto> dIEvent = await ParseDataToDTO(response);

                return dIEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DIEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DIEvent where MeterNo = '" + meterNumber + "'";

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

                List<DIEventDto> dIEvent = await ParseDataToDTO(response);

                return dIEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<DIEvent>> ParseDataToClass(List<DIEventDto> dIEventDtoList)
        {
            List<DIEvent> dIEventList = new List<DIEvent>();

            foreach (var dIEventDto in dIEventDtoList)
            {
                DIEvent dIEvent = new DIEvent();

                dIEvent.MeterNo = dIEventDto.MeterNo;
                dIEvent.CreatedOn = dIEventDto.CreatedOn;
                dIEvent.RealTimeClockDateAndTime = dIEventDto.RealTimeClockDateAndTime;
                dIEvent.EventCode = dIEventDto.Event;

                dIEventList.Add(dIEvent);
            }

            return dIEventList;
        }

        private async Task<List<DIEventDto>> ParseDataToDTO(List<DIEvent> dIEventList)
        {
            int index = 1;
            List<DIEventDto> dIEventDtoList = new List<DIEventDto>();
            foreach (var dIEvent in dIEventList)
            {
                DIEventDto dIEventDto = new DIEventDto();

                dIEventDto.Number = index;
                dIEventDto.MeterNo = dIEvent.MeterNo;
                dIEventDto.RealTimeClockDateAndTime = dIEvent.RealTimeClockDateAndTime;
                dIEventDto.Event = dIEvent.EventCode;
                dIEventDto.GenericEventLogSequenceNumber = dIEvent.GenericEventLogSequenceNumber;
                dIEventDto.CreatedOn = dIEvent.CreatedOn;

                dIEventDtoList.Add(dIEventDto);
                index++;
            }

            return dIEventDtoList;
        }
    }
}
