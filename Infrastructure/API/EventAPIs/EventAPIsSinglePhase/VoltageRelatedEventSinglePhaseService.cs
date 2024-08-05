using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class VoltageRelatedEventSinglePhaseService
    {
        private readonly IDataService<VoltageRelatedEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public VoltageRelatedEventSinglePhaseService()
        {
            _dataService = new GenericDataService<VoltageRelatedEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }
        public async Task<bool> Add(List<VoltageRelatedEventSinglePhase> voltageRelatedEventSinglePhaseModel)
        {
            try
            {
                await Delete(voltageRelatedEventSinglePhaseModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(voltageRelatedEventSinglePhaseModel);
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
                    string query = "select * from VoltageRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<VoltageRelatedEventSinglePhase>().RemoveRange(res);
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
        public async Task<List<VoltageRelatedEventSinglePhaseDTO>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from VoltageRelatedEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<VoltageRelatedEventSinglePhaseDTO> voltageRelatedEventSinglePhase = await ParseDataToDTO(response);

                return voltageRelatedEventSinglePhase;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<VoltageRelatedEventSinglePhaseDTO>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from VoltageRelatedEventSinglePhase where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.DateandTimeofEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.DateandTimeofEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                List<VoltageRelatedEventSinglePhaseDTO> voltageRelatedEventDto = await ParseDataToDTO(response);

                return voltageRelatedEventDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<VoltageRelatedEventSinglePhase>> ParseDataToClass(List<VoltageRelatedEventSinglePhaseDTO> voltageRelatedEventDtoList)
        //{
        //    List<VoltageRelatedEventSinglePhase> voltageRelatedEventSinglePhaseList = new List<VoltageRelatedEventSinglePhase>();

        //    foreach (var voltageRelatedEventSinglePhaseDto in voltageRelatedEventDtoList)
        //    {
        //        VoltageRelatedEventSinglePhase voltageRelatedEvent = new VoltageRelatedEventSinglePhase();

        //        voltageRelatedEvent.MeterNo = voltageRelatedEventSinglePhaseDto.MeterNo;
        //        voltageRelatedEvent.CreatedOn = voltageRelatedEventSinglePhaseDto.CreatedOn;
        //        voltageRelatedEvent.DateandTimeofEvent = voltageRelatedEventSinglePhaseDto.DateandTimeofEvent;
        //        voltageRelatedEvent.EventCode = voltageRelatedEventSinglePhaseDto.Event;
        //        voltageRelatedEvent.Current = voltageRelatedEventSinglePhaseDto.Current;
        //        voltageRelatedEvent.Voltage = voltageRelatedEventSinglePhaseDto.Voltage;
        //        voltageRelatedEvent.PowerFactor = voltageRelatedEventSinglePhaseDto.PowerFactor;
        //        voltageRelatedEvent.CumulativeEnergykWhImport = voltageRelatedEventSinglePhaseDto.CumulativeEnergykWhImport;
        //        voltageRelatedEvent.CumulativeEnergykWhExport = voltageRelatedEventSinglePhaseDto.CumulativeEnergykWhExport;
        //        voltageRelatedEvent.CumulativeTamperCount = voltageRelatedEventSinglePhaseDto.CumulativeTamperCount;

        //        voltageRelatedEventSinglePhaseList.Add(voltageRelatedEvent);
        //    }

        //    return voltageRelatedEventSinglePhaseList;
        //}

        private async Task<List<VoltageRelatedEventSinglePhaseDTO>> ParseDataToDTO(List<VoltageRelatedEventSinglePhase> voltageRelatedEventList)
        {
            int index = 1;
            List<VoltageRelatedEventSinglePhaseDTO> voltageRelatedEventDtoList = new List<VoltageRelatedEventSinglePhaseDTO>();
            foreach (var voltageRelatedEvent in voltageRelatedEventList)
            {
                VoltageRelatedEventSinglePhaseDTO voltageRelatedEventDto = new VoltageRelatedEventSinglePhaseDTO();

                voltageRelatedEventDto.Number = index;
                voltageRelatedEventDto.MeterNo = voltageRelatedEvent.MeterNo;
                voltageRelatedEventDto.DateandTimeofEvent = voltageRelatedEvent.DateandTimeofEvent;
                voltageRelatedEventDto.Event = voltageRelatedEvent.EventCode;
                voltageRelatedEventDto.Current = voltageRelatedEvent.Current;
                voltageRelatedEventDto.Voltage = voltageRelatedEvent.Voltage;
                voltageRelatedEventDto.PowerFactor = voltageRelatedEvent.PowerFactor;
                voltageRelatedEventDto.CumulativeEnergykWhImport = StringExtensions.CheckNullable(voltageRelatedEvent.CumulativeEnergykWhImport);
                voltageRelatedEventDto.CumulativeEnergykWhExport = StringExtensions.CheckNullable(voltageRelatedEvent.CumulativeEnergykWhExport);
                voltageRelatedEventDto.CumulativeTamperCount = voltageRelatedEvent.CumulativeTamperCount;
                voltageRelatedEventDto.CreatedOn = voltageRelatedEvent.CreatedOn;

                voltageRelatedEventDtoList.Add(voltageRelatedEventDto);
                index++;
            }

            return voltageRelatedEventDtoList;
        }
    }
}
