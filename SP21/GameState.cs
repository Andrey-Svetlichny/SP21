namespace SP21
{
    /// <summary>
    /// Состояние игры.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Текущий уровень.
        /// </summary>
        public Level Level;

        /// <summary>
        /// Жизней осталось.
        /// </summary>
        public int Life = 3;

        /// <summary>
        /// Счет.
        /// </summary>
        public int Score;
    }
}