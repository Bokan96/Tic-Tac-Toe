using UnityEngine;
using System.Collections;

namespace TicTacToe.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class MenuBackgroundScroller : MonoBehaviour
    {
        [Header("Position Settings")]
        [Tooltip("Starting X position relative to current pivot.")]
        [SerializeField] private float startX = -300f;
        
        [Tooltip("Ending X position relative to current pivot.")]
        [SerializeField] private float endX = 300f;

        [Header("Animation Settings")]
        [Tooltip("Duration in seconds for the move from start to end.")]
        [SerializeField] private float duration = 40f;

        private RectTransform _rectTransform;
        private Vector2 _initialPosition;
        private Coroutine _animationCoroutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(AnimateBackground());
        }

        private void OnDisable()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
        }

        private IEnumerator AnimateBackground()
        {
            bool movingForward = true;

            while (true)
            {
                float elapsed = 0f;
                float currentStart = movingForward ? startX : endX;
                float currentEnd = movingForward ? endX : startX;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / duration);
                    
                    float t = Mathf.SmoothStep(0f, 1f, progress);
                    
                    float x = Mathf.Lerp(currentStart, currentEnd, t);
                    _rectTransform.anchoredPosition = new Vector2(x, _initialPosition.y);
                    
                    yield return null;
                }

                _rectTransform.anchoredPosition = new Vector2(currentEnd, _initialPosition.y);
                
                movingForward = !movingForward;
                
                yield return null;
            }
        }
    }
}
