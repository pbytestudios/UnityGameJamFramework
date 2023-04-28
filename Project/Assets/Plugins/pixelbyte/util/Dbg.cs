using System;
using UnityEngine;
namespace Pixelbyte
{
    /// <summary>
    /// This class contains Debug stuff with Conditional attributes so it can be turned on/off
    /// Note: I'm using the delegates here so that double clicking in the console window will take me to the actual line where the debug funciton is being called from
    /// </summary>
    public static class Dbg
    {
        public delegate void LogDelegate(object obj, UnityEngine.Object context = null);
        public static LogDelegate Log;
        public static LogDelegate Warn;
        public static LogDelegate Err;

        static Dbg()
        {
#if DEBUG_INFO
            Log = Debug.Log;
#else
            Log = (m, c) => {};
#endif

#if DEBUG_WARN
            Warn = Debug.LogWarning;
#else
            Warn = (m, c) => {};
#endif

            //Dbg.Err is always active
            //#if DEBUG_ERR
            Err = Debug.LogError;
            //#else
            //            Err = (m, c) => {};
            //#endif
        }

#if !UNITY_WINRT
        /// <summary>
        /// Prints the name of the method you are currently in
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG_INFO")]
        public static void MethodName()
        {
            //we need the previous stack frame since we want to print the name of that method not this one
            //Note this doe NOT currently work inside of an IEnumerator coroutine method!
            System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1);

            Debug.Log("Method: " + sf.GetMethod());
        }
#endif
    }
}