using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialoguePhrase
    {
        [SerializeField] [TextArea(1,4)] private StringBuilder phrase = new StringBuilder();


        [SerializeField] private string speaker; // will be replaced with speaker object
        [SerializeField] private bool onLeft;

        
    //    [SerializeField] private string trigger;

        [SerializeField] public event Action onTrigger;

        public Dictionary<string, Action> inlineInstructions = new Dictionary<string, Action>();

        public string PhraseID { get; set; }
        public StringBuilder Phrase { get => phrase;  set => phrase = value; }
        public string Speaker { get => speaker;  set => speaker = value; }
        public bool OnLeft { get => onLeft; private set => onLeft = value; }

        public bool Queued { get; private set; } = false;

       // public string Trigger { get => trigger; set => trigger = value; }

        public readonly long conversationID;
        public readonly long phraseContextID;


        public DialoguePhrase()
        {
            //this.phrase = phrase;
            //this.speaker = speaker;

            //// this.conversationID = conversationID;

            //if(onTrigger != null)
            //    this.onTrigger += onTrigger;

            phraseContextID = GlobalPhraseContextID++;
        }

        public void TriggerActions() => onTrigger?.Invoke();

        static long GlobalPhraseContextID = 0;

        public void SetQueued() => Queued = true;
        public void UnQueue() => Queued = false;
        


    }
}