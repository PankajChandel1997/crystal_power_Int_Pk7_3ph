using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class DailyLoadProfileSinglePhaseService
    {
        private readonly IDataService<DailyLoadProfileSinglePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public DailyLoadProfileSinglePhaseService()
        {
            _dataService = new GenericDataService<DailyLoadProfileSinglePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }

        public async Task<bool> Add(List<DailyLoadProfileSinglePhase> dailyLoadProfile)
        {
            try
            {
                return await _dataService.CreateRange(dailyLoadProfile);
                //return await Delete(dailyLoadProfile.FirstOrDefault().MeterNo);
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
                    string query = "select * from DailyLoadProfileSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    List<string> fatchedDates = res.DistinctBy(x => x.RealTimeClock).Select(d => d.RealTimeClock).ToList();
                    foreach (var fatchedDate in fatchedDates)
                    {
                        var duplicateData = res.Where(x => x.RealTimeClock == fatchedDate).ToList().Skip(1);
                        if (duplicateData.Any())
                        {
                            db.Set<DailyLoadProfileSinglePhase>().RemoveRange(duplicateData);
                        }
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

        public async Task<List<DailyLoadProfileSinglePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DailyLoadProfileSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<DailyLoadProfileSinglePhaseDto> dailyLoadProfileSinglePhase = await ParseDataToDTO(response);

                return dailyLoadProfileSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<DailyLoadProfileSinglePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DailyLoadProfileSinglePhase where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else if (!string.IsNullOrEmpty(fatchDate))
                {
                    response = response.Where(x =>
                      x.CreatedOn == fatchDate).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                List<DailyLoadProfileSinglePhaseDto> dailyLoadProfileSinglePhase = await ParseDataToDTO(response);

                return dailyLoadProfileSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<DailyLoadProfileSinglePhase>> ParseDataToClass(List<DailyLoadProfileSinglePhaseDto> dailyLoadProfileSinglePhases)
        //{
        //    List<DailyLoadProfileSinglePhase> dailyLoadProfileSinglePhaseList = new List<DailyLoadProfileSinglePhase>();
        //    foreach (var dailyLoadProfileSinglePhaseDto in dailyLoadProfileSinglePhases)
        //    {
        //        DailyLoadProfileSinglePhase dailyLoadProfileSinglePhase = new DailyLoadProfileSinglePhase();

        //        dailyLoadProfileSinglePhase.MeterNo = dailyLoadProfileSinglePhaseDto.MeterNo;
        //        dailyLoadProfileSinglePhase.CreatedOn = dailyLoadProfileSinglePhaseDto.CreatedOn;
        //        dailyLoadProfileSinglePhase.RealTimeClock = dailyLoadProfileSinglePhaseDto.RealTimeClock;
        //        dailyLoadProfileSinglePhase.CumulativeEnergykWhImport = dailyLoadProfileSinglePhaseDto.CumulativeEnergykWhImport;
        //        dailyLoadProfileSinglePhase.CumulativeEnergyKVAhImport = dailyLoadProfileSinglePhaseDto.CumulativeEnergyKVAhImport;
        //        dailyLoadProfileSinglePhase.CumulativeEnergykWhExport = dailyLoadProfileSinglePhaseDto.CumulativeEnergykWhExport;
        //        dailyLoadProfileSinglePhase.CumulativeEnergykVAhExport = dailyLoadProfileSinglePhaseDto.CumulativeEnergykVAhExport;

        //        dailyLoadProfileSinglePhaseList.Add(dailyLoadProfileSinglePhase);
        //    }


        //    return dailyLoadProfileSinglePhaseList;
        //}

        private async Task<List<DailyLoadProfileSinglePhaseDto>> ParseDataToDTO(List<DailyLoadProfileSinglePhase> DailyLoadProfileSinglePhaseList)
        {
            int index = 1;
            List<DailyLoadProfileSinglePhaseDto> DailyLoadProfileSinglePhaseDtoList = new List<DailyLoadProfileSinglePhaseDto>();
            foreach (var DailyLoadProfileSinglePhase in DailyLoadProfileSinglePhaseList)
            {
                DailyLoadProfileSinglePhaseDto DailyLoadProfileSinglePhaseDto = new DailyLoadProfileSinglePhaseDto();

                DailyLoadProfileSinglePhaseDto.Number = index;
                DailyLoadProfileSinglePhaseDto.MeterNo = DailyLoadProfileSinglePhase.MeterNo;
                DailyLoadProfileSinglePhaseDto.CreatedOn = DailyLoadProfileSinglePhase.CreatedOn;
                DailyLoadProfileSinglePhaseDto.RealTimeClock = DailyLoadProfileSinglePhase.RealTimeClock;
                DailyLoadProfileSinglePhaseDto.CumulativeEnergykWhImport = StringExtensions.CheckNullable(DailyLoadProfileSinglePhase.CumulativeEnergykWhImport);
                DailyLoadProfileSinglePhaseDto.CumulativeEnergyKVAhImport = StringExtensions.CheckNullable(DailyLoadProfileSinglePhase.CumulativeEnergyKVAhImport);
                DailyLoadProfileSinglePhaseDto.CumulativeEnergykWhExport = StringExtensions.CheckNullable(DailyLoadProfileSinglePhase.CumulativeEnergykWhExport);
                DailyLoadProfileSinglePhaseDto.CumulativeEnergykVAhExport = StringExtensions.CheckNullable(DailyLoadProfileSinglePhase.CumulativeEnergykVAhExport);

                DailyLoadProfileSinglePhaseDtoList.Add(DailyLoadProfileSinglePhaseDto);
                index++;
            }

            return DailyLoadProfileSinglePhaseDtoList;
        }
    }

}
