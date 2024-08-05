using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class DIEventCommand
    {
        private readonly DIEventService _DIEventService;
        public DIEventCommand()
        {
            _DIEventService = new DIEventService();
        }

        public async Task<List<DIEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<DIEventDto> DIEvent = new List<DIEventDto>();
            try
            {
                DIEvent = await _DIEventService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return DIEvent;
        }

        public async Task<List<DIEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<DIEventDto> DIEvent = new List<DIEventDto>();
            try
            {
                DIEvent = await _DIEventService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return DIEvent;
        }
    }
}
