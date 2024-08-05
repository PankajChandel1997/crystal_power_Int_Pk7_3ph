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
    public class OtherEventSinglePhaseService
    {
        private readonly IDataService<OtherEventSinglePhase> _dataService;
        private readonly ApplicationContextFactory _contextFactory;
        public ErrorHelper _errorHelper;
        public OtherEventSinglePhaseService()
        {
            _dataService = new GenericDataService<OtherEventSinglePhase>(new ApplicationContextFactory());
            _contextFactory = new ApplicationContextFactory();
            _errorHelper = new ErrorHelper();
        }
        public async Task<bool> Add(List<OtherEventSinglePhase> OtherEventSinglePhaseModel)
        {
            try
            {
                await Delete(OtherEventSinglePhaseModel.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(OtherEventSinglePhaseModel);
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
                    string query = "select * from OtherEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<OtherEventSinglePhase>().RemoveRange(res);
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
        public async Task<List<OtherEventSinglePhaseDTO>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from OtherEventSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<OtherEventSinglePhaseDTO> OtherEventSinglePhase = await ParseDataToDTO(response);

                return OtherEventSinglePhase;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OtherEventSinglePhaseDTO>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from OtherEventSinglePhase where MeterNo = '" + meterNumber + "'";

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

                List<OtherEventSinglePhaseDTO> OtherEventDto = await ParseDataToDTO(response);

                return OtherEventDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<OtherEventSinglePhase>> ParseDataToClass(List<OtherEventSinglePhaseDTO> OtherEventDtoList)
        //{
        //    List<OtherEventSinglePhase> OtherEventSinglePhaseList = new List<OtherEventSinglePhase>();

        //    foreach (var OtherEventSinglePhaseDto in OtherEventDtoList)
        //    {
        //        OtherEventSinglePhase OtherEvent = new OtherEventSinglePhase();

        //        OtherEvent.MeterNo = OtherEventSinglePhaseDto.MeterNo;
        //        OtherEvent.CreatedOn = OtherEventSinglePhaseDto.CreatedOn;
        //        OtherEvent.DateandTimeofEvent = OtherEventSinglePhaseDto.DateandTimeofEvent;
        //        OtherEvent.EventCode = OtherEventSinglePhaseDto.Event;
        //        OtherEvent.Current = OtherEventSinglePhaseDto.Current;
        //        OtherEvent.Voltage = OtherEventSinglePhaseDto.Voltage;
        //        OtherEvent.PowerFactor = OtherEventSinglePhaseDto.PowerFactor;
        //        OtherEvent.CumulativeEnergykWhImport = OtherEventSinglePhaseDto.CumulativeEnergykWhImport;
        //        OtherEvent.CumulativeEnergykWhExport = OtherEventSinglePhaseDto.CumulativeEnergykWhExport;
        //        OtherEvent.CumulativeTamperCount = OtherEventSinglePhaseDto.CumulativeTamperCount;

        //        OtherEventSinglePhaseList.Add(OtherEvent);
        //    }

        //    return OtherEventSinglePhaseList;
        //}

        private async Task<List<OtherEventSinglePhaseDTO>> ParseDataToDTO(List<OtherEventSinglePhase> OtherEventList)
        {
            int index = 1;
            List<OtherEventSinglePhaseDTO> OtherEventDtoList = new List<OtherEventSinglePhaseDTO>();
            foreach (var OtherEvent in OtherEventList)
            {
                OtherEventSinglePhaseDTO OtherEventDto = new OtherEventSinglePhaseDTO();

                OtherEventDto.Number = index;
                OtherEventDto.MeterNo = OtherEvent.MeterNo;
                OtherEventDto.DateandTimeofEvent = OtherEvent.DateandTimeofEvent;
                OtherEventDto.Event = OtherEvent.EventCode;
                OtherEventDto.Current = OtherEvent.Current;
                OtherEventDto.Voltage = OtherEvent.Voltage;
                OtherEventDto.PowerFactor = OtherEvent.PowerFactor;
                OtherEventDto.CumulativeEnergykWhImport = StringExtensions.CheckNullable(OtherEvent.CumulativeEnergykWhImport);
                OtherEventDto.CumulativeEnergykWhExport = StringExtensions.CheckNullable(OtherEvent.CumulativeEnergykWhExport);
                OtherEventDto.CumulativeTamperCount = OtherEvent.CumulativeTamperCount;
                OtherEventDto.CreatedOn = OtherEvent.CreatedOn;

                OtherEventDtoList.Add(OtherEventDto);
                index++;
            }

            return OtherEventDtoList;
        }
    }
}
