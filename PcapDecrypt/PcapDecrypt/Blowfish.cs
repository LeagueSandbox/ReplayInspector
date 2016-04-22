//Blowfish encryption (ECB and CBC MODE) as defined by Bruce Schneier here: http://www.schneier.com/paper-blowfish-fse.html
//Complies with test vectors found here: http://www.schneier.com/code/vectors.txt
//non-standard mode profided to be usable with the javascript crypto library found here: http://etherhack.co.uk/symmetric/blowfish/blowfish.html
//By FireXware, 1/7/1010, Contact: firexware@hotmail.com
//Code is partly adopted from the javascript crypto library by Daniel Rench

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace BlowFishCS
{
    public unsafe class BlowFishCS
    {
        private const string LIB = "lib/libintlib.dll";
        [DllImport(LIB)]
        public static extern BlowFish* BlowFishCreate(byte* ucKey, IntPtr n);
        [DllImport(LIB)]
        public static extern void Encrypt1(BlowFish* handle, byte* buf, IntPtr n, int iMode = (int)BlowfishMode.ECB);

        [DllImport(LIB)]
        public static extern void Decrypt1(BlowFish* handle, byte* buf, IntPtr n, int iMode = (int)BlowfishMode.ECB);
        [DllImport(LIB)]

        public static extern void DestroyHandle(void* handle);
        [DllImport(LIB)]
        public static extern ulong Encrypt2(void* handle, ulong buf);
        [DllImport(LIB)]
        public static extern ulong Decrypt2(void* handle, ulong buf);
        [DllImport(LIB)]
        public static extern void Encrypt3(void* handle, byte* @in, byte* @out, IntPtr n, int iMode = (int)BlowfishMode.ECB);
        [DllImport(LIB)]
        public static extern void Decrypt3(void* handle, byte* @in, byte* @out, IntPtr n, int iMode = (int)BlowfishMode.ECB);
    }

    public struct BlowFish
    {

    }

    public enum BlowfishMode
    {
        ECB = 0,
        CBC = 1,
        CFB = 2
    }
}

