using Domain.Entities.ThreePhaseCTEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIThreePhaseCT
{
    public class BillingProfileThreePhaseCTService
    {
        private readonly IDataService<BillingProfileThreePhaseCT> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public BillingProfileThreePhaseCTService()
        {
            _dataService = new GenericDataService<BillingProfileThreePhaseCT>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<BillingProfileThreePhaseCT> billingProfileProfile)
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
                    string query = "select * from BillingProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<BillingProfileThreePhaseCT>().RemoveRange(res);
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

        public async Task<List<BillingProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BillingProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<BillingProfileThreePhaseCTDto> billingProfileProfileThreePhaseCT = await ParseDataToDTO(response);

                return billingProfileProfileThreePhaseCT;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BillingProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BillingProfileThreePhaseCT where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
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

                List<BillingProfileThreePhaseCTDto> billingProfileProfileThreePhaseCT = await ParseDataToDTO(response);

                if (billingProfileProfileThreePhaseCT.Count > 0)
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

        //private async Task<List<BillingProfileThreePhaseCT>> ParseDataToClass(List<BillingProfileThreePhaseCTDto> billingProfileProfileThreePhaseCTDtoList)
        //{
        //    List<BillingProfileThreePhaseCT> billingProfileProfileThreePhaseCTList = new List<BillingProfileThreePhaseCT>();

        //    foreach (var billingProfileProfileThreePhaseCTDto in billingProfileProfileThreePhaseCTDtoList)
        //    {
        //        BillingProfileThreePhaseCT billingProfileProfileThreePhaseCT = new BillingProfileThreePhaseCT();

        //        billingProfileProfileThreePhaseCT.MeterNo = billingProfileProfileThreePhaseCTDto.MeterNo;
        //        billingProfileProfileThreePhaseCT.CreatedOn = billingProfileProfileThreePhaseCTDto.CreatedOn;
        //        billingProfileProfileThreePhaseCT.BillingDate = billingProfileProfileThreePhaseCTDto.BillingDate;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWh = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWh;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ1 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ1;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ2 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ2;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ3 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ3;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ4 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ4;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ5 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ5;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ6 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ6;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ7 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ7;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhTZ8 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhTZ8;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykWhExport = billingProfileProfileThreePhaseCTDto.CumulativeEnergykWhExport;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhExport = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhExport;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhImport = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhImport;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ1 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ1;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ2 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ2;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ3 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ3;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ4 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ4;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ5 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ5;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ6 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ6;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ7 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ7;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVAhTZ8 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVAhTZ8;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkW = billingProfileProfileThreePhaseCTDto.MaximumDemandkW;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWDateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWDateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ1 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ1;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ1DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ1DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ2 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ2;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ2DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ2DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ3 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ3;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ3DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ3DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ4 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ4;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ4DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ4DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ5 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ5;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ5DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ5DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ6 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ6;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ6DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ6DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ7 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ7;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ7DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ7DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ8 = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ8;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkWForTZ8DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkWForTZ8DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVA = billingProfileProfileThreePhaseCTDto.MaximumDemandkVA;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVADateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVADateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ1 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ1;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ1DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ1DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ2 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ2;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ2DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ2DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ3 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ3;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ3DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ3DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ4 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ4;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ4DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ4DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ5 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ5;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ5DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ5DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ6 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ6;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ6DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ6DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ7 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ7;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ7DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ7DateTime;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ8 = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ8;
        //        billingProfileProfileThreePhaseCT.MaximumDemandkVAForTZ8DateTime = billingProfileProfileThreePhaseCTDto.MaximumDemandkVAForTZ8DateTime;
        //        billingProfileProfileThreePhaseCT.BillingPowerONdurationInMinutesDBP = billingProfileProfileThreePhaseCTDto.BillingPowerONdurationInMinutesDBP;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ1 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ1;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ2 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ2;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ3 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ3;
        //        billingProfileProfileThreePhaseCT.CumulativeEnergykVArhQ4 = billingProfileProfileThreePhaseCTDto.CumulativeEnergykVArhQ4;
        //        //billingProfileProfileThreePhaseCT.TamperCount = billingProfileProfileThreePhaseCTDto.TamperCount;


        //        billingProfileProfileThreePhaseCTList.Add(billingProfileProfileThreePhaseCT);
        //    }

        //    return billingProfileProfileThreePhaseCTList;
        //}

        private async Task<List<BillingProfileThreePhaseCTDto>> ParseDataToDTO(List<BillingProfileThreePhaseCT> BillingProfileThreePhaseCTList)
        {
            try
            {
                int index = 1;
                List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCTDtoList = new List<BillingProfileThreePhaseCTDto>();
                foreach (var BillingProfileThreePhaseCT in BillingProfileThreePhaseCTList)
                {
                    BillingProfileThreePhaseCTDto BillingProfileThreePhaseCTDto = new BillingProfileThreePhaseCTDto();

                    BillingProfileThreePhaseCTDto.Number = index;
                    BillingProfileThreePhaseCTDto.MeterNo = BillingProfileThreePhaseCT.MeterNo;
                    BillingProfileThreePhaseCTDto.CreatedOn = BillingProfileThreePhaseCT.CreatedOn;
                    BillingProfileThreePhaseCTDto.RealTimeClock = BillingProfileThreePhaseCT.RealTimeClock;
                    BillingProfileThreePhaseCTDto.AveragePFForBillingPeriod= (double.Parse(BillingProfileThreePhaseCT.AveragePFForBillingPeriod, NumberStyles.Any) / 1).ToString().CustomTrucate(); ;
                    BillingProfileThreePhaseCTDto.BillingDate = BillingProfileThreePhaseCT.BillingDate;
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWh = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWh, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ1 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ2 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ3 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ4 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ5 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ5, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ6 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ6, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ7 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ7, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhTZ8 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhTZ8, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykWhExport = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhExport = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhImport = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ1 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ2 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ3 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ4 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ5 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ5, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ6 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ6, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ7 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ7, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVAhTZ8 = (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVAhTZ8, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkW = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkW, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWDateTime = BillingProfileThreePhaseCT.MaximumDemandkWDateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ1 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ1DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ1DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ2 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ2DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ2DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ3 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ3DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ3DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ4 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ4DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ4DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ5 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ5, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ5DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ5DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ6 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ6, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ6DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ6DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ7 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ7, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ7DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ7DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ8 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkWForTZ8, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkWForTZ8DateTime = BillingProfileThreePhaseCT.MaximumDemandkWForTZ8DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVA = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVA, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVADateTime = BillingProfileThreePhaseCT.MaximumDemandkVADateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ1 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ1DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ1DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ2 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ2DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ2DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ3 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ3DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ3DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ4 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ4DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ4DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ5 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ5, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ5DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ5DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ6 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ6, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ6DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ6DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ7 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ7, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ7DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ7DateTime;
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ8 = (double.Parse(BillingProfileThreePhaseCT.MaximumDemandkVAForTZ8, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                    BillingProfileThreePhaseCTDto.MaximumDemandkVAForTZ8DateTime = BillingProfileThreePhaseCT.MaximumDemandkVAForTZ8DateTime;
                    BillingProfileThreePhaseCTDto.BillingPowerONdurationInMinutesDBP = BillingProfileThreePhaseCT.BillingPowerONdurationInMinutesDBP;
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVArhQ1 = !string.IsNullOrEmpty(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ1) ? (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000).ToString().CustomTrucate() : "";
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVArhQ2 = !string.IsNullOrEmpty(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ2) ? (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000).ToString().CustomTrucate() : "";
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVArhQ3 = !string.IsNullOrEmpty(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ3) ? (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000).ToString().CustomTrucate() : "";
                    BillingProfileThreePhaseCTDto.CumulativeEnergykVArhQ4 = !string.IsNullOrEmpty(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ4) ? (double.Parse(BillingProfileThreePhaseCT.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000).ToString().CustomTrucate() : "";
                    //BillingProfileThreePhaseCTDto.TamperCount = !string.IsNullOrEmpty(BillingProfileThreePhaseCT.TamperCount) ? BillingProfileThreePhaseCT.TamperCount : "";

                    BillingProfileThreePhaseCTDtoList.Add(BillingProfileThreePhaseCTDto);

                    index++;

                }
                BillingProfileThreePhaseCTDtoList = BillingProfileThreePhaseCTDtoList.OrderByDescending(x => x.Number).ToList();

                return BillingProfileThreePhaseCTDtoList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
