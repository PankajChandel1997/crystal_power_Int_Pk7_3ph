using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class VoltageRelatedEventSinglePhaseCommand
    {
        private readonly VoltageRelatedEventSinglePhaseService _voltageRelatedEventSinglePhaseService;
        public VoltageRelatedEventSinglePhaseCommand()
        {
            _voltageRelatedEventSinglePhaseService = new VoltageRelatedEventSinglePhaseService();
        }

        public async Task<List<VoltageRelatedEventSinglePhaseDTO>> GetAll(int pageSize, string meterNumber)
        {
            List<VoltageRelatedEventSinglePhaseDTO> voltageRelatedEvent = new List<VoltageRelatedEventSinglePhaseDTO>();
            try
            {
                voltageRelatedEvent = await _voltageRelatedEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return voltageRelatedEvent;
        }

        public async Task<List<VoltageRelatedEventSinglePhaseDTO>> Filter(string startDate, string endDate, int PageSize, string meterNumber)
        {
            List<VoltageRelatedEventSinglePhaseDTO> voltageRelatedEvent = new List<VoltageRelatedEventSinglePhaseDTO>();
            try
            {
                voltageRelatedEvent = await _voltageRelatedEventSinglePhaseService.Filter(startDate, endDate, PageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return voltageRelatedEvent;
        }
    }
}
