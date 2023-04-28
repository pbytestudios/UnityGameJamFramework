using Pixelbyte.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelbyte.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageColorThemer : BaseThemeable
    {
        [SerializeField] ThemeColor source = ThemeColor.Primary;
        [SerializeField, Range(0, 1)] float alphaMultiplier = 1;
        Image img;

        void ConnectComponents()
        {
            img = GetComponent<Image>();
        }

        protected override void ChangeTheme(ThemeData data)
        {
            ConnectComponents();
            var c = GetColor(source, data);
            alphaMultiplier = Mathf.Clamp(alphaMultiplier, 0, 1);
            img.color = c.Alpha(c.a * alphaMultiplier);
        }
    }
}
