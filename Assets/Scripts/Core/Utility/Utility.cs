using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        public static float Frac(float value) { return value - Mathf.Floor(value); }
        public static float Whole(float value) => Mathf.Floor(value);

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

    public class RollingAverage
    {
        readonly Queue<float> values = new Queue<float>();
        private int NumberOfValuesAveragedOver { get; set; }
        public bool ReachedAccuracy => values.Count >= NumberOfValuesAveragedOver;
        public float AverageValue
        {
            get
            {
                return CalclulateMedianAverage();
            }
        }

        public float SmallestAbsoluteValue
        {
            get
            {
                return CalculateSmallestAbsolute();
            }
        }

        private float CalculateSmallestAbsolute()
        {
            if (values.Count == 0)
                throw new Exception("Rolling average must have at least one value");
            List<float> l = new List<float>(values);

            for (int i = 0; i < l.Count; i++)
            {
                l[i] = Mathf.Abs(l[i]);
            }

            l.Sort();
            return l[0];
        }

        //public void RemoveOutliers()
        //{
        //    if (values.Count < 2) return;
        //    List<float> l = new List<float>(values);
        //    l.Sort();
        //    var med = l[l.Count / 2];
        //    for (int i = l.Count - 1; i >= 0; i--)
        //    {
        //        float v = l[i];
        //        if (Mathf.Abs(v - med) > 3 * med)
        //        {
        //            l.RemoveAt(i);
        //        }
        //    }
        //    values.Clear();
        //    foreach (var v in l)
        //    {
        //        values.Enqueue(v);
        //    }
        //}

        private float CalcualteMeanAverage()
        {
            if (values.Count == 0)
                throw new Exception("Rolling average must have at least one value");

            float sum = 0;
            foreach (var value in values)
            {
                sum += value;
            }
            return sum / values.Count;
        }

        private float CalclulateMedianAverage()
        {
            if (values.Count == 0)
                throw new Exception("Rolling average must have at least one value");

            List<float> l = new List<float>(values);
            l.Sort();

            return l[l.Count / 2];
        }

        public RollingAverage(int itterations)
        {
            this.NumberOfValuesAveragedOver = itterations;
        }

        public void Record(float v)
        {
            if (ReachedAccuracy)
            {
                values.Dequeue();
            }
            values.Enqueue(v);
        }

        public void Reset()
        {
            values.Clear();
        }
    }


}