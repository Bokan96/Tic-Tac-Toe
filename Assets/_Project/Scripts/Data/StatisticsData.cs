using System;

namespace TicTacToe.Data
{
    [Serializable]
    public class StatisticsData
    {
        public int totalGames;
        public int player1Wins;
        public int player2Wins;
        public int draws;
        public float totalDuration;
        public float AverageGameDuration =>
            totalGames > 0 ? totalDuration / totalGames : 0f;
    }
}
