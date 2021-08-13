using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmSectionLoading;
using System;
using Rythm;
using StressSystem;

public class RhythmSectionManager : MonoBehaviour
{
    Dictionary<string, Rythm.NoteSection> noteSheets = new Dictionary<string, Rythm.NoteSection> { };
    [SerializeField] List<string> identifications = new List<string> { };
    [SerializeField] List<Rythm.NoteSection> sheets = new List<Rythm.NoteSection> { };
    [SerializeField] RhythmInitialise sectionLoader;
    [SerializeField] RythmDialogueInterface rDI;
    [SerializeField] bool debugStartOnStart;
    [SerializeField] StressPointRing substressRing;
    public bool HasControl => rDI.RythmHasControl;
    private float time=0;

    // Start is called before the first frame update
    private void Update()
    {
        if (!debugStartOnStart)
        {
            return;
        }
        time = time + Time.deltaTime;
        if (time > 1)
        {
            rDI.StartNewRythm(identifications[0]);
            debugStartOnStart = false;
        }
    }
    void Awake()
    {
        if (identifications.Count == sheets.Count)
        {
            for (int i = 0; i < identifications.Count; i++)
            {
                noteSheets.Add(identifications[i], sheets[i]);
            }
        }
        else
        {
            Debug.LogError("Cannot Zip Identifications and Sheets");
        }
        
    }


    public void RythmControlReceived()
    {
        Debug.Log($"{nameof(RhythmSectionManager)} Received control");
        //recieve control
    }

    public void RythmControlYeilded()
    {
        Debug.Log($"{nameof(RhythmSectionManager)} Yeilded control");
    }
    public void PassToDialogue(float? passbackBeat)
    {
        if (!HasControl)
        {
            Debug.LogError("Passing control when does not have control");
        }
        rDI.PassControlToDialogue(passbackBeat);
    }
    public void ShowPreviewLine(string line)
    {
        rDI.AddLinePreview(line);
    }

    public void LoadAndBeginSection(string iD)
    {
        if (!noteSheets.ContainsKey(iD))
        {
            throw new System.Exception($"{nameof(noteSheets)} does not contain key {iD}");
        }


        Rythm.NoteSection noteSection = noteSheets[iD];

        if (!SectionIsValid(noteSection))
        {
            Debug.LogError($"Skipping section {iD} as it is invalid");
        }

        Rythm.RythmEngine.Instance.PlayRhytmSong(noteSection.song);
        sectionLoader.LoadAndBeginSectionNotes(noteSection.notes);

    }

    private bool SectionIsValid(NoteSection noteSection)
    {
        try
        {
            if(noteSection == null)
                throw new Exception("notesection is null");
            if(noteSection.notes == null)
                throw new Exception("notesection.notes is null");
            if(noteSection.notes.text == "")
                throw new Exception("notesection is empty");
            if(noteSection.song == null)
                throw new Exception("notesection.song is null");
            if(noteSection.song.audioClip == null)
                throw new Exception("notesection.song has no audio clip, did you forget to re-import the music files you fucking moron?");
            if(noteSection.song.BPM == 0)
                throw new Exception("notesection.song has no BPM"); 
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception loading {noteSection.name}: {e.Message}: {e.StackTrace}");
            return false;
        }
        return true;
    }

    public void EndSection()
    {
        rDI.EndRythmSection();
        sectionLoader.EndSection();
        Rythm.RythmEngine.Instance.StopRythmSong();
    }
    public void Strikethrough(NoteSystem.WordNote note)
    {
        rDI.StrikeThrough(note);
    }

    public void AddSubstress(NoteSystem.HitQuality hitQuality)
    {
        switch (hitQuality)
        {
            case NoteSystem.HitQuality.Miss:
                substressRing.AddStressPoints(2);
                break;
            case NoteSystem.HitQuality.Early:
                substressRing.AddStressPoints(1);
                break;
            case NoteSystem.HitQuality.Late:
                substressRing.AddStressPoints(1);
                break;
            case NoteSystem.HitQuality.Good:
                
                break;
            case NoteSystem.HitQuality.Great:
                substressRing.RemoveStressPoint();
                break;

            case NoteSystem.HitQuality.Perfect:
                substressRing.RemoveStressPoints(3);
                break;
            default:
                break;
        }
    }

}
   

