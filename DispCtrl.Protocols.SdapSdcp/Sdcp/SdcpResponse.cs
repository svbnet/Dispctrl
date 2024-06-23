using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispCtrl.Protocols.SdapSdcp.Sdcp
{
    public class SdcpResponse
    {
        public bool Success { get; set; }

        public byte[] Data { get; set; }
    }
}
