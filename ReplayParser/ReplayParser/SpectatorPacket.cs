using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReplayParser
{
    public class SpectatorPacket
    {
        public byte flags;
        public byte channel;
        public int length;
        public float time;
        public byte packetType;
        public int blockparam;
        public byte[] packetData;

        public SpectatorPacket(BinaryReader reader, byte lastPacketType =0, float lastTime =0, int lastBlockparam =0)
        {
            byte marker = reader.ReadByte();
            flags = (byte) (marker >> 4);
            channel = (byte) (marker & 0x0F);
            if((flags & 0x8) == 0)
            {
                time = reader.ReadSingle();
            }
            else
            {
                time = reader.ReadByte()/1000 + lastTime;
            }
            if ((flags & 0x1) == 0)
            {
                length = reader.ReadInt32();
            }
            else
            {
                length = reader.ReadByte();
            }
            if ((flags & 0x4) == 0)
            {
                packetType = reader.ReadByte();
            }
            else
            {
                packetType = lastPacketType;
            }
            if ((flags & 0x2) == 0)
            {
                blockparam = reader.ReadInt32();
            }
            else
            {
                blockparam = reader.ReadByte()+ lastBlockparam;
            }
            packetData = reader.ReadBytes(length);
        }


    }
}
