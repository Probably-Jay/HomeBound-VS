using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
   

    [CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/Dialogue/Conversation", order = 1)]
    public class Conversation : ScriptableObject
    {
        public string conversationID;
        public DialogueMode initialMode;
        public readonly List<DialoguePhrase> dialoguePhrases = new List<DialoguePhrase>();
        public long context;


        public event Action<DialogueMode> OnSetDialogueMode;
        public void SetDialougeMode(DialogueMode mode) => OnSetDialogueMode?.Invoke(mode);

        public event Action<int> OnSetColour;
        public void SetColour(int colour) => OnSetColour?.Invoke(colour);

        public event Action<string> OnTriggerRythmSection;
        public void StartRyhtmSection(string id) => OnTriggerRythmSection?.Invoke(id);

        public event Action<int> OnPause;
        /// <summary>
        /// Pause for <paramref name="v"/> <c>1/16</c> notes
        /// </summary>
        public void Pause(int v) => OnPause?.Invoke(v);

        public event Action<float> OnShake;
        public void Shake(float v) => OnShake?.Invoke(v);

        public event Action<string> OnBeginQuest;
        public void BeginQuest(string id) => OnBeginQuest?.Invoke(id);

        public event Action<string> OnCompleteQuestStep;
        public void CompleteQuestStep(string id) => OnCompleteQuestStep?.Invoke(id);

        public event Action<string> OnUnCompleteQuestStep;
        public void UnCompleteQuestStep(string id) => OnUnCompleteQuestStep?.Invoke(id);

        public event Action OnItalicise;
        public void Italicise() => OnItalicise?.Invoke();        
        
        public event Action OnUnItalicise;
        public void UnItalicise() => OnUnItalicise?.Invoke();       
        
        public event Action OnBold;
        public void Bold() => OnBold?.Invoke();        
        
        public event Action OnUnBold;
        public void UnBold() => OnUnBold?.Invoke();

        public event Action<int> OnStress;
        internal void Stress(int v) => OnStress?.Invoke(v);


        public void ClearEvents()
        {
            OnSetDialogueMode = null;
            OnSetColour = null;
            OnTriggerRythmSection = null;
            OnPause = null;
            OnShake = null;
            OnBeginQuest = null;


        }
    }


}