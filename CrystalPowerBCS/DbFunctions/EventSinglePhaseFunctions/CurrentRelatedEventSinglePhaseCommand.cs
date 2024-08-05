using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class CurrentRelatedEventSinglePhaseCommand
    {
        private readonly CurrentRelatedEventSinglePhaseService _currentRelatedEventSinglePhaseService;
        public CurrentRelatedEventSinglePhaseCommand()
        {
            _currentRelatedEventSinglePhaseService = new CurrentRelatedEventSinglePhaseService();
        }

        public async Task<List<CurrentRelatedEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<CurrentRelatedEventSinglePhaseDto> currentRelatedEventSinglePhase = new List<CurrentRelatedEventSinglePhaseDto>();
            try
            {
                currentRelatedEventSinglePhase = await _currentRelatedEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return currentRelatedEventSinglePhase;
        }

        public async Task<List<CurrentRelatedEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<CurrentRelatedEventSinglePhaseDto> currentRelatedEventSinglePhase = new List<CurrentRelatedEventSinglePhaseDto>();
            try
            {
                currentRelatedEventSinglePhase = await _currentRelatedEventSinglePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return currentRelatedEventSinglePhase;
        }
    }
}
