using System;

namespace SP21
{
    public class ConsoleInput
    {
        /// <summary>
        /// Желаемое направление движение мыши (нажата стрелка на клавиатуре).
        /// </summary>
        public static Coordinate.Direction? GetMouseDirectionFromKeyboard()
        {
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow:
                        return Coordinate.Direction.Left;
                    case ConsoleKey.RightArrow:
                        return Coordinate.Direction.Right;
                    case ConsoleKey.UpArrow:
                        return Coordinate.Direction.Up;
                    case ConsoleKey.DownArrow:
                        return Coordinate.Direction.Down;
                }
            }
            return null;
        }

        public static string EnterName(int position)
        {
            Console.SetCursorPosition(36, 11);
            Console.WriteLine("ENTER YOU NAME !");
            Console.SetCursorPosition(4, position + 2);
            Console.CursorVisible = true;
            var name = Console.ReadLine() ?? "";
            Console.CursorVisible = false;
            return name;
        }

        /// <summary>
        /// Если пользователь нажал Y, хочет играть.
        /// N - не хочет.
        /// </summary>
        /// <param name="isFirstRun"></param>
        public static bool UserWantsToPlay(bool isFirstRun)
        {
            Console.SetCursorPosition(36, 11);
            Console.WriteLine(isFirstRun ? "BEGIN ?" : "ONCE MORE ?     ");
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.N:
                        return false;
                    case ConsoleKey.Y:
                        return true;
                }
            }
        }
    }
}