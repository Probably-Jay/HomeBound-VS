using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace TestScripts
{

    public class DialogueBoxTester : MonoBehaviour
    {
        [SerializeField] DialogueManager dm;


        string[] sentences = new string[] { "Oh hello there", "how goes it", "not too bad yourself?" };
        string[] names = new string[] { "Apple", "James" };

        int wordCount = 0;
        int phraseCount = -1;


        public void EnterRythm()
        {
            dm.transform.parent.gameObject.SetActive(true);
            dm.EnterRythmEncounter("");
            ProgressPhrase();
        }

        public void AddWord()
        {
            string[] words = sentences[phraseCount].Split(' ');

            if(wordCount >= sentences.Length) { return; }

            dm.AddWordDirectly(words[wordCount] + ' ');

            wordCount++;
        }

        public void ProgressPhrase()
        {
            if(phraseCount >= sentences.Length)
            {
                dm.Close();
                return;
            }

            phraseCount++;
            dm.ProgressNewPhraseDirectly(names[phraseCount % 2]);
            wordCount = 0;

        }



    }
}