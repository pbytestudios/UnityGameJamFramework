using Pixelbyte.Extensions;
using TMPro;
using UnityEngine;

namespace Pixelbyte.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextThemer : BaseThemeable
    {
        [SerializeField] ThemeColor source = ThemeColor.Text;
        [SerializeField, Range(0, 1)] float alphaMultiplier = 1;
        TMP_Text obj;

        void ConnectComponents()
        {
            obj = GetComponent<TMP_Text>();
        }

        protected override void ChangeTheme(ThemeData data)
        {
            ConnectComponents();
            var c = GetColor(source, data);
            alphaMultiplier = Mathf.Clamp(alphaMultiplier, 0, 1);
            obj.color = c.Alpha(c.a * alphaMultiplier);
        }
    }
}
