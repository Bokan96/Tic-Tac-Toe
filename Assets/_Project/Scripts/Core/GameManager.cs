using UnityEngine;

namespace TicTacToe.Core
{
    public class GameManager : MonoBehaviour
    {
        private PlayerMark[] _board = new PlayerMark[9];
        private PlayerMark _currentPlayer;
        private int _player1Moves;
        private int _player2Moves;

        private const string STARTER_PREF = "NextStartingPlayer";
        private static PlayerMark _nextStartingPlayer = PlayerMark.X;
        private static bool _starterLoaded;
        private float _timer;
        private bool _isPlaying;
        private GameState _state = GameState.Menu;

        private static readonly int[][] WinPatterns =
        {
            new[] { 0, 1, 2 }, // top row
            new[] { 3, 4, 5 }, // middle row
            new[] { 6, 7, 8 }, // bottom row
            new[] { 0, 3, 6 }, // left column
            new[] { 1, 4, 7 }, // middle column
            new[] { 2, 5, 8 }, // right column
            new[] { 0, 4, 8 }, // main diagonal
            new[] { 2, 4, 6 }  // anti-diagonal
        };
        public float Timer => _timer;
        public PlayerMark CurrentPlayer => _currentPlayer;
        public int Player1Moves => _player1Moves;
        public int Player2Moves => _player2Moves;
        public PlayerMark[] GetBoardState() => (PlayerMark[])_board.Clone();
        private void OnEnable()
        {
            GameEvents.OnCellClicked += HandleCellClicked;
            GameEvents.OnGameRestarted += StartGame;
            GameEvents.OnReturnToMenu += ResetStartingPlayer;
        }

        private void OnDisable()
        {
            GameEvents.OnCellClicked -= HandleCellClicked;
            GameEvents.OnGameRestarted -= StartGame;
            GameEvents.OnReturnToMenu -= ResetStartingPlayer;
        }

        private void Start()
        {
            if (!_starterLoaded)
            {
                _starterLoaded = true;
                _nextStartingPlayer = PlayerPrefs.GetInt(STARTER_PREF, 0) == 0
                    ? PlayerMark.X
                    : PlayerMark.O;
            }

            StartGame();
        }

        private void Update()
        {
            if (_isPlaying)
                _timer += Time.deltaTime;
        }

        public void StartGame()
        {
            _board = new PlayerMark[9];
            _currentPlayer = _nextStartingPlayer;
            _player1Moves = 0;
            _player2Moves = 0;
            _timer = 0f;
            _isPlaying = true;
            _state = GameState.Playing;


            _nextStartingPlayer = (_nextStartingPlayer == PlayerMark.X)
                ? PlayerMark.O
                : PlayerMark.X;

            PlayerPrefs.SetInt(STARTER_PREF, _nextStartingPlayer == PlayerMark.X ? 0 : 1);

            GameEvents.GameStarted();
        }

        private static void ResetStartingPlayer()
        {
            _nextStartingPlayer = PlayerMark.X;
            PlayerPrefs.SetInt(STARTER_PREF, 0);
        }

        private void HandleCellClicked(int cellIndex)
        {

            if (_state != GameState.Playing) return;


            if (_board[cellIndex] != PlayerMark.None) return;


            _board[cellIndex] = _currentPlayer;

            if (_currentPlayer == PlayerMark.X) _player1Moves++;
            else _player2Moves++;

            GameEvents.MarkPlaced(cellIndex, _currentPlayer);


            if (CheckWin(_currentPlayer, out int[] winLine))
            {
                EndGame(_currentPlayer, winLine);
            }
            else if (CheckDraw())
            {
                EndGame(PlayerMark.None, null);
            }
            else
            {

                _currentPlayer = (_currentPlayer == PlayerMark.X)
                    ? PlayerMark.O
                    : PlayerMark.X;

                GameEvents.TurnChanged(_currentPlayer);
            }
        }

        private bool CheckWin(PlayerMark player, out int[] winLine)
        {
            foreach (var pattern in WinPatterns)
            {
                if (_board[pattern[0]] == player &&
                    _board[pattern[1]] == player &&
                    _board[pattern[2]] == player)
                {
                    winLine = pattern;
                    return true;
                }
            }
            winLine = null;
            return false;
        }

        private bool CheckDraw()
        {
            foreach (var cell in _board)
                if (cell == PlayerMark.None) return false;
            return true;
        }

        private void EndGame(PlayerMark winner, int[] winLine)
        {
            _isPlaying = false;
            _state = GameState.GameOver;

            var result = new GameResult
            {
                Winner      = winner,
                Duration    = _timer,
                Player1Moves = _player1Moves,
                Player2Moves = _player2Moves,
                WinLine     = winLine
            };

            GameEvents.GameEnded(result);
        }
    }
}
