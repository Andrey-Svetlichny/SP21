using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SP21
{
    public class Level
    {
        StringBuilder[] _lines;

        /// <summary>
        /// Точка входа в дом.
        /// </summary>
        public readonly Coordinate.Point GateIn = new Coordinate.Point { X = 39, Y = 9 };

        /// <summary>
        /// Направление входа в дом.
        /// </summary>
        public const Coordinate.Direction GateInDirection = Coordinate.Direction.Down;


        /// <summary>
        /// Точка выхода из дома
        /// </summary>
        public readonly Coordinate.Point GateOut = new Coordinate.Point { X = 39, Y = 11 };

        /// <summary>
        /// Направление выхода из дома.
        /// </summary>
        public const Coordinate.Direction GateOutDirection = Coordinate.Direction.Up;

        /// <summary>
        /// Начальное положение мыши.
        /// </summary>
        public readonly Coordinate.Point InitialMouseCoordinate = new Coordinate.Point { X = 39, Y = 16 };

        /// <summary>
        /// Начальное положение кошек.
        /// </summary>
        public readonly Coordinate.Point[] InitialCatsCoordinate =
            {
                new Coordinate.Point {X = 37, Y = 11},
                new Coordinate.Point {X = 41, Y = 11},
                new Coordinate.Point {X = 37, Y = 12},
                new Coordinate.Point {X = 41, Y = 12},
            };

        /// <summary>
        /// Время действия озверина на текущем уровне.
        /// </summary>
        public const int OzverinTime = 144;


        public Level(int levelNum)
        {
            Load(levelNum);
        }

        public int Breadcrumbs { get; private set; }

        private void Load(int level)
        {
            Debug.Assert(level > 0);
            const string resLevel = "SP21.Levels.Level{0}.txt";
            var n = (level - 1) % 3 + 1;

            string text;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(string.Format(resLevel, n)))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            _lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Select(l => new StringBuilder(l.PadRight(80))).ToArray();
            
            Breadcrumbs = text.Count(f => f == '.');
        }

        public void EatBreadcrumb()
        {
            Breadcrumbs--;
        }

        private char Get(Coordinate.Point point)
        {
            return _lines[point.Y][point.X];
        }

        public void Set(Coordinate.Point point, char c)
        {
            _lines[point.Y][point.X] = c;
        }

        public IEnumerable<char> Get(Coordinate.Point point, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var ret = Get(point);
                point.X++;
                yield return ret;
            }
        }

        public string Read(Coordinate.Point point, int count)
        {
            return new string(Get(point, count).ToArray());
        }

        public void Write(Coordinate.Point point, string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                Set(point, s[i]);
                point.X++;
            }
        }

        public void Draw(Coordinate.Point point, string s)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(s);
        }

        public void Draw(Coordinate.Point point, int length)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(Read(point, length));
        }

        public void Draw()
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(_lines[i]);
            }
        }
    }
}