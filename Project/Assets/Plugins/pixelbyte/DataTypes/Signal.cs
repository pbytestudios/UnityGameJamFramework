using System;

namespace Pixelbyte
{
    ///Note: These implementations do not check for anonymous method subscriptions
    /// anonymous methods cannot be unsubscribed, so do not do this unless you know what you're doing!

    /// <summary>
    /// Derive from this to create new empty signals
    /// We use a Generic here in order to ensure 
    /// the static 'signal' is different for each instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Signal<T> where T : Signal<T>
    {
        static Action action;

        public static event Action Handler
        {
            add => action += value;
            remove => action -= value;
        }

        public static void Fire(bool oneShot = false)
        {
            action?.Invoke();
            if (oneShot)
                Clear();
        }

        public static void Clear() => action = null;
    }

    /// <summary>
    /// Derive from this signal to send a single parameter U
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>

    public abstract class Signal<T,U> where T : Signal<T,U>
    {
        static Action<U> action;

        public static event Action<U> Handler
        {
            add => action += value;
            remove => action -= value;
        }

        public static void Fire(U val, bool oneShot = false)
        {
            action?.Invoke(val);
            if (oneShot)
                Clear();
        }
        public static void Clear() => action = null;
    }

    public abstract class Signal<T,U,V> where T : Signal<T,U,V>
    {
        static Action<U,V> action;

        public static event Action<U,V> Handler
        {
            add => action += value;
            remove => action -= value;
        }

        public static void Fire(U val, V val2, bool oneShot = false)
        {
            action?.Invoke(val, val2);
            if (oneShot)
                Clear();
        }
        public static void Clear() => action = null;
    }

    /// <summary>
    /// Derive from this class to send the class instance itself as the
    /// parameter for the event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ClassSignal<T> where T : Signal<T>
    {
        static Action<T> action;

        public static event Action<T> Handler
        {
            add => action += value;
            remove => action -= value;
        }

        public void Fire(bool oneShot = false)
        {
            action?.Invoke(this as T);
            if (oneShot)
                Clear();
        }

        public static void Clear() => action = null;
    }
}