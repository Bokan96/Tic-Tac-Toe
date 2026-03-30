using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Core;
using TicTacToe.Managers;

namespace TicTacToe.UI
{
    public class BoardController : MonoBehaviour
    {
        [Tooltip("Assign the 9 cells in order (0 to 8, top-left to bottom-right)")]
        [SerializeField] private CellView[] cells = new CellView[9];
        
        [Tooltip("The main background of the 3x3 grid")]
        [SerializeField] private Image boardBackground;

        [SerializeField] private GameManager gameManager;

        private void OnEnable()
        {
            GameEvents.OnMarkPlaced += HandleMarkPlaced;
            GameEvents.OnGameRestarted += ResetBoard;
            GameEvents.OnThemeChanged += ApplyTheme;
            GameEvents.OnGameStarted += ResetBoard;

            SyncBoardState();
        }

        private void OnDisable()
        {
            GameEvents.OnMarkPlaced -= HandleMarkPlaced;
            GameEvents.OnGameRestarted -= ResetBoard;
            GameEvents.OnThemeChanged -= ApplyTheme;
            GameEvents.OnGameStarted -= ResetBoard;
        }

        private void Start()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] != null) cells[i].Setup(i);
            }

            if (ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
            {
                ApplyTheme(ThemeManager.Instance.CurrentTheme);
            }
        }

        private void HandleMarkPlaced(int cellIndex, PlayerMark mark)
        {
            if (cellIndex < 0 || cellIndex >= cells.Length || cells[cellIndex] == null) return;

            var theme = ThemeManager.Instance.CurrentTheme;
            Sprite sprite = mark == PlayerMark.X ? theme.xSprite : theme.oSprite;
            Color color = mark == PlayerMark.X ? theme.player1Color : theme.player2Color;

            cells[cellIndex].SetMark(mark, sprite, color);
        }

        private void ApplyTheme(Data.ThemeData theme)
        {
            if (boardBackground != null)
            {
                boardBackground.sprite = theme.boardGridSprite;
                boardBackground.color = theme.gridColor;
            }

            foreach (var cell in cells)
                if (cell != null) cell.ApplyTheme(theme);
        }

        private void ResetBoard()
        {
            foreach (var cell in cells)
            {
                if (cell != null) cell.ResetCell();
            }
        }

        private void SyncBoardState()
        {
            if (gameManager == null) return;

            var theme = ThemeManager.Instance?.CurrentTheme;
            if (theme == null) return;

            PlayerMark[] boardState = gameManager.GetBoardState();

            for (int i = 0; i < cells.Length && i < boardState.Length; i++)
            {
                if (cells[i] == null) continue;

                if (boardState[i] != PlayerMark.None)
                {
                    Sprite sprite = boardState[i] == PlayerMark.X ? theme.xSprite : theme.oSprite;
                    Color color = boardState[i] == PlayerMark.X ? theme.player1Color : theme.player2Color;
                    cells[i].SetMarkImmediate(boardState[i], sprite, color);
                }
                else
                {
                    cells[i].ResetCell();
                }
            }
        }
    }
}
