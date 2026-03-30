namespace TicTacToe.Core
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }

    public enum PlayerMark
    {
        None,
        X,   // Player 1/Crveni
        O    // Player 2/Plavi
    }
    public struct GameResult
    {
        public PlayerMark Winner;
        public float Duration;
        public int Player1Moves;
        public int Player2Moves;

        public int[] WinLine;

        public bool IsDraw => Winner == PlayerMark.None;
    }
}
