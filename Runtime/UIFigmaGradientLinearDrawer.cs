using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigmaGradients
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIFigmaGradientLinearDrawer : MaskableGraphic
    {
        [SerializeField] protected Gradient gradient = new Gradient();
        [SerializeField] private GradientResolution gradientResolution = GradientResolution.k256;
        [SerializeField] protected float angle = 180;

        private Texture2D _gradientTexture;
        protected virtual TextureWrapMode WrapMode => TextureWrapMode.Clamp;
        protected virtual Material GradientMaterial => new Material(Shader.Find("UI/LinearGradientShader"));
        public override Texture mainTexture => _gradientTexture;

        public Gradient Gradient
        {
            get => gradient;
            set => gradient = value;
        }

        public float Angle
        {
            get => angle;
            set => angle = value;
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Refresh();
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            Refresh();
        }

        public Texture2D GenerateTexture(bool makeNoLongerReadable = false)
        {
            Texture2D tex = new Texture2D(1, (int)gradientResolution, TextureFormat.ARGB32, false, true);
            tex.wrapMode = WrapMode;
            tex.filterMode = FilterMode.Bilinear;
            tex.anisoLevel = 1;
            Color[] colors = new Color[(int)gradientResolution];
            float div = (float)(int)gradientResolution;
            for (int i = 0; i < (int)gradientResolution; ++i)
            {
                float t = (float)i / div;
                colors[i] = gradient.Evaluate(t);
            }

            tex.SetPixels(colors);
            tex.Apply(false, makeNoLongerReadable);

            return tex;
        }

        public void Refresh()
        {
            if (_gradientTexture != null)
            {
                DestroyImmediate(_gradientTexture);
            }

            material = GradientMaterial;
            _gradientTexture = GenerateTexture();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_gradientTexture != null)
            {
                DestroyImmediate(_gradientTexture);
            }
        }

        protected virtual void GenerateHelperUvs(VertexHelper vh)
        {
            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.uv1 = new Vector2(angle, angle);
                vh.SetUIVertex(vert, i);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            GenerateHelperUvs(vh);
        }

        public virtual void ParseCss(string css)
        {
            var parameters = UIFigmaGradientTools.ParseLinearCssParams(css);
            var angle = parameters[0].Trim().Replace("deg", "");
            this.angle = float.Parse(angle, NumberStyles.Any, CultureInfo.InvariantCulture);
            List<GradientColorKey> colorKeys = new List<GradientColorKey>();
            List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();
            for (int i = 1; i < parameters.Count; i++)
            {
                float time = 0;
                var col = UIFigmaGradientTools.ParseColor(parameters[i], out time);
                var colorKey = new GradientColorKey();
                colorKey.color = col;
                colorKey.time = time;
                var alphaKey = new GradientAlphaKey();
                alphaKey.alpha = col.a;
                alphaKey.time = time;
                colorKeys.Add(colorKey);
                alphaKeys.Add(alphaKey);
            }

            gradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            OnValidate();
        }
    }
}