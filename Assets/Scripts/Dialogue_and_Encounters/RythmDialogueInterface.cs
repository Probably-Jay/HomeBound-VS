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
        RhythmSectionManager rythmManager;

        public bool InRythmSection { get; private set; } = false;


        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
        }



        public void PassControlToDialogue()
        {
            AssertInRythmSection();
            throw new System.NotImplementedException();
        }

        public void PassControlToDialogue(float passBack)
        {
            AssertInRythmSection();

            dialogueManager.RythmControlReceive();


            throw new System.NotImplementedException(); // tell rythm to release


            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(PassControlToRythm, passBack);
        }




        public void StartNewRythm(string id)
        {
            InRythmSection = true;
            dialogueManager.StopCurrentConversation();
            dialogueManager.EnterArgument();
            rythmManager.LoadSection(id);
            
        }

        public void PassControlToRythm()
        {
            dialogueManager.RythmControlRelease();
        }

        public void EndRythm()
        {
            throw new System.NotImplementedException();
        }

        private void AssertInRythmSection()
        {
            if (!InRythmSection) throw new System.Exception("This function may only be called in a rythm section");
        }
    }
}