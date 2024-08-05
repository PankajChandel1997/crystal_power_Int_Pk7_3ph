using Domain.Entities.SinglePhaseEntities;
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

namespace Infrastructure.API.EventAPIs.EventAPIsSinglePhase
{
    public class DIEventSinglePhaseService
    {
        private readonly IDataService<DIEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public DIEventSinglePhaseService()
        {
            _dataService = new GenericDataService<DIEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<DIEventSinglePhase> dIEventModel)
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
                    string query = "select * from DIEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<DIEventSinglePhase>().RemoveRange(res);
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

        public async Task<List<DIEventSinglePhaseEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DIEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<DIEventSinglePhaseEventDto> dIEvent = await ParseDataToDTO(response);

                return dIEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DIEventSinglePhaseEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DIEventSinglePhase where MeterNo = '" + meterNumber + "'";

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

                List<DIEventSinglePhaseEventDto> dIEvent = await ParseDataToDTO(response);

                return dIEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<DIEventSinglePhase>> ParseDataToClass(List<DIEventSinglePhaseEventDto> dIEventDtoList)
        //{
        //    List<DIEventSinglePhase> dIEventList = new List<DIEventSinglePhase>();

        //    foreach (var dIEventDto in dIEventDtoList)
        //    {
        //        DIEventSinglePhase dIEvent = new DIEventSinglePhase();

        //        dIEvent.MeterNo = dIEventDto.MeterNo;
        //        dIEvent.CreatedOn = dIEventDto.CreatedOn;
        //        dIEvent.RealTimeClockDateAndTime = dIEventDto.RealTimeClockDateAndTime;
        //        dIEvent.EventCode = dIEventDto.Event;

        //        dIEventList.Add(dIEvent);
        //    }

        //    return dIEventList;
        //}

        private async Task<List<DIEventSinglePhaseEventDto>> ParseDataToDTO(List<DIEventSinglePhase> dIEventList)
        {
            int index = 1;
            List<DIEventSinglePhaseEventDto> dIEventDtoList = new List<DIEventSinglePhaseEventDto>();
            foreach (var dIEvent in dIEventList)
            {
                DIEventSinglePhaseEventDto dIEventDto = new DIEventSinglePhaseEventDto();

                dIEventDto.Number = index;
                dIEventDto.MeterNo = dIEvent.MeterNo;
                dIEventDto.RealTimeClockDateAndTime = dIEvent.RealTimeClockDateAndTime;
                dIEventDto.Event = dIEvent.EventCode;
                dIEventDto.CreatedOn = dIEvent.CreatedOn;

                dIEventDtoList.Add(dIEventDto);
                index++;
            }

            return dIEventDtoList;
        }
    }
}