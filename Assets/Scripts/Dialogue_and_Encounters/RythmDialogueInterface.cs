using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rythm;
using RhythmSectionLoading;

namespace Dialogue
{
    public enum Controler 
    {
        None,
        Rythm,
        Dialogue
    }


    public class RythmDialogueInterface : MonoBehaviour
    {
        public Controler InControl { get; private set; }
        DialogueManager dialogueManager;
        RhythmInitialise rythmSystem;

        public bool InRythmSection { get; private set; } = false;


        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
        }

        public void PassControlToDialogue()
        {
            throw new System.NotImplementedException();
        }

        public void PassControlToDialogue(float passBack)
        {
            throw new System.NotImplementedException();
        }




        public void StartNewRythm(string id)
        {
            InRythmSection = true;
            dialogueManager.StopCurrentConversation();
            
        }
        public void PassControlToRythm()
        {
            throw new System.NotImplementedException();
        }

        public void EndRythm()
        {
            throw new System.NotImplementedException();
        }
        

    }
}