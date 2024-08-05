using Infrastructure.API;
using Infrastructure.DTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions
{
    public class MeterCommand
    {
        private readonly MeterService _meterService;
        public ErrorHelper _errorHelper;

        public MeterCommand()
        {
            _meterService = new MeterService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<MeterDto>> GetAll(int pageSize)
        {
            List<MeterDto> meter = new List<MeterDto>();
            try
            {
                meter = await _meterService.GetAll(pageSize);
            }
            catch (Exception)
            {

            }
            return meter;
        }

        public async Task<List<MeterDto>> Filter(string search, int pageSize)
        {
            List<MeterDto> meter = new List<MeterDto>();
            try
            {
                meter = await _meterService.Filter(search,pageSize);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message);

            }
            return meter;
        }

        public async Task<MeterDto> GetMeterByMeterNo(string MeterNo)
        {
            MeterDto meter = new MeterDto();
            try
            {
                meter = await _meterService.GetByMeterNo(MeterNo);
            }
            catch(Exception ex)
            {
                _errorHelper.WriteLog(ex.Message);
            }
            return meter;
        }

        public async Task<bool> UpdateMeterByMeterNo(MeterDto meter)
        {
            try
            {
                return await _meterService.UpdateMeterByMeterNo(meter);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message);
                return false;
            }
        }
    }
}
