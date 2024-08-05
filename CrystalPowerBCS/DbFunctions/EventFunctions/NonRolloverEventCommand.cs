using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class NonRolloverEventCommand
    {
        private readonly NonRolloverEventService _nonRolloverEventService;
        public NonRolloverEventCommand()
        {
            _nonRolloverEventService = new NonRolloverEventService();
        }

        public async Task<List<NonRolloverEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<NonRolloverEventDto> nonRolloverEvent = new List<NonRolloverEventDto>();
            try
            {
                nonRolloverEvent = await _nonRolloverEventService.GetAll(pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return nonRolloverEvent;
        }

        public async Task<List<NonRolloverEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<NonRolloverEventDto> nonRolloverEvent = new List<NonRolloverEventDto>();
            try
            {
                nonRolloverEvent = await _nonRolloverEventService.Filter(startDate, endDate, pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return nonRolloverEvent;
        }
    }
}
 