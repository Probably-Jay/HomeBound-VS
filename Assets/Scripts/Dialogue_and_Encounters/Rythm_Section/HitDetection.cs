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
        // Start is called before the first frame update

        
        public static HitQuality CheckHit(float targetBeat, float currentBeat)
        {
            float beatsOff = (Mathf.Abs(targetBeat - currentBeat));

            if (beatsOff > 0.4) { return HitQuality.Miss; }

            //HitQuality output;
            if (beatsOff < 0.1)
            {
                return HitQuality.Perfect;
            }
            else if (beatsOff < 0.2)
            {
                return HitQuality.Great;
            }
            else if (beatsOff < 0.3)
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
        // Update is called once per frame
    }
}
