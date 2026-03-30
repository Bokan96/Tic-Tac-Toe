using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Data;
using TicTacToe.Managers;

namespace TicTacToe.UI.Popups
{
    public class ThemeOptionCard : MonoBehaviour
    {
        [SerializeField] private ThemeData theme;
        [Tooltip("The sprite to apply when this card is selected.")]
        [SerializeField] private Sprite selectedSprite;

        private Image targetImage;
        private Sprite originalSprite;

        public ThemeData Theme => theme;
        public event Action<ThemeData> OnSelected;

        private void Awake()
        {
            targetImage = GetComponent<Image>();
            if (targetImage != null)
            {
                originalSprite = targetImage.sprite;
            }

            var label = GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && theme != null)
                label.text = theme.themeName;

            var button = GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    AudioManager.Instance.PlayButtonClick();
                    OnSelected?.Invoke(theme);
                });
            }
        }

        public void SetSelected(bool selected)
        {
            if (targetImage != null && selectedSprite != null)
            {
                targetImage.sprite = selected ? selectedSprite : originalSprite;
            }
        }
    }
}
