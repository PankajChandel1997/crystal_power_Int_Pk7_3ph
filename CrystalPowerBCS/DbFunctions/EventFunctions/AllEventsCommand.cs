using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class AllEventsCommand
    {
        private readonly AllEventsService _allEventThreePhaseService;
        public AllEventsCommand()
        {
            _allEventThreePhaseService = new AllEventsService();
        }

        public async Task<List<AllEventsDTO>> GetAll(string meterNumber)
        {
            List<AllEventsDTO> allEventsSinglePhase = new List<AllEventsDTO>();
            try
            {
                allEventsSinglePhase = await _allEventThreePhaseService.GetAll(meterNumber);
            }
            catch (Exception)
            {

            }
            return allEventsSinglePhase;
        }
    }
}