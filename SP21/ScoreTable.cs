using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace SP21
{
    /// <summary>
    /// Таблица рекордов
    /// </summary>
    public class ScoreTable
    {
        private const string ScoreFile = "SP21.dat";

        private const int RowsCount = 20;

        public List<Score> Scores { get; set; }

        public Score NewScore { get; set; }

        /// <summary>
        /// Добавляет рекорд в таблицу.
        /// </summary>
        /// <param name="score"></param>
        /// <returns>Номер строки нового рекорда</returns>
        public int Add(int score)
        {
            NewScore = new Score { Value = score };
            Scores.Add(NewScore);
            Scores = Scores.OrderByDescending(o => o.Value).ToList();
            Scores.RemoveAt(Scores.Count - 1);
            return Scores.IndexOf(NewScore);
        }

        public void Load()
        {
            try
            {
                using (var fs = new FileStream(ScoreFile, FileMode.Open))
                {
                    Scores = (List<Score>)new BinaryFormatter().Deserialize(fs);
                }
            }
            catch (Exception)
            {
                Scores = new List<Score>();
                for (int i = Scores.Count; i < RowsCount; i++)
                {
                    Scores.Add(new Score());
                }
            }
            // NoName score
            Scores.Add(new Score());
        }

        public void Save()
        {
            using (var fs = new FileStream(ScoreFile, FileMode.Create))
            {
                new BinaryFormatter().Serialize(fs, Scores.Take(RowsCount).ToList());
            }
        }

    }
}
