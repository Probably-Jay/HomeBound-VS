using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmSectionLoading;

public class RhythmSectionManager : MonoBehaviour
{
    Dictionary<string, Rythm.NoteSection> noteSheets = new Dictionary<string, Rythm.NoteSection> { };
    [SerializeField] List<string> identifications = new List<string> { };
    [SerializeField] List<Rythm.NoteSection> sheets = new List<Rythm.NoteSection> { };
    [SerializeField] RhythmInitialise sectionLoader;
    [SerializeField] Dialogue.RythmDialogueInterface rDI;

    public bool HasControl => rDI.RythmHasControl;

    // Start is called before the first frame update
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
    public void PassToDialogue()
    {
        if (!HasControl)
        {
            Debug.LogError("Passing control when does not have control");
        }
        rDI.PassControlToDialogue();
    }

    public void EndSection()
    {
        rDI.EndRythmSection();
    }
    public void LoadAndBeginSection(string iD)
    {
        if (!noteSheets.ContainsKey(iD))
        {
            throw new System.Exception($"{nameof(noteSheets)} does not contain key {iD}");
        }
        Rythm.RythmEngine.Instance.Play(noteSheets[iD].song);
        sectionLoader.LoadAndBeginSectionNotes(noteSheets[iD].notes);    }
        
        
    }
   
}
