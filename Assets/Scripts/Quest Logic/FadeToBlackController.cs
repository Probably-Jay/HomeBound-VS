using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class FadeToBlackController : MonoBehaviour
    {
        Animator blackAnim;
        [SerializeField] GameComplete gameComplete;
        private void Awake()
        {
            this.AssignComponent(out blackAnim);
            this.NotNullCheck(gameComplete);
        }

        public void FadeToBlackAndBack()
        {
            blackAnim.SetTrigger("Fade");
        }

        public void FadedToBlack()
        {
            gameComplete.FadedToBlack();
        }
    }
}