using Domain.Entities;
using Infrastructure.API;
using Infrastructure.DTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions
{
    public class MeterFetchDataLogCommand
    {
        private readonly MeterFetchDataLogService _meterFetchCommandService;
        public ErrorHelper _errorHelper;

        public MeterFetchDataLogCommand()
        {
            _meterFetchCommandService = new MeterFetchDataLogService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<MeterFetchDataLogDto>> GetAll(int pageSize, string meterNo)
        {
            List<MeterFetchDataLogDto> meter = new List<MeterFetchDataLogDto>();
            try
            {
                meter = await _meterFetchCommandService.GetAll(pageSize, meterNo);
            }
            catch (Exception)
            {

            }
            return meter;
        }
    }
}
