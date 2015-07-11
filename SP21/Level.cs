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
        const string ResLevel = "SP21.Levels.Level{0}.txt";

        StringBuilder[] _lines;

        public char this[Coordinate.Point p]
        {
            get
            {
                return _lines[p.Y][p.X];
            }
            set
            {
                _lines[p.Y][p.X] = value;
            }
        }

        /// <summary>
        /// Точка входа в дом.
        /// </summary>
        public readonly Coordinate.Point GateIn = new Coordinate.Point(39, 9);

        /// <summary>
        /// Направление входа в дом.
        /// </summary>
        public const Coordinate.Direction GateInDirection = Coordinate.Direction.Down;

        /// <summary>
        /// Точка выхода из дома
        /// </summary>
        public readonly Coordinate.Point GateOut = new Coordinate.Point(39, 11);

        /// <summary>
        /// Направление выхода из дома.
        /// </summary>
        public const Coordinate.Direction GateOutDirection = Coordinate.Direction.Up;

        /// <summary>
        /// Начальное положение мыши.
        /// </summary>
        public readonly Coordinate.Point InitialMouseCoordinate = new Coordinate.Point(39, 16);

        /// <summary>
        /// Начальное положение кошек.
        /// </summary>
        public readonly Coordinate.Point[] InitialCatsCoordinate =
            {
                new Coordinate.Point(37, 11),
                new Coordinate.Point(41, 11),
                new Coordinate.Point(37, 12),
                new Coordinate.Point(41, 12)
            };

        /// <summary>
        /// Время действия озверина на текущем уровне.
        /// </summary>
        public int OzverinTime { get; private set; }

        /// <summary>
        /// Количество оставшихся хлебных крошек.
        /// </summary>
        public int Breadcrumbs { get; private set; }

        public Level(int levelNum)
        {
            Load(levelNum);
            // ToDo уточнить
            OzverinTime = 180 - (levelNum -1) * 10;
        }

        private void Load(int level)
        {
            Debug.Assert(level > 0);
            var n = (level - 1) % 3 + 1;

            string text;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(string.Format(ResLevel, n)))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            _lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Select(l => new StringBuilder(l.PadRight(80))).ToArray();
            
            Breadcrumbs = text.Count(f => f == '.');
        }

        public char EatBreadcrumb(Coordinate.Point point)
        {
            var c = this[point];
            this[point] = ' ';
            if (c == '.')
            {
                Breadcrumbs--;
            }
            return c;
        }

        public IEnumerable<char> Get(Coordinate.Point point, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var ret = this[point];
                point.X++;
                yield return ret;
            }
        }

        public bool IsHome(Coordinate.Point p)
        {
            return p.X > 35 && p.X < 43 && p.Y > 10 && p.Y < 13;
        }
    }
}