using Infrastructure.API.EventAPIs;
using Infrastructure.DTOs.EventDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions.EventFunctions
{
    public class TransactionEventCommand
    {
        private readonly TransactionEventService _transactionEventService;
        public TransactionEventCommand()
        {
            _transactionEventService = new TransactionEventService();
        }

        public async Task<List<TransactionEventDto>> GetAll(int pageSize,string meterNumber)
        {
            List<TransactionEventDto> transactionEvent = new List<TransactionEventDto>();
            try
            {
                transactionEvent = await _transactionEventService.GetAll(pageSize,  meterNumber);
            }
            catch (Exception)
            {

            }
            return transactionEvent;
        }

        public async Task<List<TransactionEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            List<TransactionEventDto> transactionEvent = new List<TransactionEventDto>();
            try
            {
                transactionEvent = await _transactionEventService.Filter(startDate, endDate, pageSize,  meterNumber);
            }
            catch (Exception)
            {

            }
            return transactionEvent;
        }
    }
}
