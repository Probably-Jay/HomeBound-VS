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
            //if (inRythmSection && !RythmEngine.Instance.PlayingMusic)
            //{
            //    throw new Exception("Cannot be in rhythm section if music is not playing");
            //}
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
    private bool dialogueReadyToReleaseControl;
    private bool rhythmReadyToReceiveControl;
    private bool playerFinishedReading;

    public bool RythmHasControl { get; private set; } = false;


    private void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
    }

    private void Update()
    {
        if (!InRythmSection)
        {
            return;
        }
        if (RythmHasControl)
        {
            return;
        }
        if (Input.GetKeyDown(dialogueManager.DialogueContextController.dialogueTyper.NextPhraseKey))
        {

            if (!dialogueReadyToReleaseControl)
            {
                Debug.LogError($" Not ready to release ");
                return;
            }
            playerFinishedReading = true;
            ResumeMusic();
        }
    }
    public void StartNewRythm(string id)
    {
        Game.GameContextController.Instance.PushContext(Game.Context.Rythm);
        dialogueManager.EnterArgument();
        InRythmSection = true;

        RythmHasControl = true;
        dialogueManager.RythmControlYeilded();
        rythmManager.RythmControlReceived();

        rythmManager.LoadAndBeginSection(id);

    }



    public void PassControlToDialogue(float? passBack)
    {
        if (dialogueManager.ThisHasControl)
            {
            Debug.LogError("Dialogue already has control");
        }
        rhythmReadyToReceiveControl = false;
        dialogueReadyToReleaseControl = false;
        playerFinishedReading = false;
        TriggerPassControlToDialogue();
        if (passBack.HasValue)
            QueuePassBackToRythm(passBack.Value);
    }
    private void TriggerPassControlToDialogue()
    {
        AssertInRythmSection();
        RythmHasControl = false;
        rythmManager.RythmControlYeilded();
        dialogueManager.RythmControlReceived();

    }

    private void QueuePassBackToRythm(float passBack)
    {
        if (passToRyrhmQueued)
        {
            Debug.LogError("Already has pasback queued");
        }
        passToRyrhmQueued = true;
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(ReachedBeatWherePassBackShouldHappen, passBack);
        Debug.LogWarning("Passback Queued");
    }

    private void ReachedBeatWherePassBackShouldHappen()
    {
        if (!dialogueReadyToReleaseControl)
        {
            Debug.LogError("Pass back queued to happen too early");
        }
        rhythmReadyToReceiveControl = true;
        if (playerFinishedReading)
        {
            PassControlToRhythm(immediate: true);
        }
    }

    public void OpponentPhraseCompletedPauseMusic()
    {
        Debug.Log("Phrase completed");
        RythmEngine.Instance.PauseSongMelody();
        dialogueReadyToReleaseControl = true;
    }

    public void ResumeMusic()
    {
        RythmEngine.Instance.ResumeSongMelody();
    }

    private void PassControlToRhythm(bool immediate)
    {
        AssertInRythmSection();
        if(!dialogueReadyToReleaseControl || !rhythmReadyToReceiveControl)
        {
            Debug.LogError("Passback not ready");
            return;
        }

        if (immediate)
        {
            TriggerPassControlToRhythm();
        }
        else
        {
            RythmEngine.Instance.QueueActionNextBar(TriggerPassControlToRhythm);
        }
    }

    //public void DialogueReadyToPassControlToRythm()
    //{
    //    //if (passToRyrhmQueued)
    //    //{
    //    //    // we have this pass back queued for the future
    //    //    Debug.Log("Pass back queued for the furure");
    //    //    return;
    //    //}
    //    RythmHasControl = true;
    //    TriggerPassbackOnBar();
    //    //  RythmEngine.Instance.QueueActionNextBar(TriggerPassbackOnBar);
    //}

    private void TriggerPassControlToRhythm()
    {
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
