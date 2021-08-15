using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class RalieghOutsideMover : MonoBehaviour
    {
        [SerializeField] Raleigh raleigh;

        private void Awake()
        {
            this.NotNullCheck(raleigh);
        }

        public void MoveOutside()
        {
            raleigh.MoveOutside();
        }
    }
}
