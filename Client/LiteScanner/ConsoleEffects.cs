using System;
using System.IO;

namespace LiteScanner
{
    public static class ConsoleEffects
    {
        static readonly object locker = new object();

        public static int lastline_num = 20;
        static readonly string logo = @"
    __    _ __      _____                                 
   / /   (_) /____ / ___/_________ _____  ____  ___  _____
  / /   / / __/ _ \\__ \/ ___/ __ `/ __ \/ __ \/ _ \/ ___/
 / /___/ / /_/  __/__/ / /__/ /_/ / / / / / / /  __/ /    
/_____/_/\__/\___/____/\___/\__,_/_/ /_/_/ /_/\___/_/     
                                                          
                                   
";

        public static string GetLogo(int padding_size)
        {
            string formatedLine = String.Empty;
            string padding = String.Empty;

            for(int i = 0; i < padding_size; i++) padding += "    ";

            using (StringReader reader = new StringReader(logo))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    formatedLine += padding + line + System.Environment.NewLine;
                }
            }
            return formatedLine;
        }


        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static int ConsolePrintColor(string txt, ConsoleColor color = ConsoleColor.White, bool ErasePrintAndGoBack = false, bool addLinesAfter = false, int startingLine = 0)
        {
            lock (locker)
            {
                bool darkmode = true;
                if (color == ConsoleColor.Green || color == ConsoleColor.Cyan || color == ConsoleColor.Yellow) darkmode = false;

                Console.ForegroundColor = ConsoleColor.White;

                // if background is brigh, let's make text dark
                if (!darkmode) Console.ForegroundColor = ConsoleColor.Black;

                if (color != ConsoleColor.White) Console.BackgroundColor = color;

                int cursorTopBefore = Console.CursorTop;
                int cursorTop = Console.CursorTop;
                if (startingLine != 0) 
                    cursorTop = startingLine;

                if (ErasePrintAndGoBack)
                {
                    Console.SetCursorPosition(0, cursorTop);
                    ClearCurrentConsoleLine();
                }              


                Console.WriteLine(txt);


                if (color != ConsoleColor.White) Console.ResetColor();
                
                if (ErasePrintAndGoBack) Console.SetCursorPosition(0, cursorTopBefore);

                if (addLinesAfter) Console.SetCursorPosition(0, cursorTopBefore + 2);

                return cursorTop;

            }
        }

        static string FormatTXT(string s1, string s2, int maxContentLength = 30, int maxFullLength = 39)
        {
            if (s1.Length > maxContentLength) s1 = s1.Substring(0, 30) + "...";
            if (s2.Length > maxContentLength) s2 = s2.Substring(0, 30) + "...";

            while (s1.Length < maxFullLength) s1 += " ";
            while (s2.Length < maxFullLength) s2 += " ";

            return $"{s1} {s2} ";
        }

    }
}
