using System;
using TicTacToe.Data;

namespace TicTacToe.Core
{
    public static class GameEvents
    {
        public static event Action<int> OnCellClicked;
        public static event Action<int, PlayerMark> OnMarkPlaced;
        public static event Action<PlayerMark> OnTurnChanged;


        public static event Action OnGameStarted;
        public static event Action<GameResult> OnGameEnded;
        public static event Action OnGameRestarted;
        public static event Action OnReturnToMenu;


        public static event Action<ThemeData> OnThemeChanged;


        public static event Action<bool> OnBGMToggled;
        public static event Action<bool> OnSFXToggled;

        public static void CellClicked(int index)           => OnCellClicked?.Invoke(index);
        public static void MarkPlaced(int i, PlayerMark m) => OnMarkPlaced?.Invoke(i, m);
        public static void TurnChanged(PlayerMark m)      => OnTurnChanged?.Invoke(m);
        public static void GameStarted()                   => OnGameStarted?.Invoke();
        public static void GameEnded(GameResult result)    => OnGameEnded?.Invoke(result);
        public static void GameRestarted()                 => OnGameRestarted?.Invoke();
        public static void ReturnToMenu()                  => OnReturnToMenu?.Invoke();
        public static void ThemeChanged(ThemeData theme)   => OnThemeChanged?.Invoke(theme);
        public static void BGMToggled(bool on)             => OnBGMToggled?.Invoke(on);
        public static void SFXToggled(bool on)             => OnSFXToggled?.Invoke(on);
    }
}
