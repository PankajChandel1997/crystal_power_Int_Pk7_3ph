using Infrastructure.API.EventAPIThreePhaseCT;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions
{
    public class DailyLoadProfileThreePhaseCTCommand
    {
        private readonly DailyLoadProfileThreePhaseCTService _dailyLoadProfileThreePhaseCTService;
        public ErrorHelper _errorHelper;
        public DailyLoadProfileThreePhaseCTCommand()
        {
            _dailyLoadProfileThreePhaseCTService = new DailyLoadProfileThreePhaseCTService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<DailyLoadProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            List<DailyLoadProfileThreePhaseCTDto> dailyLoadProfileThreePhaseCT = new List<DailyLoadProfileThreePhaseCTDto>();
            try
            {
                dailyLoadProfileThreePhaseCT = await _dailyLoadProfileThreePhaseCTService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "DailyLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return dailyLoadProfileThreePhaseCT;
        }

        public async Task<List<DailyLoadProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fatchDate, int PageSize, string meterNumber)
        {
            List<DailyLoadProfileThreePhaseCTDto> dailyLoadProfileThreePhaseCT = new List<DailyLoadProfileThreePhaseCTDto>();
            try
            {
                dailyLoadProfileThreePhaseCT = await _dailyLoadProfileThreePhaseCTService.Filter(startDate, endDate, fatchDate, PageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "DailyLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return dailyLoadProfileThreePhaseCT;
        }
    }
}
