using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SP21
{
    public static class Coordinate
    {
        private const int Maxx = 80;

        [DebuggerDisplay("X={X}; Y={Y};")]
        public struct Point
        {
            private int _x;
            private int _y;
            private bool _normalizing;
            private bool _normalized;

            public int X
            {
                get
                {
                    Normalize();
                    return _x;
                }
                set
                {
                    _x = value;
                    _normalized = false;
                }
            }

            public int Y
            {
                get
                {
                    Normalize();
                    return _y;
                }
                set
                {
                    _y = value;
                    _normalized = false;                    
                }
            }

            private void Normalize()
            {
                if (_normalizing || _normalized)
                {
                    return;
                }
                _normalizing = true;
                Y += (X + Maxx) / Maxx - 1;
                X = (X + Maxx) % Maxx;
                _normalized = true;
                _normalizing = false;
            }

            public static bool operator ==(Point a, Point b)
            {
                return a.X == b.X && a.Y == b.Y;
            }

            public static bool operator !=(Point a, Point b)
            {
                return !(a == b);
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
                    return (X * 397) ^ Y;
                }
            }

            public void Move(Direction? direction)
            {
                switch (direction)
                {
                    case Direction.Left:
                        X--;
                        break;
                    case Direction.Right:
                        X++;
                        break;
                    case Direction.Up:
                        Y--;
                        break;
                    case Direction.Down:
                        Y++;
                        break;
                }
            }

            public Point Copy(int dx, int dy)
            {
                return new Point { X = X + dx, Y = Y + dy };
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
