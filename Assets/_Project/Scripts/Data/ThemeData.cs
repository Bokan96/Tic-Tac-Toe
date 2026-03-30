using UnityEngine;

namespace TicTacToe.Data
{
    [CreateAssetMenu(fileName = "ThemeData", menuName = "TicTacToe/Theme")]
    public class ThemeData : ScriptableObject
    {
        [Header("Identity")]
        public string themeName;

        [Header("Mark Sprites")]
        public Sprite xSprite;
        public Sprite oSprite;

        [Header("Board Style")]
        public Sprite boardGridSprite;
        public Sprite strikeLineSprite;
        
        [Tooltip("Tints the board grid background image.")]
        public Color boardBackgroundColor = Color.white;
        public Color player1Color    = Color.red;
        public Color player2Color    = Color.blue;
        public Color gridColor       = Color.gray;
    }
}
