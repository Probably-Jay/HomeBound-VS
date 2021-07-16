using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Replaced by " + nameof(RythmDialogueInterface),true)]
public class NoteDialogueInterface : MonoBehaviour
{
    [SerializeField] DialogueBoxOpener DialogueBoxOpener;
    [SerializeField] Dialogue.DialogueManager DialogueBox;
    // Start is called before the first frame update
    void Start()
    {
        //DialogueBox.BeginConversation("");
       // DialogueBox.EnterRythmEncounter("");
       //Rythm.RythmEngine.Instance.InRythmSection = true;
        //DialogueBox.SetTypingMode(Dialogue.DialogueMode.Encounter_PlayerSpeak);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public void AddWord(string word, float? hitBeat=null)
    //{
    //    word += ' ';
    //    if (hitBeat != null)
    //    {
    //        DialogueBox.AddWordDirectly(word, hitBeat);
    //    }
    //    else
    //    {
    //        DialogueBox.AddWordDirectly(word);

    //    }
    //}
}
