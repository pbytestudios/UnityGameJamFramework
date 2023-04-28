using Pixelbyte.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelbyte.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonTintThemer : BaseThemeable
    {
        [SerializeField, Range(0, 1)] float alphaMultiplier = 1;

        Button btn;
        TMP_Text text;

        void ConnectComponent()
        {
            btn = GetComponent<Button>();
            text = btn?.GetComponentInChildren<TMP_Text>();
        }

        protected override void ChangeTheme(ThemeData data)
        {
            ConnectComponent();

            if (btn.transition != Selectable.Transition.ColorTint)
            {
                Dbg.Err("To use this themer, the button must be set to ColorTint!", gameObject);
                return;
            }

            var block = btn.colors;
            block.normalColor = data.accent1;
            block.highlightedColor = data.accent2;
            block.pressedColor = data.accent3;
            block.selectedColor = data.accent4;
            block.disabledColor = data.disabled;
            btn.colors = block;

            if (text != null)
            {
                var c = data.text;
                alphaMultiplier = Mathf.Clamp(alphaMultiplier, 0, 1);
                text.color = c.Alpha(c.a * alphaMultiplier);
            }
        }
    }
}
