using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SP21
{
    internal class ScoreActivity
    {
        [Serializable]
        public class Score
        {
            public String Name { get; set; } 
            public int Value { get; set; } 
        }

        private const string Datfile = "SP21.dat";

        public List<Score> Scores { get; set; }

        public ScoreActivity()
        {
            _isFirstRun = true;
            try
            {
                Load();
            }
            catch (Exception)
            {
                Scores = new List<Score>();
                //{
                //    new Score {Name = "IVAN-FARI", Value = 14763},
                //    new Score {Name = "BARMIN", Value = 13844}
                //};
                for (int i = Scores.Count; i < 20; i++)
                {
                    Scores.Add(new Score());
                }
            }
        }


        private void Save()
        {
            using (var fs = new FileStream(Datfile, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, Scores);
            }
        }

        private void Load()
        {
            using (var fs = new FileStream(Datfile, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                Scores = (List<Score>)formatter.Deserialize(fs);
            }
        }

        public int NoNameScore { get; set; }

        private bool _isFirstRun;

        /// <summary>
        /// Показывает таблицу рекордов.
        /// </summary>
        /// <returns>Начинать игру.</returns>
        public bool Show(int score)
        {
            NoNameScore = score;

            Console.Clear();
            Console.WriteLine("      PAC-HALL");
            Console.WriteLine("--------------------");

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(".{0:00}.{1,-9} {2,6}", i + 1, Scores[i].Name, Scores[i].Value);
            }
            Console.WriteLine(".NN.********* {0,6}", NoNameScore);

            Console.SetCursorPosition(36, 11);
            Console.WriteLine(_isFirstRun ? "BEGIN ?" : "ONCE MORE ?");
            _isFirstRun = false;
            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.N)
                {
                    return false;
                }

                if (keyInfo.Key == ConsoleKey.Y)
                {
                    return true;
                }
            }
        }
    }
}