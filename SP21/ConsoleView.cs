using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SP21.Animals;

namespace SP21
{
    public class ConsoleView
    {
        public ConsoleView()
        {
            // Console setup
            Console.ForegroundColor = ConsoleColor.Green;
            Console.CursorVisible = false;
        }

        public void DrawScoreTable(ScoreTable scoreTable)
        {
            Console.Clear();
            Console.WriteLine("      PAC-HALL");
            Console.WriteLine("--------------------");

            for (int i = 0; i < scoreTable.Scores.Count; i++)
            {
                bool isLastRow = i + 1 == scoreTable.Scores.Count;
                Console.WriteLine(".{0,2}.{1,-9} {2,6}",
                    isLastRow ? "NN" : $"{i + 1:00}",
                    isLastRow ? "*********" : scoreTable.Scores[i].Name,
                    scoreTable.Scores[i].Value);
            }
            Console.SetCursorPosition(0, 24);
            Console.Write("SP21 Remake by Andrey Svetlichny");
        }


        public void DrawLevel(Level level)
        {
            for (int y = 0; y < Coordinate.MaxY; y++)
                for (int x = 0; x < Coordinate.MaxX; x++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(level[new Coordinate.Point(x, y)]);
                    Pause(1);
                }
            Console.SetCursorPosition(0, 24);
            Console.Write("                                ");
        }

        public void Draw(Coordinate.Point point, string s)
        {
            var cutLength = Coordinate.MaxX - point.X;
            if (s.Length > cutLength)
            {
                var s1 = s.Substring(0, cutLength);
                var s2 = s.Substring(cutLength);
                Draw(point, s1);
                Draw(new Coordinate.Point(0, point.Y + 1), s2);
                return;
            }
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(s);
        }

        public void Draw(Animal animal, Level level)
        {
            switch (animal.Dir)
            {
                case null:
                    Draw(animal.Coord.Copy(-1, 0), animal.Skin);
                    break;
                case Coordinate.Direction.Left:
                    Draw(animal.Coord.Copy(-1, 0), animal.Skin + level[animal.Coord.Copy(2, 0)]);
                    break;
                case Coordinate.Direction.Right:
                    Draw(animal.Coord.Copy(-2, 0), level[animal.Coord.Copy(-2, 0)] + animal.Skin);
                    break;
                case Coordinate.Direction.Up:
                    Draw(animal.Coord.Copy(-1, +1), level.Get(animal.Coord.Copy(-1, +1), 3));
                    Draw(animal.Coord.Copy(-1, 0), animal.Skin);
                    break;
                case Coordinate.Direction.Down:
                    Draw(animal.Coord.Copy(-1, -1), level.Get(animal.Coord.Copy(-1, -1), 3));
                    Draw(animal.Coord.Copy(-1, 0), animal.Skin);
                    break;
            }
            StepPause();
        }

        public void DrawNewLife(int lives)
        {
            Draw(new Coordinate.Point(79, lives - 2), "X");
        }

        public void DrawScore(int score)
        {
            Draw(new Coordinate.Point(36, 18), $"{score,6}");
        }

        public void MouseDieAnimation(Level level, List<Cat> cats, Mouse mouse)
        {
            const string skinDied1 = "$+$", skinDied2 = "+$+";
            for (int i = 0; i < 4; i++)
            {
                Draw(mouse.Coord.Copy(-1, 0), skinDied1);
                Pause(100);
                Draw(mouse.Coord.Copy(-1, 0), skinDied2);
                Pause(100);
            }
        }

        private void StepPause()
        {
            Pause(12);
        }

        private void Pause(int millisecondsTimeout)
        {
            var stopwatch = Stopwatch.StartNew();
            int remain;
            while ((remain = millisecondsTimeout - (int)stopwatch.ElapsedMilliseconds) > 0)
            {
                if (remain > 5)
                {
                    Thread.Sleep(remain - 5);
                }
            }
        }

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

        /// <summary>
        /// Ввод имени в таблицу рекордов.
        /// </summary>
        /// <param name="position">Позиция (номер строки)</param>
        /// <returns></returns>
        public static string EnterName(int position)
        {
            Console.SetCursorPosition(36, 11);
            Console.WriteLine("ENTER YOU NAME !");
            Console.SetCursorPosition(4, position + 2);
            Console.CursorVisible = true;
            var name = Console.ReadLine();
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
