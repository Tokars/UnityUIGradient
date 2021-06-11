using UnityEngine;

namespace UIFigmaGradients
{
    public class UIFigmaGradinetDiamondDrawer : UIFigmaGradientRadialDrawer
    {
        protected override Material GradientMaterial => new Material(Shader.Find("UI/DiamondGradientShader"));
    }
}