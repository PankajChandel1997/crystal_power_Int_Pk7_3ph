using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions
{
    public class IPThreePhaseCommand
    {
        private readonly InstantaneousProfileThreePhaseService _instantaneousProfileThreePhaseService;
        public IPThreePhaseCommand()
        {
            _instantaneousProfileThreePhaseService = new InstantaneousProfileThreePhaseService();
        }

        public async Task<List<InstantaneousProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<InstantaneousProfileThreePhaseDto> instantaneousProfileThree = new List<InstantaneousProfileThreePhaseDto>();
            try
            {
                instantaneousProfileThree = await _instantaneousProfileThreePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return instantaneousProfileThree;
        }

        public async Task<List<InstantaneousProfileThreePhaseDto>> Filter(string startDate, string endDate, string fetchDate, int pageSize, string meterNumber)
        {
            List<InstantaneousProfileThreePhaseDto> instantaneousProfileThree = new List<InstantaneousProfileThreePhaseDto>();
            try
            {
                instantaneousProfileThree = await _instantaneousProfileThreePhaseService.Filter(startDate, endDate, fetchDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return instantaneousProfileThree;
        }
    }
}
