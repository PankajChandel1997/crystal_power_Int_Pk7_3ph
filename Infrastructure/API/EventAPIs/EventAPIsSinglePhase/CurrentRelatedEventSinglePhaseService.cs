using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class CurrentRelatedEventSinglePhaseService
    {
        private readonly IDataService<CurrentRelatedEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public CurrentRelatedEventSinglePhaseService()
        {
            _dataService = new GenericDataService<CurrentRelatedEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<CurrentRelatedEventSinglePhase> currentRelatedModel)
        {
            try
            {
                await Delete(currentRelatedModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(currentRelatedModel);
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
                    string query = "select * from CurrentRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<CurrentRelatedEventSinglePhase>().RemoveRange(res);
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

        public async Task<List<CurrentRelatedEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from CurrentRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<CurrentRelatedEventSinglePhaseDto> currentRelated = await ParseDataToDTO(response);

                return currentRelated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CurrentRelatedEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from CurrentRelatedEventSinglePhase where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.DateAndTimeOfEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.DateAndTimeOfEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                List<CurrentRelatedEventSinglePhaseDto> currentRelatedEvent = await ParseDataToDTO(response);

                return currentRelatedEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<CurrentRelatedEventSinglePhase>> ParseDataToClass(List<CurrentRelatedEventSinglePhaseDto> currentRelatedDtoList)
        //{
        //    List<CurrentRelatedEventSinglePhase> currentRelatedList = new List<CurrentRelatedEventSinglePhase>();

        //    foreach (var currentRelatedDto in currentRelatedDtoList)
        //    {
        //        CurrentRelatedEventSinglePhase currentRelated = new CurrentRelatedEventSinglePhase();

        //        currentRelated.MeterNo = currentRelatedDto.MeterNo;
        //        currentRelated.CreatedOn = currentRelatedDto.CreatedOn;
        //        currentRelated.DateAndTimeOfEvent = currentRelatedDto.DateAndTimeOfEvent;
        //        currentRelated.EventCode = currentRelatedDto.Event;
        //        currentRelated.Current = currentRelatedDto.Current;
        //        currentRelated.Voltage = currentRelatedDto.Voltage;
        //        currentRelated.PowerFactor = currentRelatedDto.PowerFactor;
        //        currentRelated.CumulativeEnergykWhImport = currentRelatedDto.CumulativeEnergykWhImport;
        //        currentRelated.CumulativeEnergykWhExport = currentRelatedDto.CumulativeEnergykWhExport;
        //        currentRelated.CumulativeTamperCount = currentRelatedDto.CumulativeTamperCount;

        //        currentRelatedList.Add(currentRelated);
        //    }

        //    return currentRelatedList;
        //}

        private async Task<List<CurrentRelatedEventSinglePhaseDto>> ParseDataToDTO(List<CurrentRelatedEventSinglePhase> currentRelatedList)
        {
            int index = 1;
            List<CurrentRelatedEventSinglePhaseDto> currentRelatedDtoList = new List<CurrentRelatedEventSinglePhaseDto>();
            foreach (var currentRelated in currentRelatedList)
            {
                CurrentRelatedEventSinglePhaseDto currentRelatedDto = new CurrentRelatedEventSinglePhaseDto();

                currentRelatedDto.Number = index;
                currentRelatedDto.DateAndTimeOfEvent = currentRelated.DateAndTimeOfEvent;
                currentRelatedDto.Event = currentRelated.EventCode;
                currentRelatedDto.Current = currentRelated.Current;
                currentRelatedDto.Voltage = currentRelated.Voltage;
                currentRelatedDto.PowerFactor = currentRelated.PowerFactor;
                currentRelatedDto.CumulativeEnergykWhImport = StringExtensions.CheckNullable(currentRelated.CumulativeEnergykWhImport); 
                currentRelatedDto.CumulativeEnergykWhExport = StringExtensions.CheckNullable(currentRelated.CumulativeEnergykWhExport);
                currentRelatedDto.CumulativeTamperCount = currentRelated.CumulativeTamperCount;
                currentRelatedDto.CreatedOn = currentRelated.CreatedOn;

                currentRelatedDtoList.Add(currentRelatedDto);
                index++;
            }

            return currentRelatedDtoList;
        }
    }
}
