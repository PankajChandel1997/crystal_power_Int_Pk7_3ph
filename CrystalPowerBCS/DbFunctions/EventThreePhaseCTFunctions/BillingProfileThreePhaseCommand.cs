using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.API.EventAPIThreePhaseCT;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions
{
    public class BillingProfileThreePhaseCTCommand
    {
        private readonly BillingProfileThreePhaseCTService _billingProfileThreePhaseCTService;
        public ErrorHelper _errorHelper;
        public BillingProfileThreePhaseCTCommand()
        {
            _billingProfileThreePhaseCTService = new BillingProfileThreePhaseCTService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<BillingProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            List<BillingProfileThreePhaseCTDto> billingProfileThreePhaseCT = new List<BillingProfileThreePhaseCTDto>();
            try
            {
                billingProfileThreePhaseCT = await _billingProfileThreePhaseCTService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BillingProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return billingProfileThreePhaseCT;
        }

        public async Task<List<BillingProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            List<BillingProfileThreePhaseCTDto> billingProfileThreePhaseCT = new List<BillingProfileThreePhaseCTDto>();
            try
            {
                billingProfileThreePhaseCT = await _billingProfileThreePhaseCTService.Filter(startDate, endDate, fatchDate, pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BillingProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return billingProfileThreePhaseCT;
        }
    }
}
