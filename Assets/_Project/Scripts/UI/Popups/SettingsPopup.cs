using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Managers;

namespace TicTacToe.UI.Popups
{
    public class SettingsPopup : PopupController
    {
        protected override PopupType Type => PopupType.Settings;

        [Header("BGM Settings")]
        [SerializeField] private Button bgmButton;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        [Header("SFX Settings")]
        [SerializeField] private Button sfxButton;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Sprite sfxOnSprite;
        [SerializeField] private Sprite sfxOffSprite;

        [Header("Close")]
        [SerializeField] private Button closeButton;

        [Header("Game Scene Extras")]
        [Tooltip("Optional button that only shows up when this popup is opened during a match.")]
        [SerializeField] private Button exitMenuButton;

        private Image _bgmImage;
        private Image _sfxImage;
        
        private Coroutine _bgmAnimCoroutine;
        private Coroutine _sfxAnimCoroutine;

        protected override void Awake()
        {
            base.Awake();

            _bgmImage = bgmButton.GetComponent<Image>();
            _sfxImage = sfxButton.GetComponent<Image>();

            closeButton.onClick.AddListener(OnCloseClicked);
            bgmButton.onClick.AddListener(OnToggleBGM);
            sfxButton.onClick.AddListener(OnToggleSFX);
            
            if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

            if (exitMenuButton != null)
                exitMenuButton.onClick.AddListener(OnExitMenuClicked);
        }

        protected override void OnOpened()
        {
            if (exitMenuButton != null)
            {
                bool inGame = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene";
                exitMenuButton.gameObject.SetActive(inGame);
            }

            RefreshUI();
            AudioManager.Instance.PlayPopupOpen();
        }

        private void OnToggleBGM()
        {
            AudioManager.Instance.PlayButtonClick();
            bool isCurrentlyEnabled = AudioManager.Instance.IsBGMEnabled && AudioManager.Instance.BGMVolume > 0f;
            float targetVolume = isCurrentlyEnabled ? 0f : 0.7f;
            
            if (_bgmAnimCoroutine != null) StopCoroutine(_bgmAnimCoroutine);
            _bgmAnimCoroutine = StartCoroutine(AnimateSlider(bgmSlider, AudioManager.Instance.BGMVolume, targetVolume, true));
        }

        private void OnToggleSFX()
        {
            AudioManager.Instance.PlayButtonClick();
            bool isCurrentlyEnabled = AudioManager.Instance.IsSFXEnabled && AudioManager.Instance.SFXVolume > 0f;
            float targetVolume = isCurrentlyEnabled ? 0f : 0.7f;
            
            if (_sfxAnimCoroutine != null) StopCoroutine(_sfxAnimCoroutine);
            _sfxAnimCoroutine = StartCoroutine(AnimateSlider(sfxSlider, AudioManager.Instance.SFXVolume, targetVolume, false));
        }

        private System.Collections.IEnumerator AnimateSlider(Slider slider, float startVal, float targetVal, bool isBGM)
        {
            float elapsed = 0f;
            float duration = 0.2f;

            if (isBGM) slider.onValueChanged.RemoveListener(OnBGMSliderChanged);
            else slider.onValueChanged.RemoveListener(OnSFXSliderChanged);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Ease out
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                
                float current = Mathf.Lerp(startVal, targetVal, t);
                slider.value = current;
                
                if (isBGM)
                {
                    AudioManager.Instance.SetBGMVolume(current);
                    if (current > 0f && !AudioManager.Instance.IsBGMEnabled) Core.GameEvents.BGMToggled(true);
                    else if (current <= 0f && AudioManager.Instance.IsBGMEnabled) Core.GameEvents.BGMToggled(false);
                }
                else
                {
                    AudioManager.Instance.SetSFXVolume(current);
                    if (current > 0f && !AudioManager.Instance.IsSFXEnabled) Core.GameEvents.SFXToggled(true);
                    else if (current <= 0f && AudioManager.Instance.IsSFXEnabled) Core.GameEvents.SFXToggled(false);
                }
                
                RefreshToggleIcons();
                yield return null;
            }

            slider.value = targetVal;
            
            if (isBGM) slider.onValueChanged.AddListener(OnBGMSliderChanged);
            else slider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        private void OnBGMSliderChanged(float value)
        {
            // Optional: Snap to 10% steps
            float snappedValue = Mathf.Round(value * 10f) / 10f;
            if (bgmSlider.value != snappedValue) bgmSlider.SetValueWithoutNotify(snappedValue);
            
            AudioManager.Instance.SetBGMVolume(snappedValue);
            if (snappedValue > 0f && !AudioManager.Instance.IsBGMEnabled)
                Core.GameEvents.BGMToggled(true);
            else if (snappedValue <= 0f && AudioManager.Instance.IsBGMEnabled)
                Core.GameEvents.BGMToggled(false);
                
            RefreshToggleIcons();
        }

        private void OnSFXSliderChanged(float value)
        {
            float snappedValue = Mathf.Round(value * 10f) / 10f;
            if (sfxSlider.value != snappedValue) sfxSlider.SetValueWithoutNotify(snappedValue);
            
            AudioManager.Instance.SetSFXVolume(snappedValue);
            if (snappedValue > 0f && !AudioManager.Instance.IsSFXEnabled)
                Core.GameEvents.SFXToggled(true);
            else if (snappedValue <= 0f && AudioManager.Instance.IsSFXEnabled)
                Core.GameEvents.SFXToggled(false);
                
            RefreshToggleIcons();
        }

        private void RefreshUI()
        {
            RefreshToggleIcons();
            
            if (bgmSlider != null) bgmSlider.SetValueWithoutNotify(AudioManager.Instance.BGMVolume);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
        }

        private void RefreshToggleIcons()
        {
            if (_bgmImage != null)
                _bgmImage.sprite = AudioManager.Instance.IsBGMEnabled && AudioManager.Instance.BGMVolume > 0f ? musicOnSprite : musicOffSprite;
            if (_sfxImage != null)
                _sfxImage.sprite = AudioManager.Instance.IsSFXEnabled && AudioManager.Instance.SFXVolume > 0f ? sfxOnSprite : sfxOffSprite;
        }

        private void OnCloseClicked()
        {
            AudioManager.Instance.PlayButtonClick();
            Close();
        }

        private void OnExitMenuClicked()
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
