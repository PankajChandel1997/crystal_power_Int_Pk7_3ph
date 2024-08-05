using System;
using Domain.Interface.Service;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Helpers;
using Domain.Entities.ThreePhaseEntities;
using Infrastructure.DTOs.ThreePhaseEventDTOs;

namespace Infrastructure.API.EventAPIThreePhase
{
    public class ESWThreePhaseService
    {
        private readonly IDataService<ESWThreePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public ESWThreePhaseService()
        {
            _dataService = new GenericDataService<ESWThreePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<ESWThreePhaseDto> eswDto)
        {
            try
            {
                List<ESWThreePhase> esw = await ParseDataToClass(eswDto);
                return await _dataService.CreateRange(esw);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ESWThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                var response = await _dataService.GetAllAsync(pageSize);

                List<ESWThreePhaseDto> eswDtos = await ParseDataToDTO(response);

                return eswDtos;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ESWThreePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = !string.IsNullOrEmpty(startDate) ? "select * from ESWThreePhase where Realtimeclock BETWEEN '" + startDate + "' and '" + endDate + "' ORDER by Id DESC LIMIT " + pageSize : "select * from ESWThreePhase ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<ESWThreePhaseDto> esw = await ParseDataToDTO(response);

                return esw;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<ESWThreePhase>> ParseDataToClass(List<ESWThreePhaseDto> eswDtos)
        {
            List<ESWThreePhase> eswsList = new List<ESWThreePhase>();

            foreach (var eswDto in eswDtos)
            {
                ESWThreePhase esw = new ESWThreePhase();

                esw.MeterNo = eswDto.MeterNo;
                esw.CreatedOn = eswDto.CreatedOn;
                esw.RPhaseVoltageMissing = eswDto.RPhaseVoltageMissing;
                esw.YPhaseVoltageMissing = eswDto.YPhaseVoltageMissing;
                esw.BPhaseVoltageMissing = eswDto.BPhaseVoltageMissing;
                esw.OverVoltage = eswDto.OverVoltage;
                esw.LowVoltage = eswDto.LowVoltage;
                esw.VoltagUnbalance = eswDto.VoltagUnbalance;
                esw.RPhaseCurrentReverse = eswDto.RPhaseCurrentReverse;
                esw.YPhaseCurrentReverse = eswDto.YPhaseCurrentReverse;
                esw.BPhaseCurrentReverse = eswDto.BPhaseCurrentReverse;
                esw.CurrentUnbalance = eswDto.CurrentUnbalance;
                esw.CurrentBypass = eswDto.CurrentBypass;
                esw.OverCurrent = eswDto.OverCurrent;
                esw.VerylowPF = eswDto.VerylowPF;
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

        private async Task<List<ESWThreePhaseDto>> ParseDataToDTO(List<ESWThreePhase> esws)
        {
            List<ESWThreePhaseDto> eswDtoList = new List<ESWThreePhaseDto>();
            foreach (var esw in esws)
            {
                ESWThreePhaseDto eswDto = new ESWThreePhaseDto();

                eswDto.MeterNo = esw.MeterNo;
                eswDto.CreatedOn = esw.CreatedOn;
                eswDto.RPhaseVoltageMissing = esw.RPhaseVoltageMissing;
                eswDto.YPhaseVoltageMissing = esw.YPhaseVoltageMissing;
                eswDto.BPhaseVoltageMissing = esw.BPhaseVoltageMissing;
                eswDto.OverVoltage = esw.OverVoltage;
                eswDto.LowVoltage = esw.LowVoltage;
                eswDto.VoltagUnbalance = esw.VoltagUnbalance;
                eswDto.RPhaseCurrentReverse = esw.RPhaseCurrentReverse;
                eswDto.YPhaseCurrentReverse = esw.YPhaseCurrentReverse;
                eswDto.BPhaseCurrentReverse = esw.BPhaseCurrentReverse;
                eswDto.CurrentUnbalance = esw.CurrentUnbalance;
                eswDto.CurrentBypass = esw.CurrentBypass;
                eswDto.OverCurrent = esw.OverCurrent;
                eswDto.VerylowPF = esw.VerylowPF;
                eswDto.InfluenceOfPermanetMagnetOorAcDc = esw.InfluenceOfPermanetMagnetOorAcDc;
                eswDto.NeutralDisturbance = esw.NeutralDisturbance;
                eswDto.MeterCoverOpen = esw.MeterCoverOpen;
                eswDto.MeterLoadDisconnectConnected = esw.MeterLoadDisconnectConnected;
                eswDto.LastGasp = esw.LastGasp;
                eswDto.FirstBreath = esw.FirstBreath;
                eswDto.IncrementInBillingCounterMRI = esw.IncrementInBillingCounterMRI;

                eswDtoList.Add(eswDto);
            }

            return eswDtoList;
        }
    }
}
