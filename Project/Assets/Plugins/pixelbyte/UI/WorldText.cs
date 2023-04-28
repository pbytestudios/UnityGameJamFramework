using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pixelbyte;

public class WorldText
{
    public string msg;
    public Vector3 position;
    public float liveTime;
    public Text guiTxt;
    public Vector2 speed = Vector2.zero;
    public Color color;

    /// <summary>
    /// This is the rect transform of the guiText object
    /// </summary>
    protected RectTransform rt;

    public WorldText(Vector3 position, float liveTime, Vector2 speed, Color txtColor, string text, params object[] args)
    {
        //if (args != null && args.Length > 0)
        msg = string.Format(text, args);
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

    public WorldText(Vector3 position, float liveTime, Vector2 speed, string text, params object[] args) :
        this(position, liveTime, speed, Color.white, text, args) { }

    public WorldText(Vector3 position, float liveTime, Color txtColor, string text, params object[] args) :
        this(position, liveTime, Vector2.zero, txtColor, text, args) { }

    public WorldText(Vector3 position, float liveTime, string text, params object[] args) :
        this(position, liveTime, Vector2.zero, Color.white, text, args) { }

    //Starting Text object properties
    int originalFontSize;
    Color originalFontColor;
    TextAnchor originalFontAlignment;
    Font originalFont;

    public void Connect(Text txt)
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

    public bool Update(float deltaTime)
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