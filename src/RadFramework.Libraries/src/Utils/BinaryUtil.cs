namespace ZeroFormatter.Internal
{
    public static class BinaryUtil
    {
        static BinaryUtil()
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new Exception(
                    "Currently ZeroFormatter only supports Little-Endian environments. If you need supports, please report your envitonments to https://github.com/neuecc/ZeroFormatter/issues .");
            }
        }

        public static void EnsureCapacity(ref byte[] bytes, int offset, int appendLength)
        {
            var newLength = offset + appendLength;

            // If null(most case fisrt time) fill byte.
            if (bytes == null)
            {
                bytes = new byte[newLength];
                return;
            }

            // like MemoryStream.EnsureCapacity
            var current = bytes.Length;
            if (newLength > current)
            {
                int num = newLength;
                if (num < 256)
                {
                    num = 256;
                    FastResize(ref bytes, num);
                    return;
                }

                if (num < current * 2)
                {
                    num = current * 2;
                }

                FastResize(ref bytes, num);
            }
        }

        // Buffer.BlockCopy version of Array.Resize
        public static void FastResize(ref byte[] array, int newSize)
        {
            if (newSize < 0) throw new ArgumentOutOfRangeException("newSize");

            byte[] array2 = array;
            if (array2 == null)
            {
                array = new byte[newSize];
                return;
            }

            if (array2.Length != newSize)
            {
                byte[] array3 = new byte[newSize];
                Buffer.BlockCopy(array2, 0, array3, 0, (array2.Length > newSize) ? newSize : array2.Length);
                array = array3;
            }
        }
        
#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int WriteInt16(ref byte[] bytes, int offset, short value)
        {
            EnsureCapacity(ref bytes, offset, 2);

            fixed (byte* ptr = bytes)
            {
                *(short*)(ptr + offset) = value;
            }

            return 2;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe short ReadInt16(ref byte[] bytes, int offset)
        {
            fixed (byte* ptr = bytes)
            {
                return *(short*)(ptr + offset);
            }
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int WriteInt32(ref byte[] bytes, int offset, int value)
        {
            EnsureCapacity(ref bytes, offset, 4);

            fixed (byte* ptr = bytes)
            {
                *(int*)(ptr + offset) = value;
            }

            return 4;
        }

        /// <summary>
        /// Unsafe! don't ensure capacity and don't return size.
        /// </summary>
#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe void WriteInt32Unsafe(ref byte[] bytes, int offset, int value)
        {
            fixed (byte* ptr = bytes)
            {
                *(int*)(ptr + offset) = value;
            }
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int ReadInt32(ref byte[] bytes, int offset)
        {
            fixed (byte* ptr = bytes)
            {
                return *(int*)(ptr + offset);
            }
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int WriteInt64(ref byte[] bytes, int offset, long value)
        {
            EnsureCapacity(ref bytes, offset, 8);

            fixed (byte* ptr = bytes)
            {
                *(long*)(ptr + offset) = value;
            }

            return 8;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe long ReadInt64(ref byte[] bytes, int offset)
        {
            fixed (byte* ptr = bytes)
            {
                return *(long*)(ptr + offset);
            }
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int WriteUInt16(ref byte[] bytes, int offset, ushort value)
        {
            EnsureCapacity(ref bytes, offset, 2);

            fixed (byte* ptr = bytes)
            {
                *(ushort*)(ptr + offset) = value;
            }

            return 2;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe ushort ReadUInt16(ref byte[] bytes, int offset)
        {
            fixed (byte* ptr = bytes)
            {
                return *(ushort*)(ptr + offset);
            }
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int WriteUInt32(ref byte[] bytes, int offset, uint value)
        {
            EnsureCapacity(ref bytes, offset, 4);

            fixed (byte* ptr = bytes)
            {
                *(uint*)(ptr + offset) = value;
            }

            return 4;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe uint ReadUInt32(ref byte[] bytes, int offset)
        {
            fixed (byte* ptr = bytes)
            {
                return *(uint*)(ptr + offset);
            }
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe int WriteUInt64(ref byte[] bytes, int offset, ulong value)
        {
            EnsureCapacity(ref bytes, offset, 8);

            fixed (byte* ptr = bytes)
            {
                *(ulong*)(ptr + offset) = value;
            }

            return 8;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe ulong ReadUInt64(ref byte[] bytes, int offset)
        {
            fixed (byte* ptr = bytes)
            {
                return *(ulong*)(ptr + offset);
            }
        }
    }
}