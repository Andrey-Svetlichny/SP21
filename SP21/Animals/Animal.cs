using System.Collections.Generic;
using System.Linq;

namespace SP21.Animals
{
    public abstract class Animal
    {
        public Level Level;
        public Coordinate.Point Coord;
        public Coordinate.Direction? Dir = null;
        public string Skin;

        public virtual void Step()
        {
            if (Dir != null)
            {
                Coord.Move((Coordinate.Direction)Dir);
            }
        }

        protected bool CanMove(Coordinate.Direction direction)
        {
            var allowedChars = new[] { ' ', '.', '@' };
            return CharsForward(direction).All(c => allowedChars.Any(allowed => c == allowed));
        }

        protected IEnumerable<char> CharsForward(Coordinate.Direction direction)
        {
            switch (direction)
            {
                case Coordinate.Direction.Left:
                    return Level.Get(Coord.Copy(-2, 0), 1);
                case Coordinate.Direction.Right:
                    return Level.Get(Coord.Copy(2, 0), 1);
                case Coordinate.Direction.Up:
                    return Level.Get(Coord.Copy(-1, -1), 3);
                case Coordinate.Direction.Down:
                    return Level.Get(Coord.Copy(-1, 1), 3);
                default:
                    return new char[] { };
            }
        }

        public override string ToString()
        {
            return $"X={Coord.X}, Y={Coord.Y}, Dir={Dir}";
        }
    }
}