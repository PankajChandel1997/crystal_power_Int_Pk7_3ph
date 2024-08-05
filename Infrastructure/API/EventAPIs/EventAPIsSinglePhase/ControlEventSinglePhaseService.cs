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

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class ControlEventSinglePhaseService
    {
        private readonly IDataService<ControlEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public ControlEventSinglePhaseService()
        {
            _dataService = new GenericDataService<ControlEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<ControlEventSinglePhase> controlEventModel)
        {
            try
            {
                await Delete(controlEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(controlEventModel);
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
                    string query = "select * from ControlEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);
                        if (res.Any())
                        {
                            db.Set<ControlEventSinglePhase>().RemoveRange(res);
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

        public async Task<List<ControlEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from ControlEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<ControlEventSinglePhaseDto> controlEvent = await ParseDataToDTO(response);

                return controlEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ControlEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from ControlEventSinglePhase where MeterNo = '" + meterNumber + "'";

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

                List<ControlEventSinglePhaseDto> controlEventSinglePhase = await ParseDataToDTO(response);

                return controlEventSinglePhase;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<ControlEventSinglePhase>> ParseDataToClass(List<ControlEventSinglePhaseDto> controlEventDtoList)
        //{
        //    List<ControlEventSinglePhase> controlEventList = new List<ControlEventSinglePhase>();

        //    foreach (var controlEventDto in controlEventDtoList)
        //    {
        //        ControlEventSinglePhase controlEvent = new ControlEventSinglePhase();
                
        //        controlEvent.MeterNo = controlEventDto.MeterNo;
        //        controlEvent.EventCode = controlEventDto.Event;
        //        controlEvent.CreatedOn = controlEventDto.CreatedOn;
        //        controlEvent.RealTimeClockDateAndTime = controlEventDto.RealTimeClockDateAndTime; 

        //        controlEventList.Add(controlEvent);
        //    }

        //    return controlEventList;
        //}

        private async Task<List<ControlEventSinglePhaseDto>> ParseDataToDTO(List<ControlEventSinglePhase> controlEventList)
        {
            int index = 1;
            List<ControlEventSinglePhaseDto> controlEventDtoList = new List<ControlEventSinglePhaseDto>();
            foreach (var controlEvent in controlEventList)
            {
                ControlEventSinglePhaseDto controlEventDto = new ControlEventSinglePhaseDto();

                controlEventDto.Number = index ;
                controlEventDto.RealTimeClockDateAndTime = controlEvent.RealTimeClockDateAndTime;
                controlEventDto.Event = controlEvent.EventCode;
                controlEventDto.CreatedOn = controlEvent.CreatedOn;

                controlEventDtoList.Add(controlEventDto);
                index++;
            }

            return controlEventDtoList;
        }
    }
}
