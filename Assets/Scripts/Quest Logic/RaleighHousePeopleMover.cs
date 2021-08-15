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
            this.NotNullCheck(raliegh);
            this.NotNullCheck(karen);
            this.NotNullCheck(dad);
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