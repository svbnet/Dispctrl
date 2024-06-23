using System.Net;

namespace DispCtrl.Protocols.SdapSdcp.Sdap;

public class SdapDevice
{
    public IPAddress IpAddress { get; set; }
    public int Version { get; set; }
    public SdapDeviceCategory Category { get; set; }
    public string Community { get; set; }
    public string ProductName { get; set; }
    public int SerialNumber { get; set; }
    public string PowerStatus { get; set; }
    public string Location { get; set; }
    public IPAddress? ConnectionIp { get; set; }
    public IPAddress?[] AcceptableIps { get; set; }
    public int? Error { get; set; }
    public string? Region { get; set; }
    public string? Name { get; set; }
    public int? GroupId { get; set; }
    public int? UnitId { get; set; }
}