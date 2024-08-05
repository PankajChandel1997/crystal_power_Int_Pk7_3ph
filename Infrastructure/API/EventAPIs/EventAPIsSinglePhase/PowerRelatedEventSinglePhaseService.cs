using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Enums;
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
    public class PowerRelatedEventSinglePhaseService
    {
        private readonly IDataService<PowerRelatedEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public PowerRelatedEventSinglePhaseService()
        {
            _dataService = new GenericDataService<PowerRelatedEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }
        public async Task<bool> Add(List<PowerRelatedEventSinglePhase> powerRelatedEventModel)
        {
            try
            {
                await Delete(powerRelatedEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(powerRelatedEventModel);
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
                    string query = "select * from PowerRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<PowerRelatedEventSinglePhase>().RemoveRange(res);
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
        public async Task<List<PowerRelatedEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from PowerRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<PowerRelatedEventSinglePhaseDto> powerRelatedEvent = await ParseDataToDTO(response);

                return powerRelatedEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PowerRelatedEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from PowerRelatedEventSinglePhase where MeterNo = '" + meterNumber + "'";

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

                List<PowerRelatedEventSinglePhaseDto> powerRelatedEvent = await ParseDataToDTO(response);

                return powerRelatedEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<PowerRelatedEventSinglePhase>> ParseDataToClass(List<PowerRelatedEventSinglePhaseDto> powerRelatedEventDtoList)
        //{
        //    List<PowerRelatedEventSinglePhase> powerRelatedEventList = new List<PowerRelatedEventSinglePhase>();

        //    foreach (var powerRelatedEventDto in powerRelatedEventDtoList)
        //    {
        //        PowerRelatedEventSinglePhase powerRelatedEvent = new PowerRelatedEventSinglePhase();

        //        powerRelatedEvent.MeterNo = powerRelatedEventDto.MeterNo;
        //        powerRelatedEvent.CreatedOn = powerRelatedEventDto.CreatedOn;
        //        powerRelatedEvent.RealTimeClockDateAndTime = powerRelatedEventDto.RealTimeClockDateAndTime;
        //        powerRelatedEvent.EventCode = powerRelatedEventDto.Event;

        //        powerRelatedEventList.Add(powerRelatedEvent);
        //    }

        //    return powerRelatedEventList;
        //}

        private async Task<List<PowerRelatedEventSinglePhaseDto>> ParseDataToDTO(List<PowerRelatedEventSinglePhase> powerRelatedEventList)
        {
            int index = 1;
            List<PowerRelatedEventSinglePhaseDto> powerRelatedEventDtoList = new List<PowerRelatedEventSinglePhaseDto>();
            for (int i = 0; i < powerRelatedEventList.Count(); i++)
            {
                PowerRelatedEventSinglePhaseDto powerRelatedEventDto = new PowerRelatedEventSinglePhaseDto();

                powerRelatedEventDto.Number = index;
                powerRelatedEventDto.RealTimeClockDateAndTime = powerRelatedEventList[i].RealTimeClockDateAndTime;
                powerRelatedEventDto.Event = powerRelatedEventList[i].EventCode;
                powerRelatedEventDto.CreatedOn = powerRelatedEventList[i].CreatedOn;

                var time = i == 0 ? "" : (powerRelatedEventList[i].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Restoration).ToString()) && powerRelatedEventList[i - 1].EventCode.Contains(((int)EventCodeTypeEnum.Power_failure_Occurrence).ToString())) ? Math.Round((DateTime.ParseExact(powerRelatedEventList[i].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.ParseExact(powerRelatedEventList[i - 1].RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).TotalMinutes, 2).ToString() : "";
                if (!string.IsNullOrEmpty(time))
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
