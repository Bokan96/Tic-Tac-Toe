namespace TicTacToe.Utilities
{    public static class TimeUtils
    {
        public static string FormatDuration(float seconds)
        {
            int mins = (int)(seconds / 60);
            int secs = (int)(seconds % 60);
            return $"{mins:00}:{secs:00}";
        }
    }
}
