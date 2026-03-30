using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Core;
using TicTacToe.Data;

namespace TicTacToe.UI
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private int cellIndex;
        [SerializeField] private Image markImage;
        [SerializeField] private Button button;

        private PlayerMark _currentMark = PlayerMark.None;

        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnCellClicked);
                
            ResetCell();
        }

        public void Setup(int index)
        {
            cellIndex = index;
        }

        private void OnCellClicked()
        {
            GameEvents.CellClicked(cellIndex);
        }

        public void ApplyTheme(ThemeData theme)
        {
            if (_currentMark == PlayerMark.None || theme == null) return;
            markImage.sprite = _currentMark == PlayerMark.X ? theme.xSprite : theme.oSprite;
            markImage.color  = _currentMark == PlayerMark.X ? theme.player1Color : theme.player2Color;
        }

        public void SetMark(PlayerMark mark, Sprite sprite, Color color)
        {
            if (mark == PlayerMark.None)
            {
                ResetCell();
                return;
            }

            _currentMark = mark;
            markImage.sprite = sprite;
            markImage.color = color;
            markImage.enabled = true;
            
            StartCoroutine(AnimateDraw());

            button.interactable = false;
        }

        private System.Collections.IEnumerator AnimateDraw()
        {
            float duration = 0.25f;
            float elapsed = 0f;
            
            markImage.transform.localScale = Vector3.zero;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;
                float easedT = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                
                markImage.transform.localScale = Vector3.one * 2f * easedT;
                yield return null;
            }
            
            markImage.transform.localScale = Vector3.one * 2f;
        }

        public void ResetCell()
        {
            _currentMark = PlayerMark.None;
            markImage.enabled = false;
            button.interactable = true;
        }

        public void SetMarkImmediate(PlayerMark mark, Sprite sprite, Color color)
        {
            if (mark == PlayerMark.None)
            {
                ResetCell();
                return;
            }

            _currentMark = mark;
            markImage.sprite = sprite;
            markImage.color = color;
            markImage.enabled = true;
            markImage.transform.localScale = Vector3.one * 2f;
            button.interactable = false;
        }
    }
}
