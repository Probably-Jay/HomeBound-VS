using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class Karen : MonoBehaviour
    {
        [SerializeField] Transform newPos;
        [SerializeField] Sprite lookRight;
        [SerializeField] string newDialogueID;
        Dialogue.DialogueInstance dialogueInstance;
        SpriteRenderer sp;

        private void Awake()
        {
            dialogueInstance = GetComponent<Dialogue.DialogueInstance>();
            sp = GetComponent<SpriteRenderer>();
            this.NotNullCheck(newPos);
            this.NotNullCheck(lookRight);
            this.NotNullCheck(newDialogueID);
            this.NotNullCheck(dialogueInstance);
            this.NotNullCheck(sp);
        }

        internal void MoveDownstairs()
        {
            transform.position = newPos.position;
            sp.sprite = lookRight;
            dialogueInstance.AddMainDialogueElement(newDialogueID);
            dialogueInstance.ClearBackupDialogues();
        }


    }
}
