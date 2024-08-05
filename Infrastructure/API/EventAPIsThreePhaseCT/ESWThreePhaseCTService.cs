using System;
using Domain.Interface.Service;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Helpers;
using Domain.Entities.ThreePhaseCTEntities;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;

namespace Infrastructure.API.EventAPIThreePhaseCT
{
    public class ESWThreePhaseCTService
    {
        private readonly IDataService<ESWThreePhaseCT> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public ESWThreePhaseCTService()
        {
            _dataService = new GenericDataService<ESWThreePhaseCT>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<ESWThreePhaseCTDto> eswDto)
        {
            try
            {
                List<ESWThreePhaseCT> esw = await ParseDataToClass(eswDto);
                return await _dataService.CreateRange(esw);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ESWThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                var response = await _dataService.GetAllAsync(pageSize);

                List<ESWThreePhaseCTDto> eswDtos = await ParseDataToDTO(response);

                return eswDtos;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ESWThreePhaseCTDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = !string.IsNullOrEmpty(startDate) ? "select * from ESWThreePhaseCT where Realtimeclock BETWEEN '" + startDate + "' and '" + endDate + "' ORDER by Id DESC LIMIT " + pageSize : "select * from ESWThreePhaseCT ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<ESWThreePhaseCTDto> esw = await ParseDataToDTO(response);

                return esw;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<ESWThreePhaseCT>> ParseDataToClass(List<ESWThreePhaseCTDto> eswDtos)
        {
            List<ESWThreePhaseCT> eswsList = new List<ESWThreePhaseCT>();

            foreach (var eswDto in eswDtos)
            {
                ESWThreePhaseCT esw = new ESWThreePhaseCT();

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

        private async Task<List<ESWThreePhaseCTDto>> ParseDataToDTO(List<ESWThreePhaseCT> esws)
        {
            List<ESWThreePhaseCTDto> eswDtoList = new List<ESWThreePhaseCTDto>();
            foreach (var esw in esws)
            {
                ESWThreePhaseCTDto eswDto = new ESWThreePhaseCTDto();

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
