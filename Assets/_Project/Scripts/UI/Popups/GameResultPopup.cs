using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Core;
using TicTacToe.Managers;
using TicTacToe.Utilities;

namespace TicTacToe.UI.Popups
{
    public class GameResultPopup : PopupController
    {
        protected override PopupType Type => PopupType.GameResult;

        [Header("Juicy Delay Settings")]
        [Tooltip("Wrap all your Result Popup visual elements (Text, Buttons) in a single 'Content' child GO and drag it here!")]
        [SerializeField] private GameObject contentContainer; 
        
        [Header("UI Slots")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;

        private static GameResult? _lastResult;

        protected override void Awake()
        {
            base.Awake();
            retryButton.onClick.AddListener(OnRetryClicked);
            exitButton.onClick.AddListener(OnExitClicked);
            
            GameEvents.OnGameEnded   += ShowResult;
            GameEvents.OnGameStarted += HandleGameReset;
        }

        private void OnDestroy()
        {
            GameEvents.OnGameEnded   -= ShowResult;
            GameEvents.OnGameStarted -= HandleGameReset;
        }
        private void HandleGameReset()
        {
            _lastResult = null;
            StopAllCoroutines();
            if (contentContainer != null) contentContainer.SetActive(true);
            gameObject.SetActive(false);
        }


        private void ShowResult(GameResult result)
        {
            _lastResult = result;

            if (transform.parent != null && !transform.parent.gameObject.activeInHierarchy) return;

            gameObject.SetActive(true);
            
            if (contentContainer != null) contentContainer.SetActive(false);
            
            StartCoroutine(DelayedShowResult(result));
        }

        private System.Collections.IEnumerator DelayedShowResult(GameResult result)
        {
            yield return new WaitForSeconds(1.5f);

            PopulateResult(result);
            
            if (contentContainer != null) contentContainer.SetActive(true);
            
            Show();
        }

        protected override void OnOpened()
        {
            if (_lastResult.HasValue)
            {
                PopulateResult(_lastResult.Value);
                if (contentContainer != null) contentContainer.SetActive(true);
            }
        }
        private void PopulateResult(GameResult result)
        {
            titleText.color = Color.white;

            if (result.IsDraw)
            {
                titleText.text = "It's a Draw!";
            }
            else
            {
                string colorHex  = result.Winner == PlayerMark.X ? "FF0700" : "61C6FF";
                string playerName = result.Winner == PlayerMark.X ? "Red" : "Blue";
                titleText.text = $"<color=#{colorHex}>{playerName}</color> Wins!";
            }

            durationText.text = "Match duration: " + TimeUtils.FormatDuration(result.Duration);
        }
        private void OnRetryClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            Close();
            GameEvents.GameRestarted();
        }

        private void OnExitClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            Close();
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.LoadScene("PlayScene");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
            }
        }
    }
}
