using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class SimpleDialogue : BaseDialogue, ISimpleDialogue
    {

        [TextArea(3,40)]
        [SerializeField] string text;
        private DialogueSceneSerialisedLoader serialiser;

        public string Text { get => $"id: {Title}\r\n {text} \r\n[end]"; }
        public string Title { get => $"__autoGenID_{(uint)text.GetHashCode()}"; }


        

        private void Awake()
        {
            FindOpener();
            serialiser = FindObjectOfType<DialogueSceneSerialisedLoader>();
        }


        private void Register() => serialiser.Register(this);

        private void OnEnable()
        {
            opener.OnBoxClose += Opener_OnBoxClose;
            serialiser.OnLoad += Register;
        }

        private void OnDisable()
        {
            opener.OnBoxClose -= Opener_OnBoxClose;
            serialiser.OnLoad -= Register;

        }

        protected override void TriggerDialogue()
        {
            StartDialogue(Title);
        }
    }
}