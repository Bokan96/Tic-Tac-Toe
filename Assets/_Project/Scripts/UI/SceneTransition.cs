using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TicTacToe.Utilities;

namespace TicTacToe.UI
{
    public class SceneTransition : SingletonMonoBehaviour<SceneTransition>
    {
        [SerializeField] private Image overlayImage;
        [SerializeField] private float fadeDuration = 0.4f;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (overlayImage != null)
            {
                overlayImage.gameObject.SetActive(true);
                SetAlpha(1f);
                StartCoroutine(FadeFromBlack());
            }
        }

        public void LoadScene(string sceneName)
        {
            gameObject.SetActive(true);
            
            if (overlayImage == null)
            {
                SceneManager.LoadScene(sceneName);
                return;
            }
            
            StartCoroutine(FadeAndLoad(sceneName));
        }

        private IEnumerator FadeAndLoad(string sceneName)
        {
            overlayImage.gameObject.SetActive(true);
            
            // trans u crno
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }
            SetAlpha(1f);

            SceneManager.LoadScene(sceneName);
            
            yield return null;

            // out iz crnog
            yield return StartCoroutine(FadeFromBlack());
        }

        private IEnumerator FadeFromBlack()
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(1f - Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }
            SetAlpha(0f);
            overlayImage.gameObject.SetActive(false);
        }

        private void SetAlpha(float alpha)
        {
            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = alpha;
                overlayImage.color = c;
            }
        }
    }
}
