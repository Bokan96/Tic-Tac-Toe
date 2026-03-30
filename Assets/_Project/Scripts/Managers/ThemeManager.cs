using UnityEngine;
using TicTacToe.Core;
using TicTacToe.Data;
using TicTacToe.Utilities;

namespace TicTacToe.Managers
{
    public class ThemeManager : SingletonMonoBehaviour<ThemeManager>
    {
        [Tooltip("The theme used on first launch, before the player picks one.")]
        [SerializeField] private ThemeData defaultTheme;

        private ThemeData _currentTheme;

        public ThemeData CurrentTheme => _currentTheme;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 120; // 120 max?
            _currentTheme = defaultTheme;
        }
        public void SetTheme(ThemeData theme)
        {
            _currentTheme = theme;
            GameEvents.ThemeChanged(theme);
        }
    }
}
