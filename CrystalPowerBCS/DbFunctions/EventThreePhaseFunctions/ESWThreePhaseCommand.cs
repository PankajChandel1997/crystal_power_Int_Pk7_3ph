using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions
{
    public class ESWThreePhaseCommand
    {
        private readonly ESWThreePhaseService _eswThreePhaseService;
        public ESWThreePhaseCommand()
        {
            _eswThreePhaseService = new ESWThreePhaseService();
        }

        public async Task<List<ESWThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<ESWThreePhaseDto> eswThreePhase = new List<ESWThreePhaseDto>();
            try
            {
                eswThreePhase = await _eswThreePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return eswThreePhase;
        }

        public async Task<List<ESWThreePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<ESWThreePhaseDto> eswThreePhase = new List<ESWThreePhaseDto>();
            try
            {
                eswThreePhase = await _eswThreePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return eswThreePhase;
        }
    }
}
