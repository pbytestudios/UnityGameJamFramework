using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pixelbyte.UI
{
    /// <summary>
    /// Attach this to a GameObject with a Text component on it and it will display the 
    /// current game version.
    /// </summary>
    public class ParameterText : MonoBehaviour
    {
        [Tooltip("This is the format string. Valid parameters: {version},{product},{year},{month},{date}")]
        [SerializeField] string textFormat = "Version: {version}";
        [SerializeField] UnityEvent<string> label;

        private void Awake()
        {
            if (label.GetPersistentEventCount() == 0)
            {
                //Dbg.Log($"{this.GetType().Name} no event listeners attached. Looking for an attached TMP_Text element", gameObject);
                var tmp = GetComponent<TMPro.TextMeshProUGUI>();
                if(tmp != null)
                {
                    //Dbg.Log("Found a TMP_Text element. Adding an event listener...");
                    label.AddListener((txt) => tmp.text = txt);
                }
                else
                    Dbg.Warn("Unable to find a TMP_Text element.");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (label.GetPersistentEventCount() == 0)
            {
                var tmp = GetComponent<TMPro.TextMeshProUGUI>();
                if (tmp != null)
                    label.AddListener((txt) => tmp.text = txt);
            }
            ParameterizeText();
        }
#endif

        void Start()
        {
            ParameterizeText();
        }

        void ParameterizeText()
        {
            var newtext = textFormat.Replace("{version}", $"{Application.version:#0.##}")
                .Replace("{product}", $"{Application.productName}")
                .Replace("{year}", DateTime.Now.Year.ToString())
                .Replace("{month}", DateTime.Today.ToString("MMMM"))
                .Replace("{date}", DateTime.Today.ToString("d"));
            label.Invoke(newtext);
        }
    } 
}
