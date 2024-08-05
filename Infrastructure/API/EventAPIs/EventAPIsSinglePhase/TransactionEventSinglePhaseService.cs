using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class TransactionEventSinglePhaseService
    {
        private readonly IDataService<TransactionEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public TransactionEventSinglePhaseService()
        {
            _dataService = new GenericDataService<TransactionEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }

        public async Task<bool> Add(List<TransactionEventSinglePhase> transactionRelatedEventModel)
        {
            try
            {
                await Delete(transactionRelatedEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(transactionRelatedEventModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> Delete(string meterNumber)
        {
            try
            {
                using (ApplicationDBContext db = _contextFactory.CreateDbContext())
                {
                    string query = "select * from TransactionEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<TransactionEventSinglePhase>().RemoveRange(res);
                        }
                    await db.SaveChangesAsync();
                };
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<TransactionEventSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from TransactionEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<TransactionEventSinglePhaseDto> transactionRelatedEvent = await ParseDataToDTO(response);

                return transactionRelatedEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TransactionEventSinglePhaseDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from TransactionEventSinglePhase where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                List<TransactionEventSinglePhaseDto> transactionEvent = await ParseDataToDTO(response);

                return transactionEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<TransactionEventSinglePhase>> ParseDataToClass(List<TransactionEventSinglePhaseDto> transactionRelatedEventDtoList)
        //{
        //    List<TransactionEventSinglePhase> transactionRelatedEventList = new List<TransactionEventSinglePhase>();

        //    foreach (var transactionRelatedEventDto in transactionRelatedEventDtoList)
        //    {
        //        TransactionEventSinglePhase transactionRelatedEvent = new TransactionEventSinglePhase();

        //        transactionRelatedEvent.MeterNo = transactionRelatedEventDto.MeterNo;
        //        transactionRelatedEvent.CreatedOn = transactionRelatedEventDto.CreatedOn;
        //        transactionRelatedEvent.RealTimeClockDateAndTime = transactionRelatedEventDto.RealTimeClockDateAndTime;
        //        transactionRelatedEvent.EventCode = transactionRelatedEventDto.Event;

        //        transactionRelatedEventList.Add(transactionRelatedEvent);
        //    }

        //    return transactionRelatedEventList;
        //}

        private async Task<List<TransactionEventSinglePhaseDto>> ParseDataToDTO(List<TransactionEventSinglePhase> transactionRelatedEventList)
        {
            int index = 1;
            List<TransactionEventSinglePhaseDto> transactionRelatedEventDtoList = new List<TransactionEventSinglePhaseDto>();
            foreach (var transactionRelatedEvent in transactionRelatedEventList)
            {
                TransactionEventSinglePhaseDto transactionRelatedEventDto = new TransactionEventSinglePhaseDto();

                transactionRelatedEventDto.Number = index;
                transactionRelatedEventDto.RealTimeClockDateAndTime = transactionRelatedEvent.RealTimeClockDateAndTime;
                transactionRelatedEventDto.Event = transactionRelatedEvent.EventCode;
                transactionRelatedEventDto.CreatedOn = transactionRelatedEvent.CreatedOn;

                transactionRelatedEventDtoList.Add(transactionRelatedEventDto);
                index++;
            }

            return transactionRelatedEventDtoList;
        }
    }
}
