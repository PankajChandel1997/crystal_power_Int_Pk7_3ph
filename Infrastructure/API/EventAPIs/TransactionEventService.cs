using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIs
{
    public class TransactionEventService
    {
        private readonly IDataService<TransactionEvent> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public TransactionEventService()
        {
            _dataService = new GenericDataService<TransactionEvent>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<TransactionEvent> transactionEventModel)
        {
            try
            {
                await Delete(transactionEventModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(transactionEventModel);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> Delete(string meterNumber)
        {
            try
            {
                using (ApplicationDBContext db = _contextFactory.CreateDbContext())
                {
                    string query = "select * from TransactionEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<TransactionEvent>().RemoveRange(res);
                            await db.SaveChangesAsync();

                        }
                };
                return true;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<TransactionEventDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from TransactionEvent where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<TransactionEventDto> transactionEvent = await ParseDataToDTO(response);

                return transactionEvent;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<TransactionEvent>> ParseDataToClass(List<TransactionEventDto> transactionEventDtoList)
        {
            List<TransactionEvent> transactionEventList = new List<TransactionEvent>();

            foreach (var transactionEventDto in transactionEventDtoList)
            {
                TransactionEvent transactionEvent = new TransactionEvent();

                transactionEvent.MeterNo = transactionEventDto.MeterNo;
                transactionEvent.CreatedOn = transactionEventDto.CreatedOn;
                transactionEvent.RealTimeClockDateAndTime = transactionEventDto.RealTimeClockDateAndTime;
                transactionEvent.EventCode = transactionEventDto.Event;
                transactionEvent.GenericEventLogSequenceNumber = transactionEventDto.GenericEventLogSequenceNumber;

                transactionEventList.Add(transactionEvent);
            }

            return transactionEventList;
        }

        public async Task<List<TransactionEventDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from TransactionEvent where MeterNo = '" + meterNumber + "' order by RealTimeClockDateAndTime desc";

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

                List<TransactionEventDto> transactionEventDto = await ParseDataToDTO(response);

                return transactionEventDto;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<TransactionEventDto>> ParseDataToDTO(List<TransactionEvent> transactionEventList)
        {
            int index = 1;
            List<TransactionEventDto> transactionEventDtoList = new List<TransactionEventDto>();
            foreach (var transactionEvent in transactionEventList)
            {
                TransactionEventDto transactionEventDto = new TransactionEventDto();

                transactionEventDto.Number = index;
                transactionEventDto.RealTimeClockDateAndTime = transactionEvent.RealTimeClockDateAndTime;
                transactionEventDto.Event = transactionEvent.EventCode;
                transactionEventDto.CreatedOn = transactionEvent.CreatedOn;
                transactionEventDto.GenericEventLogSequenceNumber = transactionEvent.GenericEventLogSequenceNumber;

                transactionEventDtoList.Add(transactionEventDto);
                index++;
            }

            return transactionEventDtoList;
        }
    }
}