using Infrastructure.API.EventAPIThreePhaseCT;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions
{
    public class IPThreePhaseCTCommand
    {
        private readonly InstantaneousProfileThreePhaseCTService _instantaneousProfileThreePhaseCTService;
        public IPThreePhaseCTCommand()
        {
            _instantaneousProfileThreePhaseCTService = new InstantaneousProfileThreePhaseCTService();
        }

        public async Task<List<InstantaneousProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            List<InstantaneousProfileThreePhaseCTDto> instantaneousProfileThreeCT = new List<InstantaneousProfileThreePhaseCTDto>();
            try
            {
                instantaneousProfileThreeCT = await _instantaneousProfileThreePhaseCTService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return instantaneousProfileThreeCT;
        }

        public async Task<List<InstantaneousProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fetchDate,int pageSize, string meterNumber)
        {
            List<InstantaneousProfileThreePhaseCTDto> instantaneousProfileThreeCT = new List<InstantaneousProfileThreePhaseCTDto>();
            try
            {
                instantaneousProfileThreeCT = await _instantaneousProfileThreePhaseCTService.Filter(startDate, endDate, fetchDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return instantaneousProfileThreeCT;
        }
    }
}
