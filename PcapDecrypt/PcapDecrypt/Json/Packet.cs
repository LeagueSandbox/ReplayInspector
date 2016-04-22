using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDecrypt.Json
{
    public class Packet
    {
        public float Time { get; set; }
        public int Length { get; set; }
        public byte[] Bytes { get; set; }
    }
}
