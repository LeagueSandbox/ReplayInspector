using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ReplayParser
{
    public class Compression
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr LocalAlloc(int uFlags, IntPtr sizetdwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("lib/Compression.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr compress([In] byte[] src, int srcLength, ref int destLength);

        [DllImport("lib/Compression.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr decompress([In] byte[] src, int srcLength, ref int destLength);

        [DllImport("lib/Compression.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void free_mem();

        public static byte[] Compress(byte[] source)
        {
            var num = IntPtr.Zero;
            try
            {
                int destLength = 0;
                var source1 = compress(source, source.Length, ref destLength);

                System.Diagnostics.Debug.WriteLine("Compress: " + destLength);

                if (destLength > 0)
                {
                    if (source1 != IntPtr.Zero)
                    {
                        var destination = new byte[destLength];
                        Marshal.Copy(source1, destination, 0, destLength);
                        return destination;
                    }
                }
            }
            finally
            {
                free_mem();
            }
            return null;
        }

        public static byte[] Decompress(byte[] source)
        {
            var num = IntPtr.Zero;
            try
            {
                int destLength = 0;
                var source1 = decompress(source, source.Length, ref destLength);

                System.Diagnostics.Debug.WriteLine("Decompress: " + destLength);

                if (destLength > 0)
                {
                    if (source1 != IntPtr.Zero)
                    {
                        var destination = new byte[destLength];
                        Marshal.Copy(source1, destination, 0, destLength);
                        return destination;
                    }
                }
            }
            finally
            {
                free_mem();
            }

            return null;
        }

        public static void Compress(string inFile, string outFile)
        {
            var binaryReader = new BinaryReader(File.Open(inFile, FileMode.Open));
            var source = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
            binaryReader.Close();

            var buffer = Compression.Compress(source);
            if (buffer == null)
                return;

            var binaryWriter = new BinaryWriter(File.Open(outFile, FileMode.Create));
            binaryWriter.Write(buffer);
            binaryWriter.Close();
        }

        public static void Decompress(string inFile, string outFile)
        {
            var binaryReader = new BinaryReader(File.Open(inFile, FileMode.Open));
            var source = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
            binaryReader.Close();

            var buffer = Decompress(source);
            if (buffer == null)
                return;

            var binaryWriter = new BinaryWriter(File.Open(outFile, FileMode.Create));
            binaryWriter.Write(buffer);
            binaryWriter.Close();
        }
    }
}
