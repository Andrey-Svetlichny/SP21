using System;
using System.Collections.Generic;
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

        private const int GameModeSkipStep = 4;

        public ModeEnum Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    Dir = null;
                }
            }
        }


        /// <summary>
        /// Осталось времени текущему озверину.
        /// </summary>
        public int OzverinRemains;

        private ModeEnum _mode;

        public Cat(View view, GameState state, Coordinate.Point coord, Mouse mouse)
            : base(view, state, coord)
        {
            _random = new Random();
            _mouse = mouse;
            OzverinRemains = 0;
            Mode = ModeEnum.Normal;
            Skin = SkinNornal;
        }

        public override void Step()
        {
            switch (Mode)
            {
                case ModeEnum.Normal:
                    if (Coord == State.Level.GateOut)
                    {
                        // мышь на точке выхода из дома
                        Dir = Level.GateOutDirection;
                    }
                    else if (State.Level.IsHome(Coord))
                    {
                        // мышь в доме
                        var availableDirs = Enum.GetValues(typeof(Coordinate.Direction)).OfType<Coordinate.Direction>()
                            .Where(CanMove).ToArray();
                        Dir = (Coordinate.Direction)availableDirs.GetValue(_random.Next(availableDirs.Length));
                    }
                    else
                    {
                        SelectDirection(Coordinate.GetDirection(Coord, _mouse.Coord));
                    }
                    break;

                case ModeEnum.Game:
                    Debug.Assert(OzverinRemains >= 0);

                    if (--OzverinRemains == 0)
                    {
                        Mode = ModeEnum.Normal;
                    } 
                    else if ((OzverinRemains % GameModeSkipStep == 0))
                    {
                        return;
                    }

                    SelectDirection(Coordinate.GetDirection(_mouse.Coord, Coord));
                    break;

                case ModeEnum.Shadow:
                    if (Coord == State.Level.GateIn)
                    {
                        // тень мыши на точке входа в дом
                        Dir = Level.GateInDirection;
                    }
                    else if (Coord == State.Level.GateOut)
                    {
                        // тень мыши вошла в дом
                        Mode = ModeEnum.Normal;
                    }
                    else
                    {
                        SelectDirection(Coordinate.GetDirection(Coord, State.Level.GateIn));
                    }
                    break;
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

        public override void Reset()
        {
            OzverinRemains = 0;
            Mode = ModeEnum.Normal;
            Skin = SkinNornal;
            base.Reset();
        }

        /// <summary>
        /// Выбор направления движения.
        /// </summary>
        /// <param name="targetDirs">Направления в сторону цели</param>
        private void SelectDirection(IEnumerable<Coordinate.Direction> targetDirs)
        {
            // направления, в которых можно двигаться
            var availableDirs = Enum.GetValues(typeof (Coordinate.Direction)).OfType<Coordinate.Direction>()
                .Where(CanMove).ToArray();

            // исключаем направление, откуда только что пришли, если есть другие
            if (Dir != null && availableDirs.Length > 1)
            {
                availableDirs = availableDirs.Except(new[] {((Coordinate.Direction) Dir).Reverse()}).ToArray();
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

        public void StartOzverinMode()
        {
            if (Mode != ModeEnum.Shadow)
            {
                Mode = ModeEnum.Game;
                OzverinRemains = State.Level.OzverinTime;
            }
        }
    }
}