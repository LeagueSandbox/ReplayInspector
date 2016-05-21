using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PcapDecrypt.Packets
{
    public class Packet
    {
        /*private*/
        public readonly byte[] Bytes;
        internal BinaryReader Reader;
        internal List<PacketField> Payload = new List<PacketField>();
        //private Dictionary<PacketCmdS2C, Packets> _packetAnalyzers = new Dictionary<PacketCmdS2C, Packets>();

        public Packet(byte[] Bytes)
        {
            Reader = new BinaryReader(new MemoryStream(Bytes));
            this.Bytes = Bytes;
        }

        internal byte readByte(string name)
        {
            byte b = 0;
            try
            {
                b = Reader.ReadByte();
            }
            catch { }
            byte[] arr = { 0 };
            arr[0] = b;
            Payload.Add(new PacketField("b", name, b, arr));
            return b;
        }
        internal short readShort(string name)
        {
            short s = 0;
            try
            {
                int size = sizeof(Int16);
                byte[] arr = Reader.ReadBytes(size);
                Reader.BaseStream.Position -= size;

                s = Reader.ReadInt16();

                Payload.Add(new PacketField("s", name, s, arr));
            }
            catch { }
            //Payload.Add(new PacketField("s", name, s, arr));
            return s;
        }
        internal int readInt(string name)
        {
            int i = 0;
            try
            {
                int size = sizeof(Int32);
                byte[] arr = Reader.ReadBytes(size);
                Reader.BaseStream.Position -= size;

                i = Reader.ReadInt32();

                Payload.Add(new PacketField("d", name, i, arr));
            }
            catch { }
            //Payload.Add(new PacketField("d", name, i));
            return i;
        }
        internal uint readUInt(string name)
        {
            uint ui = 0;
            try
            {
                int size = sizeof(UInt16);
                byte[] arr = Reader.ReadBytes(size);
                Reader.BaseStream.Position -= size;

                ui = Reader.ReadUInt32();

                Payload.Add(new PacketField("d+", name, ui, arr));
            }
            catch { }
            //Payload.Add(new PacketField("d+", name, ui));
            return ui;
        }
        internal long readLong(string name)
        {
            long l = 0;
            try
            {
                int size = sizeof(Int64);
                byte[] arr = Reader.ReadBytes(size);
                Reader.BaseStream.Position -= size;

                l = Reader.ReadInt64();

                Payload.Add(new PacketField("l", name, l, arr));
            }
            catch { }
            //Payload.Add(new PacketField("l", name, l));
            return l;
        }
        internal ulong readULong(string name)
        {
            ulong ul = 0;
            try
            {
                int size = sizeof(UInt64);
                byte[] arr = Reader.ReadBytes(size);
                Reader.BaseStream.Position -= size;

                ul = Reader.ReadUInt64();

                Payload.Add(new PacketField("ul", name, ul, arr));
            }
            catch { }
            //Payload.Add(new PacketField("l", name, l));
            return ul;
        }
        internal float readFloat(string name)
        {
            var f = float.NaN;
            try
            {
                int size = sizeof(float);
                byte[] arr = Reader.ReadBytes(size);
                Reader.BaseStream.Position -= size;

                f = Reader.ReadSingle();

                Payload.Add(new PacketField("f", name, f, arr));
            }
            catch { }
            //Payload.Add(new PacketField("f", name, f));
            return f;
        }
        internal byte[] readFill(int len, string name)
        {
            byte[] arr = new byte[len];
            try
            {
                arr = Reader.ReadBytes(len);
            }
            catch { }
            Payload.Add(new PacketField("fill", name, arr, arr));
            return arr;
        }
        internal string readString(int len, string name)
        {
            var buff = new List<byte>(len);
            try
            {
                for (var i = 0; i < len; i++)
                    buff.Add(Reader.ReadByte());
            }
            catch { }

            var s = Encoding.Default.GetString(buff.ToArray());
            Payload.Add(new PacketField("str", name, s, buff.ToArray()));
            return s;
        }

        internal string readZeroTerminatedString(string name)
        {
            var buff = new List<byte>();
            try
            {
                byte b = 0;
                do
                {
                    b = Reader.ReadByte();
                    buff.Add(b);
                } while (b != 0);
            }
            catch { }

            var s = Encoding.Default.GetString(buff.ToArray());
            Payload.Add(new PacketField("str", name, s, buff.ToArray()));
            return s;
        }
        internal void close()
        {
            if (Reader.BaseStream.Position < Reader.BaseStream.Length)
                readFill((int)(Reader.BaseStream.Length - Reader.BaseStream.Position), "unk(Not defined)");
            Reader.Close();
        }
        internal int getBufferLength()
        {
            return (int)Reader.BaseStream.Length;
        }

        internal bool isEnterVisionPacket()
        {
            if (Bytes[0] != (byte)PacketCmdS2C.PKT_S2C_ObjectSpawn)
                return false;

            bool isEnterVision = true;
            for (var i = 5; i < 18; i++)
                if (Bytes[i] != 0)
                    isEnterVision = false;
            isEnterVision = isEnterVision && BitConverter.ToSingle(Bytes.Skip(18).Take(4).ToArray(), 0) == 1.0f;
            for (var i = 22; i < 35; i++)
                if (Bytes[i] != 0)
                    isEnterVision = false;

            return isEnterVision;
        }

        internal bool isHeroSpawn()
        {
            if (Bytes[0] != (byte)PacketCmdS2C.PKT_S2C_HeroSpawn)
                return false;

            bool isHeroSpawn = true;
            for (int i = 5; i < 20; i++)
                if (Bytes[i] != 0)
                    isHeroSpawn = false;
            isHeroSpawn = isHeroSpawn && Bytes[20] == 0x80;
            isHeroSpawn = isHeroSpawn && Bytes[21] == 0x3F;
            for (int i = 22; i < 35; i++)
                if (Bytes[i] != 0)
                    isHeroSpawn = false;

            return isHeroSpawn;
        }
        internal bool isTeleport()
        {
            if (Bytes[0] != (byte)PacketCmdS2C.PKT_S2C_MoveAns)
                return false;

            return Bytes[9] == 0x01 && Bytes[10] == 0x00;
        }
    }

    public class PacketField
    {
        public string Type;
        public string Name;
        public object Payload;
        public byte[] Bytes;
        public int Length;

        public PacketField(string type, string name, object Payload, byte[] Bytes)
        {
            this.Type = type;
            this.Name = name;
            this.Payload = Payload;
            this.Bytes = Bytes;
            this.Length = Bytes.Length;
            /*if (Payload is string)
            {
                var str = Payload as string;
                this.Length = str.Length;
            }
            else if (Payload is byte[])
            {
                var arr = Payload as byte[];
                this.Length = arr.Length;
            }
            else
            {
                this.Length = Marshal.SizeOf(Payload.GetType());
            }*/
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
