using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispCtrl.Protocols.SdapSdcp.Sdcp
{
    internal class SdcpPacket
    {
        public const int DefaultVersion = 3;

        public byte Version { get; set; }
        public byte GroupId { get; set; }
        public byte UnitId { get; set; }
        public byte RequestResponse { get; set; }
        public MonitorCommandKind ItemNo { get; set; }
        public byte[] Data { get; set; }
    }
}
