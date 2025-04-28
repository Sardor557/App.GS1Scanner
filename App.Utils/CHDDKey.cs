using System.Runtime.InteropServices;

namespace App.Utils
{
    public class CHDDKey
    {
        [DllImport("HddKey.dll", EntryPoint = "GetAcKey", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern nint GetAcKey();

        public static string GetHDDSerialNo()
        {
            nint key = GetAcKey();
            string sresult = Marshal.PtrToStringAnsi(key);
            return sresult;
        }
    }
}