using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class BillingProfileSinglePhaseCommand
    {
        private readonly BillingProfileSinglePhaseService _billingProfileSinglePhaseService;
        public ErrorHelper _errorHelper;
        public BillingProfileSinglePhaseCommand()
        {
            _billingProfileSinglePhaseService = new BillingProfileSinglePhaseService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<BillingProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<BillingProfileSinglePhaseDto> billingProfileSinglePhase = new List<BillingProfileSinglePhaseDto>();
            try
            {
                billingProfileSinglePhase = await _billingProfileSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BillingProfileSinglePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return billingProfileSinglePhase;
        }

        public async Task<List<BillingProfileSinglePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            List<BillingProfileSinglePhaseDto> billingProfileSinglePhase = new List<BillingProfileSinglePhaseDto>();
            try
            {
                billingProfileSinglePhase = await _billingProfileSinglePhaseService.Filter(startDate, endDate, fatchDate, pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BillingProfileSinglePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return billingProfileSinglePhase;
        }
    }
}
