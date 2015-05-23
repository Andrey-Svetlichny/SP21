using System;

namespace SP21
{
    internal class ScoreActivity
    {
        /// <summary>
        /// Показывает таблицу рекордов.
        /// </summary>
        /// <returns>Начинать игру.</returns>
        public bool Show()
        {
            Console.Clear();
            Console.SetCursorPosition(36, 11);
            Console.WriteLine("Begin ?");
            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.N)
                {
                    return false;
                }

                if (keyInfo.Key == ConsoleKey.Y)
                {
                    return true;
                }
            }
        }
    }
}