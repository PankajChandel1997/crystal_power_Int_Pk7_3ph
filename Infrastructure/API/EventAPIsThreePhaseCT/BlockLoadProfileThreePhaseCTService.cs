using Domain.Entities.ThreePhaseCTEntities;
using Domain.Interface.Service;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.Helpers;
using Infrastructure.Services;
using System.Globalization;

namespace Infrastructure.API.EventAPIThreePhaseCT
{
    public class BlockLoadProfileThreePhaseCTService
    {
        private readonly IDataService<BlockLoadProfileThreePhaseCT> _dataService;
        public ErrorHelper _errorHelper;
        private readonly ApplicationContextFactory _contextFactory;

        public BlockLoadProfileThreePhaseCTService()
        {
            _dataService = new GenericDataService<BlockLoadProfileThreePhaseCT>(new ApplicationContextFactory());
            _errorHelper = new ErrorHelper();
            _contextFactory = new ApplicationContextFactory();
        }
        public async Task<bool> Add(List<BlockLoadProfileThreePhaseCT> blockLoadProfile)
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
                    string query = "select * from BlockLoadProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC";

                    var res = await _dataService.Filter(query);

                    List<string> fatchedDates = res.DistinctBy(x => x.RealTimeClock).OrderByDescending(c => c.Id).Select(d => d.RealTimeClock).ToList();
                    foreach (var fatchedDate in fatchedDates)
                    {
                        var duplicateData = res.Where(x => x.RealTimeClock == fatchedDate).ToList().Skip(1);
                        if (duplicateData.Any())
                        {
                            db.Set<BlockLoadProfileThreePhaseCT>().RemoveRange(duplicateData);
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

        public async Task<List<BlockLoadProfileThreePhaseCTDto>> GetAll(int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BlockLoadProfileThreePhaseCT where MeterNo = '" + meterNumber + "' ORDER by Id DESC LIMIT " + pageSize;

                var response = await _dataService.Filter(query);

                List<BlockLoadProfileThreePhaseCTDto> blockLoadProfileThreePhase = await ParseDataToDTO(response);

                return blockLoadProfileThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BlockLoadProfileThreePhaseCTDto>> Filter(string startDate, string endDate, string fatchDate, int pageSize, string meterNumber)
        {
            try
            {
                string query = "select * from BlockLoadProfileThreePhaseCT where MeterNo = '" + meterNumber + "'";

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

                List<BlockLoadProfileThreePhaseCTDto> blockLoadProfileThreePhase = await ParseDataToDTO(response);

                return blockLoadProfileThreePhase;
            }
            catch (Exception ex)
            {
                _errorHelper.WriteLog(ex.Message + "inner Exception ==> " + ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<BlockLoadProfileThreePhaseCT>> ParseDataToClass(List<BlockLoadProfileThreePhaseCTDto> blockLoadProfileThreePhaseCTDtoList)
        {
            List<BlockLoadProfileThreePhaseCT> blockLoadProfileThreePhaseCTList = new List<BlockLoadProfileThreePhaseCT>();

            foreach (var blockLoadProfileThreePhaseCTDto in blockLoadProfileThreePhaseCTDtoList)
            {
                BlockLoadProfileThreePhaseCT blockLoadProfileThreePhaseCT = new BlockLoadProfileThreePhaseCT();

                blockLoadProfileThreePhaseCT.MeterNo = blockLoadProfileThreePhaseCTDto.MeterNo;
                blockLoadProfileThreePhaseCT.CreatedOn = blockLoadProfileThreePhaseCTDto.CreatedOn;
                blockLoadProfileThreePhaseCT.RealTimeClock = blockLoadProfileThreePhaseCTDto.RealTimeClock;
                blockLoadProfileThreePhaseCT.CurrentR = blockLoadProfileThreePhaseCTDto.CurrentR;
                blockLoadProfileThreePhaseCT.CurrentY = blockLoadProfileThreePhaseCTDto.CurrentY;
                blockLoadProfileThreePhaseCT.CurrentB = blockLoadProfileThreePhaseCTDto.CurrentB;
                blockLoadProfileThreePhaseCT.VoltageR = blockLoadProfileThreePhaseCTDto.VoltageR;
                blockLoadProfileThreePhaseCT.VoltageY = blockLoadProfileThreePhaseCTDto.VoltageY;
                blockLoadProfileThreePhaseCT.VoltageB = blockLoadProfileThreePhaseCTDto.VoltageB;

                blockLoadProfileThreePhaseCT.BlockEnergykWhImport = blockLoadProfileThreePhaseCTDto.BlockEnergykWhImport;
                blockLoadProfileThreePhaseCT.BlockEnergykVAhImport = blockLoadProfileThreePhaseCTDto.BlockEnergykVAhImport;
                blockLoadProfileThreePhaseCT.BlockEnergykWhExport = blockLoadProfileThreePhaseCTDto.BlockEnergykWhExport;
                blockLoadProfileThreePhaseCT.BlockEnergykVAhExport = blockLoadProfileThreePhaseCTDto.BlockEnergykVAhExport;
                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ1 = blockLoadProfileThreePhaseCTDto.BlockEnergykVArhQ1;
                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ2 = blockLoadProfileThreePhaseCTDto.BlockEnergykVArhQ2;
                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ3 = blockLoadProfileThreePhaseCTDto.BlockEnergykVArhQ3;
                blockLoadProfileThreePhaseCT.BlockEnergykVArhQ4 = blockLoadProfileThreePhaseCTDto.BlockEnergykVArhQ4;




                blockLoadProfileThreePhaseCTList.Add(blockLoadProfileThreePhaseCT);
            }



            return blockLoadProfileThreePhaseCTList;
        }

        private async Task<List<BlockLoadProfileThreePhaseCTDto>> ParseDataToDTO(List<BlockLoadProfileThreePhaseCT> BlockLoadProfileThreePhaseList)
        {
            int index = 1;
            List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseDtoList = new List<BlockLoadProfileThreePhaseCTDto>();
            foreach (var BlockLoadProfileThreePhase in BlockLoadProfileThreePhaseList)
            {
                BlockLoadProfileThreePhaseCTDto BlockLoadProfileThreePhaseDto = new BlockLoadProfileThreePhaseCTDto();

                BlockLoadProfileThreePhaseDto.Number = index;
                BlockLoadProfileThreePhaseDto.MeterNo = BlockLoadProfileThreePhase.MeterNo;
                BlockLoadProfileThreePhaseDto.CreatedOn = BlockLoadProfileThreePhase.CreatedOn;
                BlockLoadProfileThreePhaseDto.RealTimeClock = BlockLoadProfileThreePhase.RealTimeClock;
                BlockLoadProfileThreePhaseDto.CurrentR = (double.Parse(BlockLoadProfileThreePhase.CurrentR, NumberStyles.Any) / 1) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentR, NumberStyles.Any) / 1).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.CurrentY = (double.Parse(BlockLoadProfileThreePhase.CurrentY, NumberStyles.Any) / 1) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentY, NumberStyles.Any) / 1).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.CurrentB = (double.Parse(BlockLoadProfileThreePhase.CurrentB, NumberStyles.Any) / 1) == 0 ? "0" : (double.Parse(BlockLoadProfileThreePhase.CurrentB, NumberStyles.Any) / 1).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.VoltageR = (double.Parse(BlockLoadProfileThreePhase.VoltageR, NumberStyles.Any) / 1).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.VoltageY = (double.Parse(BlockLoadProfileThreePhase.VoltageY, NumberStyles.Any) / 1).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.VoltageB = (double.Parse(BlockLoadProfileThreePhase.VoltageB, NumberStyles.Any) / 1).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykWhImport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykWhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVAhImport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVAhImport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykWhExport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykWhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVAhExport = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVAhExport, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                BlockLoadProfileThreePhaseDto.BlockEnergykVArhQ1 = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVArhQ1, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVArhQ2 = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVArhQ2, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVArhQ3 = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVArhQ3, NumberStyles.Any) / 1000).ToString().CustomTrucate();
                BlockLoadProfileThreePhaseDto.BlockEnergykVArhQ4 = (double.Parse(BlockLoadProfileThreePhase.BlockEnergykVArhQ4, NumberStyles.Any) / 1000).ToString().CustomTrucate();

                BlockLoadProfileThreePhaseDto.MeterHealthIndicator = BlockLoadProfileThreePhase.MeterHealthIndicator;


                BlockLoadProfileThreePhaseDtoList.Add(BlockLoadProfileThreePhaseDto);
                index++;
            }

            return BlockLoadProfileThreePhaseDtoList;
        }
    }
}
