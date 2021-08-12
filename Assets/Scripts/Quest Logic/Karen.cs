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
            this.NullCheck(newPos);
            this.NullCheck(lookRight);
            this.NullCheck(newDialogueID);
            this.NullCheck(dialogueInstance);
            this.NullCheck(sp);
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
