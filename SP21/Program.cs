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
            var gameActivity = new GameActivity();

            while (scoreActivity.Show(gameActivity.Score))
            {
                gameActivity.Show();
            }
        }
    }
}