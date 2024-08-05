using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class BlockLoadProfileSinglePhaseCommand
    {
        private readonly BlockLoadProfileSinglePhaseService _blockLoadProfileSinglePhaseService;
        public ErrorHelper _errorHelper;
        public BlockLoadProfileSinglePhaseCommand()
        {
            _blockLoadProfileSinglePhaseService = new BlockLoadProfileSinglePhaseService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<BlockLoadProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<BlockLoadProfileSinglePhaseDto> blockLoadProfileSinglePhase = new List<BlockLoadProfileSinglePhaseDto>();
            try
            {
                blockLoadProfileSinglePhase = await _blockLoadProfileSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {

            }
            return blockLoadProfileSinglePhase;
        }

        public async Task<List<BlockLoadProfileSinglePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            List<BlockLoadProfileSinglePhaseDto> blockLoadProfileSinglePhase = new List<BlockLoadProfileSinglePhaseDto>();
            try
            {
                blockLoadProfileSinglePhase = await _blockLoadProfileSinglePhaseService.Filter(startDate, endDate, fatchDate, pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
            }
            return blockLoadProfileSinglePhase;
        }
    }
}
