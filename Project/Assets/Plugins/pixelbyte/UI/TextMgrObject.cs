using TMPro;
using UnityEngine;

namespace Pixelbyte.UI
{
    public class TextMgrObject
    {
        public string msg;
        public Vector3 position;
        public float liveTime;
        public TMP_Text guiTxt;
        public Vector2 speed = Vector2.zero;
        public Color color;

        /// <summary>
        /// This is the rect transform of the guiText object
        /// </summary>
        protected RectTransform rt;

        public TextMgrObject(Vector3 position, float liveTime, Vector2 speed, Color txtColor, string text)
        {
            //if (args != null && args.Length > 0)
            msg = text;
            this.position = position;
            this.speed = speed;
            color = txtColor;
            if (liveTime <= 0)
            {
                //Dbg.Warn("LiveTime <=0 means velocity will be 0 also");
                this.speed = Vector2.zero;
            }
            this.liveTime = liveTime;
        }

        public TextMgrObject(Vector3 position, float liveTime, Vector2 speed, string text) :
            this(position, liveTime, speed, Color.white, text)
        { }

        public TextMgrObject(Vector3 position, float liveTime, Color txtColor, string text) :
            this(position, liveTime, Vector2.zero, txtColor, text)
        { }

        public TextMgrObject(Vector3 position, float liveTime, string text) :
            this(position, liveTime, Vector2.zero, Color.white, text)
        { }

        //Starting Text object properties
        float originalFontSize;
        Color originalFontColor;
        TextAlignmentOptions originalFontAlignment;
        TMP_FontAsset originalFont;

        public virtual void Connect(TMP_Text txt)
        {
            guiTxt = txt;
            txt.text = msg;

            //Save the properties for later recycling
            originalFontSize = guiTxt.fontSize;
            originalFontColor = guiTxt.color;
            originalFontAlignment = guiTxt.alignment;
            originalFont = guiTxt.font;

            //Now set any desired new properties
            guiTxt.color = color;
            //Go ahead and start fading the alpha of the text
            if (liveTime > 0)
                guiTxt.CrossFadeAlpha(0, liveTime, false);

            rt = guiTxt.transform as RectTransform;
            rt.transform.position = position;
        }

        public virtual bool Update(float deltaTime)
        {
            if (liveTime > 0)
            {
                liveTime -= deltaTime;

                //Move the text if velocity is not 0
                var p = rt.transform.position;
                p = p + new Vector3(speed.x, speed.y) * deltaTime;
                rt.transform.position = p;

                if (liveTime <= 0)
                {
                    Pool.Recycle(guiTxt);

                    //Restore all original text settings
                    guiTxt.fontSize = originalFontSize;
                    guiTxt.color = originalFontColor;
                    guiTxt.alignment = originalFontAlignment;
                    guiTxt.font = originalFont;

                    guiTxt = null;
                    return false;
                }
            }
            return true;
        }
    } 
}