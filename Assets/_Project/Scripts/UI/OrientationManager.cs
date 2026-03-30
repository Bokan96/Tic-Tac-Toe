using UnityEngine;
using UnityEngine.UI;
namespace TicTacToe.UI
{
    public class OrientationManager : MonoBehaviour
    {
        [Tooltip("The root GameObject containing all Portrait UI elements.")]
        [SerializeField] private GameObject portraitUI;
        
        [Tooltip("The root GameObject containing all Landscape UI elements.")]
        [SerializeField] private GameObject landscapeUI;

        [Tooltip("Audio clip to play when the device rotates.")]
        [SerializeField] private AudioClip rotationClip;

        private bool _isPortrait;
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            UpdateOrientation();
        }

        private void Update()
        {
            bool currentPortrait = Screen.height > Screen.width;
            
            if (currentPortrait != _isPortrait)
            {
                UpdateOrientation();
            }
        }

        private void UpdateOrientation()
        {
            _isPortrait = Screen.height > Screen.width;
            
            if (_canvasScaler != null && _canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                _canvasScaler.matchWidthOrHeight = _isPortrait ? 0f : 1f; // 0 = sirina, 1 = visina
            }
            
            var activePopups = PopupController.GetActivePopupTypes();
            
            PopupController.ForceCloseAll();
            
            if (portraitUI != null) portraitUI.SetActive(_isPortrait);
            if (landscapeUI != null) landscapeUI.SetActive(!_isPortrait);
            
            foreach (var pt in activePopups)
            {
                PopupController.Open(pt);
            }
            
            if (Managers.AudioManager.Instance != null && rotationClip != null)
            {
                Managers.AudioManager.Instance.PlaySFX(rotationClip);
            }
        }
    }
}
