using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class AllEventsSinglePhaseCommand
    {
        private readonly AllEventsSinglePhaseService _allEventSinglePhaseService;
        public AllEventsSinglePhaseCommand()
        {
            _allEventSinglePhaseService = new AllEventsSinglePhaseService();
        }

        public async Task<List<AllEventsSinglePhaseDto>> GetAll(string meterNumber)
        {
            List<AllEventsSinglePhaseDto> allEventsSinglePhase = new List<AllEventsSinglePhaseDto>();
            try
            {
                allEventsSinglePhase = await _allEventSinglePhaseService.GetAll(meterNumber);
            }
            catch (Exception)
            {

            }
            return allEventsSinglePhase;
        }

    }
}