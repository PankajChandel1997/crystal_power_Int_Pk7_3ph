using Domain.Entities.ThreePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIThreePhase
{
    public class BillingProfileThreePhaseService
    {
        private readonly IDataService<BillingProfileThreePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public BillingProfileThreePhaseService()
        {
            _dataService = new GenericDataService<BillingProfileThreePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<BillingProfileThreePhase> billingProfileProfile)
        {
            try
            {
                return await _dataService.CreateRange(billingProfileProfile);
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
                    string query = "select * from BillingProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<BillingProfileThreePhase>().RemoveRange(res);
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

        public async Task<List<BillingProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BillingProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);
                var billingProfileProfileThreePhaseCT = new List<BillingProfileThreePhaseDto>();
                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    billingProfileProfileThreePhaseCT = await ParseDataToDTO(response);
                }
                else
                {
                    billingProfileProfileThreePhaseCT = await ParseDataToDTOUMD(response);
                }


                return billingProfileProfileThreePhaseCT;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BillingProfileThreePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BillingProfileThreePhase where MeterNo = '" + meterNumber + "'";

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
                var billingProfileProfileThreePhaseCT = new List<BillingProfileThreePhaseDto>();
                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    billingProfileProfileThreePhaseCT = await ParseDataToDTO(response);
                }
                else
                {
                    billingProfileProfileThreePhaseCT = await ParseDataToDTOUMD(response);
                }

                if (billingProfileProfileThreePhaseCT.Count > 1)
                {
                    int currentMonth = DateTime.Now.Month;
                    int currentYear = DateTime.Now.Year;

                    billingProfileProfileThreePhaseCT.RemoveAt(billingProfileProfileThreePhaseCT.Count - 1);
                }

                return billingProfileProfileThreePhaseCT;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<BillingProfileThreePhase>> ParseDataToClass(List<BillingProfileThreePhaseDto> billingProfileProfileThreePhaseCTDtoList)
        {
            List<BillingProfileThreePhase> billingProfileProfileThreePhaseCTList = new List<BillingProfileThreePhase>();

            foreach (var billingProfileProfileThreePhaseCTDto in billingProfileProfileThreePhaseCTDtoList)
            {
                BillingProfileThreePhase billingProfileProfileThreePhaseCT = new BillingProfileThreePhase();

                billingProfileProfileThreePhaseCT.MeterNo = billingProfileProfileThreePhaseCTDto.MeterNo;
                billingProfileProfileThreePhaseCT.CreatedOn = billingProfileProfileThreePhaseCTDto.CreatedOn;
                billingProfileProfileThreePhaseCT.RealTimeClock = billingProfileProfileThreePhaseCTDto.RealTimeClock;
                billingProfileProfileThreePhaseCT.SystemPowerFactorImport = billingProfileProfileThreePhaseCTDto.SystemPowerFactorImport;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWh = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWh;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ1 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ1;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ2 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ2;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ3 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ3;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ4 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ4;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ5 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ5;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ6 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ6;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ7 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ7;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ8 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ8;
                billingProfileProfileThreePhaseCT.CumulativeEnergykWhExport = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhExport;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhExport = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhExport;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhImport = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhImport;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ1 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ1;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ2 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ2;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ3 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ3;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ4 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ4;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ5 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ5;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ6 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ6;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ7 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ7;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ8 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ8;
                billingProfileProfileThreePhaseCT.MaximumDemandkW = billingProfileProfileThreePhaseCTDto.MaximumDemandkW;
                billingProfileProfileThreePhaseCT.MaximumDemandkWDateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWDateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ1 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ1;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ1DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ1DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ2 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ2;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ2DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ2DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ3 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ3;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ3DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ3DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ4 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ4;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ4DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ4DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ5 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ5;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ5DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ5DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ6 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ6;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ6DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ6DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ7 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ7;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ7DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ7DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ8 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ8;
                billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ8DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ8DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVA = billingProfileProfileThreePhaseCTDto.MaximumDemandkVA;
                billingProfileProfileThreePhaseCT.MaximumDemandkVADateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVADateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ1 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ1;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ1DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ1DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ2 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ2;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ2DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ2DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ3 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ3;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ3DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ3DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ4 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ4;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ4DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ4DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ5 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ5;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ5DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ5DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ6 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ6;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ6DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ6DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ7 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ7;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ7DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ7DateTime;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ8 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ8;
                billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ8DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ8DateTime;
                billingProfileProfileThreePhaseCT.BillingPowerONdurationInMinutesDBP = billingProfileProfileThreePhaseCTDto.BillingPowerONdurationInMinutesDBP;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ1 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ1;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ2 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ2;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ3 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ3;
                billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ4 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ4;
                billingProfileProfileThreePhaseCT.TamperCount = billingProfileProfileThreePhaseCTDto.TamperCount;


                billingProfileProfileThreePhaseCTList.Add(billingProfileProfileThreePhaseCT);
            }

            return billingProfileProfileThreePhaseCTList;
        }

        private async Task<List<BillingProfileThreePhaseDto>> ParseDataToDTO(List<BillingProfileThreePhase> BillingProfileThreePhaseList)
        {
            int index = 1;
            List<BillingProfileThreePhaseDto> BillingProfileThreePhaseDtoList = new List<BillingProfileThreePhaseDto>();
            foreach (var BillingProfileThreePhase in BillingProfileThreePhaseList)
            {
                BillingProfileThreePhaseDto BillingProfileThreePhaseDto = new BillingProfileThreePhaseDto();

                BillingProfileThreePhaseDto.Number = index;
                BillingProfileThreePhaseDto.MeterNo = BillingProfileThreePhase.MeterNo;
                BillingProfileThreePhaseDto.CreatedOn = BillingProfileThreePhase.CreatedOn;
                BillingProfileThreePhaseDto.RealTimeClock = BillingProfileThreePhase.RealTimeClock;
                BillingProfileThreePhaseDto.SystemPowerFactorImport = Math.Round(double.Parse(BillingProfileThreePhase.SystemPowerFactorImport, NumberStyles.Any), 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWh = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWh, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhExport = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhExport, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhExport = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhImport = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkW = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkW, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWDateTime = BillingProfileThreePhase.MaximumDemandkWDateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ1DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ1DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ2DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ2DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ3DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ3DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ4DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ4DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ5DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ5DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ6DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ6DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ7DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ7DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ8DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ8DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVA = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVA, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVADateTime = BillingProfileThreePhase.MaximumDemandkVADateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ1DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ1DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ2DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ2DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ3DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ3DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ4DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ4DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ5DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ5DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ6DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ6DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ7DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ7DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ8DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ8DateTime;
                BillingProfileThreePhaseDto.BillingPowerONdurationInMinutesDBP = BillingProfileThreePhase.BillingPowerONdurationInMinutesDBP;
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ1 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ1) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ2 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ2) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ3 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ3) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ4 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ4) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.TamperCount = !string.IsNullOrEmpty(BillingProfileThreePhase.TamperCount) ? BillingProfileThreePhase.TamperCount : "";

                BillingProfileThreePhaseDto.PowerFactorExport = !string.IsNullOrEmpty(BillingProfileThreePhase.PowerFactorExport) ? BillingProfileThreePhase.PowerFactorExport : "";

                BillingProfileThreePhaseDto.MaximumDemandkWExport = StringExtensions.CheckNullable(BillingProfileThreePhase.MaximumDemandkWExport);
                BillingProfileThreePhaseDto.MaximumDemandkWExportDateTime = BillingProfileThreePhase.MaximumDemandkWExportDateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAExport = StringExtensions.CheckNullable(BillingProfileThreePhase.MaximumDemandkVAExport);
                BillingProfileThreePhaseDto.MaximumDemandkVAExportDateTime = BillingProfileThreePhase.MaximumDemandkVAExportDateTime;

                BillingProfileThreePhaseDtoList.Add(BillingProfileThreePhaseDto);

                index++;
            }

            return BillingProfileThreePhaseDtoList;
        }

        private async Task<List<BillingProfileThreePhaseDto>> ParseDataToDTOUMD(List<BillingProfileThreePhase> BillingProfileThreePhaseList)
        {
            int index = 1;
            List<BillingProfileThreePhaseDto> BillingProfileThreePhaseDtoList = new List<BillingProfileThreePhaseDto>();
            foreach (var BillingProfileThreePhase in BillingProfileThreePhaseList)
            {
                BillingProfileThreePhaseDto BillingProfileThreePhaseDto = new BillingProfileThreePhaseDto();

                BillingProfileThreePhaseDto.Number = index;
                BillingProfileThreePhaseDto.MeterNo = BillingProfileThreePhase.MeterNo;
                BillingProfileThreePhaseDto.CreatedOn = BillingProfileThreePhase.CreatedOn;
                BillingProfileThreePhaseDto.RealTimeClock = BillingProfileThreePhase.RealTimeClock;
                BillingProfileThreePhaseDto.SystemPowerFactorImport = Math.Round(double.Parse(BillingProfileThreePhase.SystemPowerFactorImport, NumberStyles.Any), 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWh = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWh, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykWhExport = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykWhExport, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhExport = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhImport = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.CumulativeEnergykVAhTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVAhTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkW = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkW, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWDateTime = BillingProfileThreePhase.MaximumDemandkWDateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ1DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ1DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ2DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ2DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ3DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ3DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ4DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ4DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ5DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ5DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ6DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ6DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ7DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ7DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkWForTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkWForTZ8DateTime = BillingProfileThreePhase.MaximumDemandkWForTZ8DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVA = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVA, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVADateTime = BillingProfileThreePhase.MaximumDemandkVADateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ1 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ1, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ1DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ1DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ2 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ2, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ2DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ2DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ3 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ3, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ3DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ3DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ4 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ4, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ4DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ4DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ5 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ5, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ5DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ5DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ6 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ6, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ6DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ6DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ7 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ7, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ7DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ7DateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ8 = Math.Round(double.Parse(BillingProfileThreePhase.MaximumDemandkVAForTZ8, NumberStyles.Any) / 1000, 2).ToString();
                BillingProfileThreePhaseDto.MaximumDemandkVAForTZ8DateTime = BillingProfileThreePhase.MaximumDemandkVAForTZ8DateTime;
                BillingProfileThreePhaseDto.BillingPowerONdurationInMinutesDBP = BillingProfileThreePhase.BillingPowerONdurationInMinutesDBP;
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ1 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ1) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ2 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ2) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ3 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ3) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.CumulativeEnergykVArhQ4 = !string.IsNullOrEmpty(BillingProfileThreePhase.CumulativeEnergykVArhQ4) ? Math.Round(double.Parse(BillingProfileThreePhase.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000, 2).ToString() : "";
                BillingProfileThreePhaseDto.TamperCount = !string.IsNullOrEmpty(BillingProfileThreePhase.TamperCount) ? BillingProfileThreePhase.TamperCount : "";

                BillingProfileThreePhaseDto.PowerFactorExport = !string.IsNullOrEmpty(BillingProfileThreePhase.PowerFactorExport) ? BillingProfileThreePhase.PowerFactorExport : "";

                BillingProfileThreePhaseDto.MaximumDemandkWExport = StringExtensions.CheckNullable(BillingProfileThreePhase.MaximumDemandkWExport);
                BillingProfileThreePhaseDto.MaximumDemandkWExportDateTime = BillingProfileThreePhase.MaximumDemandkWExportDateTime;
                BillingProfileThreePhaseDto.MaximumDemandkVAExport = StringExtensions.CheckNullable(BillingProfileThreePhase.MaximumDemandkVAExport);
                BillingProfileThreePhaseDto.MaximumDemandkVAExportDateTime = BillingProfileThreePhase.MaximumDemandkVAExportDateTime;


                BillingProfileThreePhaseDtoList.Add(BillingProfileThreePhaseDto);

                index++;
            }

            return BillingProfileThreePhaseDtoList;
        }
    }
}
