using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [Tooltip("If true, then this component is disabled when interacted with the 1st time")]
    [SerializeField] bool singleInteract;

    [Header("Events")]
    [SerializeField] UnityEvent interact;
    [SerializeField] UnityEvent focus;
    [SerializeField] UnityEvent unfocus;

    bool focused = false;

    public bool Enabled => enabled;
    public bool Focused => focused;

    public void Interact()
    {
        if(enabled)
        {
            interact.Invoke();
            if (singleInteract)
                enabled = false;
        }
    }

    private void OnDisable() => UnFocus();

    public void Focus()
    {
        if (enabled && !focused)
        {
            focused = true;
            focus.Invoke();
        }
    }

    public void UnFocus()
    {
        if (focused)
        {
            focused = false;
            unfocus.Invoke();
        }
    }
}