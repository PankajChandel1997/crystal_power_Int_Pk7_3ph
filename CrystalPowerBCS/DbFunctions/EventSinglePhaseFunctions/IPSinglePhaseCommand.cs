using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class IPSinglePhaseCommand
    {
        private readonly InstantaneousProfileSinglePhaseService _instantaneousProfileSinglePhaseService;
        public IPSinglePhaseCommand()
        {
            _instantaneousProfileSinglePhaseService = new InstantaneousProfileSinglePhaseService();
        }

        public async Task<List<InstantaneousProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<InstantaneousProfileSinglePhaseDto> instantaneousProfileSingle = new List<InstantaneousProfileSinglePhaseDto>();
            try
            {
                instantaneousProfileSingle = await _instantaneousProfileSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return instantaneousProfileSingle;
        }

        public async Task<List<InstantaneousProfileSinglePhaseDto>> Filter(string startDate, string endDate, string fetchDate, int pageSize, string meterNumber)
        {
            List<InstantaneousProfileSinglePhaseDto> instantaneousProfileSingle = new List<InstantaneousProfileSinglePhaseDto>();
            try
            {
                instantaneousProfileSingle = await _instantaneousProfileSinglePhaseService.Filter(startDate, endDate,fetchDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return instantaneousProfileSingle;
        }
    }
}
