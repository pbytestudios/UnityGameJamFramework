using UnityEngine;

namespace Pixelbyte.YieldInstructions
{
    /// <summary>
    /// A custom class to allow waiting for a variable number of seconds without creating
    /// a new instance of the WaitForSeconds class everytime
    /// </summary>
    public class WaitSeconds : CustomYieldInstruction
    {
        float timer;
        public override bool keepWaiting { get { timer -= Time.deltaTime; return timer > 0; } }

        public WaitSeconds Wait(float seconds) { timer = seconds; return this; }
    }
}