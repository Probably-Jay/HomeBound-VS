using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class BridgeMan : MonoBehaviour
    {
         Animator blackAnim;
        private void Awake()
        {
            this.AssignComponent(out blackAnim);
        }

        public void FadeToBlack()
        {
            blackAnim.SetTrigger("Fade");
        }
    }
}