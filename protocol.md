# Overview of the Sony SDAP/SDCP protocol and VMC commands

There are a number of programmer's manuals that cover various monitor models and how to control them through Ethernet.
This document attempts to summarise everything as well as provide examples of an existing monitor (Sony LMD-2451MD).

The following sources were used:
* Interface manual for programmers - Sony LMD-1951MD
* Interface manual for programmers - Sony PVM-740
* https://immerhax.com/?p=797

## Introduction to protocol



## SDAP
According to the Sony manuals, SDAP is the  Simple Display Advertisement Protocol.
When connected to the network, monitors will broadcast a UDP multicast packet on port **53682** every 15 seconds.

### Overview
```
+----------+-------------------------------------------------------------------+
+ char[2]  + Fixed string, always "DA"                                         + 0x44 0x41 +
+----------+-------------------------------------------------------------------+
+ char     + Protocol version, always seems to be 0x04 according to the manual + 0x04
+----------+-------------------------------------------------------------------+
+ char     + Type of device: 0x0b if monitor, 0x0c if controller               + 0x0b
+----------+-------------------------------------------------------------------+
+ char[4]  + Community: Fixed string, always "SONY"                            + 0x53 0x4f 0x4e 0x59
+----------+-------------------------------------------------------------------+
+ char[12] + Product name: 12-char string, null padded (but not terminated)    + 0x4c 0x4d 0x44 0x33 
                                                                                 0x32 0x35 0x30 0x4d
                                                                                 0x44 0x00 0x00 0x00
                                                                                 (LMD3250MD)
+----------+-------------------------------------------------------------------+
+ int      + Serial number                                                     + 0x00 0x2f 0x68 0x1e
+
+ int      + Connection IP (v4 only)                                           +
+
+ int[4]   + Acceptable IPs (v4 only, not used?)                               +
+
+ short    + Error (possibly not used for monitors)                            +
+
+ char[24] + Region (possibly not used)                                        +
+
+ char[24] + Name (possibly not used)                                          +
+
+ char     + Group ID, set in the monitor config                               + 0x01
+
+ char     + Unit ID, set in the monitor config                                +


```

### Example
```
0000   44 41 04 0b 53 4f 4e 59 4c 4d 44 33 32 35 30 4d   DA..SONYLMD3250M
0010   44 00 00 00 00 2f 68 1e 00 00 00 00 00 00 00 00   D..../h.........
0020   00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   ................
0030   00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   ................
0040   00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   ................
0050   00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   ................
0060   00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   ................
0070   00 00 00 00 00 00 00 00 01 01                     ..........
```

