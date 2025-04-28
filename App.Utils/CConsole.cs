namespace App.Utils
{
    public static class CConsole
    {
        public static void WriteLine(this string txt, ConsoleColor color, int left = -1, int top = -1, int linecount = 1)
        {
            if (left != -1 && top != -1)
                Console.SetCursorPosition(20, 0);

            Console.ForegroundColor = color;
            Console.WriteLine(txt);
            Console.ResetColor();

            if (linecount > 1)
                Console.WriteLine();
        }
    }
}
