using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Phrase", menuName = "ScriptableObjects/Dialouge", order = 1)]
    public class DialoguePhraseObject : ScriptableObject
    {
        public DialoguePhrase phrase;
    }
}