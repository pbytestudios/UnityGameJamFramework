using System;
using UnityEngine;

namespace Pixelbyte.UI
{
    public class Modal : Dialog
    {
        /// <summary>
        /// This lets us have more than one modal dialog open at a time
        /// </summary>
        protected static int modalsOpen;

        public static int ModalsOpen => modalsOpen;

        static Modal() => modalsOpen = 0;

        /// <summary>
        /// This will be enabled when the dialog is shown
        /// It is intended to be a UI element that covers the entire screen and
        /// sits behind the Modal dialog to both draw attention to it and
        /// prevent extranneous clicks from getting to other objects
        /// </summary>
        [SerializeField] GameObject veil;

        public bool VeilVisible
        {
            get
            {
                if (veil == null)
                    return false;
                else
                    return veil.activeSelf;
            }
        }

        public GameObject Veil => veil;

#if UNITY_EDITOR
        public void SetVeil(GameObject v) => veil = v;
#endif

        public override void Show(string msg = null)
        {
            if (Visible) return;
            veil?.SetActive(true);
            modalsOpen++;
            base.Show(msg);
        }

        public override void Hide(Action<Dialog> callback = null)
        {
            if (!Visible) return;
            modalsOpen = Mathf.Max(0, modalsOpen - 1);
            //Dbg.Log($"Modals Still Open: {modalsOpen}");
            if (modalsOpen == 0)
                veil?.SetActive(false);

            base.Hide(callback);
        }

        private void OnDestroy()
        {
            if (!Visible) return;
            modalsOpen = Mathf.Max(modalsOpen - 1, 0);
        }
    } 
}