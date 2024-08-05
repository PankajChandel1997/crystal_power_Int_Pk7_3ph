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
    public class MeterService
    {
        private readonly IDataService<Meter> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public MeterService()
        {
            _dataService = new GenericDataService<Meter>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<MeterDto> meterDto)
        {
            try
            {
                string query = "Select * from Meter where MeterNo = '" + meterDto[0].MeterNumber+ "'";
                var meterDeatils = await _dataService.Filter(query);
                if (meterDeatils.Count > 0)
                {
                    meterDeatils[0].DeviceId = meterDto[0].DeviceId;
                    meterDeatils[0].ManufacturerName = meterDto[0].ManufacturerName;
                    meterDeatils[0].FirmwareVersion = meterDto[0].FirmwareVersion;
                    meterDeatils[0].MeterType = meterDto[0].MeterType;
                    meterDeatils[0].Category = meterDto[0].Category;
                    meterDeatils[0].CurrentRating = meterDto[0].CurrentRating;
                    meterDeatils[0].MeterYearManufacturer = meterDto[0].MeterYearManufacturer;
                    meterDeatils[0].CTRatio = meterDto[0].CTRatio;
                    meterDeatils[0].PTRatio = meterDto[0].PTRatio;
                    meterDeatils[0].ManSpecificFirmwareVersion = meterDto[0].ManSpecificFirmwareVersion;
                    meterDeatils[0].ConsumerNo = meterDto[0].ConsumerNo;
                    meterDeatils[0].ConsumerName = meterDto[0].ConsumerName;
                    meterDeatils[0].ConsumerAddress = meterDto[0].ConsumerAddress;

                    var res = await _dataService.Update(meterDeatils[0]);
                    return true;
                }
                else
                {
                    List<Meter> meter = await ParseDataToClass(meterDto);
                    return await _dataService.CreateRange(meter);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<MeterDto>> GetAll(int pageSize)
        {
            try
            {
                var response = await _dataService.GetAllAsync(pageSize);

                List<MeterDto> meterDtos = await ParseDataToDTO(response);

                return meterDtos;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MeterDto>> Filter(string search,int pageSize)
        {
            try
            {
                string query = !string.IsNullOrEmpty(search) ? "select * from Meter where MeterNo like '"+ search + "%' LIMIT "+ pageSize : "select * from MeterNo LIMIT " + pageSize; ;

                var response = await _dataService.Filter(query);

                List<MeterDto> meterDtos = await ParseDataToDTO(response);

                return meterDtos;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<MeterDto> GetByMeterNo(string MeterNo)
        {
            try
            {
                string query = "select * from Meter where MeterNo = '"+ MeterNo + "'";

                var response = await _dataService.Filter(query);

                List<MeterDto> meterDtos = await ParseDataToDTO(response);

                return meterDtos[0];
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException?.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMeterByMeterNo(MeterDto meter)
        {
            try
            {
                string query = "select * from Meter where MeterNo = '" + meter.MeterNumber + "'";
                var response = await _dataService.Filter(query);

                if (response != null)
                {
                    response[0].ConsumerNo = meter.ConsumerNo;
                    response[0].ConsumerName = meter.ConsumerName;
                    response[0].ConsumerAddress = meter.ConsumerAddress;

                    await _dataService.Update(response[0]);
                }
                
                return true;
            }
            catch(Exception ex) 
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException?.Message);
                return false;
            }
        }

        private async Task<List<Meter>> ParseDataToClass(List<MeterDto> meterDtos)
        {
            List<Meter> metersList = new List<Meter>();

            foreach (var meterDto in meterDtos)
            {
                Meter meter = new Meter();

                meter.MeterNo = meterDto.MeterNumber;
                meter.DeviceId = meterDto.DeviceId;
                meter.ManufacturerName = meterDto.ManufacturerName;
                meter.FirmwareVersion = meterDto.FirmwareVersion;
                meter.MeterType = meterDto.MeterType;
                meter.Category = meterDto.Category;
                meter.CurrentRating = meterDto.CurrentRating;
                meter.MeterYearManufacturer = meterDto.MeterYearManufacturer;
                meter.CTRatio = meterDto.CTRatio ?? "1";
                meter.PTRatio = meterDto.PTRatio ?? "1";
                meter.ManSpecificFirmwareVersion=meterDto.ManSpecificFirmwareVersion;
                meter.ConsumerName = meterDto.ConsumerName;
                meter.ConsumerAddress = meterDto.ConsumerAddress;
                meter.ConsumerNo = meterDto.ConsumerNo;

                metersList.Add(meter);
            }
            return metersList;
        }

        private async Task<List<MeterDto>> ParseDataToDTO(List<Meter> meters)
        {
            List<MeterDto> meterDtoList = new List<MeterDto>();
            foreach (var meter in meters)
            {
                MeterDto meterDto = new MeterDto();

                meterDto.MeterNumber = meter.MeterNo;
                meterDto.DeviceId = meter.DeviceId;
                meterDto.ManufacturerName = meter.ManufacturerName;
                meterDto.FirmwareVersion = meter.FirmwareVersion;
                meterDto.MeterType = meter.MeterType;
                meterDto.Category = meter.Category;
                meterDto.CurrentRating = meter.CurrentRating;
                meterDto.MeterYearManufacturer = meter.MeterYearManufacturer;
                meterDto.CTRatio = meter.CTRatio;
                meterDto.PTRatio = meter.PTRatio;
                meterDto.ConsumerName = meter.ConsumerName;
                meterDto.ConsumerAddress = meter.ConsumerAddress;
                meterDto.ConsumerNo = meter.ConsumerNo;

                meterDtoList.Add(meterDto);
            }

            return meterDtoList;
        }
    }
}
