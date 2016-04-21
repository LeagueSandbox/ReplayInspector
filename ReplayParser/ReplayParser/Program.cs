using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayParser
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
                return;
            if (!args[0].ToLower().EndsWith(".lrf"))
                return;

            // this is overkill lmao
            var file = ReplayFile.Open(args[0]);

            if (file == null)
                return;

            var reader = new PacketReader(file.GetReplayStream());
            if (!reader.loaded)
                return;

            new PacketWriter(reader.getPackets(), file).writeJson(args[1]);
        }
    }
}
