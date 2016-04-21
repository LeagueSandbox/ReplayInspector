using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ReplayParser
{
    public class PacketWriter
    {
        private List<byte[]> Packets;
        private ReplayFile Replay;

        public PacketWriter(List<byte[]> packets, ReplayFile replay)
        {
            Packets = packets;
            Replay = replay;
        }

        public void writeJson(string file)
        {
            var buffer = new Dictionary<string, object>();
        }
    }
}
