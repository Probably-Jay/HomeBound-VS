using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmSectionLoading;

public class RhythmSectionManager : MonoBehaviour
{
    Dictionary<string, TextAsset> noteSheets = new Dictionary<string, TextAsset> { };
    [SerializeField] List<string> identifications = new List<string> { };
    [SerializeField] List<TextAsset> sheets = new List<TextAsset> { };
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
    public void LoadSection(string iD)
    {
        if (!noteSheets.ContainsKey(iD))
        {
            throw new System.Exception($"{nameof(noteSheets)} does not contain key {iD}");
        }
        
        sectionLoader.LoadAndBeginSection(noteSheets[iD]);
        
    }
   
}
