using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ReplayParser
{
    public class Packet
    {
        public float Time;
        public int Length;
        public byte[] Bytes;

        public Packet()
        {
        }

        public Packet(BinaryReader file)
        {
            Time = file.ReadSingle();
            Length = file.ReadInt32();

            if (file.BaseStream.Position + Length > file.BaseStream.Length)
            {
                file.ReadByte(); //unk
                Time = file.ReadSingle();
                Length = file.ReadInt32();
            }

            Bytes = file.ReadBytes(Length);
            file.ReadByte(); //unk
        }
    }
}
