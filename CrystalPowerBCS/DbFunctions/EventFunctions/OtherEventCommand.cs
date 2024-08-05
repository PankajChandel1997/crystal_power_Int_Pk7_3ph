using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class OtherEventCommand
    {
        private readonly OtherEventService _otherEventService;
        public OtherEventCommand()
        {
            _otherEventService = new OtherEventService();
        }

        public async Task<List<OtherEventDto>> GetAll(int pageSize,string meterNumber)
        {
            List<OtherEventDto> otherEvent = new List<OtherEventDto>();
            try
            {
                otherEvent = await _otherEventService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return otherEvent;
        }

        public async Task<List<OtherEventDto>> Filter(string startDate, string endDate, int pageSize,string meterNumber)
        {
            List<OtherEventDto> otherEvent = new List<OtherEventDto>();
            try
            {
                otherEvent = await _otherEventService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return otherEvent;
        }
    }
}
