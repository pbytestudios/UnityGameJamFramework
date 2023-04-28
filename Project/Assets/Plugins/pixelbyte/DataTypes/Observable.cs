using System;

namespace Pixelbyte
{
    /// <summary>
    /// This implements a value that is observable meaning we can subscribe to the ValueChanged event 
    /// to be notified if this value ever changes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Observable<T>
    {
        T val;
        public event Action<T> ValueChanged;

        /// <summary>
        /// Used to see if the event should be invoked when a new Value is set.
        /// The default is null which will cause the value to ALWAYS be updated when 
        /// set via the Value property
        /// Parameters: oldVal, newVal
        /// Returns: true if the value should be reported via the event, false otherwise
        /// </summary>
        public Func<T, T, bool> ShouldUpdate;

        public T Value
        {
            get { return val; }
            set
            {
                if (ShouldUpdate == null || ShouldUpdate(val, value))
                {
                    val = value;
                    ValueChanged?.Invoke(val);
                }
            }
        }

        public Observable() { }
        public Observable(T initialVal) { val = initialVal; }

        /// <summary>
        /// Sets the value of the observed variable and triggers the event regardless of whether the value set
        /// is different than the one stored
        /// </summary>
        /// <param name="initialVal"></param>
        //public void Set(T initialVal) { val = initialVal; ValueChanged?.Invoke(val); }

        /// <summary>
        /// Set the internal value without triggering the event
        /// </summary>
        /// <param name="newVal"></param>
        public void Set(T newVal) { val = newVal; }

        public override string ToString() => val.ToString();
    }
}
