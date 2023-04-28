using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Pixelbyte.UI;
using Pixelbyte;
using TMPro;
namespace Pixelbyte.UI
{
    /// <summary>
    /// Given a Canvas and a prefab of the text object to use,
    /// This class allows us to create text objects in world space
    /// </summary>
    public class TextMgr : Singleton<TextMgr>
    {
        const string TXT_CONTAINER_NAME = "Texts";
        const int TXT_POOL_SIZE = 10;

        RenderMode canvasMode;
        public Canvas canvas = null;
        public TMP_Text textPrefab = null;

        GameObject _textContainer;
        List<TextMgrObject> _texts;
        List<TextMgrObject> _remove;

        protected override void Awake()
        {
            base.Awake();

            _textContainer = new GameObject(TXT_CONTAINER_NAME);
            _textContainer.transform.SetParent(canvas.transform, false);
            _textContainer.layer = canvas.gameObject.layer;

            canvasMode = canvas.renderMode;

            var rt = _textContainer.AddComponent<RectTransform>();
            _textContainer.transform.localPosition = Vector3.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            rt.sizeDelta = Vector2.zero;

            _texts = new List<TextMgrObject>();
            _remove = new List<TextMgrObject>();

            Pool.CreatePool(textPrefab, 0, TXT_POOL_SIZE);
        }

        void Update()
        {
            //Update the WorldText objects so they can move if needed
            if (_texts == null) return;
            for (int i = 0; i < _texts.Count; i++)
            {
                if (!_texts[i].Update(Time.deltaTime))
                    _remove.Add(_texts[i]);
            }

            if (_remove.Count > 0)
            {
                for (int i = 0; i < _remove.Count; i++)
                {
                    _texts.Remove(_remove[i]);
                }
                _remove.Clear();
            }
        }

        /// <summary>
        /// Create a new Text object using the textPrefab object.
        /// It uses the parameters in the WorldText object that is passed in
        /// Note that if the WorldText's lifetime is <= 0 then it will not be destroyed
        /// *IF YOU WANT TO DESTROY ONE OF THESE OBJECTS, USE DestroyText()!
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static TMP_Text NewMsg(TextMgrObject msg)
        {
            if (I.canvasMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 vpos = Camera.main.WorldToViewportPoint(msg.position);
                msg.position = vpos;
            }

            var txt = Pool.Spawn(I.textPrefab, msg.position, I._textContainer.transform);
            txt.transform.localScale = Vector3.one;

            //if (I.canvasMode == RenderMode.ScreenSpaceCamera)
            //{
            //    var rt = txt.transform as RectTransform;
            //    rt.anchorMax = msg.position;
            //    rt.anchorMin = msg.position;
            //}
            //else
            txt.transform.position = msg.position;

            msg.Connect(txt);
            I._texts.Add(msg);
            return txt;
        }

        /// <summary>
        /// Call this method with the given Text object you'd like to
        /// destroy and if it is in the list, it will be recycled
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool RecycleText(TMP_Text obj)
        {
            if (!Exists || obj == null || applicationQuitting) return false;

            for (int i = 0; i < I._texts.Count; i++)
            {
                if (I._texts[i].guiTxt == obj)
                {
                    I._texts.Remove(I._texts[i]);
                    Pool.Recycle(obj);
                    return true;
                }
            }
            return false; //couldn't find the text object
        }

        public static void RecycleAllTextObjects()
        {
            for (int i = I._texts.Count - 1; i >= 0; i--)
            {
                RecycleText(I._texts[i].guiTxt);
            }
            I._texts.Clear();
        }
    }
}
