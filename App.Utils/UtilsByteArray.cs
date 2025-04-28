namespace App.Utils
{
    public static class UtilsByteArray
    {
        public static ulong ToLong(this byte[] bigEndianBinary)
        {
            return (ulong)bigEndianBinary[0] << 56 |
                   (ulong)bigEndianBinary[1] << 48 |
                   (ulong)bigEndianBinary[2] << 40 |
                   (ulong)bigEndianBinary[3] << 32 |
                   (ulong)bigEndianBinary[4] << 24 |
                   (ulong)bigEndianBinary[5] << 16 |
                   (ulong)bigEndianBinary[6] << 8 |
                           bigEndianBinary[7];
        }
    }
}
