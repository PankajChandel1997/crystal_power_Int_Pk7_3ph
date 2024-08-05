using Infrastructure.API.EventAPIsSinglePhase;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions
{
    public class TransactionEventSinglePhaseCommand
    {
        private readonly TransactionEventSinglePhaseService _transactionEventSinglePhaseService;
        public TransactionEventSinglePhaseCommand()
        {
            _transactionEventSinglePhaseService = new TransactionEventSinglePhaseService();
        }

        public async Task<List<TransactionEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            List<TransactionEventSinglePhaseDto> transactionEventSinglePhase = new List<TransactionEventSinglePhaseDto>();
            try
            {
                transactionEventSinglePhase = await _transactionEventSinglePhaseService.GetAll(pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return transactionEventSinglePhase;
        }

        public async Task<List<TransactionEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<TransactionEventSinglePhaseDto> transactionEventSinglePhase = new List<TransactionEventSinglePhaseDto>();
            try
            {
                transactionEventSinglePhase = await _transactionEventSinglePhaseService.Filter(startDate, endDate, pageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return transactionEventSinglePhase;
        }
    }
}
