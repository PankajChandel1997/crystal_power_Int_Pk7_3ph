using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class NonRolloverEventSinglePhaseCommand
    {
        private readonly NonRolloverEventSinglePhaseService _nonRolloverEventSinglePhaseService;
        public NonRolloverEventSinglePhaseCommand()
        {
            _nonRolloverEventSinglePhaseService = new NonRolloverEventSinglePhaseService();
        }

        public async Task<List<NonRolloverEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<NonRolloverEventSinglePhaseDto> nonRolloverEventSinglePhase = new List<NonRolloverEventSinglePhaseDto>();
            try
            {
                nonRolloverEventSinglePhase = await _nonRolloverEventSinglePhaseService.GetAll(pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return nonRolloverEventSinglePhase;
        }

        public async Task<List<NonRolloverEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<NonRolloverEventSinglePhaseDto> nonRolloverEventSinglePhase = new List<NonRolloverEventSinglePhaseDto>();
            try
            {
                nonRolloverEventSinglePhase = await _nonRolloverEventSinglePhaseService.Filter(startDate, endDate, pageSize,meterNumber);
            }
            catch (Exception)
            {

            }
            return nonRolloverEventSinglePhase;
        }
    }
}
