using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exdchar
{
    static class SuperConsole
    {
        private static char[,] BackBuffer, CurrentBuffer;
        private static int X, Y;

        public static void Initialize()
        {
            BackBuffer = new char[Console.WindowWidth, Console.WindowHeight];
            CurrentBuffer = new char[Console.WindowWidth, Console.WindowHeight];
            X = Y = 0;
        }

        public static void SetCursorPosition(int left, int top)
        {
            X = left; Y = top;
        }

        public static void Write(char c)
        {
            if (X >= 0 && X < CurrentBuffer.GetLength(0) && Y >= 0 && Y < CurrentBuffer.GetLength(1))
            {
                CurrentBuffer[X, Y] = c;
                if (++X >= CurrentBuffer.GetLength(0))
                {
                    X = 0;
                    ++Y;
                }
            }
        }

        public static void Write(string s)
        {
            foreach (var ch in s)
                Write(ch);
        }

        public static void WriteLine()
        {
            X = 0; ++Y;
        }

        public static void FinishFrame()
        {
            var xend = CurrentBuffer.GetLength(0);
            var yend = CurrentBuffer.GetLength(1);

            bool lastmoved = false;

            for (int y = 0; y < yend; ++y)
                for (int x = 0; x < xend; ++x)
                    if (CurrentBuffer[x, y] != BackBuffer[x, y])
                        if (lastmoved)
                        {
                            Console.Write(CurrentBuffer[x, y]);
                        }
                        else
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(CurrentBuffer[x, y]);
                            lastmoved = true;
                        }
                    else
                        lastmoved = false;

            BackBuffer = CurrentBuffer;
            CurrentBuffer = new char[Console.WindowWidth, Console.WindowHeight];
        }
    }
}
