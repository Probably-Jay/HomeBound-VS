using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteSystem;

namespace NoteSystem {
    public class WordUI : MonoBehaviour
    {
        string word;
        WordNote wordNote;
        const float yOffset = 1f;

        [SerializeField] TMPro.TextMeshProUGUI text;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(wordNote.gameObject);
            this.transform.position = Camera.main.WorldToScreenPoint(wordNote.gameObject.transform.position + (Vector3.up * yOffset));
        }
        void UpdateWord(string passedWord)
        {
            word = passedWord;
            text.text = (word);
        }
        public void Initialise(string text, WordNote passedWordNote)
        {
            wordNote = passedWordNote;

            UpdateWord(text);

        }
        public void Remove()
        {
            this.gameObject.SetActive(false);
        }
    }

}
