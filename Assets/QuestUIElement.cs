using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    public class QuestUIElement : MonoBehaviour
    {
        private Quest quest;
        [SerializeField] Transform childParent;
        Transform transformParent;
        List<Transform> ts = new List<Transform>();
        bool p;

        public void Init(Quest quest)
        {
            this.quest = quest;
        }

        private void Start()
        {
            transformParent = transform.parent;
            for (int i = 0; i < childParent.childCount; i++)
            {
                ts.Add(childParent.GetChild(i));
            }
            Reparent();
        }

        public void Clicked()
        {
            if (p)
            {
                Unparent();
            }
            else
            {
                Reparent();
            }
        }

        private void Unparent()
        {
            int thisIndex = transform.GetSiblingIndex();
            for (int i = 0; i < ts.Count; i++)
            {
                Transform child = ts[i];
                child.gameObject.SetActive(true);
                child.SetParent(transformParent);
                child.SetSiblingIndex(thisIndex + 1 + i);
            }
            p = false;
        }

        private void Reparent()
        {
            foreach (var child in ts)
            {
                child.SetParent(childParent);
                child.gameObject.SetActive(false);
            }
            p = true;
        }
    }
}