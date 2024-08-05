using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class ESWSinglePhaseCommand
    {
        private readonly ESWSinglePhaseService _eswSinglePhaseService;
        public ESWSinglePhaseCommand()
        {
            _eswSinglePhaseService = new ESWSinglePhaseService();
        }

        public async Task<List<ESWSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<ESWSinglePhaseDto> eswSinglePhase = new List<ESWSinglePhaseDto>();
            try
            {
                eswSinglePhase = await _eswSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return eswSinglePhase;
        }

        public async Task<List<ESWSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<ESWSinglePhaseDto> eswSinglePhase = new List<ESWSinglePhaseDto>();
            try
            {
                eswSinglePhase = await _eswSinglePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return eswSinglePhase;
        }
    }
}
