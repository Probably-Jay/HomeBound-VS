using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class Raleigh : MonoBehaviour
    {

        [SerializeField] Transform downstairsPos;
        [SerializeField] Transform outsidePos;
        [SerializeField] string outsideString;

        Dialogue.DialogueInstance dialogueInstance;

        // Start is called before the first frame update
        void Awake()
        {
            dialogueInstance = GetComponent<Dialogue.DialogueInstance>();
            this.NullCheck(downstairsPos);
            this.NullCheck(outsidePos);
            this.NullCheck(outsideString);
            this.NullCheck(dialogueInstance);
        }

        internal void MoveDownstairs()
        {
            transform.position = downstairsPos.position;
        }

        internal void MoveOutside()
        {
            transform.position = outsidePos.position;
            dialogueInstance.AddMainDialogueElement(outsideString);
        }

    }
}