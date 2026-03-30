using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI
{
    public enum PopupType
    {
        ThemeSelection,
        Statistics,
        Settings,
        ExitConfirmation,
        GameResult
    }

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PopupController : MonoBehaviour
    {
        protected abstract PopupType Type { get; }

        [Header("Animation Settings")]
        [SerializeField] private float animDuration = 0.22f;

        private CanvasGroup _canvasGroup;
        private RectTransform _rect;
        private Coroutine _animCoroutine;
        private Vector2 _restingAnchoredPos;
        private Vector3 _restingScale;

        private static List<PopupController> _allPopups = new List<PopupController>();

        public static void Open(PopupType targetType)
        {
            for (int i = _allPopups.Count - 1; i >= 0; i--)
            {
                var p = _allPopups[i];
                if (p == null) 
                {
                    _allPopups.RemoveAt(i);
                    continue;
                }

                if (p.Type == targetType && p.IsAttachedToActiveUI())
                    p.Show();
            }
        }

        public static void CloseAll()
        {
            for (int i = _allPopups.Count - 1; i >= 0; i--)
            {
                var p = _allPopups[i];
                if (p == null)
                {
                    _allPopups.RemoveAt(i);
                    continue;
                }

                if (p.gameObject.activeSelf && p.IsAttachedToActiveUI())
                    p.Close();
            }
        }

        public static void ForceCloseAll()
        {
            for (int i = _allPopups.Count - 1; i >= 0; i--)
            {
                var p = _allPopups[i];
                if (p == null)
                {
                    _allPopups.RemoveAt(i);
                    continue;
                }
                p.ForceClose();
            }
        }

        public static List<PopupType> GetActivePopupTypes()
        {
            var activeTypes = new List<PopupType>();
            foreach (var p in _allPopups)
            {
                if (p != null && p.gameObject.activeSelf && p.IsAttachedToActiveUI())
                {
                    if (!activeTypes.Contains(p.Type))
                    {
                        activeTypes.Add(p.Type);
                    }
                }
            }
            return activeTypes;
        }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rect = GetComponent<RectTransform>();

            Canvas.ForceUpdateCanvases();

            _restingAnchoredPos = _rect.anchoredPosition;
            _restingScale = _rect.localScale;
            _allPopups.Add(this);
            gameObject.SetActive(false); 
        }

        private void OnDestroy()
        {
            _allPopups.Remove(this);
        }

        private bool IsAttachedToActiveUI()
        {
            return transform.parent.gameObject.activeInHierarchy;
        }


        public void Show()
        {
            gameObject.SetActive(true);
            if (_animCoroutine != null) StopCoroutine(_animCoroutine);
            _animCoroutine = StartCoroutine(AnimateOpen());
            OnOpened();
        }

        public void Close()
        {
            if (_animCoroutine != null) StopCoroutine(_animCoroutine);
            _animCoroutine = StartCoroutine(AnimateClose());
        }

        public void ForceClose()
        {
            if (gameObject.activeInHierarchy && _animCoroutine != null)
                StopCoroutine(_animCoroutine);
            _animCoroutine = null;

            if (_rect != null)
            {
                _rect.anchoredPosition = _restingAnchoredPos;
                _rect.localScale = _restingScale;
            }
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
        }

        protected virtual void OnOpened() { }

        private const float DropOffset = 3000f;

        private IEnumerator AnimateOpen()
        {
            _canvasGroup.alpha = 0f;
            Vector2 offScreenPos = _restingAnchoredPos + new Vector2(0, -DropOffset);
            
            _rect.anchoredPosition = offScreenPos;
            _rect.localScale = _restingScale * 0.8f;

            float elapsed = 0f;
            while (elapsed < animDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animDuration);
                _canvasGroup.alpha = t;
                float easeT = EaseOutBack(t);
                _rect.anchoredPosition = Vector2.LerpUnclamped(offScreenPos, _restingAnchoredPos, easeT);
                _rect.localScale = Vector3.LerpUnclamped(_restingScale * 0.8f, _restingScale, easeT);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
            _rect.anchoredPosition = _restingAnchoredPos;
            _rect.localScale = _restingScale;
        }

        private IEnumerator AnimateClose()
        {
            float elapsed = 0f;
            Vector2 offScreenPos = _restingAnchoredPos + new Vector2(0, -DropOffset);

            while (elapsed < animDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animDuration);
                float easeT = t * t; // EaseInQuad
                
                _canvasGroup.alpha = 1f - t;
                _rect.anchoredPosition = Vector2.Lerp(_restingAnchoredPos, offScreenPos, easeT);
                _rect.localScale = Vector3.Lerp(_restingScale, _restingScale * 0.8f, easeT);
                yield return null;
            }

            _rect.anchoredPosition = _restingAnchoredPos;
            _rect.localScale = _restingScale;
            gameObject.SetActive(false);
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
