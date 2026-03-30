using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using TicTacToe.Managers;

namespace TicTacToe.UI.Popups
{
    public class ExitConfirmPopup : PopupController
    {
        protected override PopupType Type => PopupType.ExitConfirmation;

        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private bool _isGameScene;

        protected override void Awake()
        {
            base.Awake();
            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);
        }

        protected override void OnOpened()
        {
            AudioManager.Instance.PlayPopupOpen();
            
            _isGameScene = SceneManager.GetActiveScene().name == "GameScene";
            
            if (messageText != null)
            {
                messageText.text = _isGameScene 
                    ? "Are you sure you want to exit to the main menu?" 
                    : "Are you sure you want to exit the game?";
            }
        }

        private void OnConfirmClicked()
        {
            AudioManager.Instance.PlayButtonClick();

            if (_isGameScene)
            {
                if (SceneTransition.Instance != null)
                    SceneTransition.Instance.LoadScene("PlayScene");
                else
                    SceneManager.LoadScene("PlayScene");
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        private void OnCancelClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            Close();
        }
    }
}
