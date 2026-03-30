using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Core;
using TicTacToe.Data;
using TicTacToe.Managers;

namespace TicTacToe.UI
{
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("UI/Effects/Theme Gradient Background")]
    public class ThemeGradientBackground : BaseMeshEffect
    {
        [Header("Gradient Settings")]
        [Tooltip("How high (0 = none, 1 = full screen) the player colour accent rises.")]
        [SerializeField, Range(0f, 1f)] private float gradientReach = 0.25f;

        [Tooltip("0 = soft fade across the whole reach zone. 1 = solid band with a sharp cutoff edge.")]
        [SerializeField, Range(0f, 1f)] private float gradientSharpness = 0.3f;

        private const int Subdivisions = 32;

        private Color _bottomColor = Color.gray;
        private Color _targetBottomColor = Color.gray;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (graphic != null) graphic.SetVerticesDirty();
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            GameEvents.OnThemeChanged += HandleThemeChanged;
            GameEvents.OnTurnChanged  += HandleTurnChanged;
            GameEvents.OnGameStarted  += HandleGameStarted;

            if (ThemeManager.Instance?.CurrentTheme != null)
                HandleThemeChanged(ThemeManager.Instance.CurrentTheme);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEvents.OnThemeChanged -= HandleThemeChanged;
            GameEvents.OnTurnChanged  -= HandleTurnChanged;
            GameEvents.OnGameStarted  -= HandleGameStarted;
        }

        private void Update()
        {
            if (graphic == null) return;

            if (_bottomColor != _targetBottomColor)
            {
                _bottomColor = Color.Lerp(_bottomColor, _targetBottomColor, Time.deltaTime * 5f);
                graphic.SetVerticesDirty();
            }
        }

        private void HandleGameStarted()
        {
            var gm = FindFirstObjectByType<GameManager>();
            PlayerMark starter = gm != null ? gm.CurrentPlayer : PlayerMark.X;

            _targetBottomColor = PlayerColor(starter);
            _bottomColor = _targetBottomColor; // snap immediately
            if (graphic != null) graphic.SetVerticesDirty();
        }

        private void HandleThemeChanged(ThemeData theme)
        {
            if (theme == null) return;
            var gm = FindFirstObjectByType<GameManager>();
            PlayerMark current = gm != null ? gm.CurrentPlayer : PlayerMark.None;
            if (current == PlayerMark.None) current = PlayerMark.X;

            _targetBottomColor = PlayerColor(current);
            _bottomColor = _targetBottomColor; // snap on theme change
            if (graphic != null) graphic.SetVerticesDirty();
        }

        private void HandleTurnChanged(PlayerMark playerMark)
        {
            if (ThemeManager.Instance?.CurrentTheme == null) return;
            _targetBottomColor = PlayerColor(playerMark);

        }

        private static Color PlayerColor(PlayerMark mark)
        {
            var theme = ThemeManager.Instance?.CurrentTheme;
            if (theme == null) return mark == PlayerMark.X ? Color.red : Color.blue;
            return mark == PlayerMark.X ? theme.player1Color : theme.player2Color;
        }

        private static Color WithAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);

        private Color SampleGradient(float t)
        {
            if (gradientReach <= 0f) return WithAlpha(_bottomColor, 0f);

            float fadeWidth = gradientReach * (1f - gradientSharpness);
            float solidTop  = gradientReach - fadeWidth;

            if (t >= gradientReach)  return WithAlpha(_bottomColor, 0f);
            if (fadeWidth <= 0f)     return t < gradientReach ? _bottomColor : WithAlpha(_bottomColor, 0f);
            if (t <= solidTop)       return _bottomColor;

            float localT = (t - solidTop) / fadeWidth;
            return Color.Lerp(_bottomColor, WithAlpha(_bottomColor, 0f), localT);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) return;

            Rect rect = graphic.rectTransform.rect;

            vh.Clear();

            for (int i = 0; i < Subdivisions; i++)
            {
                float t0 = i       / (float)Subdivisions;
                float t1 = (i + 1) / (float)Subdivisions;

                float y0 = Mathf.Lerp(rect.yMin, rect.yMax, t0);
                float y1 = Mathf.Lerp(rect.yMin, rect.yMax, t1);

                Color c0 = SampleGradient(t0);
                Color c1 = SampleGradient(t1);

                int idx = i * 4;

                UIVertex vert = UIVertex.simpleVert;

                vert.position = new Vector3(rect.xMin, y0); vert.color = c0; vh.AddVert(vert);
                vert.position = new Vector3(rect.xMax, y0); vert.color = c0; vh.AddVert(vert);
                vert.position = new Vector3(rect.xMax, y1); vert.color = c1; vh.AddVert(vert);
                vert.position = new Vector3(rect.xMin, y1); vert.color = c1; vh.AddVert(vert);

                vh.AddTriangle(idx,     idx + 1, idx + 2);
                vh.AddTriangle(idx,     idx + 2, idx + 3);
            }
        }
    }
}
