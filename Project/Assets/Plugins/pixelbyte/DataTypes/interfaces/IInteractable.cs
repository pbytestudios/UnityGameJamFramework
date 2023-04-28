public interface IInteractable 
{
    bool Enabled { get; }
    void Interact();
    void Focus();
    void UnFocus();
}
