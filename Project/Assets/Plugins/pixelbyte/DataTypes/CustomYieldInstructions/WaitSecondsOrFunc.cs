using Pixelbyte;
using System;
using UnityEngine;
/// <summary>
/// Waits until either the given amount of time has elapsed, or
/// until the given function returns false
/// </summary>
public class WaitSecondsOrFunc : CustomYieldInstruction
{
    /// <summary>
    /// We keep waiting until this function returns false.
    /// Note: THIS CANNOT BE NULL!
    /// </summary>
    Func<bool> predicate;

    /// <summary>
    /// Amount of time to wait
    /// </summary>
    float waitTime;

    /// <summary>
    /// If the wait time was interrupted by the function value, this will return true
    /// </summary>
    public bool Interrupted => waitTime > 0;

    /// <summary>
    /// If true then the output of predicate function is inverted
    /// </summary>
    bool invertPredicate;

    public WaitSecondsOrFunc(bool invertPredicateOutput = false) => invertPredicate = invertPredicateOutput;

    public override bool keepWaiting
    {
        get
        {
            waitTime -= Time.deltaTime;
            return (predicate() ^ invertPredicate) & (waitTime > 0);
        }
    }

    public WaitSecondsOrFunc Wait(float time, Func<bool> _predicate)
    {
        waitTime = time;
        predicate = _predicate;

        if (predicate == null)
            throw new NullReferenceException("_predicate cannot be null!");
        return this;
    }
}
