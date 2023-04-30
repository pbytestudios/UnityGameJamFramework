using UnityEngine;

namespace Pixelbyte.UI
{
    public enum ThemeColor { Primary, Secondary, Accent1, Accent2, Accent3, Accent4, Disabled, Text };

    public interface IThemeable { void SetTheme(ThemeData data); }
    public class SigThemeChanged : Signal<SigThemeChanged, ThemeData> { }

    public abstract class BaseThemeable : MonoBehaviour, IThemeable
    {
        protected virtual void OnEnable()
        {
            SigThemeChanged.Handler += SetTheme;

            //We only need to ask for the current theme if we have NOT been created from a prefab
            //Note: This assumes that you use the ThemeMgr.SpawnUI() method to create ALL UI Prefabs!
            //if (PrefabUtility.GetPrefabInstanceStatus(gameObject) != PrefabInstanceStatus.Connected)
            if (Themer.I != null)
                ChangeTheme(Themer.I.Theme);
        }
        private void OnDisable()
        {
            SigThemeChanged.Handler -= SetTheme;
        }

        public void SetTheme(ThemeData data) { if (data != null && enabled) ChangeTheme(data); }
        protected abstract void ChangeTheme(ThemeData data);

        protected Color GetColor(ThemeColor src, ThemeData data)
        {
            if (data == null) return Color.white;
            switch (src)
            {
                case ThemeColor.Primary:
                    return data.primary;
                case ThemeColor.Secondary:
                    return data.secondary;
                case ThemeColor.Accent1:
                    return data.accent1;
                case ThemeColor.Accent2:
                    return data.accent2;
                case ThemeColor.Accent3:
                    return data.accent3;
                case ThemeColor.Accent4:
                    return data.accent4;
                case ThemeColor.Disabled:
                    return data.disabled;
                case ThemeColor.Text:
                    return data.text;
                default:
                    return data.primary;
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && Themer.I != null)
                ChangeTheme(Themer.I.Theme);
        }
    }

    [DefaultExecutionOrder(-250)]
    public class Themer : Singleton<Themer>
    {
        [SerializeField] ThemeData currentTheme;

        public ThemeData Theme
        {
            get => currentTheme;
            set
            {
                currentTheme = value;
#if UNITY_EDITOR
                OnValidate();
#else
                SigThemeChanged.Fire(currentTheme);
#endif
            }
        }

        void OnThemeRequest(IThemeable uiObject)
        {
            if (uiObject != null && currentTheme != null)
                uiObject.SetTheme(currentTheme);
        }

        //Reverts to previously-saved theme
        public void Revert()
        {
            Theme = currentTheme;
        }

        private void Start() => Revert();

        private void OnValidate()
        {
            var cmp = FindObjectsOfType<BaseThemeable>(true);
            foreach (var c in cmp)
                c.SetTheme(currentTheme);
        }
    }
}
