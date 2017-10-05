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
            Coord = Coord.Move(Dir);
        }

        protected bool CanMove(Coordinate.Direction direction)
        {
            var allowedChars = new[] { ' ', '.', '@', '*', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            switch (direction)
            {
                case Coordinate.Direction.Left:
                    return allowedChars.Contains(Level[Coord - 2]);
                case Coordinate.Direction.Right:
                    return allowedChars.Contains(Level[Coord + 2]);
                case Coordinate.Direction.Up:
                    return Level.Get(Coord.Add(-1, -1), 3)
                        .All(c => allowedChars.Any(allowed => c == allowed));
                case Coordinate.Direction.Down:
                    return Level.Get(Coord.Add(-1, 1), 3)
                        .All(c => allowedChars.Any(allowed => c == allowed));
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return $"X={Coord.X}, Y={Coord.Y}, Dir={Dir}";
        }
    }
}