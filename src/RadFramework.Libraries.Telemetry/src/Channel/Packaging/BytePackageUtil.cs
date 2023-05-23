using System.IO;
using System.Linq;

namespace Tests
{
    public static class BytePackageUtil
    {
        public static void WritePackage(Stream stream, byte[] package)
        {
            int len = package.Length;

            if (len > ushort.MaxValue)
            {
                len = ushort.MaxValue;
            }

            byte[] packageWithHeader = new byte[] 
                    { (byte)(len / 256), (byte)(len & 255) }
                .Concat(package)
                .ToArray();
                    
            stream.Write(packageWithHeader, 0, packageWithHeader.Length);
            stream.Flush();
        }
            
        public static byte[] ReadPackage(Stream stream)
        {
            byte[] inBuffer;
            int len = 0;
            len = stream.ReadByte() * 256;
            len += stream.ReadByte();

            if (len <= 0)
            {
                return null;
            }

            inBuffer = new byte[len];
            stream.Read(inBuffer, 0, len);

            return inBuffer;
        }
    }
}