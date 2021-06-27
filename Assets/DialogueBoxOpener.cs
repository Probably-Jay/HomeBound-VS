using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using System;

// refactor when proper game management system is implimented

public class DialogueBoxOpener : MonoBehaviour
{
    [SerializeField] DialogueManager dialogeBox;
    GameObject dialogeBoxParent;


    private void OnEnable()
    {
        dialogeBox.OnQueueDepleated += CloseBox;
    }

    private void OnDisable()
    {
        dialogeBox.OnQueueDepleated -= CloseBox;
    }

    private void CloseBox()
    {
        dialogeBox.Close();
        Game.GameContextController.Instance.ReturnToPreviousContext();

    }

    private void Awake()
    {
        dialogeBoxParent = dialogeBox.transform.parent.gameObject;
    }
        

    private void Start()
    {
        dialogeBoxParent.SetActive(false);
        dialogeBox.Load(Game.TextAssetFolders.Test);
    }

    public void StartDialogue(string id)
    {
        dialogeBoxParent.SetActive(true);
        dialogeBox.BeginConversation(id);
        Game.GameContextController.Instance.PushContext(Game.Context.Dialogue);
    }
}
