using System.Collections.Generic;
using System.Linq;
using SP21.Animals;

namespace SP21
{
    public class Game
    {
        readonly Level _level;
        readonly ConsoleView _view;
        readonly ScoreTable _scoreTable;
        List<Cat> _cats;
        Mouse _mouse;
        int _score;
        int _breadcrumbs;
        int _catsEatenDuringCurrentOzverin;
        int _bonus;

        /// <summary>
        /// Осталось времени текущему озверину.
        /// </summary>
        int _ozverinRemains;

        /// <summary>
        /// Осталось времени текущему бонусу.
        /// </summary>
        int _bonusRemains;

        public Game()
        {
            _level = new Level();
            _view = new ConsoleView();
            _scoreTable = new ScoreTable();
            _scoreTable.Load();
        }

        public void Run()
        {
            _view.DrawScoreTable(_scoreTable);
            if (!ConsoleView.UserWantsToPlay(true))
            {
                return;
            }

            while (true)
            {
                _level.LevelNum = 1;
                _level.Lives = 3;
                _breadcrumbs = _level.Breadcrumbs;
                _score = 0;
                _bonus = 0;
                _ozverinRemains = 0;
                _bonusRemains = 0;
                InitAnimals();
                _view.DrawLevel(_level);
                foreach (var cat in _cats)
                {
                    _view.Draw(cat, _level);
                }
                _view.Draw(_mouse, _level);

                while (_level.Lives > 0)
                {
                    GameStep();
                }

                var position = _scoreTable.Add(_score);
                _view.DrawScoreTable(_scoreTable);
                if (position != -1)
                {
                    _scoreTable.Scores[position].Name = ConsoleView.EnterName(position);
                    _scoreTable.Save();
                }
                if (!ConsoleView.UserWantsToPlay(false))
                    break;
            }
        }

        private void GameStep()
        {
            if (_score%100 == 0 && _score > 0)
            {
                // время показать бонус
                _bonus = 20;
                _bonusRemains = 100;
                _level.SetBonus(_bonus);
                _view.DrawBonus(_level);
            }

            if (_ozverinRemains > 0 && --_ozverinRemains == 0)
            {
                // кончилось время работы озверина
                _cats.ForEach(o => o.Mode = Cat.ModeEnum.Normal);
            }

            if (_bonusRemains > 0 && --_bonusRemains == 0)
            {
                // кончилось время показа бонуса
                _bonus = 0;
                _level.SetBonus(_bonus);
                _view.DrawBonus(_level);
            }

            if (_ozverinRemains == 0 || _ozverinRemains%Cat.PreyModeSkipStep != 0)
            {
                foreach (var cat in _cats)
                {
                    cat.Step(_mouse.Coord);
                    _view.Draw(cat, _level);
                    CheckCatch(cat, _mouse);
                }
            }

            var direction = ConsoleView.GetMouseDirectionFromKeyboard();
            if (direction != null)
            {
                _mouse.WantMove(direction.Value);
            }
            _mouse.Step();
            _view.Draw(_mouse, _level);
            var item = _level.Take(
                _mouse.Dir == Coordinate.Direction.Left ? _mouse.Coord - 1 :
                _mouse.Dir == Coordinate.Direction.Right ? _mouse.Coord + 1 : _mouse.Coord);
            switch (item)
            {
                case '*':
                    IncreaseScore(_bonus);
                    _bonus = 0;
                    _level.SetBonus(_bonus);
                    break;
                case '@':
                    IncreaseScore(5);
                    _view.DrawScore(_score);
                    _ozverinRemains = _level.OzverinTime;
                    _cats.Where(c => !_level.IsHome(c.Coord) && c.Mode != Cat.ModeEnum.Shadow)
                        .ToList().ForEach(c => c.Mode = Cat.ModeEnum.Prey);
                    _catsEatenDuringCurrentOzverin = 0;
                    break;
                case '.':
                    IncreaseScore(1);
                    _view.DrawScore(_score);
                    if (--_breadcrumbs == 0)
                    {
                        _level.LevelNum++;
                        _breadcrumbs = _level.Breadcrumbs;
                        _view.DrawLevel(_level);
                        _view.DrawScore(_score);
                        InitAnimals();
                    }
                    break;
            }
            _cats.ForEach(cat => CheckCatch(cat, _mouse));
        }

        /// <summary>
        /// Кошка поймала мышь (или озверевшая мышь поймала кошку)?
        /// </summary>
        private void CheckCatch(Cat cat, Mouse mouse1)
        {
            if (cat.Coord != mouse1.Coord)
            {
                return;
            }

            // кошка поймала мышь 
            if (cat.Mode == Cat.ModeEnum.Normal)
            {
                _level.Lives--;
                _bonus = 0;
                _level.SetBonus(_bonus );
                _view.MouseDieAnimation(_level, _cats, _mouse);
                _view.DrawLevel(_level);
                InitAnimals();
            }

            // мышь поймала кошку
            if (cat.Mode == Cat.ModeEnum.Prey)
            {
                IncreaseScore(++_catsEatenDuringCurrentOzverin * 20);
                cat.Mode = Cat.ModeEnum.Shadow;
            }
        }

        public void IncreaseScore(int scoreDiff)
        {
            var newScore = _score + scoreDiff;
            if (newScore / 500 > _score / 500)
            {
                _view.DrawNewLife(++_level.Lives);
            }
            _score = newScore;
            _view.DrawScore(_score);
        }


        private void InitAnimals()
        {
            if (_cats == null)
            {
                _cats = _level.InitialCatsCoordinateDirection.Select(o => new Cat()).ToList();
                _mouse = new Mouse();
            }

            for (int i = 0; i < _level.InitialCatsCoordinateDirection.Length; i++)
            {
                _cats[i].Level = _level;
                _cats[i].Coord = _level.InitialCatsCoordinateDirection[i].Item1;
                _cats[i].Dir = _level.InitialCatsCoordinateDirection[i].Item2;
                _cats[i].Mode = Cat.ModeEnum.Normal;
            }
            _mouse.Level = _level;
            _mouse.Coord = _level.InitialMouseCoordinate;
        }
    }
}
