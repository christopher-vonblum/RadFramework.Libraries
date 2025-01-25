namespace RadFramework.Libraries.Utils;

public static class ByteUtil
{
    public static byte[] GenerateRandomToken(int length)
    {
        byte[] tokenBytes = new byte[length];
        Random.Shared.NextBytes(tokenBytes);
        return tokenBytes;
    }
}