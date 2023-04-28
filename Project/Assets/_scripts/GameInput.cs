using Pixelbyte.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#endif
/// <summary>
/// To use this, be sure to toggle the "Generate C# class" toggle on the
/// InputActionAsset!
/// </summary>
public class GameInput : Singleton<GameInput>
{
    public const int UI_LAYER = 5;

    public static bool IsMouseOveUI => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    public static GameObject SelectedUIObject => (EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null;

    public static bool IsMouseLocked => UnityEngine.Cursor.lockState == CursorLockMode.Locked;
    public static bool IsMouseVisible => UnityEngine.Cursor.visible;

    delegate T MouseCheckCallback<T>(GameObject gob, ref bool success);
    static List<RaycastResult> results = new List<RaycastResult>();

    public event Action interacted;
    public event Action escaped;

#if ENABLE_INPUT_SYSTEM
    public static class Kb
    {
        public static Keyboard Current => Keyboard.current;
        public static bool Down(Key k) => Current[k].wasPressedThisFrame;
        public static bool Pressed(Key k) => Current[k].isPressed;
        public static bool Up(Key k) => Current[k].wasReleasedThisFrame;
    }

    public static Vector2 MousePosition => Mouse.current.position.ReadValue();
    public static Vector2 MouseScrollDelta => Mouse.current.scroll.ReadValue();
    public static bool LMBDown => Mouse.current.leftButton.wasPressedThisFrame;
    public static bool RMBDown => Mouse.current.rightButton.wasPressedThisFrame;
    public static bool MMBDown => Mouse.current.middleButton.wasPressedThisFrame;

    Controls inputActions;
    Controls.DefaultActions map;

    protected override void Awake()
    {
        base.Awake();

        inputActions = new Controls();
        //Enable the default mapping
        inputActions.Default.Enable();
        map = inputActions.Default;
        map.Interact.performed += (_) => interacted?.Invoke();
        map.Esc.performed += (_) => escaped?.Invoke();
    }

    public Vector2 DMoveVectorNormalized() => map.DMove.ReadValue<Vector2>().normalized;

#else
    public static Vector2 MousePosition => Input.mousePosition;
    public static Vector2 MouseScrollDelta => Input.mouseScrollDelta;
    public static bool LMBDown => Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
    public static bool RMBDown => Input.GetMouseButtonDown((int)MouseButton.RightMouse);
    public static bool MMBDown => Input.GetMouseButtonDown((int)MouseButton.MiddleMouse);
#endif

    public static void EnableUIEvents(bool enable)
    {
        if (EventSystem.current == null)
        {
            var esystem = FindObjectOfType<EventSystem>(true);
            esystem.enabled = enable;
        }
        else
        {
            EventSystem.current.enabled = enable;
        }
    }

    public static void DeselectUI()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public static void MLock(bool cursorVisible = false)
    {
        if (cursorVisible)
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        else
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = cursorVisible;
    }

    public static void MUnlock()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    /// <summary>
    /// Returns the FIRST object (there may be more) directly under the mouse AND in the specified Layer whether it is selected or not.
    /// If you are not getting what you expect, be sure to disable raycasts for objects you Don't want this to catch
    /// I Use this to get ToolTipText components for my tooltip system.
    /// Set onlyCheckFirstResult to false to search through ALL of the results and not just the first one
    /// NOTE: onlyCheckFirstResult should be true when looking for ToolTips!
    /// </summary>
    public static T GetComponentUnderMouse<T>(bool onlyCheckFirstResult = true, int layer = UI_LAYER) where T : MonoBehaviour
    {
        T CheckObject(GameObject gameObject, ref bool success)
        {
            if (gameObject.layer == layer)
            {
                var comp = gameObject.GetComponent<T>();
                if (comp != null)
                {
                    success = true;
                    return comp;
                }
            }
            return null;
        }
        return CheckUnderMouse(CheckObject, onlyCheckFirstResult);
    }

    static T CheckUnderMouse<T>(MouseCheckCallback<T> callback, bool onlyCheckFirstResult = true)
    {
        var es = EventSystem.current;
        if (es == null)
            return default(T);

        T result = default(T);
        bool success = false;
        if (es.IsPointerOverGameObject())
        {
            es.RaycastAll(new PointerEventData(es) { position = MousePosition }, results);
            for (int i = 0; i < results.Count; i++)
            {
                result = callback(results[i].gameObject, ref success);
                if (success || onlyCheckFirstResult)
                    break;
            }
            results.Clear();
        }
        return result;
    }

    public static Vector3 ToWorld(Camera cam = null)
    {
        if (cam == null) cam = Camera.main;
        return cam.ScreenToWorldPoint(MousePosition);
    }

    public static Vector2 ToWorld2D(Camera cam = null) => ToWorld(cam);

    public static Vector2 GetPositionInCanvas(Canvas canvas)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            MousePosition, canvas.worldCamera, out Vector2 pos);
        return pos;
    }

    public static Vector2 GetLocalPositionInRect(RectTransform rt)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, MousePosition, rt.ParentCanvas().worldCamera, out Vector2 position);
        return position;
    }
}
