namespace DispCtrl.Protocols.SdapSdcp.Sdap;

public class DeviceFoundEventArgs : EventArgs
{
    public SdapDevice Device { get; set; }
}