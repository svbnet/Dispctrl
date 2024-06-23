# Dispctrl
Control Sony PVM and LMD monitors from your computer. Mostly made as a lower-level debugging aid.
Very much a WIP, as it has been tested on one monitor only (Sony LMD-2451MD).

## Resources and sources
Please see the [Resources](Resources) directory for interface manuals as well as the VMC commands.
Also credit to this [blog post](https://immerhax.com/?p=797) that gives an example of the traffic between an official Sony controller and monitor.

## Main application
Dispctrl is a C# .NET console application.
On startup, it will scan for 15 seconds detecting any UDP multicast packets from the monitor. Note it currently doesn't discard duplicate entries.
Then you can choose a monitor to connect to.
It will then connect to the monitor through TCP and you can issue a VMC command directly.

### Caveats and bugs
For some reason, the monitor will occasionally send back a truncated response. At minimum, and according to the manual, the response should be 13 bytes long.
More research will be needed.

## DispCtrl.Protocols.SdapSdcp
This is an implementation of the Sony SDAP/SDCP protocol in C#. It is not very well documented or tested.
