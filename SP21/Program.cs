using System;

namespace SP21
{
    internal class Program
    {
        private static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.CursorVisible = false;

            var scoreActivity = new ScoreActivity();

            while (scoreActivity.Show())
            {
                new GameActivity().Show();
            }
        }
    }
}