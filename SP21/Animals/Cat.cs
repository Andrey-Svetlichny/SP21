using System;
using System.Collections.Generic;
using System.Linq;

namespace SP21.Animals
{
    public class Cat : Animal
    {
        private static Random _random;
        private const string SkinNornal = "=0=", SkinGame = "<->", SkinShadow = "---";

        public enum ModeEnum
        {
            /// <summary>
            /// Нормальная кошка, охотится на мышь.
            /// </summary>
            Normal,

            /// <summary>
            /// Кошка стала предметом охоты и спасается от озверевшей мыши.
            /// </summary>
            Prey,

            /// <summary>
            /// Тень, оставшаяся от кошки, после того как ее съела озверевшая мышь,
            /// возвращается к дому, чтобы возродиться.
            /// </summary>
            Shadow
        }

        /// <summary>
        /// Кошка пропускает каждый 4-й ход
        /// </summary>
        public const int PreyModeSkipStep = 4;

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

        private ModeEnum _mode;

        public Cat()
        {
            _random = new Random();
            Mode = ModeEnum.Normal;
            Skin = SkinNornal;
        }

        public void Step(Coordinate.Point mouseCoord, int score)
        {
            switch (Mode)
            {
                case ModeEnum.Normal:
                    if (Coord == Level.GateOut)
                    {
                        // кошка на точке выхода из дома
                        Dir = Level.GateOutDirection;
                    }
                    else if (Level.IsHome(Coord))
                    {
                        // кошка в доме
                        SelectDirection(Coordinate.GetDirection(Coord, Level.GateOut));
                    }
                    else
                    {
                        SelectDirection(Coordinate.GetDirection(Coord, mouseCoord));
                    }
                    break;

                case ModeEnum.Prey:
                    SelectDirection(Coordinate.GetDirection(mouseCoord, Coord));
                    break;

                case ModeEnum.Shadow:
                    // тень кошки вышла из дома
                    if (Coord == Level.GateIn && Dir == Coordinate.Direction.Up)
                    {
                        Mode = ModeEnum.Normal;
                    }
                    // кошка вошла в дом
                    if (Coord == Level.GateOut)
                    {
                        if(score > 1000)
                        {
                            // при счете > 1000 сразу выходить из дома
                            Dir = Coordinate.Direction.Up;
                            break;
                        }
                    }
                    SelectDirection(Coordinate.GetDirection(Coord, Level.Gate));
                    break;
            }

            switch (Mode)
            {
                case ModeEnum.Normal:
                    Skin = SkinNornal;
                    break;
                case ModeEnum.Prey:
                    Skin = SkinGame;
                    break;
                case ModeEnum.Shadow:
                    Skin = SkinShadow;
                    break;
            }

            base.Step();
        }

        /// <summary>
        /// Выбор направления движения.
        /// </summary>
        /// <param name="targetDirs">Направления в сторону цели</param>
        private void SelectDirection(IEnumerable<Coordinate.Direction> targetDirs)
        {
            // направления, в которых можно двигаться
            var availableDirs = Enum.GetValues(typeof(Coordinate.Direction)).OfType<Coordinate.Direction>()
                .Where(CanMove).ToList();

            // в режиме тени разрешено входить в дом и выходить из него
            if (Mode == ModeEnum.Shadow)
            {
                if(Coord == Level.GateIn)
                    availableDirs.Add(Coordinate.Direction.Down);
                if(Coord == Level.GateOut)
                    availableDirs.Add(Coordinate.Direction.Up);
            }

            // если это тупик, можно проходить сквозь стены
            if (availableDirs.Count == 1)
            {
                availableDirs = Enum.GetValues(typeof(Coordinate.Direction)).OfType<Coordinate.Direction>().ToList();
            }

            // исключаем направление, откуда только что пришли, если есть другие
            if (Dir != null && availableDirs.Count > 1)
            {
                availableDirs = availableDirs.Except(new[] {((Coordinate.Direction) Dir).Reverse()}).ToList();
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
}