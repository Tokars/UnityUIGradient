﻿using UnityEngine;
using UnityEngine.UI;

namespace UIFigmaGradients
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(UIFigmaGradientAngularDrawer))]
    public class UIFigmaGradientAngularDrawerEditor : Editor
    {
        private UIFigmaGradientAngularDrawer _Target;

        private void OnEnable()
        {
            _Target = target as UIFigmaGradientAngularDrawer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Process Gradient"))
                _Target.ProcessGradient();
        }
    }

#endif


    public class UIFigmaGradientAngularDrawer : UIFigmaGradientLinearDrawer
    {
        [SerializeField] protected Vector2 _Center;
        protected override Material GradientMaterial => new Material(Shader.Find("UI/AngularGradientShader"));
        protected override TextureWrapMode WrapMode => TextureWrapMode.Repeat;


        protected override void GenerateHelperUvs(VertexHelper vh)
        {
            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.normal = new Vector3(_Center.x, 1 - _Center.y, angle);
                vh.SetUIVertex(vert, i);
            }
        }
#if UNITY_EDITOR

        // [EditorButton]
        public void ProcessGradient()
        {
            var colorKeys = gradient.colorKeys;
            var alphaKeys = gradient.alphaKeys;
            var lastColorKey = gradient.colorKeys[colorKeys.Length - 1];
            if (lastColorKey.time < 1)
            {
                var newColorKeys = new GradientColorKey[colorKeys.Length + 1];
                var newAlphaKeys = new GradientAlphaKey[alphaKeys.Length + 1];
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    newColorKeys[i] = colorKeys[i];
                }

                for (int i = 0; i < alphaKeys.Length; i++)
                {
                    newAlphaKeys[i] = alphaKeys[i];
                }

                newColorKeys[newColorKeys.Length - 1] = colorKeys[0];
                newAlphaKeys[newAlphaKeys.Length - 1] = alphaKeys[0];
                newColorKeys[newColorKeys.Length - 1].time = 1;
                newAlphaKeys[newAlphaKeys.Length - 1].time = 1;
                gradient.colorKeys = newColorKeys;
                gradient.alphaKeys = newAlphaKeys;
                OnValidate();
            }
        }
#endif
    }
}