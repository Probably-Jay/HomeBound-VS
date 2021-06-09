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

        public string Phrase { get => phrase; private set => phrase = value; }
        public string Speaker { get => speaker; private set => speaker = value; }
        public bool OnLeft { get => onLeft; private set => onLeft = value; }
        public string Trigger { get => trigger; set => trigger = value; }

        public readonly long phraseContextID;


        public DialoguePhrase(string phrase, string speaker, bool onLeft, string trigger)
        {
            this.phrase = phrase;
            this.speaker = speaker;
            this.onLeft = onLeft;
            this.trigger = trigger;

            phraseContextID = GlobalPhraseContextID++;
        }

        static long GlobalPhraseContextID = 0;

    }
}