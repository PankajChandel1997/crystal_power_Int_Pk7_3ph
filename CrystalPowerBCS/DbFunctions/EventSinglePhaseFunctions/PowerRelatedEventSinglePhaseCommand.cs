using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class PowerRelatedEventSinglePhaseCommand
    {
        private readonly PowerRelatedEventSinglePhaseService _powerRelatedEventSinglePhaseService;
        public PowerRelatedEventSinglePhaseCommand()
        {
            _powerRelatedEventSinglePhaseService = new PowerRelatedEventSinglePhaseService();
        }

        public async Task<List<PowerRelatedEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<PowerRelatedEventSinglePhaseDto> powerRelatedEventSinglePhase = new List<PowerRelatedEventSinglePhaseDto>();
            try
            {
                powerRelatedEventSinglePhase = await _powerRelatedEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return powerRelatedEventSinglePhase;
        }

        public async Task<List<PowerRelatedEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<PowerRelatedEventSinglePhaseDto> powerRelatedEventSinglePhase = new List<PowerRelatedEventSinglePhaseDto>();
            try
            {
                powerRelatedEventSinglePhase = await _powerRelatedEventSinglePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return powerRelatedEventSinglePhase;
        }
    }
}
