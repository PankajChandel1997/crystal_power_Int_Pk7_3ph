using Infrastructure.API.EventAPIs.EventAPIsSinglePhase;
using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class DIEventSinglePhaseCommand
    {
        private readonly DIEventSinglePhaseService _DIEventSinglePhaseService;
        public DIEventSinglePhaseCommand()
        {
            _DIEventSinglePhaseService = new DIEventSinglePhaseService();
        }

        public async Task<List<DIEventSinglePhaseEventDto>> GetAll(int pageSize, string meterNumber)
        {
            List<DIEventSinglePhaseEventDto> DIEventSinglePhase = new List<DIEventSinglePhaseEventDto>();
            try
            {
                DIEventSinglePhase = await _DIEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return DIEventSinglePhase;
        }

        public async Task<List<DIEventSinglePhaseEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<DIEventSinglePhaseEventDto> DIEventSinglePhase = new List<DIEventSinglePhaseEventDto>();
            try
            {
                DIEventSinglePhase = await _DIEventSinglePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return DIEventSinglePhase;
        }
    }
}
