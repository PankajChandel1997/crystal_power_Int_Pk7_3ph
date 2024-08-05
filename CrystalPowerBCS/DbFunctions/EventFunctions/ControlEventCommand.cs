using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class ControlEventCommand
    {
        private readonly ControlEventService _controlEventService;
        public ControlEventCommand()
        {
            _controlEventService = new ControlEventService();
        }

        public async Task<List<ControlEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<ControlEventDto> controlEvent = new List<ControlEventDto>();
            try
            {
                controlEvent = await _controlEventService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return controlEvent;
        }

        public async Task<List<ControlEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<ControlEventDto> controlEvent = new List<ControlEventDto>();
            try
            {
                controlEvent = await _controlEventService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return controlEvent;
        }
    }
}
