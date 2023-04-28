using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] LayerMask castMask;
    [SerializeField] float interactDistance = 2.0f;

    public bool CanInteract => last != null && last.Enabled;

    IInteractable last;

    void Update() => CheckForInteractable();
    public bool Interact()
    {
        if (CanInteract)
        {
            last?.Interact();
            return true;
        }
        else
            return false;
    }

    private void OnDisable()
    {
        last?.UnFocus();
        last = null;
    }

    void CheckForInteractable()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * interactDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactDistance, castMask))
        {
            var interactable = hit.transform.GetComponentInParent<IInteractable>();
            if (interactable != last)
            {
                last?.UnFocus();
                last = interactable;
            }

            interactable?.Focus();
        }
        else
        {
            last?.UnFocus();
            last = null;
        }
    }
}
