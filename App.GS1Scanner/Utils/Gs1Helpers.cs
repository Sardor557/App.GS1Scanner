namespace App.GS1Scanner.Utils
{
    public static class Gs1Helpers
    {
        private const char GS = '\u001d';   // ASCII 29
        public static string TrimLeadingGs(this string code)
            => string.IsNullOrEmpty(code) || code[0] != GS
               ? code
               : code.Substring(1);
    }
}
