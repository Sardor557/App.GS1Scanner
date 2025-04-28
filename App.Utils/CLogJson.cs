using System.Security;

namespace App.Utils
{
    [SecuritySafeCritical]
    public class CLogJson
    {
        private static readonly object obj = new object();

        private static string FileName
        {
            get
            {
                var directoryPath = "";
                if (Environment.UserInteractive)
                    directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                else
                {
                    try
                    {
                        directoryPath = Directory.GetCurrentDirectory();
                    }
                    catch (Exception)
                    {
                        directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                    }
                }
                return directoryPath + "\\LogJson.txt";
            }
        }

        public static void Write(Exception e, string app = "", string param = "", string method = "")
        {
            var li = new LogItem
            {
                App = app,
                Stacktrace = e.GetStackTrace(5),
                Message = e.GetAllMessages(),
                Params = param,
                Method = method,
                Mestype = "e"
            };
            Write(li);
        }

        public static void Write(LogItem li)
        {
            lock (obj)
            {
                if (li.Mestype == "e")
                {
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine(li.Message);
                    Console.WriteLine("");
                    Console.WriteLine(li.Stacktrace);
                    Console.WriteLine("---------------------------------------------------");
                }


                li.Serveraddress = $"IP - {CNet.LocalIpAddressAll()} MAC - {CNet.GetMAC()}";
                li.DivisionName = Environment.GetEnvironmentVariable("DivisionName");
                li.Username = Environment.GetEnvironmentVariable("AppUser");
                li.AppVerion = Environment.GetEnvironmentVariable("AppVerion");
                li.CreateDate = DateTime.Now;
                li.Os = Environment.OSVersion.VersionString;
                li.Status = 1;
                li.Mestype = "e";


                var sr = !File.Exists(FileName) ? File.CreateText(FileName) : File.AppendText(FileName);
                sr.Write(li.ToString() + ',');
                sr.Close();


            }
        }
    }

    public class LogItem
    {
        public DateTime CreateDate { get; set; }
        public string App { get; set; }
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        public string Mestype { get; set; }
        public string Method { get; set; }
        public string Params { get; set; }
        public string Username { get; set; }
        public string Url { get; set; }
        public string Serveraddress { get; set; }
        public string Remoteaddress { get; set; }
        public string Useragent { get; set; }
        public string DivisionName { get; set; }

        public string AppVerion { get; set; }
        public string Os { get; set; }
        public int Status { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}