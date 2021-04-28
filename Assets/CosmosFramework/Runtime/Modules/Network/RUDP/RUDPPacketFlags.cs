using System;

namespace RUDP
{
    [Flags]
    public enum RUDPPacketFlags:byte
    {
        NUL = 0,
        ACK,
        RST
    }
}