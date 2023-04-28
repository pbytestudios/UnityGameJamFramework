using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

namespace Pixelbyte
{
    public class StatusText : MonoBehaviour
    {
        public class Sig : Signal<Sig, string> { }
        public class SigClear : Signal<SigClear> { }

        public static void Add(string txt) => Sig.Fire(txt);
        public static void Clear() => SigClear.Fire();

        [SerializeField] CanvasGroup group;
        [SerializeField] float minAlpha = 0;
        [SerializeField] float timeBeforeFade = 6;
        [SerializeField] float fadeInDuration = 0.5f;
        [SerializeField] float fadeOutDuration = 0.75f;

        //Called when the line is advanced (use this to play a sound maybe)
        [SerializeField] UnityEvent lineAdvanced;
        //Called when the status text has been displayed or re-displayed
        [SerializeField] UnityEvent statusActivated;

        TextMeshProUGUI[] lines;
        Coroutine routine;
        float originalAlpha;

        private void Awake()
        {
            lines = new TextMeshProUGUI[transform.childCount];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                lines[i].text = "";
            }
            originalAlpha = group.alpha;
            group.alpha = minAlpha;
        }

        void OnEnable()
        {
            Sig.Handler += AddStatus;
            SigClear.Handler += ClearText;
        }

        void OnDisable()
        {
            Sig.Handler -= AddStatus;
            SigClear.Handler -= ClearText;
        }

        void ClearText()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].text = "";
            }
            timeBeforeFade = 0;
            StopRoutine();
            group.DOFade(minAlpha, fadeOutDuration / 2f);
        }

        void ActivateStatus()
        {
            if (timeBeforeFade == 0)
            {
                group.alpha = originalAlpha;
                statusActivated.Invoke();
                return;
            }

            StopRoutine();
            group.DOKill();

            if (group.alpha <= originalAlpha)
            {
                //group.alpha = minAlpha;
                group.DOFade(originalAlpha, fadeInDuration).onComplete =
                    () =>
                    {
                        routine = StartCoroutine(WaitThenFadeOut(timeBeforeFade));
                        statusActivated.Invoke();
                    };
            }
            else
            {
                routine = StartCoroutine(WaitThenFadeOut(timeBeforeFade));
                statusActivated.Invoke();
            }

        }

        private void StopRoutine()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
        }

        public void AddStatus(string txt)
        {
            ActivateStatus();

            //If it is the same line, then just activate the status display again
            if (lines[lines.Length - 1].text == txt)
                return;

            AdvanceLines();
            lines[lines.Length - 1].text = txt;
        }

        void AdvanceLines()
        {
            for (int i = 0; i < lines.Length - 1; i++)
            {
                lines[i].text = lines[i + 1].text;
            }
            lineAdvanced.Invoke();
        }

        IEnumerator WaitThenFadeOut(float time)
        {
            yield return new WaitForSeconds(time);
            group.DOFade(minAlpha, fadeOutDuration);
            routine = null;
        }

        //void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        //{
        //    group.DOKill();
        //    if (routine != null)
        //        StopCoroutine(routine);
        //    routine = null;
        //    group.DOFade(1, 0.75f);
        //}

        //void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        //{
        //    if (routine != null)
        //    {
        //        routine = StartCoroutine(WaitThenFadeOut(timeBeforeFade / 2f));
        //    }
        //}
    }
}