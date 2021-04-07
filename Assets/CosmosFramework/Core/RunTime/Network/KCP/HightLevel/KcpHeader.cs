namespace kcp 
{
    public enum KcpHeader : byte
    {
        // don't react on 0x00. might help to filter out random noise.
        Handshake = 0x01,
        // ping goes over reliable & KcpHeader for now. could go over reliable
        // too. there is no real difference except that this is easier because
        // we already have a KcpHeader for reliable messages.
        // ping is only used to keep it alive, so latency doesn't matter.
        Ping = 0x02,
        Data = 0x03,
        Disconnect = 0x04
    }
}