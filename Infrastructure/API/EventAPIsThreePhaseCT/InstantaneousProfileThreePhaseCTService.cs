using Domain.Entities.ThreePhaseCTEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.API.EventAPIThreePhaseCT
{
    public class InstantaneousProfileThreePhaseCTService
    {
        private readonly IDataService<InstantaneousProfileThreePhaseCT> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public InstantaneousProfileThreePhaseCTService()
        {
            _dataService = new GenericDataService<InstantaneousProfileThreePhaseCT>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }

        public async Task<bool> Add(List<InstantaneousProfileThreePhaseCT> instantaneousProfile)
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
                    string query = "select * from InstantaneousProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    if (res.Any())
                    {
                        db.Set<InstantaneousProfileThreePhaseCT>().RemoveRange(res);
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

        public async Task<List<InstantaneousProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from InstantaneousProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);
                if (response == null)
                {
                    return null;
                }
                List<InstantaneousProfileThreePhaseCTDto> instantaneousProfile = await ParseDataToDTO(response);
                if (instantaneousProfile == null)
                {
                    return null;
                }
                return instantaneousProfile;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<InstantaneousProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fetchDate,int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from InstantaneousProfileThreePhaseCT where MeterNo = '" + meterNumber + "'";

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

                List<InstantaneousProfileThreePhaseCTDto> instantaneousProfile = await ParseDataToDTO(response);

                return instantaneousProfile;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);

                throw new Exception(ex.Message);
            }
        }

        private async Task<List<InstantaneousProfileThreePhaseCT>> ParseDataToClass(List<InstantaneousProfileThreePhaseCTDto> instantaneousProfileThreePhaseCTDtoList)
        {
            List<InstantaneousProfileThreePhaseCT> instantaneousProfileThreePhaseCTList = new List<InstantaneousProfileThreePhaseCT>();

            foreach (var instantaneousProfileThreePhaseCTDto in instantaneousProfileThreePhaseCTDtoList)
            {
                InstantaneousProfileThreePhaseCT instantaneousProfileThreePhaseCT = new InstantaneousProfileThreePhaseCT();

                instantaneousProfileThreePhaseCT.MeterNo = instantaneousProfileThreePhaseCTDto.MeterNo;
                instantaneousProfileThreePhaseCT.CreatedOn = instantaneousProfileThreePhaseCTDto.CreatedOn;
                instantaneousProfileThreePhaseCT.RealTimeClockDateAndTime = instantaneousProfileThreePhaseCTDto.RealTimeClockDateAndTime;
                instantaneousProfileThreePhaseCT.CurrentR = instantaneousProfileThreePhaseCTDto.CurrentR;
                instantaneousProfileThreePhaseCT.CurrentY = instantaneousProfileThreePhaseCTDto.CurrentY;
                instantaneousProfileThreePhaseCT.CurrentB = instantaneousProfileThreePhaseCTDto.CurrentB;
                instantaneousProfileThreePhaseCT.VoltageR = instantaneousProfileThreePhaseCTDto.VoltageR;
                instantaneousProfileThreePhaseCT.VoltageY = instantaneousProfileThreePhaseCTDto.VoltageY;
                instantaneousProfileThreePhaseCT.VoltageB = instantaneousProfileThreePhaseCTDto.VoltageB;
                instantaneousProfileThreePhaseCT.SignedPowerFactorRPhase = instantaneousProfileThreePhaseCTDto.SignedPowerFactorRPhase;
                instantaneousProfileThreePhaseCT.SignedPowerFactorYPhase = instantaneousProfileThreePhaseCTDto.SignedPowerFactorYPhase;
                instantaneousProfileThreePhaseCT.SignedPowerFactorBPhase = instantaneousProfileThreePhaseCTDto.SignedPowerFactorBPhase;
                instantaneousProfileThreePhaseCT.ThreePhasePowerFactorPF = instantaneousProfileThreePhaseCTDto.ThreePhasePowerFactoPF;
                instantaneousProfileThreePhaseCT.FrequencyHz = instantaneousProfileThreePhaseCTDto.FrequencyHz;
                instantaneousProfileThreePhaseCT.ApparentPowerKVA = instantaneousProfileThreePhaseCTDto.ApparentPowerKVA;
                instantaneousProfileThreePhaseCT.SignedActivePowerkW = instantaneousProfileThreePhaseCTDto.SignedActivePowerkW;
                instantaneousProfileThreePhaseCT.SignedReactivePowerkvar = instantaneousProfileThreePhaseCTDto.SignedReactivePowerkvar;
                instantaneousProfileThreePhaseCT.CumulativeEnergykWhImport = instantaneousProfileThreePhaseCTDto.CumulativeEnergykWhImport;
                instantaneousProfileThreePhaseCT.CumulativeEnergykWhExport = instantaneousProfileThreePhaseCTDto.CumulativeEnergykWhExport;
                instantaneousProfileThreePhaseCT.CumulativeEnergykVAhImport = instantaneousProfileThreePhaseCTDto.CumulativeEnergykVAhImport;
                instantaneousProfileThreePhaseCT.CumulativeEnergykVAhExport = instantaneousProfileThreePhaseCTDto.CumulativeEnergykVAhExport;
                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ1 = instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ1;
                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ2 = instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ2;
                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ3 = instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ3;
                instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ4 = instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ4;
                instantaneousProfileThreePhaseCT.NumberOfPowerFailures = instantaneousProfileThreePhaseCTDto.NumberOfPowerFailures;
                instantaneousProfileThreePhaseCT.CumulativePowerOFFDurationInMin = instantaneousProfileThreePhaseCTDto.CumulativePowerOFFDurationInMin;
                instantaneousProfileThreePhaseCT.CumulativeTamperCount = instantaneousProfileThreePhaseCTDto.CumulativeTamperCount;
                instantaneousProfileThreePhaseCT.BillingPeriodCounter = instantaneousProfileThreePhaseCTDto.BillingPeriodCounter;
                instantaneousProfileThreePhaseCT.CumulativeProgrammingCount = instantaneousProfileThreePhaseCTDto.CumulativeProgrammingCount;
                instantaneousProfileThreePhaseCT.BillingDateImportMode = instantaneousProfileThreePhaseCTDto.BillingDateImportMode;
                instantaneousProfileThreePhaseCT.MaximumDemandkW = instantaneousProfileThreePhaseCTDto.MaximumDemandkW;
                instantaneousProfileThreePhaseCT.MaximumDemandkWDateTime = instantaneousProfileThreePhaseCTDto.MaximumDemandkWDateTime;
                instantaneousProfileThreePhaseCT.MaximumDemandkVA = instantaneousProfileThreePhaseCTDto.MaximumDemandkVA;
                instantaneousProfileThreePhaseCT.MaximumDemandkVADateTime = instantaneousProfileThreePhaseCTDto.MaximumDemandkVADateTime;

                instantaneousProfileThreePhaseCTList.Add(instantaneousProfileThreePhaseCT);
            }


            return instantaneousProfileThreePhaseCTList;
        }

        private async Task<List<InstantaneousProfileThreePhaseCTDto>> ParseDataToDTO(List<InstantaneousProfileThreePhaseCT> instantaneousProfileThreePhaseCTList)
        {
            int index = 1;
            List<InstantaneousProfileThreePhaseCTDto> instantaneousProfileThreePhaseCTDtoList = new List<InstantaneousProfileThreePhaseCTDto>();
            foreach (var instantaneousProfileThreePhaseCT in instantaneousProfileThreePhaseCTList)
            {
                InstantaneousProfileThreePhaseCTDto instantaneousProfileThreePhaseCTDto = new InstantaneousProfileThreePhaseCTDto();

                instantaneousProfileThreePhaseCTDto.Number = index;
                instantaneousProfileThreePhaseCTDto.MeterNo = instantaneousProfileThreePhaseCT.MeterNo;
                instantaneousProfileThreePhaseCTDto.CreatedOn = instantaneousProfileThreePhaseCT.CreatedOn;
                instantaneousProfileThreePhaseCTDto.RealTimeClockDateAndTime = instantaneousProfileThreePhaseCT.RealTimeClockDateAndTime;
                instantaneousProfileThreePhaseCTDto.CurrentR = (double.Parse(instantaneousProfileThreePhaseCT.CurrentR, NumberStyles.Any) / 1) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhaseCT.CurrentR, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CurrentY = (double.Parse(instantaneousProfileThreePhaseCT.CurrentY, NumberStyles.Any) / 1) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhaseCT.CurrentY, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CurrentB = (double.Parse(instantaneousProfileThreePhaseCT.CurrentB, NumberStyles.Any) / 1) == 0 ? "0" : (double.Parse(instantaneousProfileThreePhaseCT.CurrentB, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.VoltageR = (double.Parse(instantaneousProfileThreePhaseCT.VoltageR, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.VoltageY = (double.Parse(instantaneousProfileThreePhaseCT.VoltageY, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.VoltageB = (double.Parse(instantaneousProfileThreePhaseCT.VoltageB, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.SignedPowerFactorRPhase = (double.Parse(instantaneousProfileThreePhaseCT.SignedPowerFactorRPhase, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.SignedPowerFactorYPhase = (double.Parse(instantaneousProfileThreePhaseCT.SignedPowerFactorYPhase, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.SignedPowerFactorBPhase = (double.Parse(instantaneousProfileThreePhaseCT.SignedPowerFactorBPhase, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.ThreePhasePowerFactoPF = instantaneousProfileThreePhaseCT.ThreePhasePowerFactorPF != null ? (double.Parse(instantaneousProfileThreePhaseCT.ThreePhasePowerFactorPF, NumberStyles.Any) / 1).ToString().CustomTrucate() :"0";
                instantaneousProfileThreePhaseCTDto.FrequencyHz = (double.Parse(instantaneousProfileThreePhaseCT.FrequencyHz, NumberStyles.Any) / 1).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.ApparentPowerKVA = (double.Parse(instantaneousProfileThreePhaseCT.ApparentPowerKVA, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.SignedActivePowerkW = (double.Parse(instantaneousProfileThreePhaseCT.SignedActivePowerkW, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.SignedReactivePowerkvar = (double.Parse(instantaneousProfileThreePhaseCT.SignedReactivePowerkvar, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykWhImport = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykWhExport = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykVAhImport = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykVAhExport = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ1 = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ2 = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ3 = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.CumulativeEnergykVArhQ4 = (double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVArhQ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                instantaneousProfileThreePhaseCTDto.NumberOfPowerFailures = instantaneousProfileThreePhaseCT.NumberOfPowerFailures;
                instantaneousProfileThreePhaseCTDto.CumulativePowerOFFDurationInMin = (double.Parse(instantaneousProfileThreePhaseCT.CumulativePowerOFFDurationInMin, NumberStyles.Any)).ToString();
                instantaneousProfileThreePhaseCTDto.CumulativeTamperCount = instantaneousProfileThreePhaseCT.CumulativeTamperCount;
                instantaneousProfileThreePhaseCTDto.BillingPeriodCounter = instantaneousProfileThreePhaseCT.BillingPeriodCounter;
                instantaneousProfileThreePhaseCTDto.CumulativeProgrammingCount = instantaneousProfileThreePhaseCT.CumulativeProgrammingCount;
                instantaneousProfileThreePhaseCTDto.BillingDateImportMode = instantaneousProfileThreePhaseCT.BillingDateImportMode;
                instantaneousProfileThreePhaseCTDto.MaximumDemandkW = (double.Parse(instantaneousProfileThreePhaseCT.MaximumDemandkW, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.MaximumDemandkWDateTime = instantaneousProfileThreePhaseCT.MaximumDemandkWDateTime;
                instantaneousProfileThreePhaseCTDto.MaximumDemandkVA = (double.Parse(instantaneousProfileThreePhaseCT.MaximumDemandkVA, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                instantaneousProfileThreePhaseCTDto.MaximumDemandkVADateTime = instantaneousProfileThreePhaseCT.MaximumDemandkVADateTime;

                //High Resolution values
                instantaneousProfileThreePhaseCTDto.CumulativeEnergyWhImport = Math.Round(double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykWhImport, NumberStyles.Any) / 1, 5).ToString("0.00000");
                instantaneousProfileThreePhaseCTDto.CumulativeEnergyWhExport = Math.Round(double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykWhExport, NumberStyles.Any) / 1, 5).ToString("0.00000");
                instantaneousProfileThreePhaseCTDto.CumulativeEnergyVAhImport = Math.Round(double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVAhImport, NumberStyles.Any) / 1, 5).ToString("0.00000");
                instantaneousProfileThreePhaseCTDto.CumulativeEnergyVAhExport = Math.Round(double.Parse(instantaneousProfileThreePhaseCT.CumulativeEnergykVAhExport, NumberStyles.Any) / 1, 5).ToString("0.00000");

                instantaneousProfileThreePhaseCTDtoList.Add(instantaneousProfileThreePhaseCTDto);
                index++;
            }

            return instantaneousProfileThreePhaseCTDtoList;
        }
    }
}
