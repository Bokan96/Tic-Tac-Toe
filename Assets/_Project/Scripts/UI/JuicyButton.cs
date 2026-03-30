using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.UI
{
    public class JuicyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField] private float pressedScale = 0.9f;
        [SerializeField] private float animationSpeed = 15f;
        
        private Vector3 _originalScale;
        private Vector3 _targetScale;
        private bool _isPressed = false;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _targetScale = _originalScale;
        }

        private void OnEnable()
        {
            transform.localScale = _originalScale;
            _targetScale = _originalScale;
            _isPressed = false;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                _targetScale, 
                Time.deltaTime * animationSpeed
            );
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            _targetScale = _originalScale * pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
            _targetScale = _originalScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isPressed)
            {
                _targetScale = _originalScale * pressedScale;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isPressed)
            {
                _targetScale = _originalScale;
            }
        }
    }
}
