using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class EventProfileSinglePhaseService
    {
        private readonly IDataService<EventProfileSinglePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;
        public EventProfileSinglePhaseService()
        {
            _dataService = new GenericDataService<EventProfileSinglePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<EventProfileSinglePhase> Add(EventProfileSinglePhaseDto eventProfile)
        {
            try
            {
                EventProfileSinglePhase eventProfileSinglePhase = await ParseDataToClass(eventProfile);
                return await _dataService.Create(eventProfileSinglePhase);
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<EventProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                var response = await _dataService.GetAllAsync(pageSize);

                List<EventProfileSinglePhaseDto> eventProfileSinglePhase = await ParseDataToDTO(response);

                return eventProfileSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<EventProfileSinglePhase> ParseDataToClass(EventProfileSinglePhaseDto eventProfileSinglePhaseDto)
        {
            EventProfileSinglePhase eventProfileSinglePhase = new EventProfileSinglePhase();

            eventProfileSinglePhase.MeterNo = eventProfileSinglePhaseDto.MeterNo;
            eventProfileSinglePhase.CreatedOn = eventProfileSinglePhaseDto.CreatedOn;
            eventProfileSinglePhase.DateAndTimeOfEvent = eventProfileSinglePhaseDto.DateAndTimeOfEvent;
            eventProfileSinglePhase.EventCode = eventProfileSinglePhaseDto.EventCode;
            eventProfileSinglePhase.Current = eventProfileSinglePhaseDto.Current;
            eventProfileSinglePhase.Voltage = eventProfileSinglePhaseDto.Voltage;
            eventProfileSinglePhase.PowerFactor = eventProfileSinglePhaseDto.PowerFactor;
            eventProfileSinglePhase.CumulativeEnergyKwhImprot = StringExtensions.CheckNullable(eventProfileSinglePhaseDto.CumulativeEnergyKwhImprot);
            eventProfileSinglePhase.CumulativeEnergyKwhExport = StringExtensions.CheckNullable(eventProfileSinglePhaseDto.CumulativeEnergyKwhExport);
            eventProfileSinglePhase.CumulativeTamperCount = StringExtensions.CheckNullable(eventProfileSinglePhaseDto.CumulativeTamperCount);

            return eventProfileSinglePhase;
        }

        private async Task<List<EventProfileSinglePhaseDto>> ParseDataToDTO(List<EventProfileSinglePhase> EventProfileSinglePhaseList)
        {
            List<EventProfileSinglePhaseDto> EventProfileSinglePhaseDtoList = new List<EventProfileSinglePhaseDto>();
            foreach (var EventProfileSinglePhase in EventProfileSinglePhaseList)
            {
                EventProfileSinglePhaseDto EventProfileSinglePhaseDto = new EventProfileSinglePhaseDto();

                EventProfileSinglePhaseDto.MeterNo = EventProfileSinglePhase.MeterNo;
                EventProfileSinglePhaseDto.CreatedOn = EventProfileSinglePhase.CreatedOn;
                EventProfileSinglePhaseDto.EventCode = EventProfileSinglePhase.EventCode;
                EventProfileSinglePhaseDto.Current = EventProfileSinglePhase.Current;
                EventProfileSinglePhaseDto.Voltage = EventProfileSinglePhase.Voltage;
                EventProfileSinglePhaseDto.PowerFactor = EventProfileSinglePhase.PowerFactor;
                EventProfileSinglePhaseDto.CumulativeEnergyKwhImprot = StringExtensions.CheckNullable(EventProfileSinglePhase.CumulativeEnergyKwhImprot);
                EventProfileSinglePhaseDto.CumulativeEnergyKwhExport = StringExtensions.CheckNullable(EventProfileSinglePhase.CumulativeEnergyKwhExport);
                EventProfileSinglePhaseDto.CumulativeTamperCount = StringExtensions.CheckNullable(EventProfileSinglePhase.CumulativeTamperCount);

                EventProfileSinglePhaseDtoList.Add(EventProfileSinglePhaseDto);
            }

            return EventProfileSinglePhaseDtoList;
        }

    }
}
