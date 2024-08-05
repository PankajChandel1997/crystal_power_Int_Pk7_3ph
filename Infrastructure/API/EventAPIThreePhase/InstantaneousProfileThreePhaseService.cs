using Domain.Entities.ThreePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.API.EventAPIThreePhase
{
    public class InstantaneousProfileThreePhaseService
    {
        private readonly IDataService<InstantaneousProfileThreePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public InstantaneousProfileThreePhaseService()
        {
            _dataService = new GenericDataService<InstantaneousProfileThreePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }

        public async Task<bool> Add(List<InstantaneousProfileThreePhase> instantaneousProfile)
        {
            try
            {
                //await Delete(instantaneousProfile.FirstOrDefault().MeterNo);
                return await _dataService.CreateRange(instantaneousProfile);
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
                    string query = "select * from InstantaneousProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<InstantaneousProfileThreePhase>().RemoveRange(res);
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

        public async Task<List<InstantaneousProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from InstantaneousProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var instantaneousProfile = new List<InstantaneousProfileThreePhaseDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    instantaneousProfile = await ParseDataToDTO(response);
                }
                else
                {
                    instantaneousProfile = await ParseDataToDTOUMD(response);
                }
                return instantaneousProfile;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<InstantaneousProfileThreePhaseDto>> Filter(string startDate, string endDate, string fetchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from InstantaneousProfileThreePhase where MeterNo = '" + meterNumber + "'";

                var response = await _dataService.Filter(query);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var startDateTime = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var endDateTime = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    response = response.Where(x =>
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date >= startDateTime.Date &&
                        DateTime.ParseExact(x.RealTimeClockDateAndTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date <= endDateTime.Date
                    ).Take(pageSize).ToList();
                }
                else if (!string.IsNullOrEmpty(fetchDate))
                {
                    response = response.Where(x =>
                      x.RealTimeClockDateAndTime == fetchDate).Take(pageSize).ToList();
                }
                else
                {
                    response = response.Take(pageSize).ToList();
                }

                List<InstantaneousProfileThreePhaseDto> instantaneousProfile = await ParseDataToDTO(response);

                return instantaneousProfile;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<InstantaneousProfileThreePhase>> ParseDataToClass(List<InstantaneousProfileThreePhaseDto> instantaneousProfileThreePhaseDtoList)
        {
            List<InstantaneousProfileThreePhase> instantaneousProfileThreePhaseList = new List<InstantaneousProfileThreePhase>();

            foreach (var instantaneousProfileThreePhaseDto in instantaneousProfileThreePhaseDtoList)
            {
                InstantaneousProfileThreePhase instantaneousProfileThreePhase = new InstantaneousProfileThreePhase();

                instantaneousProfileThreePhase.MeterNo = instantaneousProfileThreePhaseDto.MeterNo;
                instantaneousProfileThreePhase.CreatedOn = instantaneousProfileThreePhaseDto.CreatedOn;
                instantaneousProfileThreePhase.RealTimeClockDateAndTime = instantaneousProfileThreePhaseDto.RealTimeClockDateAndTime;
                instantaneousProfileThreePhase.CurrentR = instantaneousProfileThreePhaseDto.CurrentR;
                instantaneousProfileThreePhase.CurrentY = instantaneousProfileThreePhaseDto.CurrentY;
                instantaneousProfileThreePhase.CurrentB = instantaneousProfileThreePhaseDto.CurrentB;
                instantaneousProfileThreePhase.VoltageR = instantaneousProfileThreePhaseDto.VoltageR;
                instantaneousProfileThreePhase.VoltageY = instantaneousProfileThreePhaseDto.VoltageY;
                instantaneousProfileThreePhase.VoltageB = instantaneousProfileThreePhaseDto.VoltageB;
                instantaneousProfileThreePhase.SignedPowerFactorRPhase = instantaneousProfileThreePhaseDto.SignedPowerFactorRPhase;
                instantaneousProfileThreePhase.SignedPowerFactorYPhase = instantaneousProfileThreePhaseDto.SignedPowerFactorYPhase;
                instantaneousProfileThreePhase.SignedPowerFactorBPhase = instantaneousProfileThreePhaseDto.SignedPowerFactorBPhase;
                instantaneousProfileThreePhase.ThreePhasePowerFactoRF = instantaneousProfileThreePhaseDto.ThreePhasePowerFactoRF;
                instantaneousProfileThreePhase.FrequencyHz = instantaneousProfileThreePhaseDto.FrequencyHz;
                instantaneousProfileThreePhase.ApparentPowerKVA = instantaneousProfileThreePhaseDto.ApparentPowerKVA;
                instantaneousProfileThreePhase.SignedActivePowerkW = instantaneousProfileThreePhaseDto.SignedActivePowerkW;
                instantaneousProfileThreePhase.SignedReactivePowerkvar = instantaneousProfileThreePhaseDto.SignedReactivePowerkvar;
                instantaneousProfileThreePhase.CumulativeEnergykWhImport = instantaneousProfileThreePhaseDto.CumulativeEnergykWhImport;
                instantaneousProfileThreePhase.CumulativeEnergykWhExport = instantaneousProfileThreePhaseDto.CumulativeEnergykWhExport;
                instantaneousProfileThreePhase.CumulativeEnergykVAhImport = instantaneousProfileThreePhaseDto.CumulativeEnergykVAhImport;
                instantaneousProfileThreePhase.CumulativeEnergykVAhExport = instantaneousProfileThreePhaseDto.CumulativeEnergykVAhExport;
                instantaneousProfileThreePhase.CumulativeEnergykVArhQ1 = instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ1;
                instantaneousProfileThreePhase.CumulativeEnergykVArhQ2 = instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ2;
                instantaneousProfileThreePhase.CumulativeEnergykVArhQ3 = instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ3;
                instantaneousProfileThreePhase.CumulativeEnergykVArhQ4 = instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ4;
                instantaneousProfileThreePhase.NumberOfPowerFailures = instantaneousProfileThreePhaseDto.NumberOfPowerFailures;
                instantaneousProfileThreePhase.CumulativePowerOFFDurationInMin = instantaneousProfileThreePhaseDto.CumulativePowerOFFDurationInMin;
                instantaneousProfileThreePhase.CumulativeTamperCount = instantaneousProfileThreePhaseDto.CumulativeTamperCount;
                instantaneousProfileThreePhase.BillingPeriodCounter = instantaneousProfileThreePhaseDto.BillingPeriodCounter;
                instantaneousProfileThreePhase.CumulativeProgrammingCount = instantaneousProfileThreePhaseDto.CumulativeProgrammingCount;
                instantaneousProfileThreePhase.BillingDateImportMode = instantaneousProfileThreePhaseDto.BillingDateImportMode;
                instantaneousProfileThreePhase.MaximumDemandkW = instantaneousProfileThreePhaseDto.MaximumDemandkW;
                instantaneousProfileThreePhase.MaximumDemandkWDateTime = instantaneousProfileThreePhaseDto.MaximumDemandkWDateTime;
                instantaneousProfileThreePhase.MaximumDemandkVA = instantaneousProfileThreePhaseDto.MaximumDemandkVA;
                instantaneousProfileThreePhase.MaximumDemandkVADateTime = instantaneousProfileThreePhaseDto.MaximumDemandkVADateTime;
                instantaneousProfileThreePhase.LoadLimitFunctionStatus = instantaneousProfileThreePhaseDto.LoadLimitFunctionStatus;
                instantaneousProfileThreePhase.LoadLimitThresholdkW = instantaneousProfileThreePhaseDto.LoadLimitThresholdkW;

                instantaneousProfileThreePhaseList.Add(instantaneousProfileThreePhase);
            }


            return instantaneousProfileThreePhaseList;
        }

        private async Task<List<InstantaneousProfileThreePhaseDto>> ParseDataToDTO(List<InstantaneousProfileThreePhase> instantaneousProfileThreePhaseList)
        {
            int index = 1;
            List<InstantaneousProfileThreePhaseDto> instantaneousProfileThreePhaseDtoList = new List<InstantaneousProfileThreePhaseDto>();
            foreach (var instantaneousProfileThreePhase in instantaneousProfileThreePhaseList)
            {
                InstantaneousProfileThreePhaseDto instantaneousProfileThreePhaseDto = new InstantaneousProfileThreePhaseDto();

                instantaneousProfileThreePhaseDto.Number = index;
                instantaneousProfileThreePhaseDto.MeterNo = instantaneousProfileThreePhase.MeterNo;
                instantaneousProfileThreePhaseDto.CreatedOn = instantaneousProfileThreePhase.CreatedOn;
                instantaneousProfileThreePhaseDto.RealTimeClockDateAndTime = instantaneousProfileThreePhase.RealTimeClockDateAndTime;
                instantaneousProfileThreePhaseDto.CurrentR = (double.Parse(instantaneousProfileThreePhase.CurrentR, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhase.CurrentR, NumberStyles.Any) ).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CurrentY = (double.Parse(instantaneousProfileThreePhase.CurrentY, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhase.CurrentY, NumberStyles.Any) ).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CurrentB = (double.Parse(instantaneousProfileThreePhase.CurrentB, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhase.CurrentB, NumberStyles.Any) ).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.VoltageR = (double.Parse(instantaneousProfileThreePhase.VoltageR, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.VoltageY = (double.Parse(instantaneousProfileThreePhase.VoltageY, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.VoltageB = (double.Parse(instantaneousProfileThreePhase.VoltageB, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.SignedPowerFactorRPhase = (double.Parse(instantaneousProfileThreePhase.SignedPowerFactorRPhase, NumberStyles.Any) ).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.SignedPowerFactorYPhase = (double.Parse(instantaneousProfileThreePhase.SignedPowerFactorYPhase, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.SignedPowerFactorBPhase = (double.Parse(instantaneousProfileThreePhase.SignedPowerFactorBPhase, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.ThreePhasePowerFactoRF = (double.Parse(instantaneousProfileThreePhase.ThreePhasePowerFactoRF, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.FrequencyHz = (double.Parse(instantaneousProfileThreePhase.FrequencyHz, NumberStyles.Any)).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.ApparentPowerKVA = (double.Parse(instantaneousProfileThreePhase.ApparentPowerKVA, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.SignedActivePowerkW = (double.Parse(instantaneousProfileThreePhase.SignedActivePowerkW, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.SignedReactivePowerkvar = (double.Parse(instantaneousProfileThreePhase.SignedReactivePowerkvar, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykWhImport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykWhExport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVAhImport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVAhExport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ1 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ2 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ3 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ4 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.NumberOfPowerFailures = instantaneousProfileThreePhase.NumberOfPowerFailures;
                instantaneousProfileThreePhaseDto.CumulativePowerOFFDurationInMin = (double.Parse(instantaneousProfileThreePhase.CumulativePowerOFFDurationInMin, NumberStyles.Any)).ToString();
                instantaneousProfileThreePhaseDto.CumulativeTamperCount = instantaneousProfileThreePhase.CumulativeTamperCount;
                instantaneousProfileThreePhaseDto.BillingPeriodCounter = instantaneousProfileThreePhase.BillingPeriodCounter;
                instantaneousProfileThreePhaseDto.CumulativeProgrammingCount = instantaneousProfileThreePhase.CumulativeProgrammingCount;
                instantaneousProfileThreePhaseDto.BillingDateImportMode = instantaneousProfileThreePhase.BillingDateImportMode;
                instantaneousProfileThreePhaseDto.MaximumDemandkW = (double.Parse(instantaneousProfileThreePhase.MaximumDemandkW, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.MaximumDemandkWDateTime = instantaneousProfileThreePhase.MaximumDemandkWDateTime;
                instantaneousProfileThreePhaseDto.MaximumDemandkVA = (double.Parse(instantaneousProfileThreePhase.MaximumDemandkVA, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseDto.MaximumDemandkVADateTime = instantaneousProfileThreePhase.MaximumDemandkVADateTime;
                instantaneousProfileThreePhaseDto.LoadLimitFunctionStatus = instantaneousProfileThreePhase.LoadLimitFunctionStatus;
                instantaneousProfileThreePhaseDto.LoadLimitThresholdkW = (double.Parse(instantaneousProfileThreePhase.LoadLimitThresholdkW, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                instantaneousProfileThreePhaseDtoList.Add(instantaneousProfileThreePhaseDto);
                index++;
            }

            return instantaneousProfileThreePhaseDtoList;
        }

        private async Task<List<InstantaneousProfileThreePhaseDto>> ParseDataToDTOUMD(List<InstantaneousProfileThreePhase> instantaneousProfileThreePhaseList)
        {
            int index = 1;
            List<InstantaneousProfileThreePhaseDto> instantaneousProfileThreePhaseDtoList = new List<InstantaneousProfileThreePhaseDto>();
            foreach (var instantaneousProfileThreePhase in instantaneousProfileThreePhaseList)
            {
                InstantaneousProfileThreePhaseDto instantaneousProfileThreePhaseDto = new InstantaneousProfileThreePhaseDto();

                instantaneousProfileThreePhaseDto.Number = index;
                instantaneousProfileThreePhaseDto.MeterNo = instantaneousProfileThreePhase.MeterNo;
                instantaneousProfileThreePhaseDto.CreatedOn = instantaneousProfileThreePhase.CreatedOn;
                instantaneousProfileThreePhaseDto.RealTimeClockDateAndTime = instantaneousProfileThreePhase.RealTimeClockDateAndTime;
                instantaneousProfileThreePhaseDto.CurrentR = (double.Parse(instantaneousProfileThreePhase.CurrentR, NumberStyles.Any) / 100) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhase.CurrentR, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.CurrentY = (double.Parse(instantaneousProfileThreePhase.CurrentY, NumberStyles.Any) / 100) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhase.CurrentY, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.CurrentB = (double.Parse(instantaneousProfileThreePhase.CurrentB, NumberStyles.Any) / 100) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhase.CurrentB, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.VoltageR = (double.Parse(instantaneousProfileThreePhase.VoltageR, NumberStyles.Any) / 10).ToString();
                instantaneousProfileThreePhaseDto.VoltageY = (double.Parse(instantaneousProfileThreePhase.VoltageY, NumberStyles.Any) / 10).ToString();
                instantaneousProfileThreePhaseDto.VoltageB = (double.Parse(instantaneousProfileThreePhase.VoltageB, NumberStyles.Any) / 10).ToString();
                instantaneousProfileThreePhaseDto.SignedPowerFactorRPhase = (double.Parse(instantaneousProfileThreePhase.SignedPowerFactorRPhase, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.SignedPowerFactorYPhase = (double.Parse(instantaneousProfileThreePhase.SignedPowerFactorYPhase, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.SignedPowerFactorBPhase = (double.Parse(instantaneousProfileThreePhase.SignedPowerFactorBPhase, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.ThreePhasePowerFactoRF = (double.Parse(instantaneousProfileThreePhase.ThreePhasePowerFactoRF, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.FrequencyHz = (double.Parse(instantaneousProfileThreePhase.FrequencyHz, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.ApparentPowerKVA = (double.Parse(instantaneousProfileThreePhase.ApparentPowerKVA, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.SignedActivePowerkW = (double.Parse(instantaneousProfileThreePhase.SignedActivePowerkW, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.SignedReactivePowerkvar = (double.Parse(instantaneousProfileThreePhase.SignedReactivePowerkvar, NumberStyles.Any) / 100).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykWhImport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykWhImport, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykWhExport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVAhImport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVAhExport = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ1 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ2 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ3 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.CumulativeEnergykVArhQ4 = (double.Parse(instantaneousProfileThreePhase.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.NumberOfPowerFailures = instantaneousProfileThreePhase.NumberOfPowerFailures;
                instantaneousProfileThreePhaseDto.CumulativePowerOFFDurationInMin = (double.Parse(instantaneousProfileThreePhase.CumulativePowerOFFDurationInMin, NumberStyles.Any)).ToString();
                instantaneousProfileThreePhaseDto.CumulativeTamperCount = instantaneousProfileThreePhase.CumulativeTamperCount;
                instantaneousProfileThreePhaseDto.BillingPeriodCounter = instantaneousProfileThreePhase.BillingPeriodCounter;
                instantaneousProfileThreePhaseDto.CumulativeProgrammingCount = instantaneousProfileThreePhase.CumulativeProgrammingCount;
                instantaneousProfileThreePhaseDto.BillingDateImportMode = instantaneousProfileThreePhase.BillingDateImportMode;
                instantaneousProfileThreePhaseDto.MaximumDemandkW = (double.Parse(instantaneousProfileThreePhase.MaximumDemandkW, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.MaximumDemandkWDateTime = instantaneousProfileThreePhase.MaximumDemandkWDateTime;
                instantaneousProfileThreePhaseDto.MaximumDemandkVA = (double.Parse(instantaneousProfileThreePhase.MaximumDemandkVA, NumberStyles.Any) / 1000).ToString();
                instantaneousProfileThreePhaseDto.MaximumDemandkVADateTime = instantaneousProfileThreePhase.MaximumDemandkVADateTime;
                instantaneousProfileThreePhaseDto.LoadLimitFunctionStatus = instantaneousProfileThreePhase.LoadLimitFunctionStatus;
                instantaneousProfileThreePhaseDto.LoadLimitThresholdkW = (double.Parse(instantaneousProfileThreePhase.LoadLimitThresholdkW, NumberStyles.Any) / 1000).ToString();

                instantaneousProfileThreePhaseDtoList.Add(instantaneousProfileThreePhaseDto);
                index++;
            }

            return instantaneousProfileThreePhaseDtoList;
        }
    }
}
