using System.Text;

namespace App.Utils.Compress
{
    public class CLzo
    {
        public static byte[] Compress(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return bytes.Decompress();
        }

        public static byte[] Compress<T>(T obj)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return Compress(str);
        }

        public static string Decompress(byte[] bytes)
        {
            var ba = bytes.Decompress();
            return Encoding.UTF8.GetString(ba);
        }

        public static T Decompress<T>(Stream msi)
        {
            using (var mso = new MemoryStream())
            {
                msi.CopyTo(mso);
                var ba = mso.ToArray();

                var s = Encoding.UTF8.GetString(ba.Decompress());
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            }
        }
        public static async Task<T> DecompressAsync<T>(Stream msi)
        {
            using (var mso = new MemoryStream())
            {
                await msi.CopyToAsync(mso);
                var ba = mso.ToArray();

                var s = Encoding.UTF8.GetString(ba.Decompress());
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            }
        }

        public static T Decompress<T>(byte[] bytes)
        {
            var ba = bytes.Decompress();
            var s = Encoding.UTF8.GetString(ba);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
        }
    }
}
