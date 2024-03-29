﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteSystem;

namespace NoteSystem {
    public class WordUI : MonoBehaviour
    {
        string word = "";
        WordNote wordNote;
        const float yOffset = 0.66f;
        const float zOffset = 0.1f;

        [SerializeField] TMPro.TextMeshProUGUI text;


        // Update is called once per frame
        void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            //this.transform.position = Camera.main.WorldToScreenPoint(wordNote.gameObject.transform.position + (Vector3.up * yOffset));
            this.transform.position = wordNote.gameObject.transform.position + (Vector3.up * yOffset) + (-Vector3.forward*zOffset);
        }

        void UpdateWord(string passedWord)
        {
            word = passedWord;
            text.text = (word);
        }
        public void Initialise(string text, WordNote passedWordNote)
        {
            wordNote = passedWordNote;
            UpdatePosition();
            UpdateWord(text);

        }
        public void Remove()
        {
            this.gameObject.SetActive(false);
        }
    }

}
