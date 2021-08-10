using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using System;

// refactor when proper game management system is implimented

public class DialogueBoxOpener : MonoBehaviour
{
    [SerializeField] DialogueManager dialogeBox;
    [SerializeField] bool debugStartBoxOpen;
    GameObject dialogeBoxParent;

    public event Action OnBoxOpen;
    public event Action OnBoxClose;


    public DialogueManager DialogeBox { get => dialogeBox; }

    private void OnEnable()
    {
        DialogeBox.OnQueueDepleated += CloseBox;
    }

    private void OnDisable()
    {
        DialogeBox.OnQueueDepleated -= CloseBox;
    }

    public void CloseBox()
    {
        DialogeBox.Close();
        Game.GameContextController.Instance.ReturnToPreviousContext();
        OnBoxClose?.Invoke();
    }

    private void Awake()
    {
        dialogeBoxParent = DialogeBox.transform.parent.gameObject;
    }
        

    private void Start()
    {
        if (!debugStartBoxOpen)
        {

            dialogeBoxParent.SetActive(false);
        }
        DialogeBox.Load(Game.TextAssetFolders.Main);
    }

    internal void StartDialogue(string id)
    {
        id = id.ToLowerInvariant();
        dialogeBoxParent.SetActive(true);
        DialogeBox.BeginConversation(id);
        Game.GameContextController.Instance.PushContext(Game.Context.Dialogue);
        OnBoxOpen?.Invoke();
    }


#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseBox();
        }
    }

#endif
}
