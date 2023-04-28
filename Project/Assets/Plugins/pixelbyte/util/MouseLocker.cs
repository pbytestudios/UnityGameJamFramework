using UnityEngine;

namespace Pixelbyte
{
    /// <summary>
    /// Locks the mouse cursor when a mouse button is pressed 
    /// </summary>
    public class MouseLocker : MonoBehaviour
    {
        public bool forceCursor = false;

        public bool IsLocked { get { return Cursor.lockState == CursorLockMode.Locked; } }

#if !UNITY_STANDALONE
        private void Awake()
        {
            Destroy(this);
        }
#endif

        //    void Update()
        //    {
        //        //Check mousebuttons
        //        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        //        {
        //            LockCursor();
        //        }
        //    }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void LockCursor()
        {
            if (forceCursor)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    } 
}