using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rythm;
using Dialogue;
using RhythmSectionLoading;
using System;


    public enum Controler 
    {
        None,
        Rythm,
        Dialogue
    }

   
    public class RythmDialogueInterface : MonoBehaviour, IRythmDialogeControlInterface
    {
        public Controler InControl { get; private set; }
        DialogueManager dialogueManager;
        [SerializeField] RhythmSectionManager rythmManager;

        bool passToRyrhmQueued = false;
        public bool InRythmSection
        {
            get
            {
                if (inRythmSection && !RythmEngine.Instance.PlayingMusic)
                {
                    throw new Exception("Cannot be in rhythm section if music is not playing");
                }
                return inRythmSection;
            }

            private set
            {
                if (value && !RythmEngine.Instance.PlayingMusic)
                {
                    throw new Exception("Cannot enter rhythm section if music is not playing");
                }
                inRythmSection = value;
            }
        }

        private bool inRythmSection = false;

        public bool RythmHasControl { get; private set; } = false;


        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
        }


        public void StartNewRythm(string id)
        {
            Game.GameContextController.Instance.PushContext(Game.Context.Rythm);
            dialogueManager.EnterArgument();
            PassControlToRythm();
            InRythmSection = true;
            rythmManager.LoadAndBeginSection(id);
        }


        private void PassControlToDialogue()
        {
            AssertInRythmSection();
            RythmHasControl = false;
            rythmManager.RythmControlYeilded();
            dialogueManager.RythmControlReceived();
        }

        public void PassControlToDialogue(float? passBack)
        {
            if (dialogueManager.ThisHasControl)
            {
                Debug.LogError("Dialogue already has control");
            }
            PassControlToDialogue();
            if (passBack.HasValue)
                QueuePassBackToRythm(passBack.Value);
        }

        private void QueuePassBackToRythm(float passBack)
        {
            if (passToRyrhmQueued)
            {
                Debug.LogError("Already has pasback queued");
            }
            passToRyrhmQueued = true;
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(PassControlToRythmOnBeat, passBack);
        }

        private void PassControlToRythmOnBeat()
        {
            if (!dialogueManager.ReadyToPassToRythm)
            {
                Debug.LogError("Not ready to pass to rythm");
            }
            if (!passToRyrhmQueued)
            {
                Debug.LogError("This pass back was not queued!");
            }
            passToRyrhmQueued = false;
            PassControlToRythm();
        }


        public void PassControlToRythm()
        {
            if (passToRyrhmQueued)
            {
                // we have this pass back queued for the future
                Debug.Log("Pass back queued for the furure");
                return;
            }
            RythmHasControl = true;
            dialogueManager.RythmControlYeilded();
            rythmManager.RythmControlReceived();
        }

        public void EndRythmSection()
        {
            RythmHasControl = false;
            InRythmSection = false;
            dialogueManager.LeaveArgument();
            Game.GameContextController.Instance.ReturnToPreviousContext();
        }

        private void AssertInRythmSection()
        {
            if (!InRythmSection) throw new System.Exception("This function may only be called in a rythm section");
        }




        public void UnGreyOutHitWord(string word, NoteSystem.HitQuality hitQuality, float? beatWhenDisplayed = null)
        {
           // word += ' ';
            if (beatWhenDisplayed != null)
            {
                dialogueManager.UnGreyOutHitWord(word, hitQuality, beatWhenDisplayed);
            }
            else
            {
                dialogueManager.UnGreyOutHitWord(word, hitQuality);
            }
        }

        public void AddLinePreview(string line)
        {
            Debug.Log($"Adding line Preview {line}");
            dialogueManager.AddLinePreview(line);
        }
        
        public void StrikeThrough(NoteSystem.WordNote note)
        {
           // dialogueManager.StrikeThroughMissedWord(note.word,RythmEngine.Instance.GetNextBeat());
            dialogueManager.StrikeThroughMissedWord(note.word);
        }


    }
