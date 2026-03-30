using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TicTacToe.Data;
using TicTacToe.Managers;

namespace TicTacToe.UI.Popups
{
    public class ThemeSelectPopup : PopupController
    {
        protected override PopupType Type => PopupType.ThemeSelection;

        [Header("Master Preview Configuration")]
        [Tooltip("The big image that shows the X icon of the selected theme")]
        [SerializeField] private Image masterXPreview;
        [Tooltip("The big image that shows the O icon of the selected theme")]
        [SerializeField] private Image masterOPreview;
        [Tooltip("The image that previews the board grid for the selected theme")]
        [SerializeField] private Image masterGridPreview;

        [Header("Menu Elements")]
        [SerializeField] private ThemeOptionCard[] themeCards;
        [SerializeField] private Button startButton;
        [SerializeField] private Button closeButton;

        private ThemeData _selectedTheme;

        protected override void Awake()
        {
            base.Awake();
            
            if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
            if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);

            if (themeCards != null)
            {
                foreach (var card in themeCards)
                {
                    if (card != null) card.OnSelected += OnThemeCardSelected;
                }
            }
        }

        protected override void OnOpened()
        {
            AudioManager.Instance.PlayPopupOpen();

            _selectedTheme = ThemeManager.Instance.CurrentTheme;
            UpdateSelectionUI();
        }

        private void OnThemeCardSelected(ThemeData theme)
        {
            _selectedTheme = theme;
            UpdateSelectionUI();
        }

        private void UpdateSelectionUI()
        {
            foreach (var card in themeCards)
                card.SetSelected(card.Theme == _selectedTheme);

            if (_selectedTheme != null)
            {
                if (masterXPreview) 
                {
                    masterXPreview.sprite = _selectedTheme.xSprite;
                    masterXPreview.color = _selectedTheme.player1Color;
                }
                if (masterOPreview) 
                {
                    masterOPreview.sprite = _selectedTheme.oSprite;
                    masterOPreview.color = _selectedTheme.player2Color;
                }
                if (masterGridPreview)
                {
                    masterGridPreview.sprite = _selectedTheme.boardGridSprite;
                    masterGridPreview.color = _selectedTheme.gridColor;
                }
            }
        }

        private void OnStartClicked()
        {
            if (_selectedTheme == null) return;
            
            AudioManager.Instance.PlayButtonClick();
            ThemeManager.Instance.SetTheme(_selectedTheme);
            
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.LoadScene("GameScene");
            }
            else
            {
                SceneManager.LoadScene("GameScene");
            }
        }

        private void OnCloseClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            Close();
        }
    }
}
