using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.DTOs;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API
{
    public class SelfDiagnosticService
    {
        private readonly IDataService<SelfDiagnostic> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;
        public SelfDiagnosticService()
        {
            _dataService = new GenericDataService<SelfDiagnostic>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<SelfDiagnostic> selfDiagnosticDto)
        {
            try
            {
                await Delete(selfDiagnosticDto.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(selfDiagnosticDto);              
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "Self Diagnostic Service : inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(string meterNumber)
        {
            try
            {
                using (ApplicationDBContext db = _contextFactory.CreateDbContext())
                {
                    string query = "select * from SelfDiagnostic where MeterNo = '" + meterNumber + "'";

                    var res = await _dataService.Filter(query);

                        if (res.Any())
                        {
                            db.Set<SelfDiagnostic>().RemoveRange(res);
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
        public async Task<List<SelfDiagnosticDto>> GetAll(int PageSize, string meterNumber)
        {
            try
            {
                string query = "select * from SelfDiagnostic where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + PageSize;

                var response = await _dataService.Filter(query);

                List<SelfDiagnosticDto> selfDiagnostic = await ParseDataToDTO(response);

                return selfDiagnostic;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "SelfDiagnostic Service : inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<SelfDiagnosticDto>> Filter(string startDate, string endDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from SelfDiagnostic where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.CreatedOn, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                List<SelfDiagnosticDto> selfDiagnostic = await ParseDataToDTO(response);

                return selfDiagnostic;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "Self Diagnostic Service : inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<SelfDiagnosticDto> GetByMeterNo(string MeterNo)
        {
            try
            {
                string query = "select * from SelfDiagnostic where MeterNo = '" + MeterNo + "'";

                var response = await _dataService.Filter(query);

                List<SelfDiagnosticDto> selfDiagnosticDto = await ParseDataToDTO(response);

                return selfDiagnosticDto[0];
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "Self Diagnostic Service : inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<SelfDiagnostic>> ParseDataToClass(List<SelfDiagnosticDto> selfDiagnosticDtos)
        {
            List<SelfDiagnostic> selfDiagnosticList = new List<SelfDiagnostic>();

            foreach (var selfDiagnosticDto in selfDiagnosticDtos)
            {
                SelfDiagnostic selfDiagnostic = new SelfDiagnostic();

                selfDiagnostic.MeterNo = selfDiagnosticDto.MeterNo;
                selfDiagnostic.CreatedOn = selfDiagnosticDto.CreatedOn;
                selfDiagnostic.Status = selfDiagnosticDto.Status;

                selfDiagnosticList.Add(selfDiagnostic);
            }
            return selfDiagnosticList;
        }

        private async Task<List<SelfDiagnosticDto>> ParseDataToDTO(List<SelfDiagnostic> selfDiagnostics)
        {
            int index = 1;
            List<SelfDiagnosticDto> selfDiagnosticDtoList = new List<SelfDiagnosticDto>();
            foreach (var selfDiagnostic in selfDiagnostics)
            {
                SelfDiagnosticDto selfDiagnosticDto = new SelfDiagnosticDto();

                selfDiagnosticDto.Number = index;
                selfDiagnosticDto.MeterNo = selfDiagnostic.MeterNo;
                selfDiagnosticDto.CreatedOn = selfDiagnostic.CreatedOn;
                selfDiagnosticDto.Status = Enum.GetName(typeof(DiagnosticStatus), Convert.ToInt32(selfDiagnostic.Status)).ToString(); //selfDiagnostic.Status;
                selfDiagnosticDto.RTCBattery = Convert.ToInt32(selfDiagnostic.Status) == 1 ? "false" : Convert.ToInt32(selfDiagnostic.Status) == 17 ? "false" : "true";
                selfDiagnosticDto.MainBattery = Convert.ToInt32(selfDiagnostic.Status) == 16 ? "false" : Convert.ToInt32(selfDiagnostic.Status) == 17 ? "false" : "true";

                selfDiagnosticDtoList.Add(selfDiagnosticDto);
                index++;
            }

            return selfDiagnosticDtoList;
        }
    }
}