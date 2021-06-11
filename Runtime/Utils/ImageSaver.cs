using System.IO;
using UnityEngine;

namespace UIFigmaGradients
{
#if UNITY_EDITOR
using UnityEditor;
    [CustomEditor(typeof(ImageSaver))]
    public class ImageSaverEditor : Editor
    {
        private ImageSaver _Target;

        private void OnEnable()
        {
            _Target = target as ImageSaver;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Save Image"))
                _Target.SaveImage();
        }
    }

#endif

    [RequireComponent(typeof(UIFigmaGradientLinearDrawer))]
    public class ImageSaver : MonoBehaviour
    {
        // [EditorButton]
        public void SaveImage()
        {
            var testImagesPath = Path.Combine(Application.dataPath, "Figma Gradients/GeneratedGradientImages");
            var image = GetComponent<UIFigmaGradientLinearDrawer>();
            File.WriteAllBytes(
                Path.Combine(testImagesPath, $"{image.name}{image.GetHashCode()}.png"),
                (image.mainTexture as Texture2D).EncodeToPNG());
        }
    }
}