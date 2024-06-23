namespace DispCtrl.Protocols.SdapSdcp.Sdcp;

internal enum MonitorCommandKind : ushort
{
    Unknown = 0,
    Monitor = 0xb000,
    MonitorController = 0xb001
}