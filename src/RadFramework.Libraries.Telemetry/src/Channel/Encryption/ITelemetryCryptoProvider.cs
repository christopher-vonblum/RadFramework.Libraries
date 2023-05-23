namespace RadFramework.Libraries.Telemetry.Encryption
{
    public interface ITelemetryCryptoProvider
    {
        byte[] Encrypt(byte[] package);
        byte[] Decrypt(byte[] encryptedPackage);
    }
}