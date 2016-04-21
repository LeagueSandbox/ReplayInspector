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

            var file = ReplayFile.Open(args[0]);

            if (file == null)
                return;

            var reader = new PacketReader(file.GetReplayStream(), file.encryptionKey);
            if (!reader.loaded)
                return;

            reader.writeToFile(args[1]);
        }
    }
}
