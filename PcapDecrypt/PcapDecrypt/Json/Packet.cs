namespace PcapDecrypt.Json
{
    public class Packet
    {
        public float Time { get; set; }
        public int Length { get; set; }
        public byte[] Bytes { get; set; }
    }
}
