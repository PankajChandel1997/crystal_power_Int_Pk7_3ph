using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class ControlEventSinglePhaseCommand
    {
        private readonly ControlEventSinglePhaseService _controlEventSinglePhaseService;
        public ControlEventSinglePhaseCommand()
        {
            _controlEventSinglePhaseService = new ControlEventSinglePhaseService();
        }

        public async Task<List<ControlEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<ControlEventSinglePhaseDto> controlEventSinglePhase = new List<ControlEventSinglePhaseDto>();
            try
            {
                controlEventSinglePhase = await _controlEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return controlEventSinglePhase;
        }

        public async Task<List<ControlEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<ControlEventSinglePhaseDto> controlEventSinglePhase = new List<ControlEventSinglePhaseDto>();
            try
            {
                controlEventSinglePhase = await _controlEventSinglePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return controlEventSinglePhase;
        }
    }
}
