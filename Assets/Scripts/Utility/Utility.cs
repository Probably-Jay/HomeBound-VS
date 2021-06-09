using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public static class Utility 
    {
        /// <summary>
        /// Get an enumerable list of all enum values of a particular type
        /// </summary>
        public static T[] GetEnumValues<T>() => (T[])System.Enum.GetValues(typeof(T));
    }

    public static class Maths 
    {
        /// <summary>
        /// Gets the fraction of a float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Frac(float value) { return value - Mathf.Round(value); }
        public static float Whole(float value) => Mathf.Round(value);

        public static (float whole, float frac) WholeAndFrac(float value) { return (Whole(value), Frac(value)); }

        /// <summary>
        /// Rounds up to the nearest whole number, or fraction at resolution <c>1/<paramref name="fractionResolution"/></c>
        /// </summary>
        /// <param name="value">The value to be rounded</param>
        /// <param name="fractionResolution">Rounds to the nearest <c>1/<paramref name="fractionResolution"/></c>, defaults to whole number</param>
        /// <returns></returns>
        public static float RoundUpTo(float value, float fractionResolution = 1)
        {
            decimal invFrac = 1 / (decimal)fractionResolution;
            return (float)((decimal)Mathf.Ceil((float)((decimal)value * invFrac)) / invFrac);
        }

    }

}