using UnityEngine;

namespace Pixelbyte.Math
{
    public static class PMath
    {
        /// <summary>
        /// Given a percentage between 0 and 1, returns true if
        /// the random number is <= to the given percentage
        /// ex: 45% change of success:
        ///    Prob(0.45f);
        ///
        /// </summary>
        /// <param name="percent">A percentage between 0 and 1 for how likely true will be returned</param>
        /// <returns>true when random number <= percent, false otherwise</returns>
        public static bool Prob(float likelihood)
        {
            if (likelihood >= 1) return true;
            return UnityEngine.Random.value <= likelihood;
        }


        /// <summary>
        /// Returns a quaternion that when applied to A would result in B
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Quaternion RelativeRotation(Quaternion a, Quaternion b)
        {
            return Quaternion.Inverse(a) * b;
        }

        /// <summary>
        /// Maps a value within the given min/max space into the 0 to 1 range
        /// </summary>
        /// <param name="val">Value to map</param>
        /// <param name="min">Min value within the space to map from</param>
        /// <param name="max">Max value within the space to map from</param>
        /// <returns></returns>
        public static float Map01(float val, float min, float max)
        {
            return (val - min) / (max - min);
        }

        /// <summary>
        /// Maps a value within the given min/max space into the 1 to 0 range
        /// </summary>
        /// <param name="val">Value to map</param>
        /// <param name="min">Min value within the space to map from</param>
        /// <param name="max">Max value within the space to map from</param>
        /// <returns></returns>
        public static float Map10(float val, float min, float max)
        {
            return 1 - Map01(val, min, max);
        }

        /// <summary>
        /// Maps the given value which should be in the range: min to max
        /// into the newMin to newMax range
        /// </summary>
        /// <param name="val">Value to map</param>
        /// <param name="min">Original min of the space</param>
        /// <param name="max">Original max of the space</param>
        /// <param name="newMin">New minimum of the space to map to</param>
        /// <param name="newMax">New maximum of the space to map to</param>
        /// <returns></returns>
        public static float Map(float val, float min, float max, float newMin, float newMax)
        {
            return newMin + (val - min) * (newMax - newMin) / (max - min);
        }
    }
}