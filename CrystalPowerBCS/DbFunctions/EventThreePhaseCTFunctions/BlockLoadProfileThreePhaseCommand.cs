using Infrastructure.API.EventAPIThreePhaseCT;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventThreePhaseCTFunctions
{
    public class BlockLoadProfileThreePhaseCTCommand
    {
        private readonly BlockLoadProfileThreePhaseCTService _blockLoadProfileThreePhaseCTService;
        public ErrorHelper _errorHelper;
        public BlockLoadProfileThreePhaseCTCommand()
        {
            _blockLoadProfileThreePhaseCTService = new BlockLoadProfileThreePhaseCTService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<BlockLoadProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            List<BlockLoadProfileThreePhaseCTDto> blockLoadProfileThreePhaseCT = new List<BlockLoadProfileThreePhaseCTDto>();
            try
            {
                blockLoadProfileThreePhaseCT = await _blockLoadProfileThreePhaseCTService.GetAll(pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BlockLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return blockLoadProfileThreePhaseCT;
        }

        public async Task<List<BlockLoadProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            List<BlockLoadProfileThreePhaseCTDto> blockLoadProfileThreePhaseCT = new List<BlockLoadProfileThreePhaseCTDto>();
            try
            {
                blockLoadProfileThreePhaseCT = await _blockLoadProfileThreePhaseCTService.Filter(startDate, endDate, fatchDate, pageSize, meterNumber);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "BlockLoadProfileThreePhaseCommand : inner Exception ==> " + ex.InnerException.Message);
            }
            return blockLoadProfileThreePhaseCT;
        }
    }
}
