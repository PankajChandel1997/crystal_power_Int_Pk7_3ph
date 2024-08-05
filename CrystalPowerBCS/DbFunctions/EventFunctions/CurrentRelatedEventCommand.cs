using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class CurrentRelatedEventCommand
    {
        private readonly CurrentRelatedEventService _currentRelatedEventService;
        public ErrorHelper _errorHelper;

        public CurrentRelatedEventCommand()
        {
            _currentRelatedEventService = new CurrentRelatedEventService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<CurrentRelatedEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<CurrentRelatedEventDto> currentRelatedEvent = new List<CurrentRelatedEventDto>();
            try
            {
                currentRelatedEvent = await _currentRelatedEventService.GetAll(pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return currentRelatedEvent;
        }

        public async Task<List<CurrentRelatedEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<CurrentRelatedEventDto> currentRelatedEvent = new List<CurrentRelatedEventDto>();
            try
            {
                currentRelatedEvent = await _currentRelatedEventService.Filter(startDate, endDate, pageSize,meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);


            }
            return currentRelatedEvent;
        }
    }
}
