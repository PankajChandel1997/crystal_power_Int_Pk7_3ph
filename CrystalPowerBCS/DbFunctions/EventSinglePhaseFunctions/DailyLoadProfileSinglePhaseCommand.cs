using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class DailyLoadProfileSinglePhaseCommand
    {
        private readonly DailyLoadProfileSinglePhaseService _dailyLoadProfileSinglePhaseService;
        public DailyLoadProfileSinglePhaseCommand()
        {
            _dailyLoadProfileSinglePhaseService = new DailyLoadProfileSinglePhaseService();
        }

        public async Task<List<DailyLoadProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<DailyLoadProfileSinglePhaseDto> dailyLoadProfileSinglePhase = new List<DailyLoadProfileSinglePhaseDto>();
            try
            {
                dailyLoadProfileSinglePhase = await _dailyLoadProfileSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return dailyLoadProfileSinglePhase;
        }

        public async Task<List<DailyLoadProfileSinglePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int PageSize, string meterNumber)
        {
            List<DailyLoadProfileSinglePhaseDto> dailyLoadProfileSinglePhase = new List<DailyLoadProfileSinglePhaseDto>();
            try
            {
                dailyLoadProfileSinglePhase = await _dailyLoadProfileSinglePhaseService.Filter(startDate, endDate, fatchDate, PageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return dailyLoadProfileSinglePhase;
        }
    }
}
