﻿using System;
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

        const string ResLevel = "SP21.Levels.Level{0}.txt";

        private int _levelNum;
        StringBuilder[] _lines;

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

        /// <summary>
        /// Время действия озверина на текущем уровне.
        /// </summary>
        public int OzverinTime { get; private set; }

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

        public string Get(Coordinate.Point point, int count)
        {
            string ret = "";
            for (int i = 0; i < count; i++)
            {
                ret += this[point];
                point.Move(Coordinate.Direction.Right);
            }
            return ret;
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
            var ret = this[point];
            this[point] = ' ';
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
            _lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Select(l => new StringBuilder(l.PadRight(80))).ToArray();

            Breadcrumbs = text.Count(f => f == '.');

            // ToDo уточнить
            OzverinTime = 180 - (levelNum - 1) * 10;
        }

        /// <summary>
        /// Поставить отметки X, в соответствии с количеством жизней.
        /// </summary>
        private void SetLivesMarks()
        {
            for (int i = 0; i < 10; i++)
            {
                this[new Coordinate.Point(79, i)] = i < _lives - 1 ? 'X' : ' ';
            }
        }
    }
}