using Infrastructure.API;
using Infrastructure.DTOs;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.DbFunctions
{
    public class SelfDiagnosticCommand
    {
        private readonly SelfDiagnosticService _selfDiagnosticService;
        public ErrorHelper _errorHelper;

        public SelfDiagnosticCommand()
        {
            _selfDiagnosticService = new SelfDiagnosticService();
            _errorHelper = new ErrorHelper();
        }

        public async Task<List<SelfDiagnosticDto>> GetAll(int pageSize, string meterNo)
        {
            List<SelfDiagnosticDto> meter = new List<SelfDiagnosticDto>();
            try
            {
                meter = await _selfDiagnosticService.GetAll(pageSize, meterNo);
            }
            catch (Exception)
            {

            }
            return meter;
        }

        public async Task<List<SelfDiagnosticDto>> Filter(string startDate, string endDate, int PageSize, string meterNumber)
        {
            List<SelfDiagnosticDto> selfDiagnostic = new List<SelfDiagnosticDto>();
            try
            {
                selfDiagnostic = await _selfDiagnosticService.Filter(startDate, endDate, PageSize, meterNumber);
            }
            catch (Exception)
            {

            }
            return selfDiagnostic;
        }
    }
}
