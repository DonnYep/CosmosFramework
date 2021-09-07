using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace RUDP
{
    public class RUDPInternalPacket
    {
        public class ACKPacket
        {
            public byte[] header;
            public int sequence;

            public byte[] Serialize()
            {
                byte[] rv;
                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(header);
                    bw.Write(sequence);
                    rv = ms.ToArray();
                }
                return rv;
            }

            public static ACKPacket Deserialize(byte[] data)
            {
                ACKPacket rv = new ACKPacket();
                using (MemoryStream ms = new MemoryStream(data))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    rv.header = br.ReadBytes(4);
                    rv.sequence = br.ReadInt32();
                }
                return rv;
            }
        }
    }
}
