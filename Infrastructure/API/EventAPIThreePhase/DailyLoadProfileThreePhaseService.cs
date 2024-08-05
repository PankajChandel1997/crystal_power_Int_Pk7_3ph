using Domain.Entities.ThreePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIThreePhase
{
    public class DailyLoadProfileThreePhaseService
    {
        private readonly IDataService<DailyLoadProfileThreePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public DailyLoadProfileThreePhaseService()
        {
            _dataService = new GenericDataService<DailyLoadProfileThreePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }

        public async Task<bool> Add(List<DailyLoadProfileThreePhase> dailyLoadProfile)
        {
            try
            {
                return await _dataService.CreateRange(dailyLoadProfile);
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
                    string query = "select * from DailyLoadProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    List<string> fatchedDates = res.DistinctBy(x => x.RealTimeClock).OrderByDescending(c => c.Id).Select(d => d.RealTimeClock).ToList();
                    foreach (var fatchedDate in fatchedDates)
                    {
                        var duplicateData = res.Where(x => x.RealTimeClock == fatchedDate).ToList().Skip(1);
                        if (duplicateData.Any())
                        {
                            db.Set<DailyLoadProfileThreePhase>().RemoveRange(duplicateData);
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

        public async Task<List<DailyLoadProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DailyLoadProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var dailyLoadProfileThreePhase = new List<DailyLoadProfileThreePhaseDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    dailyLoadProfileThreePhase = await ParseDataToDTO(response);
                }
                else
                {
                    dailyLoadProfileThreePhase = await ParseDataToDTOUMD(response);
                }

                return dailyLoadProfileThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DailyLoadProfileThreePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DailyLoadProfileThreePhase where MeterNo = '" + meterNumber + "'";

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

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var dailyLoadProfileThreePhase = new List<DailyLoadProfileThreePhaseDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    dailyLoadProfileThreePhase = await ParseDataToDTO(response);
                }
                else
                {
                    dailyLoadProfileThreePhase = await ParseDataToDTOUMD(response);
                }
                return dailyLoadProfileThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<DailyLoadProfileThreePhase>> ParseDataToClass(List<DailyLoadProfileThreePhaseDto> dailyLoadProfileThreePhases)
        {
            List<DailyLoadProfileThreePhase> dailyLoadProfileThreePhaseList = new List<DailyLoadProfileThreePhase>();
            foreach (var dailyLoadProfileThreePhaseDto in dailyLoadProfileThreePhases)
            {
                DailyLoadProfileThreePhase dailyLoadProfileThreePhase = new DailyLoadProfileThreePhase();

                dailyLoadProfileThreePhase.MeterNo = dailyLoadProfileThreePhaseDto.MeterNo;
                dailyLoadProfileThreePhase.CreatedOn = dailyLoadProfileThreePhaseDto.CreatedOn;
                dailyLoadProfileThreePhase.RealTimeClock = dailyLoadProfileThreePhaseDto.RealTimeClock;
                dailyLoadProfileThreePhase.CumulativeEnergykWhImport = dailyLoadProfileThreePhaseDto.CumulativeEnergykWhImport;
                dailyLoadProfileThreePhase.CumulativeEnergykVAhImport = dailyLoadProfileThreePhaseDto.CumulativeEnergykVAhImport;
                dailyLoadProfileThreePhase.CumulativeEnergykWhExport = dailyLoadProfileThreePhaseDto.CumulativeEnergykWhExport;
                dailyLoadProfileThreePhase.CumulativeEnergykVAhExport = dailyLoadProfileThreePhaseDto.CumulativeEnergykVAhExport;


                dailyLoadProfileThreePhaseList.Add(dailyLoadProfileThreePhase);
            }


            return dailyLoadProfileThreePhaseList;
        }

        private async Task<List<DailyLoadProfileThreePhaseDto>> ParseDataToDTO(List<DailyLoadProfileThreePhase> DailyLoadProfileThreePhaseList)
        {
            int index = 1;
            List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhaseDtoList = new List<DailyLoadProfileThreePhaseDto>();
            foreach (var DailyLoadProfileThreePhase in DailyLoadProfileThreePhaseList)
            {
                DailyLoadProfileThreePhaseDto DailyLoadProfileThreePhaseDto = new DailyLoadProfileThreePhaseDto();

                DailyLoadProfileThreePhaseDto.Number = index;
                DailyLoadProfileThreePhaseDto.MeterNo = DailyLoadProfileThreePhase.MeterNo;
                DailyLoadProfileThreePhaseDto.CreatedOn = DailyLoadProfileThreePhase.CreatedOn;
                DailyLoadProfileThreePhaseDto.RealTimeClock = DailyLoadProfileThreePhase.RealTimeClock;
                DailyLoadProfileThreePhaseDto.CumulativeEnergykWhImport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseDto.CumulativeEnergykVAhImport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseDto.CumulativeEnergykWhExport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseDto.CumulativeEnergykVAhExport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                DailyLoadProfileThreePhaseDtoList.Add(DailyLoadProfileThreePhaseDto);
                index++;
            }

            return DailyLoadProfileThreePhaseDtoList;
        }

        private async Task<List<DailyLoadProfileThreePhaseDto>> ParseDataToDTOUMD(List<DailyLoadProfileThreePhase> DailyLoadProfileThreePhaseList)
        {
            int index = 1;
            List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhaseDtoList = new List<DailyLoadProfileThreePhaseDto>();
            foreach (var DailyLoadProfileThreePhase in DailyLoadProfileThreePhaseList)
            {
                DailyLoadProfileThreePhaseDto DailyLoadProfileThreePhaseDto = new DailyLoadProfileThreePhaseDto();

                DailyLoadProfileThreePhaseDto.Number = index;
                DailyLoadProfileThreePhaseDto.MeterNo = DailyLoadProfileThreePhase.MeterNo;
                DailyLoadProfileThreePhaseDto.CreatedOn = DailyLoadProfileThreePhase.CreatedOn;
                DailyLoadProfileThreePhaseDto.RealTimeClock = DailyLoadProfileThreePhase.RealTimeClock;
                DailyLoadProfileThreePhaseDto.CumulativeEnergykWhImport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseDto.CumulativeEnergykVAhImport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseDto.CumulativeEnergykWhExport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseDto.CumulativeEnergykVAhExport = (double.Parse(DailyLoadProfileThreePhase.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                DailyLoadProfileThreePhaseDtoList.Add(DailyLoadProfileThreePhaseDto);
                index++;
            }

            return DailyLoadProfileThreePhaseDtoList;
        }
    }
}
