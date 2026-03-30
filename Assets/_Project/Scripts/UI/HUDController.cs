using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Core;
using TicTacToe.Managers;
using TicTacToe.Utilities;

namespace TicTacToe.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        
        [Header("UI Labels")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI p1MovesText;
        [SerializeField] private TextMeshProUGUI p2MovesText;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI p1NameText;
        [SerializeField] private TextMeshProUGUI p2NameText;



        [Header("Controls")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;



        private void Awake()
        {
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }

        }

        private void OnEnable()
        {
            GameEvents.OnMarkPlaced += HandleMarkPlaced;
            GameEvents.OnTurnChanged += HandleTurnChanged;
            GameEvents.OnThemeChanged += ApplyTheme;
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnGameEnded += HandleGameEnded;


            RefreshUI();
        }

        private void OnDisable()
        {
            GameEvents.OnMarkPlaced -= HandleMarkPlaced;
            GameEvents.OnTurnChanged -= HandleTurnChanged;
            GameEvents.OnThemeChanged -= ApplyTheme;
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnGameEnded -= HandleGameEnded;
        }

        private void Start()
        {
            if (ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
                ApplyTheme(ThemeManager.Instance.CurrentTheme);
                
            RefreshUI();
        }

        private void Update()
        {
            if (gameManager != null && timerText != null)
                timerText.text = TimeUtils.FormatDuration(gameManager.Timer);
        }


        private void HandleMarkPlaced(int _, PlayerMark __) => RefreshUI();
        private void HandleTurnChanged(PlayerMark _)       => RefreshUI();
        private void HandleGameStarted()                   => RefreshUI();

        private void HandleGameEnded(GameResult result)
        {
            // ipak ne
        }
        private void RefreshUI()
        {
            if (gameManager == null) return;

            if (p1MovesText) p1MovesText.text = $"Moves\n{gameManager.Player1Moves}";
            if (p2MovesText) p2MovesText.text = $"Moves\n{gameManager.Player2Moves}";
        }

        private void ApplyTheme(Data.ThemeData theme)
        {
            RefreshUI();
        }

        private void OnSettingsClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            PopupController.CloseAll();
            PopupController.Open(PopupType.Settings);
        }

        private void OnExitClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            PopupController.CloseAll();
            PopupController.Open(PopupType.ExitConfirmation);
        }

    }
}
