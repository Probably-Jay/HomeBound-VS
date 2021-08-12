using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class Raleigh : MonoBehaviour
    {

        [SerializeField] Transform downstairsPos;
        [SerializeField] Transform outsidePos;
        [SerializeField] Transform withFriendsPos;
        [SerializeField] string outsideString;
        [SerializeField] string withFriendsString;

        [SerializeField] bool skipMissedDialogue;

        Dialogue.DialogueInstance dialogueInstance;

        // Start is called before the first frame update
        void Awake()
        {
            dialogueInstance = GetComponent<Dialogue.DialogueInstance>();
            this.NullCheck(downstairsPos);
            this.NullCheck(outsidePos);
            this.NullCheck(withFriendsPos);
            this.NullCheck(outsideString);
            this.NullCheck(withFriendsString);
            this.NullCheck(dialogueInstance);
        }

        internal void MoveDownstairs()
        {
            transform.position = downstairsPos.position;
        }

        internal void MoveOutside()
        {
            transform.position = outsidePos.position;
            if (skipMissedDialogue)
            {
                ResetDialogue();
            }
            dialogueInstance.AddMainDialogueElement(outsideString);
        }

        internal void MoveBackToGroup()
        {
            transform.position = withFriendsPos.position;
            if (skipMissedDialogue)
            {
                ResetDialogue();
            }
            dialogueInstance.AddMainDialogueElement(withFriendsString);

        }
        private void ResetDialogue()
        {
            dialogueInstance.ClearMainDialogues();
            dialogueInstance.ResetMainDialogueIndex();
        }
    }
}