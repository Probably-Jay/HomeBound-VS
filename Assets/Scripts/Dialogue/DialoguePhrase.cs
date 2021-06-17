using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialoguePhrase
    {
        [SerializeField] [TextArea(1,4)] private string phrase;
        [SerializeField] private string speaker; // will be replaced with speaker object
        [SerializeField] private bool onLeft;
        [SerializeField] private string trigger;

        [SerializeField] private event Action onTrigger;

        public string Phrase { get => phrase; private set => phrase = value; }
        public string Speaker { get => speaker; private set => speaker = value; }
        public bool OnLeft { get => onLeft; private set => onLeft = value; }
        public string Trigger { get => trigger; set => trigger = value; }
        public Action OnTrigger { get => onTrigger; set => onTrigger = value; }

        public readonly long conversationID;
        public readonly long phraseContextID;


        public DialoguePhrase(string phrase, string speaker, Action onTrigger = null)
        {
            this.phrase = phrase;
            this.speaker = speaker;

            // this.conversationID = conversationID;

            if(onTrigger != null)
                this.onTrigger += onTrigger;

            phraseContextID = GlobalPhraseContextID++;
        }

        static long GlobalPhraseContextID = 0;

    }
}