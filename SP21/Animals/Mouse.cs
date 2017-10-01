using System.Collections.Generic;

namespace SP21.Animals
{
    public class Mouse : Animal
    {
        private Coordinate.Direction? _nextDirection;
        private const string Skin1 = "<$>", Skin2 = ">$<";

        public Mouse()
        {
            Skin = Skin1;
        }

        public List<Cat> Cats { get; set; }

        public override void Step()
        {
            if (_nextDirection != null && CanMove((Coordinate.Direction)_nextDirection))
            {
                Dir = _nextDirection;
            }

            if (Dir != null && !CanMove((Coordinate.Direction)Dir))
            {
                Dir = null;
            }

            if (Dir != null)
            {
                Skin = Skin == Skin1 ? Skin2 : Skin1;
            }

            base.Step();
        }

        public void WantMove(Coordinate.Direction direction)
        {
            _nextDirection = direction;
        }
    }
}
