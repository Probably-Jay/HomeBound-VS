using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{
    public class RaleighHousePeopleMover : MonoBehaviour
    {
        [SerializeField] Raleigh raliegh;
        [SerializeField] Karen karen;
        [SerializeField] GameObject dad;




        private void Awake()
        {
            this.NullCheck(raliegh);
            this.NullCheck(karen);
            this.NullCheck(dad);
        }

        public void MoveRalieghDownstairsAndRepositionParents()
        {
            raliegh.MoveDownstairs();
            karen.MoveDownstairs();
            dad.SetActive(true);
        }
        public void HideRaliegh()
        {
            raliegh.gameObject.SetActive(false);
        }
    }
}