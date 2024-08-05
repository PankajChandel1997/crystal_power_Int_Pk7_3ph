using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class MeterDto
    {
       // public string CreatedOn { get; set; }
        public string MeterNumber { get; set; }
        public string DeviceId { get; set; }
        public string ManufacturerName { get; set; }
        public string FirmwareVersion { get; set; }
        public MeterType MeterType { get; set; }
        public string Category { get; set; }
        public string CurrentRating { get; set; }
        public string MeterYearManufacturer { get; set; }
        public string CTRatio { get; set; }
        public string PTRatio { get; set; }
        public string ManSpecificFirmwareVersion { get; set; }

        //Consumer Details
        public string ConsumerNo { get; set; }
        public string ConsumerName { get; set; }
        public string ConsumerAddress { get; set; }
    }
}
