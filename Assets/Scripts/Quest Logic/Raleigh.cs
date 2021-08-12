using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class Raleigh : MonoBehaviour
    {

        [SerializeField] Transform downstairsPos;

        // Start is called before the first frame update
        void Start()
        {
            this.NullCheck(downstairsPos);
        }

        internal void MoveDownstairs()
        {
            transform.position = downstairsPos.position;
        }

    }
}