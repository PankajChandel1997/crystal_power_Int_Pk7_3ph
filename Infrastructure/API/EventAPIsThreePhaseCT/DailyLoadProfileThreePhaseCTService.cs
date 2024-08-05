using Domain.Entities.ThreePhaseCTEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIThreePhaseCT
{
    public class DailyLoadProfileThreePhaseCTService
    {
        private readonly IDataService<DailyLoadProfileThreePhaseCT> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public DailyLoadProfileThreePhaseCTService()
        {
            _dataService = new GenericDataService<DailyLoadProfileThreePhaseCT>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }

        public async Task<bool> Add(List<DailyLoadProfileThreePhaseCT> dailyLoadProfile)
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
                    string query = "select * from DailyLoadProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    List<string> fatchedDates = res.DistinctBy(x => x.RealTimeClock).OrderByDescending(c => c.Id).Select(d => d.RealTimeClock).ToList();
                    foreach (var fatchedDate in fatchedDates)
                    {
                        var duplicateData = res.Where(x => x.RealTimeClock == fatchedDate).ToList().Skip(1);
                        if (duplicateData.Any())
                        {
                            db.Set<DailyLoadProfileThreePhaseCT>().RemoveRange(duplicateData);
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

        public async Task<List<DailyLoadProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DailyLoadProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<DailyLoadProfileThreePhaseCTDto> dailyLoadProfileThreePhaseCT = await ParseDataToDTO(response);

                return dailyLoadProfileThreePhaseCT;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DailyLoadProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from DailyLoadProfileThreePhaseCT where MeterNo = '" + meterNumber + "'";

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

                List<DailyLoadProfileThreePhaseCTDto> dailyLoadProfileThreePhaseCT = await ParseDataToDTO(response);

                return dailyLoadProfileThreePhaseCT;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<DailyLoadProfileThreePhaseCT>> ParseDataToClass(List<DailyLoadProfileThreePhaseCTDto> dailyLoadProfileThreePhasesCT)
        {
            List<DailyLoadProfileThreePhaseCT> dailyLoadProfileThreePhaseCTList = new List<DailyLoadProfileThreePhaseCT>();
            foreach (var dailyLoadProfileThreePhaseCTDto in dailyLoadProfileThreePhasesCT)
            {
                DailyLoadProfileThreePhaseCT dailyLoadProfileThreePhaseCT = new DailyLoadProfileThreePhaseCT();

                dailyLoadProfileThreePhaseCT.MeterNo = dailyLoadProfileThreePhaseCTDto.MeterNo;
                dailyLoadProfileThreePhaseCT.CreatedOn = dailyLoadProfileThreePhaseCTDto.CreatedOn;
                dailyLoadProfileThreePhaseCT.RealTimeClock = dailyLoadProfileThreePhaseCTDto.RealTimeClock;
                dailyLoadProfileThreePhaseCT.CumulativeEnergykWhImport = dailyLoadProfileThreePhaseCTDto.CumulativeEnergykWhImport;
                dailyLoadProfileThreePhaseCT.CumulativeEnergykVAhImport = dailyLoadProfileThreePhaseCTDto.CumulativeEnergykVAhImport;
                dailyLoadProfileThreePhaseCT.CumulativeEnergykWhExport = dailyLoadProfileThreePhaseCTDto.CumulativeEnergykWhExport;
                dailyLoadProfileThreePhaseCT.CumulativeEnergykVAhExport = dailyLoadProfileThreePhaseCTDto.CumulativeEnergykVAhExport;


                dailyLoadProfileThreePhaseCTList.Add(dailyLoadProfileThreePhaseCT);
            }


            return dailyLoadProfileThreePhaseCTList;
        }

        private async Task<List<DailyLoadProfileThreePhaseCTDto>> ParseDataToDTO(List<DailyLoadProfileThreePhaseCT> DailyLoadProfileThreePhaseCTList)
        {
            int index = 1;
            List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCTDtoList = new List<DailyLoadProfileThreePhaseCTDto>();
            foreach (var DailyLoadProfileThreePhaseCT in DailyLoadProfileThreePhaseCTList)
            {
                DailyLoadProfileThreePhaseCTDto DailyLoadProfileThreePhaseCTDto = new DailyLoadProfileThreePhaseCTDto();

                DailyLoadProfileThreePhaseCTDto.Number = index;
                DailyLoadProfileThreePhaseCTDto.MeterNo = DailyLoadProfileThreePhaseCT.MeterNo;
                DailyLoadProfileThreePhaseCTDto.CreatedOn = DailyLoadProfileThreePhaseCT.CreatedOn;
                DailyLoadProfileThreePhaseCTDto.RealTimeClock = DailyLoadProfileThreePhaseCT.RealTimeClock;
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykWhImport = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykVAhImport = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykWhExport = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykVAhExport = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykVArhQ1 = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykVArhQ2 = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykVArhQ3 = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                DailyLoadProfileThreePhaseCTDto.CumulativeEnergykVArhQ4 = (double.Parse(DailyLoadProfileThreePhaseCT.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                DailyLoadProfileThreePhaseCTDtoList.Add(DailyLoadProfileThreePhaseCTDto);
                index++;
            }

            return DailyLoadProfileThreePhaseCTDtoList;
        }
    }
}
