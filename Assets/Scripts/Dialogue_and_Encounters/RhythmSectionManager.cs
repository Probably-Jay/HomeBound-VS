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

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadSection(string iD)
    {
        sectionLoader.LoadSection(noteSheets[iD]);
    }
    public void PassToDialogue()
    {
        rDI.PassControlToDialogue();
    }
    
}
