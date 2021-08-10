using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{

    public class KarensDialogue : BaseDialogue
    {
        [SerializeField] string dialogue;
        protected override void TriggerDialogue()
        {
            StartDialogue(dialogue);
        }

        private void OnEnable()
        {
            opener.OnBoxClose += Opener_OnBoxClose;
        }

        private void OnDisable()
        {
            opener.OnBoxClose -= Opener_OnBoxClose;
        }


    }
}