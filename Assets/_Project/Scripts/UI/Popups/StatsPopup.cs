using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Managers;
using TicTacToe.Utilities;

namespace TicTacToe.UI.Popups
{
    public class StatsPopup : PopupController
    {
        protected override PopupType Type => PopupType.Statistics;

        [Header("Stat Labels")]
        [SerializeField] private TextMeshProUGUI totalGamesText;
        [SerializeField] private TextMeshProUGUI player1WinsText;
        [SerializeField] private TextMeshProUGUI player2WinsText;
        [SerializeField] private TextMeshProUGUI drawsText;
        [SerializeField] private TextMeshProUGUI avgDurationText;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button resetButton;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(OnCloseClicked);
            resetButton.onClick.AddListener(OnResetClicked);
        }

        protected override void OnOpened()
        {
            AudioManager.Instance.PlayPopupOpen();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            var d = StatisticsManager.Instance.Data;
            totalGamesText.text  = d.totalGames.ToString();
            player1WinsText.text = d.player1Wins.ToString();
            player2WinsText.text = d.player2Wins.ToString();
            drawsText.text       = d.draws.ToString();
            avgDurationText.text = TimeUtils.FormatDuration(d.AverageGameDuration);
        }

        private void OnCloseClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            Close();
        }

        private void OnResetClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            StatisticsManager.Instance.ResetStats();
            RefreshDisplay();
        }

    }
}
