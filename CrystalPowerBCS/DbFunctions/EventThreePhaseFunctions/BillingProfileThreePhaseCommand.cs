using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions
{
    public class BillingProfileThreePhaseCommand
    {
        private readonly BillingProfileThreePhaseService _billingProfileThreePhaseService;
        public ErrorHelper _errorHelper;
        public BillingProfileThreePhaseCommand()
        {
            _billingProfileThreePhaseService = new BillingProfileThreePhaseService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<BillingProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<BillingProfileThreePhaseDto> billingProfileThreePhase = new List<BillingProfileThreePhaseDto>();
            try
            {
                billingProfileThreePhase = await _billingProfileThreePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BillingProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return billingProfileThreePhase;
        }

        public async Task<List<BillingProfileThreePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            List<BillingProfileThreePhaseDto> billingProfileThreePhase = new List<BillingProfileThreePhaseDto>();
            try
            {
                billingProfileThreePhase = await _billingProfileThreePhaseService.Filter(startDate, endDate, fatchDate, pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BillingProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return billingProfileThreePhase;
        }
    }
}
