using Domain.Entities.ThreePhaseEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIThreePhase
{
    public class BlockLoadProfileThreePhaseService
    {
        private readonly IDataService<BlockLoadProfileThreePhase> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public BlockLoadProfileThreePhaseService()
        {
            _dataService = new GenericDataService<BlockLoadProfileThreePhase>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<BlockLoadProfileThreePhase> blockLoadProfile)
        {
            try
            {
                return await _dataService.CreateRange(blockLoadProfile);
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
                    string query = "select * from BlockLoadProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    List<string> fatchedDates = res.DistinctBy(x => x.RealTimeClock).OrderByDescending(c => c.Id).Select(d => d.RealTimeClock).ToList();
                    foreach (var fatchedDate in fatchedDates)
                    {
                        var duplicateData = res.Where(x => x.RealTimeClock == fatchedDate).ToList().Skip(1);
                        if (duplicateData.Any())
                        {
                            db.Set<BlockLoadProfileThreePhase>().RemoveRange(duplicateData);
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

        public async Task<List<BlockLoadProfileThreePhaseDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BlockLoadProfileThreePhase where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                var meter = await _dataService.GetByMeterNoAsync(meterNumber);

                var blockLoadProfileThreePhase = new List<BlockLoadProfileThreePhaseDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    blockLoadProfileThreePhase = await ParseDataToDTO(response);
                }
                else
                {
                    blockLoadProfileThreePhase = await ParseDataToDTOUMD(response);
                }

                return blockLoadProfileThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BlockLoadProfileThreePhaseDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BlockLoadProfileThreePhase where MeterNo = '" + meterNumber + "'";

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

                var blockLoadProfileThreePhase = new List<BlockLoadProfileThreePhaseDto>();

                if (string.IsNullOrEmpty(meter.ManSpecificFirmwareVersion))
                {
                    blockLoadProfileThreePhase = await ParseDataToDTO(response);
                }
                else
                {
                    blockLoadProfileThreePhase = await ParseDataToDTOUMD(response);
                }
                return blockLoadProfileThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<BlockLoadProfileThreePhase>> ParseDataToClass(List<BlockLoadProfileThreePhaseDto> blockLoadProfileThreePhaseDtoList)
        {
            List<BlockLoadProfileThreePhase> blockLoadProfileThreePhaseList = new List<BlockLoadProfileThreePhase>();

            foreach (var blockLoadProfileThreePhaseDto in blockLoadProfileThreePhaseDtoList)
            {
                BlockLoadProfileThreePhase blockLoadProfileThreePhase = new BlockLoadProfileThreePhase();

                blockLoadProfileThreePhase.MeterNo = blockLoadProfileThreePhaseDto.MeterNo;
                blockLoadProfileThreePhase.CreatedOn = blockLoadProfileThreePhaseDto.CreatedOn;
                blockLoadProfileThreePhase.RealTimeClock = blockLoadProfileThreePhaseDto.RealTimeClock;
                blockLoadProfileThreePhase.CurrentR = blockLoadProfileThreePhaseDto.CurrentR;
                blockLoadProfileThreePhase.CurrentY = blockLoadProfileThreePhaseDto.CurrentY;
                blockLoadProfileThreePhase.CurrentB = blockLoadProfileThreePhaseDto.CurrentB;
                blockLoadProfileThreePhase.VoltageR = blockLoadProfileThreePhaseDto.VoltageR;
                blockLoadProfileThreePhase.VoltageY = blockLoadProfileThreePhaseDto.VoltageY;
                blockLoadProfileThreePhase.VoltageB = blockLoadProfileThreePhaseDto.VoltageB;
                blockLoadProfileThreePhase.PowerFactorRPhase = blockLoadProfileThreePhaseDto.PowerFactorRPhase;
                blockLoadProfileThreePhase.PowerFactorYPhase = blockLoadProfileThreePhaseDto.PowerFactorYPhase;
                blockLoadProfileThreePhase.PowerFactorBPhase = blockLoadProfileThreePhaseDto.PowerFactorBPhase;
                blockLoadProfileThreePhase.BlockEnergykWhImport = blockLoadProfileThreePhaseDto.BlockEnergykWhImport;
                blockLoadProfileThreePhase.BlockEnergykVAhImport = blockLoadProfileThreePhaseDto.BlockEnergykVAhImport;
                blockLoadProfileThreePhase.BlockEnergykWhExport = blockLoadProfileThreePhaseDto.BlockEnergykWhExport;
                blockLoadProfileThreePhase.BlockEnergykVAhExport = blockLoadProfileThreePhaseDto.BlockEnergykVAhExport;
                blockLoadProfileThreePhase.MeterHealthIndicator = blockLoadProfileThreePhaseDto.MeterHealthIndicator;
                blockLoadProfileThreePhase.ImportAvgPF = blockLoadProfileThreePhaseDto.ImportAvgPF;

                blockLoadProfileThreePhaseList.Add(blockLoadProfileThreePhase);
            }



            return blockLoadProfileThreePhaseList;
        }

        private async Task<List<BlockLoadProfileThreePhaseDto>> ParseDataToDTO(List<BlockLoadProfileThreePhase> BlockLoadProfileThreePhaseList)
        {
            int index = 1;
            List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhaseDtoList = new List<BlockLoadProfileThreePhaseDto>();
            foreach (var BlockLoadProfileThreePhase in BlockLoadProfileThreePhaseList)
            {
                BlockLoadProfileThreePhaseDto BlockLoadProfileThreePhaseDto = new BlockLoadProfileThreePhaseDto();

                BlockLoadProfileThreePhaseDto.Number = index;
                BlockLoadProfileThreePhaseDto.MeterNo = BlockLoadProfileThreePhase.MeterNo;
                BlockLoadProfileThreePhaseDto.CreatedOn = BlockLoadProfileThreePhase.CreatedOn;
                BlockLoadProfileThreePhaseDto.RealTimeClock = BlockLoadProfileThreePhase.RealTimeClock;
                BlockLoadProfileThreePhaseDto.CurrentR = (double.Parse(BlockLoadProfileThreePhase.CurrentR, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentR, NumberStyles.Any) / 100).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.CurrentY = (double.Parse(BlockLoadProfileThreePhase.CurrentY, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentY, NumberStyles.Any) / 100).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.CurrentB = (double.Parse(BlockLoadProfileThreePhase.CurrentB, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentB, NumberStyles.Any) / 100).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.VoltageR = (double.Parse(BlockLoadProfileThreePhase.VoltageR, NumberStyles.Any) ).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.VoltageY = (double.Parse(BlockLoadProfileThreePhase.VoltageY, NumberStyles.Any) ).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.VoltageB = (double.Parse(BlockLoadProfileThreePhase.VoltageB, NumberStyles.Any) ).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.PowerFactorRPhase = (double.Parse(BlockLoadProfileThreePhase.PowerFactorRPhase, NumberStyles.Any) ).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.PowerFactorYPhase = (double.Parse(BlockLoadProfileThreePhase.PowerFactorYPhase, NumberStyles.Any) ).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.PowerFactorBPhase = (double.Parse(BlockLoadProfileThreePhase.PowerFactorBPhase, NumberStyles.Any) ).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykWhImport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVAhImport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykWhExport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVAhExport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.MeterHealthIndicator = BlockLoadProfileThreePhase.MeterHealthIndicator;
                BlockLoadProfileThreePhaseDto.ImportAvgPF = (double.Parse(BlockLoadProfileThreePhase.ImportAvgPF, NumberStyles.Any)).ToString().CustomTrucate();

                BlockLoadProfileThreePhaseDtoList.Add(BlockLoadProfileThreePhaseDto);
                index++;
            }

            return BlockLoadProfileThreePhaseDtoList;
        }

        private async Task<List<BlockLoadProfileThreePhaseDto>> ParseDataToDTOUMD(List<BlockLoadProfileThreePhase> BlockLoadProfileThreePhaseList)
        {
            int index = 1;
            List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhaseDtoList = new List<BlockLoadProfileThreePhaseDto>();
            foreach (var BlockLoadProfileThreePhase in BlockLoadProfileThreePhaseList)
            {
                BlockLoadProfileThreePhaseDto BlockLoadProfileThreePhaseDto = new BlockLoadProfileThreePhaseDto();

                BlockLoadProfileThreePhaseDto.Number = index;
                BlockLoadProfileThreePhaseDto.MeterNo = BlockLoadProfileThreePhase.MeterNo;
                BlockLoadProfileThreePhaseDto.CreatedOn = BlockLoadProfileThreePhase.CreatedOn;
                BlockLoadProfileThreePhaseDto.RealTimeClock = BlockLoadProfileThreePhase.RealTimeClock;
                BlockLoadProfileThreePhaseDto.CurrentR = (double.Parse(BlockLoadProfileThreePhase.CurrentR, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentR, NumberStyles.Any) ).ToString();
                BlockLoadProfileThreePhaseDto.CurrentY = (double.Parse(BlockLoadProfileThreePhase.CurrentY, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentY, NumberStyles.Any) ).ToString();
                BlockLoadProfileThreePhaseDto.CurrentB = (double.Parse(BlockLoadProfileThreePhase.CurrentB, NumberStyles.Any) ) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentB, NumberStyles.Any) ).ToString();
                BlockLoadProfileThreePhaseDto.VoltageR = (double.Parse(BlockLoadProfileThreePhase.VoltageR, NumberStyles.Any) ).ToString();
                BlockLoadProfileThreePhaseDto.VoltageY = (double.Parse(BlockLoadProfileThreePhase.VoltageY, NumberStyles.Any) ).ToString();
                BlockLoadProfileThreePhaseDto.VoltageB = (double.Parse(BlockLoadProfileThreePhase.VoltageB, NumberStyles.Any) ).ToString();
                BlockLoadProfileThreePhaseDto.PowerFactorRPhase = !string.IsNullOrEmpty(BlockLoadProfileThreePhase.PowerFactorRPhase) ? (double.Parse(BlockLoadProfileThreePhase.PowerFactorRPhase, NumberStyles.Any) ).ToString() : "0";
                BlockLoadProfileThreePhaseDto.PowerFactorYPhase = !string.IsNullOrEmpty(BlockLoadProfileThreePhase.PowerFactorRPhase) ?(double.Parse(BlockLoadProfileThreePhase.PowerFactorYPhase, NumberStyles.Any) ).ToString() : "0";
                BlockLoadProfileThreePhaseDto.PowerFactorBPhase = !string.IsNullOrEmpty(BlockLoadProfileThreePhase.PowerFactorRPhase) ? (double.Parse(BlockLoadProfileThreePhase.PowerFactorBPhase, NumberStyles.Any) ).ToString() : "0";
                BlockLoadProfileThreePhaseDto.BlockEnergykWhImport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVAhImport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykWhExport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVAhExport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.MeterHealthIndicator = BlockLoadProfileThreePhase.MeterHealthIndicator;
                BlockLoadProfileThreePhaseDto.ImportAvgPF = (double.Parse(BlockLoadProfileThreePhase.ImportAvgPF, NumberStyles.Any)).ToString();

                BlockLoadProfileThreePhaseDtoList.Add(BlockLoadProfileThreePhaseDto);
                index++;
            }

            return BlockLoadProfileThreePhaseDtoList;
        }
    }
}
