using System;
using System.Diagnostics;
using System.Linq;

namespace SP21.Animals
{
    class Cat : Animal
    {
        private readonly Mouse _mouse;
        private static Random _random;
        private const string SkinNornal = "=0=", SkinGame = "<->", SkinShadow = "---";

        public enum ModeEnum
        {
            Normal,
            Game,
            Shadow
        }

        public ModeEnum Mode;

        /// <summary>
        /// Осталось времени текущему озверину.
        /// </summary>
        public int OzverinRemains = 0;

        public Cat(View view, GameState state, Coordinate.Point coord, Mouse mouse)
            : base(view, state, coord)
        {
            _random = new Random();
            _mouse = mouse;
            Mode = ModeEnum.Normal;
            Skin = SkinNornal;
        }

        public override void Step()
        {
            if (Mode == ModeEnum.Shadow && Coord == State.Level.GateIn)
            {
                // тень мыши на точке входа в дом
                Dir = Level.GateInDirection;
            }
            else if (Mode == ModeEnum.Shadow && Coord == State.Level.GateOut)
            {
                // тень мыши вошла в дом
                Mode = ModeEnum.Normal;
            }
            else if (Coord == State.Level.GateOut)
            {
                // мышь на точке выхода из дома
                Dir = Level.GateOutDirection;
            }
            else if (State.Level.IsHome(Coord))
            {
                // мышь в доме

                // направления, в которых можно двигаться
                var availableDirs = Enum.GetValues(typeof(Coordinate.Direction)).OfType<Coordinate.Direction>()
                    .Where(CanMove).ToArray();

                Dir = (Coordinate.Direction)availableDirs.GetValue(_random.Next(availableDirs.Length));
            }
            else
            {
                {
                    // направления, в которых можно двигаться
                    var availableDirs = Enum.GetValues(typeof(Coordinate.Direction)).OfType<Coordinate.Direction>()
                        .Where(CanMove).ToArray();

                    // исключаем направление, откуда только что пришли, если есть другие
                    if (Dir != null && availableDirs.Length > 1)
                    {
                        availableDirs = availableDirs.Except(new[] { ((Coordinate.Direction)Dir).Reverse() }).ToArray();
                    }

                    // направления к цели - на мышь / от озверевшей мыши / на дом (если кошка мертва)
                    Coordinate.Direction[] targetDirs;
                    if (Mode == ModeEnum.Shadow)
                    {
                        targetDirs = Coordinate.GetDirection(Coord, State.Level.GateIn);
                    }
                    else
                    {
                        targetDirs = Coordinate.GetDirection(Coord, _mouse.Coord);
                        if (Mode == ModeEnum.Game)
                        {
                            targetDirs = targetDirs.Select(o => o.Reverse()).ToArray();
                        }
                    }

                    // предпочтительные направления - в сторону цели 
                    var preferredDirs = targetDirs.Intersect(availableDirs).ToArray();

                    var dirsToSelect = availableDirs.ToList();


                    // вероятность двинуться в правильном направлении в 6 раз выше
                    for (int i = 0; i < 5; i++)
                    {
                        dirsToSelect.AddRange(preferredDirs);
                    }

                    Dir = dirsToSelect[_random.Next(dirsToSelect.Count)];
                }
            }

            Debug.Assert(OzverinRemains >= 0);

            if (Mode == ModeEnum.Game && --OzverinRemains == 0)
            {
                Mode = ModeEnum.Normal;
            }

            switch (Mode)
            {
                case ModeEnum.Normal:
                    Skin = SkinNornal;
                    break;
                case ModeEnum.Game:
                    Skin = SkinGame;
                    break;
                case ModeEnum.Shadow:
                    Skin = SkinShadow;
                    break;
            }

            base.Step();
        }

        public void StartOzverinMode()
        {
            if (Mode != ModeEnum.Shadow)
            {
                Mode = ModeEnum.Game;
                OzverinRemains = Level.OzverinTime;
            }
        }
    }
}