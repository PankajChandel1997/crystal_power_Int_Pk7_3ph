using Gurux.DLMS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.ViewModels
{
    public class MeterConnectionViewModel
    {
        public string Port { get; set; }
        public int SerialConnected { get; set; } = 0;
        public InterfaceType InterfaceTypeisWrapper { get; set; }
        public string DeviceId { get; set; } = "256";
        public int ServerAddress { get; set; }
        public bool IsConnected { get; set; }
        public bool IsInit { get; set; }
    }
}
