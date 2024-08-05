using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class OtherEventSinglePhaseCommand
    {
        private readonly OtherEventSinglePhaseService _OtherEventSinglePhaseService;
        public OtherEventSinglePhaseCommand()
        {
            _OtherEventSinglePhaseService = new OtherEventSinglePhaseService();
        }

        public async Task<List<OtherEventSinglePhaseDTO>> GetAll(int pageSize, string meterNumber)
        {
            List<OtherEventSinglePhaseDTO> OtherEvent = new List<OtherEventSinglePhaseDTO>();
            try
            {
                OtherEvent = await _OtherEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return OtherEvent;
        }

        public async Task<List<OtherEventSinglePhaseDTO>> Filter(string startDate, string endDate, int PageSize, string meterNumber)
        {
            List<OtherEventSinglePhaseDTO> OtherEvent = new List<OtherEventSinglePhaseDTO>();
            try
            {
                OtherEvent = await _OtherEventSinglePhaseService.Filter(startDate, endDate, PageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return OtherEvent;
        }
    }
}
