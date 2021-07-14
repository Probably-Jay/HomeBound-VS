using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleQuestLogic : MonoBehaviour
{
    [SerializeField] Dialogue.DialogueInstance dialogue;


    public void QuestBegun()
    {
        dialogue.ClearMainDialogues();
        dialogue.SetBackupDialogue(new List<string>() {"example_begin_1","example_begin_2" });
    }

    public void TaskOneComplete()
    {
        dialogue.SetMainDialogue(new List<string>() { "example_taskDone_1","example_taskDone_2","example_taskDone_3" });
        dialogue.ClearBackupDialogues();
    }

    public void QuestComplete()
    {
        Debug.Log("Quest complete");
    }

}
