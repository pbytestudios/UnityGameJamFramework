using Pixelbyte.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelbyte.UI
{
    public class ColorThemer : BaseThemeable
    {
        [SerializeField] ThemeColor source = ThemeColor.Primary;
        [SerializeField, Range(0, 1)] float alphaMultiplier = 1;
        [SerializeField] UnityObservable<Color> color;

        protected override void ChangeTheme(ThemeData data)
        {
            var c = GetColor(source, data);
            alphaMultiplier = Mathf.Clamp(alphaMultiplier, 0, 1);
            c = c.Alpha(c.a * alphaMultiplier);
            color.Invoke(c);
        }
    }
}
