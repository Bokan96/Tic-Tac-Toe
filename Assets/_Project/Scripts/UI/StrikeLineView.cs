using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Core;
using TicTacToe.Managers;

namespace TicTacToe.UI
{
    public class StrikeLineView : MonoBehaviour
    {
        [SerializeField] private Image strikeLineImage;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform[] cellRects = new RectTransform[9];


        private void Awake()
        {
            GameEvents.OnGameRestarted += HideLine;
            GameEvents.OnGameStarted  += HideLine;
            HideLine();
        }

        private void OnEnable()
        {
            GameEvents.OnGameEnded += HandleGameEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnGameEnded -= HandleGameEnded;
        }

        private void OnDestroy()
        {
            GameEvents.OnGameRestarted -= HideLine;
            GameEvents.OnGameStarted  -= HideLine;
            GameEvents.OnGameEnded    -= HandleGameEnded;
        }

        private void HideLine()
        {
            if (gameObject.activeInHierarchy)
                StopAllCoroutines();
            if (strikeLineImage) strikeLineImage.enabled = false;
        }

        private void HandleGameEnded(GameResult result)
        {
            if (result.IsDraw || result.WinLine == null || result.WinLine.Length < 3) return;

            StartCoroutine(ShowWinLineCoroutine(result));
        }

        private IEnumerator ShowWinLineCoroutine(GameResult result)
        {
            yield return new WaitForSeconds(0.4f);

            int startIdx = result.WinLine[0];
            int endIdx = result.WinLine[2];

            if (cellRects == null || cellRects.Length < 9) yield break;
            if (cellRects[startIdx] == null || cellRects[endIdx] == null) yield break;

            Vector3 startPos = cellRects[startIdx].position;
            Vector3 endPos = cellRects[endIdx].position;

            if (endPos.x < startPos.x)
            {
                Vector3 temp = startPos;
                startPos = endPos;
                endPos = temp;
            }

            Vector3 dir = endPos - startPos;
            Vector3 normDir = dir.normalized;
            float finalLength = dir.magnitude + 50f;

            Vector3 shiftedStart = startPos - normDir * 25f;

            rectTransform.pivot = new Vector2(0.1f, 0.5f);
            rectTransform.position = shiftedStart;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);

            if (ThemeManager.Instance?.CurrentTheme != null)
            {
                var theme = ThemeManager.Instance.CurrentTheme;
                strikeLineImage.sprite = theme.strikeLineSprite;
                strikeLineImage.color = result.Winner == PlayerMark.X
                    ? theme.player1Color
                    : theme.player2Color;
            }

            strikeLineImage.enabled = true;
            yield return StartCoroutine(AnimateLineRender(finalLength));
        }

        private IEnumerator AnimateLineRender(float finalLength)
        {
            float duration = 0.25f;
            float elapsed = 0f;

            rectTransform.sizeDelta = new Vector2(0f, rectTransform.sizeDelta.y);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float easedT = 1f - Mathf.Pow(1f - t, 3f);

                rectTransform.sizeDelta = new Vector2(finalLength * easedT, rectTransform.sizeDelta.y);
                yield return null;
            }

            rectTransform.sizeDelta = new Vector2(finalLength, rectTransform.sizeDelta.y);
        }
    }
}
