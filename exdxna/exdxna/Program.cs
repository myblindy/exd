using System;

namespace exdxna
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ExdGame game = new ExdGame())
            {
                game.Run();
            }
        }
    }
#endif
}

