using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class PowerRelatedEventCommand
    {
        private readonly PowerRelatedEventService _powerRelatedEventService;
        public PowerRelatedEventCommand()
        {
            _powerRelatedEventService = new PowerRelatedEventService();
        }

        public async Task<List<PowerRelatedEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<PowerRelatedEventDto> powerRelatedEvent = new List<PowerRelatedEventDto>();
            try
            {
                powerRelatedEvent = await _powerRelatedEventService.GetAll(pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return powerRelatedEvent;
        }
        public async Task<List<PowerRelatedEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<PowerRelatedEventDto> powerRelatedEvent = new List<PowerRelatedEventDto>();
            try
            {
                powerRelatedEvent = await _powerRelatedEventService.Filter(startDate, endDate, pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return powerRelatedEvent;
        }

    }
}
