using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NoteSystem
{
    public enum HitQuality
    {
        Miss
       , Early
       , Late
       , Good
       , Great
       , Perfect
    }

    public static class HitDetection
    {
        // 120 BPM ==> 0.5 SPB ==> 0.01B == 0.05s == 50ms 

        private const double PERFECT_BEATS_OFF = 0.017; 
        private const double GREAT_BEATS_OFF = 0.06;
        private const double GOOD_BEATS_OFF = 0.12;
        private const double POOR_BEATS_OFF = 0.3;


        public static HitQuality CheckHit(float targetBeat, float currentBeat )
        {
            float beatsOff = (Mathf.Abs(targetBeat - currentBeat));

            if (beatsOff > POOR_BEATS_OFF) { return HitQuality.Miss; }

            
            if (beatsOff < PERFECT_BEATS_OFF)
            {
                Debug.Log($"Perfect: {currentBeat}/{targetBeat} ({beatsOff} beats off)");
                //m.StartCoroutine(DebugNextFrame());
                return HitQuality.Perfect;
            }
            else if (beatsOff < GREAT_BEATS_OFF)
            {
                return HitQuality.Great;
            }
            else if (beatsOff < GOOD_BEATS_OFF)
            {
                return HitQuality.Good;
            }

            else if (targetBeat > currentBeat)
            {
                return HitQuality.Early;
            }
            else
            {
                return HitQuality.Late;
            }


        }

        private static IEnumerator DebugNextFrame()
        {
            yield return null;
          //  yield return null;
            Debug.Log($"Next frame: {Rythm.RythmEngine.Instance.CurrentBeat}");
        }
    }
}
