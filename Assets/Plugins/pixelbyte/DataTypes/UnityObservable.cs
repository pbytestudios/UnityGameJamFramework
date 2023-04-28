using System;
using UnityEngine.Events;

namespace Pixelbyte
{
    /// <summary>
    /// This implements a value that is observable meaning we can subscribe to the ValueChanged event 
    /// to be notified if this value ever changes. This implementation uses the UnityEvent to send out
    /// the notification thus it can be hooked up in the editor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class UnityObservable<T> : UnityEvent<T>
    {
        T val;

        public T Value
        {
            get { return val; }
            set
            {
                if (ShouldUpdate == null || ShouldUpdate(val, value))
                {
                    val = value;
                    Invoke(val);
                }
            }
        }

        /// <summary>
        /// Used to see if the event should be invoked when a new Value is set
        /// Parameters: oldVal, newVal
        /// Returns: true if the value should be reported via the event, false otherwise
        /// </summary>
        public Func<T, T, bool> ShouldUpdate;

        public UnityObservable() { }

        /// <summary>
        /// Sets the value without triggering the Event
        /// </summary>
        /// <param name="newVal"></param>
        public void Set(T newVal) { val = newVal; }

        /// <summary>
        /// Sets the value and Invokes the event bypassing the ShouldUpdate() method
        /// </summary>
        /// <param name="newVal"></param>
        public void SetAndInvoke(T newVal) { val = newVal; Invoke(val); }

        public override string ToString() => val.ToString();
    }

    /// <summary>
    /// Derive from this class when you want to store a data value of type T
    /// and convert it to an event of type V. This is very useful for 
    /// connecting up UI components such as a Unity UI Text component
    /// You could derive a class from this one to be able to send the
    /// int as a string value formatted to your liking
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class UnityObservable<T, V> : UnityEvent<V>
    {
        /// <summary>
        /// The actual stored data
        /// </summary>
        protected T val;

        /// <summary>
        /// Subscribe to this to get just the raw value
        /// </summary>
        public event Action<T> RawValueChanged;

        public T Value
        {
            get { return val; }
            set
            {
                if (ShouldUpdate == null || ShouldUpdate(val, value))
                {
                    val = value;
                    if (FormatValue != null)
                        Notify();
                    else
                    {
                        Dbg.Err($"{nameof(FormatValue)} must be set!");
                        throw new ArgumentException($"{nameof(FormatValue)}");
                    }
                }
            }
        }

        /// <summary>
        /// This is called to convert the value of type T
        /// to the sent type of V
        /// Set this to your own function to change what is returned for the given value.
        /// Inputs: A reference to the sender so we can limit what the value can be set to
        /// returns: a string that is sent to any listeners
        /// </summary>
        /// <param name="newVal"></param>
        /// <returns></returns>
        public Func<UnityObservable<T, V>, V> FormatValue;

        /// <summary>
        /// Used to see if the event should be invoked when a new Value is set.
        /// The default is null which will cause the value to ALWAYS be updated when 
        /// set via the Value property
        /// Parameters: oldVal, newVal
        /// Returns: true if the value should be reported via the event, false otherwise
        /// </summary>
        public Func<T, T, bool> ShouldUpdate;

        public UnityObservable() { }

        /// <summary>
        /// Sets the value without triggering the Event
        /// </summary>
        /// <param name="newVal"></param>
        public void Set(T newVal) { val = newVal; }

        /// <summary>
        /// Sets the value and Invokes the event bypassing the ShouldUpdate() method
        /// </summary>
        /// <param name="newVal"></param>
        public void SetAndInvoke(T newVal) { val = newVal; Invoke(FormatValue(this)); }

        void Notify()
        {
            Invoke(FormatValue(this));
            RawValueChanged?.Invoke(val);
        }

        public override string ToString() => val.ToString();
    }
}

