using Infrastructure.API.EventAPIThreePhase;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions
{
    public class BlockLoadProfileThreePhaseCommand
    {
        private readonly BlockLoadProfileThreePhaseService _blockLoadProfileThreePhaseService;
        public ErrorHelper _errorHelper;
        public BlockLoadProfileThreePhaseCommand()
        {
            _blockLoadProfileThreePhaseService = new BlockLoadProfileThreePhaseService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<BlockLoadProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<BlockLoadProfileThreePhaseDto> blockLoadProfileThreePhase = new List<BlockLoadProfileThreePhaseDto>();
            try
            {
                blockLoadProfileThreePhase = await _blockLoadProfileThreePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BlockLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return blockLoadProfileThreePhase;
        }

        public async Task<List<BlockLoadProfileThreePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            List<BlockLoadProfileThreePhaseDto> blockLoadProfileThreePhase = new List<BlockLoadProfileThreePhaseDto>();
            try
            {
                blockLoadProfileThreePhase = await _blockLoadProfileThreePhaseService.Filter(startDate, endDate, fatchDate, pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BlockLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return blockLoadProfileThreePhase;
        }
    }
}
