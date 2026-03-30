using UnityEngine;
using TicTacToe.Managers;

namespace TicTacToe.UI
{
    public class PlaySceneController : MonoBehaviour
    {

        public void OnPlayButtonClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            PopupController.Open(PopupType.ThemeSelection);
        }

        public void OnStatsButtonClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            PopupController.Open(PopupType.Statistics);
        }

        public void OnSettingsButtonClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            PopupController.Open(PopupType.Settings);
        }

        public void OnExitButtonClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            PopupController.Open(PopupType.ExitConfirmation);
        }
    }
}
