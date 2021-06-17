using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [RequireComponent(typeof(DialogueTyper),typeof(DialogueContextController))]
    public class DialogueManager : MonoBehaviour
    {
        DialogueLoader dialogueLoader;
        DialogueParser dialogueParser;
        DialogueTyper dialogueTyper;
        DialogueContextController dialogueContextController;

        private void Awake()
        {
            dialogueLoader = new DialogueLoader();
            dialogueParser = new DialogueParser();
            dialogueTyper = GetComponent<DialogueTyper>();
            dialogueContextController = GetComponent<DialogueContextController>();

            
        }

        public void Load(Game.TextAssetFolders folder) => dialogueLoader.Load(folder);

        public bool BeginConversation(string conversationID)
        {
            //  if(!dial)
            return default;
        }




    }
}