using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.API
{
    public class MeterFetchDataLogService
    {
        private readonly IDataService<MeterFetchDataLog> _dataService;
        public ErrorHelper _errorHelper;
        public MeterFetchDataLogService()
        {
            _dataService = new GenericDataService<MeterFetchDataLog>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<MeterFetchDataLogDto> meterDto)
        {
            try
            {
                List<MeterFetchDataLog> meter = await ParseDataToClass(meterDto);
                return await _dataService.CreateRange(meter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<MeterFetchDataLogDto>> GetAll(int pageSize, string meterNo)
        {
            try
            {
                string query = "select * from MeterFetchDataLog where MeterNo = '" + meterNo + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<MeterFetchDataLogDto> meterDtos = await ParseDataToDTO(response);

                return meterDtos;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<MeterFetchDataLog>> ParseDataToClass(List<MeterFetchDataLogDto> meterDtos)
        {
            List<MeterFetchDataLog> metersList = new List<MeterFetchDataLog>();

            foreach (var meterDto in meterDtos)
            {
                MeterFetchDataLog meter = new MeterFetchDataLog();

                meter.MeterNo = meterDto.MeterNo;

                metersList.Add(meter);
            }
            return metersList;
        }

        private async Task<List<MeterFetchDataLogDto>> ParseDataToDTO(List<MeterFetchDataLog> meters)
        {
            List<MeterFetchDataLogDto> meterDtoList = new List<MeterFetchDataLogDto>();
            foreach (var meter in meters)
            {
                MeterFetchDataLogDto meterDto = new MeterFetchDataLogDto();

                meterDto.MeterNo = meter.MeterNo;
                meterDto.DateTime = meter.CreatedOn;

                meterDtoList.Add(meterDto);
            }

            return meterDtoList;
        }
    }
}
