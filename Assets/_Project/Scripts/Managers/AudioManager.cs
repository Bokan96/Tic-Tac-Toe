using UnityEngine;
using TicTacToe.Core;
using TicTacToe.Utilities;

namespace TicTacToe.Managers
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        private AudioSource bgmSource;
        private AudioSource sfxSource;

        [Header("SFX Clips — drag in the Inspector")]
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip placementClip;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip drawClip;
        [SerializeField] private AudioClip popupOpenClip;

        private const string BGM_PREF = "BGMEnabled";
        private const string SFX_PREF = "SFXEnabled";
        private const string BGM_VOL_PREF = "BGMVolume";
        private const string SFX_VOL_PREF = "SFXVolume";

        private bool _bgmEnabled;
        private bool _sfxEnabled;
        private float _bgmVolume = 0.7f;
        private float _sfxVolume = 0.7f;

        public bool IsBGMEnabled => _bgmEnabled;
        public bool IsSFXEnabled => _sfxEnabled;
        public float BGMVolume => _bgmVolume;
        public float SFXVolume => _sfxVolume;

        protected override void Awake()
        {
            base.Awake();

            var sources = GetComponents<AudioSource>();
            bgmSource = sources[0];
            sfxSource = sources[1];

            _bgmEnabled = PlayerPrefs.GetInt(BGM_PREF, 1) == 1;
            _sfxEnabled = PlayerPrefs.GetInt(SFX_PREF, 1) == 1;
            
            _bgmVolume = PlayerPrefs.GetFloat(BGM_VOL_PREF, 0.7f);
            _sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_PREF, 0.7f);

            ApplyBGMState();
            ApplySFXState();
        }

        private void OnEnable()
        {
            GameEvents.OnBGMToggled  += SetBGMEnabled;
            GameEvents.OnSFXToggled  += SetSFXEnabled;
            GameEvents.OnMarkPlaced  += HandleMarkPlaced;
            GameEvents.OnGameEnded   += HandleGameEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnBGMToggled  -= SetBGMEnabled;
            GameEvents.OnSFXToggled  -= SetSFXEnabled;
            GameEvents.OnMarkPlaced  -= HandleMarkPlaced;
            GameEvents.OnGameEnded   -= HandleGameEnded;
        }

        public void PlaySFX(AudioClip clip)
        {
            if (!_sfxEnabled || clip == null) return;

            sfxSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(clip);
        }

        public void PlayButtonClick() => PlaySFX(buttonClickClip);
        public void PlayPopupOpen()   => PlaySFX(popupOpenClip);
        private void HandleMarkPlaced(int _, PlayerMark __) => PlaySFX(placementClip);

        private void HandleGameEnded(GameResult result)
        {
            if (result.IsDraw)
            {
                PlaySFX(drawClip);
            }
            else
            {
                StartCoroutine(PlayWinSFXDelayed());
            }
        }

        private System.Collections.IEnumerator PlayWinSFXDelayed()
        {
            yield return new WaitForSeconds(0.4f);
            PlaySFX(winClip);
        }

        private void SetBGMEnabled(bool enabled)
        {
            _bgmEnabled = enabled;
            PlayerPrefs.SetInt(BGM_PREF, enabled ? 1 : 0);
            ApplyBGMState();
        }

        private void SetSFXEnabled(bool enabled)
        {
            _sfxEnabled = enabled;
            PlayerPrefs.SetInt(SFX_PREF, enabled ? 1 : 0);
            ApplySFXState();
        }

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(BGM_VOL_PREF, _bgmVolume);
            ApplyBGMState();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFX_VOL_PREF, _sfxVolume);
            ApplySFXState();
        }

        private void ApplyBGMState()
        {
            if (bgmSource == null) return;
            
            bgmSource.volume = _bgmVolume;
            if (_bgmEnabled && _bgmVolume > 0f)
            {
                bgmSource.enabled = true;
                if (!bgmSource.isPlaying)
                    bgmSource.Play();
            }
            else
            {
                bgmSource.Stop();
            }
        }
        
        private void ApplySFXState()
        {
            if (sfxSource == null) return;
            sfxSource.volume = _sfxVolume;
        }
    }
}
