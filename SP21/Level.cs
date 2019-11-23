using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SP21
{
    /// <summary>
    /// Уровень.
    /// Его текущее состояние.
    /// </summary>
    public class Level
    {
        /// <summary>
        /// Направление входа в дом.
        /// </summary>
        public const Coordinate.Direction GateInDirection = Coordinate.Direction.Down;

        /// <summary>
        /// Направление выхода из дома.
        /// </summary>
        public const Coordinate.Direction GateOutDirection = Coordinate.Direction.Up;

        /// <summary>
        /// Точка входа в дом.
        /// </summary>
        public readonly Coordinate.Point GateIn = new Coordinate.Point(39, 9);

        /// <summary>
        /// Дверь в дом.
        /// </summary>
        public readonly Coordinate.Point Gate = new Coordinate.Point(39, 10);

        /// <summary>
        /// Точка выхода из дома
        /// </summary>
        public readonly Coordinate.Point GateOut = new Coordinate.Point(39, 11);

        /// <summary>
        /// Начальное положение и направление движения кошек.
        /// </summary>
        public readonly Tuple<Coordinate.Point, Coordinate.Direction>[] InitialCatsCoordinateDirection =
            {
                new Tuple<Coordinate.Point, Coordinate.Direction>(new Coordinate.Point(37, 11), Coordinate.Direction.Right), 
                new Tuple<Coordinate.Point, Coordinate.Direction>(new Coordinate.Point(41, 11), Coordinate.Direction.Left), 
                new Tuple<Coordinate.Point, Coordinate.Direction>(new Coordinate.Point(37, 12), Coordinate.Direction.Right), 
                new Tuple<Coordinate.Point, Coordinate.Direction>(new Coordinate.Point(41, 12), Coordinate.Direction.Left)
            };

        /// <summary>
        /// Начальное положение мыши.
        /// </summary>
        public readonly Coordinate.Point InitialMouseCoordinate = new Coordinate.Point(39, 16);

        /// <summary>
        /// Расположение бонуса.
        /// </summary>
        public readonly Coordinate.Point BonusCoordinate = new Coordinate.Point(38, 14);

        const string ResLevel = "SP21.Levels.Level{0}.txt";

        private int _levelNum;

        /// <summary>
        /// Текущее состояние уровня (стены, крошки, озверин)
        /// </summary>
        char[][] _lines;

        /// <summary>
        /// Первоначальное состояние уровня (стены, крошки, озверин)
        /// </summary>
        char[][] _linesOrig;

        private int _lives;

        /// <summary>
        /// Количество оставшихся хлебных крошек.
        /// </summary>
        public int Breadcrumbs { get; private set; }

        /// <summary>
        /// Номер текущего уровня.
        /// </summary>
        public int LevelNum
        {
            get { return _levelNum; }
            set
            {
                _levelNum = value;
                Load(_levelNum);
                SetLivesMarks();
            }
        }

        public int Lives
        {
            get { return _lives; }
            set
            {
                _lives = value;
                SetLivesMarks();
            }
        }

        public char Get(int x, int y)
        {
            return _lines[y][x];
        }

        public char Get(Coordinate.Point p)
        {
            return _lines[p.Y][p.X];
        }

        public char GetOrig(Coordinate.Point p)
        {
            return _linesOrig[p.Y][p.X];
        }

        public void Set(Coordinate.Point p, char value)
        {
            _lines[p.Y][p.X] = value;
        }

        public string Get(Coordinate.Point point, int count)
        {
            var result = "";
            for (int i = 0; i < count; i++)
            {
                result += _lines[point.Y][point.X];
                point = point + 1;
            }
            return result;
        }

        public string GetOrig(Coordinate.Point point, int count)
        {
            var result = "";
            for (int i = 0; i < count; i++)
            {
                result += _linesOrig[point.Y][point.X];
                point = point + 1;
            }
            return result;
        }

        /// <summary>
        /// Координата находится внутри дома.
        /// </summary>
        public bool IsHome(Coordinate.Point p)
        {
            return p.X > 35 && p.X < 43 && p.Y > 10 && p.Y < 13;
        }

        /// <summary>
        /// Забирает объект из точки.
        /// </summary>
        public char Take(Coordinate.Point point)
        {
            var ret = Get(point);
            Set(point, ' ');
            return ret;
        }

        private void Load(int levelNum)
        {
            Debug.Assert(levelNum > 0);
            var n = (levelNum - 1) % 3 + 1;

            string text;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(string.Format(ResLevel, n)))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            _linesOrig = text.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Select(l => l.PadRight(80).ToCharArray()).ToArray();
            _lines = _linesOrig.Select(o => (char[])o.Clone()).ToArray();

            Breadcrumbs = text.Count(f => f == '.');
        }

        /// <summary>
        /// Поставить отметки X, в соответствии с количеством жизней.
        /// </summary>
        private void SetLivesMarks()
        {
            for (int i = 0; i < 10; i++)
            {
                Set(new Coordinate.Point(79, i), i < _lives - 1 ? 'X' : ' ');
            }
        }

        public void SetBonus(int bonus)
        {
            var s =  bonus == 0 ? "   " : $"{bonus / 10}*{bonus % 10}";

            Set(BonusCoordinate, s[0]);
            Set(BonusCoordinate + 1, s[1]);
            Set(BonusCoordinate + 2, s[2]);
        }
    }
}