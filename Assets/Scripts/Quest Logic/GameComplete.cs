using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{
    public class GameComplete : MonoBehaviour
    {
        [SerializeField] Animator ftb;
        private bool fadedToBlack;

        private void Awake()
        {
            this.NotNullCheck(ftb);
        }

        public void FadedToBlack() => fadedToBlack = true;

        public void EndChapterOne()
        {
            StartCoroutine(EndChapterOneCoroutine());
        }

        private IEnumerator EndChapterOneCoroutine()
        {
            ftb.SetBool("Chapter1Complete", true);
            yield return new WaitUntil(() => fadedToBlack);
            SceneChange.SceneChangeController.Instance.ChangeScene(SceneChange.Scenes.Credits);
        }
    }
}