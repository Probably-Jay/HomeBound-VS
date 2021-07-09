using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rythm
{
    
    public class BeatIndicator : MonoBehaviour
    {

        SpriteRenderer r;
        private void Start()
        {
            r = GetComponent<SpriteRenderer>();
        }

       // int i = 0;

        // Update is called once per frame
        void Update()
        {
            float beat = (float)Rythm.RythmEngine.Instance.CurrentBeat;
         //   int b = (int)beat;

          
            
            r.color = Color.Lerp(Color.black, Color.white, (1 - (Helper.Maths.Frac(beat)*2)) );
                
            
            



        }


       

    }
}