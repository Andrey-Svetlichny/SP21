using System;
using System.Collections.Generic;

namespace SP21
{
    public static class Coordinate
    {
        public const int MaxX = 80;
        public const int MaxY = 24;

        public enum Direction
        {
            Left, Right, Up, Down
        }

        public static Direction[] GetDirection(Point from, Point to)
        {
            var ret = new List<Direction>();
            var dx = to.X - from.X;
            if (dx < 0)
            {
                ret.Add(Direction.Left);
            }
            if (dx > 0)
            {
                ret.Add(Direction.Right);
            }
            var dy = to.Y - from.Y;
            if (dy < 0)
            {
                ret.Add(Direction.Up);
            }
            if (dy > 0)
            {
                ret.Add(Direction.Down);
            }
            return ret.ToArray();
        }

        public struct Point
        {
            private readonly int _value;

            public Point(int x, int y)
            {
                _value = y * MaxX + x;
            }

            public int X => _value % MaxX;

            public int Y => _value / MaxX;

            public static Point operator -(Point p, int x)
            {
                return new Point(p.X - x, p.Y);
            }

            public static bool operator !=(Point a, Point b)
            {
                return a._value != b._value;
            }

            public static Point operator +(Point p, int x)
            {
                return new Point(p.X + x, p.Y);
            }

            public static bool operator ==(Point a, Point b)
            {
                return a._value == b._value;
            }

            public Point Add(int dx, int dy)
            {
                return new Point(X + dx, Y + dy);
            }

            public bool Equals(Point other)
            {
                return this == other;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Point && Equals((Point)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }

            public Point Move(Direction? direction)
            {
                switch (direction)
                {
                    case Direction.Left:
                        return this - 1;
                    case Direction.Right:
                        return this + 1;
                    case Direction.Up:
                        return this - MaxX;
                    case Direction.Down:
                        return this + MaxX;
                    default:
                        return this;
                }
            }

            public override string ToString()
            {
                return $"{X} {Y}";
            }
        }
    }

    public static class DirectionExtension
    {
        public static Coordinate.Direction Reverse(this Coordinate.Direction direction)
        {
            switch (direction)
            {
                case Coordinate.Direction.Left:
                    return Coordinate.Direction.Right;
                case Coordinate.Direction.Right:
                    return Coordinate.Direction.Left;
                case Coordinate.Direction.Up:
                    return Coordinate.Direction.Down;
                case Coordinate.Direction.Down:
                    return Coordinate.Direction.Up;
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }
    }
}