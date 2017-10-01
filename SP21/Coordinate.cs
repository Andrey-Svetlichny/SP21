using System;
using System.Collections.Generic;

namespace SP21
{
    public static class Coordinate
    {
        public const int MaxX = 80;
        public const int MaxY = 24;

        public struct Point
        {
            private int _value;

            public Point(int x, int y)
            {
                _value = y * MaxX + x;
            }

            public int X => _value % MaxX;

            public int Y => _value / MaxX;

            public override string ToString()
            {
                return $"{X} {Y}";
            }

            public static bool operator ==(Point a, Point b)
            {
                return a._value == b._value;
            }

            public static bool operator !=(Point a, Point b)
            {
                return a._value != b._value;
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
                unchecked
                {
                    return Y * MaxX + X;
                }
            }

            public void Move(Direction direction)
            {
                switch (direction)
                {
                    case Direction.Left:
                        _value--;
                        break;
                    case Direction.Right:
                        _value++;
                        break;
                    case Direction.Up:
                        _value -= MaxX;
                        break;
                    case Direction.Down:
                        _value += MaxX;
                        break;
                }
                if (_value < 0 || _value > MaxX * MaxY)
                {
                    throw new ArgumentException();
                }
            }

            public Point Copy(int dx, int dy)
            {
                return new Point(X + dx, Y + dy);
            }
        }

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
