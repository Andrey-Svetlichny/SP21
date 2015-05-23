using System.Collections.Generic;
using System.Linq;

namespace SP21.Animals
{
    class Mouse : Animal
    {
        private Coordinate.Direction? _nextDirection = null;
        private const string skin1 = "<$>", skin2 = ">$<", skinDied = "+$+";

        public Mouse(GameState state, Coordinate.Point coord)
            : base(state, coord)
        {
            Skin = skin1;
        }

        public List<Cat> Cats { get; set; }

        public override void Step()
        {
            if (_nextDirection != null && CanMove((Coordinate.Direction)_nextDirection))
            {
                Dir = _nextDirection;
            }
            else if (Dir == null || !CanMove((Coordinate.Direction)Dir))
            {
                return;
            }

            base.Step();
            Skin = Skin == skin1 ? skin2 : skin1;
            var read = State.Level.Read(Coord.Copy(-1, 0), 3);
            var scoreDiff = read.Count(c => c == '.');
            State.Score += scoreDiff;
            for (int i = 0; i < scoreDiff; i++)
            {
                State.Level.EatBreadcrumb();
            }
            if (read.Contains('@'))
            {
                Cats.ForEach(c => c.StartOzverinMode());
            }
            State.Level.Write(Coord.Copy(-1, 0), "   ");
            State.Level.Draw(new Coordinate.Point { X = 36, Y = 18 }, string.Format("{0,6}", State.Score));
        }

        protected bool CanMove(Coordinate.Direction direction)
        {
            var allowedChars = new[] { ' ', '.', '@' };
            return CharsForward(direction).All(c => allowedChars.Any(allowed => c == allowed));
        }

        public void Die()
        {
            Skin = skinDied;
            Draw();
        }

        public override void Reset()
        {
            Skin = skin1;
            base.Reset();
        }

        public void WantMove(Coordinate.Direction direction)
        {
            _nextDirection = direction;
        }
    }
}
