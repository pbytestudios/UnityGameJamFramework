using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelbyte.UI
{
    public class Dialog : MonoBehaviour
    {
        [Tooltip("The game object that contains the buttons for the dialog")]
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI message;
        [SerializeField] GameObject buttonContainer;

        [SerializeField] bool focusFirstButton = true;
        [SerializeField] Vector2 popupAnchoredPos = Vector2.zero;

        [Tooltip("Click to Update the PopupPosition to the current position")]
        [SerializeField] bool updatePopupPosition;

        protected Button[] buttons;

        #region Properties
        public bool Visible { get; private set; }
        public Vector2 PopupPos => popupAnchoredPos;

        public string Title
        {
            get => title.text;
            set => title.text = value;
        }

        public string Message
        {
            get
            {
                if (message == null)
                    return string.Empty;
                return message.text;
            }
            set
            {
                if (message == null)
                    Dbg.Warn("Message text is not set for this Menu!");
                else
                    message.text = value;
            }
        }
        #endregion

        protected virtual void Awake()
        {
            if (buttonContainer == null)
            {
                Dbg.Log("The button container object was not set!", gameObject);
            }
            else
                buttons = buttonContainer.GetComponentsInChildren<Button>();

            if (message == null)
                Dbg.Log("Message object was not set!", gameObject);
            if (title == null)
                Dbg.Log("Title object was not set!", gameObject);
        }

        protected virtual void Start()
        {
            gameObject.SetActive(false);
        }

        TextMeshProUGUI GetText(Button btn) => btn.GetComponentInChildren<TextMeshProUGUI>();

        public virtual void SetButtons(params string[] buttonLabels)
        {
            if (buttonLabels == null)
            {
                Dbg.Err("You must set at least one button label!", gameObject);
                return;
            }
            else if (buttonLabels.Length > buttons.Length)
            {
                Dbg.Err($"This dialog has {buttons.Length} buttons. Not enough to set {buttonLabels.Length} labels!");
                Array.Resize(ref buttonLabels, buttons.Length);
            }

            for (int i = 0; i < buttonLabels.Length; i++)
            {
                buttons[i].gameObject.SetActive(true);
                var tm = GetText(buttons[i]);
                tm.text = buttonLabels[i];
            }

            for (int i = buttonLabels.Length; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        public virtual void SetCallbacks(params Action<Dialog>[] callbacks)
        {
            //remove any existing callbacks
            foreach (var btn in buttons)
                btn.onClick.RemoveAllListeners();

            if (callbacks == null)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].gameObject.activeSelf)
                        buttons[i].onClick.AddListener(() => Hide(null));
                }
                return;
            }

            if (callbacks.Length > buttons.Length)
            {
                Dbg.Err($"Dialog has {buttons.Length} buttons. Not enough to set {callbacks.Length} callbacks!");
                Array.Resize(ref callbacks, buttons.Length);
            }

            for (int i = 0; i < callbacks.Length; i++)
            {
                var callback = callbacks[i];
                buttons[i].onClick.AddListener(() => Hide(callback));
            }
        }

        public virtual void Hide(Action<Dialog> callback = null)
        {
            if (!Visible && callback == null)
                return;
            gameObject.SetActive(false);
            Visible = false;
            callback?.Invoke(this);
        }

        public virtual void Show(string msg = null) => Show(popupAnchoredPos, msg);
        public void Show(Vector2 anchoredPosition, string msg = null)
        {
            if (Visible)
                return;
            if (msg != null)
                Message = msg;
            if (focusFirstButton)
                buttons[0].Select();
            var rt = transform as RectTransform;
            rt.anchoredPosition = anchoredPosition;
            gameObject.SetActive(true);
            Visible = true;
        }

#if UNITY_EDITOR
        public void UpdateAnchoredPos()
        {
            var rt = transform as RectTransform;
            popupAnchoredPos = rt.anchoredPosition;
        }

        private void OnValidate()
        {
            if (updatePopupPosition)
            {
                UpdateAnchoredPos();
                updatePopupPosition = false;
            }
        }
#endif
    }
}
