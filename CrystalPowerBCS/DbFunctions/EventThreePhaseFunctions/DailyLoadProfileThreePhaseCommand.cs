using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions
{
    public class DailyLoadProfileThreePhaseCommand
    {
        private readonly DailyLoadProfileThreePhaseService _dailyLoadProfileThreePhaseService;
        public ErrorHelper _errorHelper;
        public DailyLoadProfileThreePhaseCommand()
        {
            _dailyLoadProfileThreePhaseService = new DailyLoadProfileThreePhaseService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<DailyLoadProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<DailyLoadProfileThreePhaseDto> dailyLoadProfileThreePhase = new List<DailyLoadProfileThreePhaseDto>();
            try
            {
                dailyLoadProfileThreePhase = await _dailyLoadProfileThreePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "DailyLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return dailyLoadProfileThreePhase;
        }

        public async Task<List<DailyLoadProfileThreePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int PageSize, string meterNumber)
        {
            List<DailyLoadProfileThreePhaseDto> dailyLoadProfileThreePhase = new List<DailyLoadProfileThreePhaseDto>();
            try
            {
                dailyLoadProfileThreePhase = await _dailyLoadProfileThreePhaseService.Filter(startDate, endDate, fatchDate, PageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "DailyLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return dailyLoadProfileThreePhase;
        }
    }
}
