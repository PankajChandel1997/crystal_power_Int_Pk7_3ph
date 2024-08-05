using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class ESWSinglePhaseService
    {
        private readonly IDataService<ESWSinglePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public ESWSinglePhaseService()
        {
            _dataService = new GenericDataService<ESWSinglePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<ESWSinglePhaseDto> eswDto)
        {
            try
            {
                List<ESWSinglePhase> esw = await ParseDataToClass(eswDto);
                return await _dataService.CreateRange(esw);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ESWSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                var response = await _dataService.GetAllAsync(pageSize);

                List<ESWSinglePhaseDto> eswDtos = await ParseDataToDTO(response);

                return eswDtos;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ESWSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = !string.IsNullOrEmpty(startDate) ? "select * from ESWSinglePhase where Realtimeclock BETWEEN '" + startDate + "' and '" + endDate + "' ORDER by Id DESC LIMIT " + pageSize : "select * from ESWSinglePhase ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<ESWSinglePhaseDto> esw = await ParseDataToDTO(response);

                return esw;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<ESWSinglePhase>> ParseDataToClass(List<ESWSinglePhaseDto> eswDtos)
        {
            List<ESWSinglePhase> eswsList = new List<ESWSinglePhase>();

            foreach (var eswDto in eswDtos)
            {
                ESWSinglePhase esw = new ESWSinglePhase();

                esw.MeterNo = eswDto.MeterNo;
                esw.CreatedOn = eswDto.CreatedOn;
                esw.OverVoltage = eswDto.OverVoltage;
                esw.LowVoltage = eswDto.LowVoltage;
                esw.OverCurrent = eswDto.OverCurrent;
                esw.VerylowPF = eswDto.VerylowPF;
                esw.EarthLoading = eswDto.EarthLoading;
                esw.InfluenceOfPermanetMagnetOorAcDc = eswDto.InfluenceOfPermanetMagnetOorAcDc;
                esw.NeutralDisturbance = eswDto.NeutralDisturbance;
                esw.MeterCoverOpen = eswDto.MeterCoverOpen;
                esw.MeterLoadDisconnectConnected = eswDto.MeterLoadDisconnectConnected;
                esw.LastGasp = eswDto.LastGasp;
                esw.FirstBreath = eswDto.FirstBreath;
                esw.IncrementInBillingCounterMRI = eswDto.IncrementInBillingCounterMRI;

                eswsList.Add(esw);
            }
            return eswsList;
        }

        private async Task<List<ESWSinglePhaseDto>> ParseDataToDTO(List<ESWSinglePhase> esws)
        {
            List<ESWSinglePhaseDto> eswDtoList = new List<ESWSinglePhaseDto>();
            foreach (var esw in esws)
            {
                ESWSinglePhaseDto eswDto = new ESWSinglePhaseDto();

                eswDto.MeterNo = esw.MeterNo;
                eswDto.CreatedOn = esw.CreatedOn;
                eswDto.OverVoltage = esw.OverVoltage;
                eswDto.LowVoltage = esw.LowVoltage;
                eswDto.OverCurrent = esw.OverCurrent;
                eswDto.VerylowPF = esw.VerylowPF;
                eswDto.EarthLoading = esw.EarthLoading;
                eswDto.InfluenceOfPermanetMagnetOorAcDc = esw.InfluenceOfPermanetMagnetOorAcDc;
                eswDto.NeutralDisturbance = esw.NeutralDisturbance;
                eswDto.MeterCoverOpen = esw.MeterCoverOpen;
                eswDto.MeterLoadDisconnectConnected = esw.MeterLoadDisconnectConnected;
                eswDto.LastGasp = eswDto.LastGasp;
                eswDto.FirstBreath = eswDto.FirstBreath;
                eswDto.IncrementInBillingCounterMRI = eswDto.IncrementInBillingCounterMRI;

                eswDtoList.Add(eswDto);
            }

            return eswDtoList;
        }
    }
}
