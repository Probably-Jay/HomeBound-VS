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
    //    [SerializeField] string downstairsString;
        [SerializeField] string outsideString;
        [SerializeField] string withFriendsString;

        [SerializeField] bool skipMissedDialogue;

        Dialogue.DialogueInstance dialogueInstance;

        // Start is called before the first frame update
        void Awake()
        {
            this.AssignComponent(out dialogueInstance);
            this.NotNullCheck(downstairsPos);
            this.NotNullCheck(outsidePos);
            this.NotNullCheck(withFriendsPos);
          //  this.NotNullCheck(downstairsString);
            this.NotNullCheck(outsideString);
            this.NotNullCheck(withFriendsString);
        }

        internal void MoveDownstairs()
        {
            transform.position = downstairsPos.position;
           // dialogueInstance.AddMainDialogueElement(downstairsString);
          //  dialogueInstance.AddBackupDialogueElement(downstairsString);
        }

        internal void MoveOutside()
        {
            transform.position = outsidePos.position;

            ResetDialogue();
            
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