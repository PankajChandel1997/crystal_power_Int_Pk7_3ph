using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class VoltageRelatedEventCommand
    {
        private readonly VoltageRelatedEventService _voltageRelatedEventService;
        public VoltageRelatedEventCommand()
        {
            _voltageRelatedEventService = new VoltageRelatedEventService();
        }

        public async Task<List<VoltageRelatedEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<VoltageRelatedEventDto> voltageRelatedEvent = new List<VoltageRelatedEventDto>();
            try
            {
                voltageRelatedEvent = await _voltageRelatedEventService.GetAll(pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return voltageRelatedEvent;
        }

        public async Task<List<VoltageRelatedEventDto>> Filter(string startDate, string endDate, int PageSize, string meterNumber)
        {
            List<VoltageRelatedEventDto> voltageRelatedEvent = new List<VoltageRelatedEventDto>();
            try
            {
                voltageRelatedEvent = await _voltageRelatedEventService.Filter(startDate, endDate, PageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return voltageRelatedEvent;
        }
    }
}
