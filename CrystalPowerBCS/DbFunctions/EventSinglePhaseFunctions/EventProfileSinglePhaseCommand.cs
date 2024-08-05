using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class EventProfileSinglePhaseCommand
    {
        private readonly EventProfileSinglePhaseService _eventProfileSinglePhaseService;
        public EventProfileSinglePhaseCommand()
        {
            _eventProfileSinglePhaseService = new EventProfileSinglePhaseService();
        }

        public async Task<List<EventProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<EventProfileSinglePhaseDto> eventProfileSinglePhase = new List<EventProfileSinglePhaseDto>();
            try
            {
                eventProfileSinglePhase = await _eventProfileSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return eventProfileSinglePhase;
        }
    }
}
