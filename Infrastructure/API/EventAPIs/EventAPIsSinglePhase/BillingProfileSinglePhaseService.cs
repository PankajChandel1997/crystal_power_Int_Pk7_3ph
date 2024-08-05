using Domain.Entities.SinglePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIsSinglePhase
{
    public class BillingProfileSinglePhaseService
    {
        private readonly IDataService<BillingProfileSinglePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;
        public BillingProfileSinglePhaseService()
        {
            _dataService = new GenericDataService<BillingProfileSinglePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<BillingProfileSinglePhase> billingProfileProfile)
        {
            try
            {
                //await Delete(billingProfileProfile.FirstOrDefault().MeterNo);
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
                    string query = "select * from BillingProfileSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<BillingProfileSinglePhase>().RemoveRange(res);
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

        public async Task<List<BillingProfileSinglePhaseDto>> GetAll(int PageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BillingProfileSinglePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + PageSize;

                var response = await _dataService.Filter(query);

                List<BillingProfileSinglePhaseDto> billingProfileProfileSinglePhase = await ParseDataToDTO(response);

                return billingProfileProfileSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);

            }
        }

        public async Task<List<BillingProfileSinglePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BillingProfileSinglePhase where MeterNo = '" + meterNumber + "'";

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

                List<BillingProfileSinglePhaseDto> billingProfileProfileSinglePhase = await ParseDataToDTO(response);

                if (billingProfileProfileSinglePhase.Count > 0)
                {
                    int currentMonth = DateTime.Now.Month;
                    int currentYear = DateTime.Now.Year;
                }

                return billingProfileProfileSinglePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        //private async Task<List<BillingProfileSinglePhase>> ParseDataToClass(List<BillingProfileSinglePhaseDto> billingProfileProfileSinglePhaseDtoList)
        //{
        //    List<BillingProfileSinglePhase> billingProfileProfileSinglePhaseList = new List<BillingProfileSinglePhase>();

        //    foreach (var billingProfileProfileSinglePhaseDto in billingProfileProfileSinglePhaseDtoList)
        //    {
        //        BillingProfileSinglePhase billingProfileProfileSinglePhase = new BillingProfileSinglePhase();

        //        billingProfileProfileSinglePhase.MeterNo = billingProfileProfileSinglePhaseDto.MeterNo;
        //        billingProfileProfileSinglePhase.CreatedOn = billingProfileProfileSinglePhaseDto.CreatedOn;
        //        billingProfileProfileSinglePhase.RealTimeClock = billingProfileProfileSinglePhaseDto.RealTimeClock;
        //        billingProfileProfileSinglePhase.AveragePowerFactor = billingProfileProfileSinglePhaseDto.AveragePowerFactor;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhImport = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhImport;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhTZ1 = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhTZ1;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhTZ2 = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhTZ2;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhTZ3 = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhTZ3;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhTZ4 = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhTZ4;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhTZ5 = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhTZ5;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhTZ6 = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhTZ6;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhImport = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhImport;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhTZ1 = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhTZ1;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhTZ2 = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhTZ2;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhTZ3 = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhTZ3;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhTZ4 = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhTZ4;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhTZ5 = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhTZ5;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhTZ6 = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhTZ6;
        //        billingProfileProfileSinglePhase.MaximumDemandkW = billingProfileProfileSinglePhaseDto.MaximumDemandkW;
        //        billingProfileProfileSinglePhase.MaximumDemandkWDateTime = billingProfileProfileSinglePhaseDto.MaximumDemandkWDateTime;
        //        billingProfileProfileSinglePhase.MaximumDemandkVA = billingProfileProfileSinglePhaseDto.MaximumDemandkVA;
        //        billingProfileProfileSinglePhase.MaximumDemandkVADateTime = billingProfileProfileSinglePhaseDto.MaximumDemandkVADateTime;
        //        billingProfileProfileSinglePhase.BillingPowerONdurationinMinutes = billingProfileProfileSinglePhaseDto.BillingPowerONdurationinMinutes;
        //        billingProfileProfileSinglePhase.CumulativeEnergykWhExport = billingProfileProfileSinglePhaseDto.CumulativeEnergykWhExport;
        //        billingProfileProfileSinglePhase.CumulativeEnergykVAhExport = billingProfileProfileSinglePhaseDto.CumulativeEnergykVAhExport;

        //        billingProfileProfileSinglePhaseList.Add(billingProfileProfileSinglePhase);
        //    }

        //    return billingProfileProfileSinglePhaseList;
        //}

        private async Task<List<BillingProfileSinglePhaseDto>> ParseDataToDTO(List<BillingProfileSinglePhase> BillingProfileSinglePhaseList)
        {
            string dateFormat = "dd-MM-yyyy HH:mm:ss";

            //BillingProfileSinglePhaseList = BillingProfileSinglePhaseList.OrderBy(y => DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).GroupBy(y => new { DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Month, DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Year }).Select(group => group.First()).ToList();

            var currentMonthEntries = BillingProfileSinglePhaseList.Where(x => DateTime.ParseExact(x.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Month == DateTime.Now.Month).ToList();

            var latestEntry = currentMonthEntries
                .OrderByDescending(x => DateTime.ParseExact(x.RealTimeClock, dateFormat, CultureInfo.InvariantCulture))
                .FirstOrDefault();

            if (latestEntry != null)
            {
                BillingProfileSinglePhaseList.RemoveAll(x => x.Id == latestEntry.Id);
            }

            int index = 1;
            List<BillingProfileSinglePhaseDto> BillingProfileSinglePhaseDtoList = new List<BillingProfileSinglePhaseDto>();
            foreach (var BillingProfileSinglePhase in BillingProfileSinglePhaseList)
            {
                BillingProfileSinglePhaseDto BillingProfileSinglePhaseDto = new BillingProfileSinglePhaseDto();

                BillingProfileSinglePhaseDto.Number = index;
                BillingProfileSinglePhaseDto.MeterNo = BillingProfileSinglePhase.MeterNo;
                BillingProfileSinglePhaseDto.CreatedOn = BillingProfileSinglePhase.CreatedOn;
                BillingProfileSinglePhaseDto.RealTimeClock = BillingProfileSinglePhase.RealTimeClock;
                BillingProfileSinglePhaseDto.AveragePowerFactor = StringExtensions.CheckNullableOnly(BillingProfileSinglePhase.AveragePowerFactor);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhImport = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhImport);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhTZ1 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhTZ1);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhTZ2 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhTZ2);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhTZ3 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhTZ3);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhTZ4 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhTZ4);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhTZ5 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhTZ5);
                BillingProfileSinglePhaseDto.CumulativeEnergykWhTZ6 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhTZ6);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhImport = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhImport);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhTZ1 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhTZ1);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhTZ2 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhTZ2);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhTZ3 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhTZ3);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhTZ4 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhTZ4);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhTZ5 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhTZ5);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhTZ6 = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhTZ6);

                BillingProfileSinglePhaseDto.MaximumDemandkW = StringExtensions.CheckNullable(BillingProfileSinglePhase.MaximumDemandkW);

                BillingProfileSinglePhaseDto.MaximumDemandkWDateTime = BillingProfileSinglePhase.MaximumDemandkWDateTime;
                BillingProfileSinglePhaseDto.MaximumDemandkVA = StringExtensions.CheckNullable(BillingProfileSinglePhase.MaximumDemandkVA);

                BillingProfileSinglePhaseDto.MaximumDemandkVADateTime = BillingProfileSinglePhase.MaximumDemandkVADateTime;
                BillingProfileSinglePhaseDto.BillingPowerONdurationinMinutes = BillingProfileSinglePhase.BillingPowerONdurationinMinutes;
                BillingProfileSinglePhaseDto.CumulativeEnergykWhExport = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykWhExport);
                BillingProfileSinglePhaseDto.CumulativeEnergykVAhExport = StringExtensions.CheckNullable(BillingProfileSinglePhase.CumulativeEnergykVAhExport);


                BillingProfileSinglePhaseDto.MaximumDemandkWExport = StringExtensions.CheckNullable(BillingProfileSinglePhase.MaximumDemandkWExport);

                BillingProfileSinglePhaseDto.MaximumDemandkWExportDateTime = BillingProfileSinglePhase.MaximumDemandkWExportDateTime;
                BillingProfileSinglePhaseDto.MaximumDemandkvaExport = StringExtensions.CheckNullable(BillingProfileSinglePhase.MaximumDemandkvaExport);

                BillingProfileSinglePhaseDto.MaximumDemandkvaExportDateTime = BillingProfileSinglePhase.MaximumDemandkvaExportDateTime;
                BillingProfileSinglePhaseDto.AveragePowerFactorExport = StringExtensions.CheckNullableOnly(BillingProfileSinglePhase.AveragePowerFactorExport);



                BillingProfileSinglePhaseDtoList.Add(BillingProfileSinglePhaseDto);
                index++;
            }
            
            return BillingProfileSinglePhaseDtoList;
        }
    }
}
