using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;
using Rythm;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dialogue
{
    public enum TypingMode
    {
        Instant
          , Word
          , WordByCharacter
          , Character
    }

    [RequireComponent(typeof(TMP_Text))]
    public class DialogueTyper : MonoBehaviour
    {

        public const string greyTag = "<color=#777777>";
        public const string escapedGreyTag = @"<color=\#777777>";
        public const string whiteTag = "<color=#FFFFFF>";
        public const string escapedWhiteTag = @"<color=\#FFFFFF>";

        public const string tealTag = "<color=#00ffe5>";
        public const string greeenTag = "<color=#42db5b>"; //#4fff4f
        public const string amberTag = "<color=#dba542>";
        public const string redTag = "<color=#d10000z>";
        


        TMP_Text display;
        [SerializeField] TMP_Text nameOutdisplay;
        [SerializeField] TMP_Text continuedisplay;
        [SerializeField] AnimationCurve whitenCurve;
        [SerializeField] AnimationCurve perfectWhitenCurve;


        BufferAndLivePhrase bufferAndLivePhrase = new BufferAndLivePhrase();


        Queue<DialoguePhrase> dialogueQueue = new Queue<DialoguePhrase>();

        public float StandardTypingDelay { get => standardDelay; set => standardDelay = value; }
        [SerializeField, Range(0, 0.75f)] float standardDelay = 0.05f; 
        public float RandomTypingDelayDelta { get => randomDelayDelta; set => randomDelayDelta = value; }
        [SerializeField, Range(0, 0.75f)] float randomDelayDelta = 0.04f;

        [SerializeField] KeyCode nextPhraseKey = KeyCode.RightArrow;
        private Coroutine textCoroutine;
        Coroutine typingCoroutine;

        public bool HasDialougeQueued => dialogueQueue.Count > 0;


       

        public event Action OnReachedEndOfQueue;
        public event Action OnTypedPhrase;
      

        public bool OnBeat { get 
            {
                if (onBeat && !RythmEngine.Instance.PlayingMusic)
                {
                    Debug.LogError("Cannot type on-beat when there is no beat!");
                    return false;
                }
                return onBeat;
            } 
            set => onBeat = value; 
        }
        [SerializeField] bool onBeat = false; 

        public TypingMode TypingMode { get => typingMode; set => typingMode = value; }
        [SerializeField] TypingMode typingMode;

        public float DisplayActionsPerBeat { get => displayActionsPerBeat; set => displayActionsPerBeat = value; }
        [SerializeField,Range(0.03125f,32)] float displayActionsPerBeat = 2f;

        public float SpaceWordByCharacterFillsInBeat { get => spaceWordByCharacterFillsInBeat; set => spaceWordByCharacterFillsInBeat = value; }
        [SerializeField, Range(0,1)] float spaceWordByCharacterFillsInBeat = 0.3f;
      
        private string nameString;


        //  private Coroutine fillingCoroutine;

        bool beenQueuedThisConversation = false;
        private GrayedOutText grayedOutText = new GrayedOutText();

        public long Context { get; private set; } = 0;
        public bool Paused { get; private set; }

        public void IncrimentContext() => Context++;

        private void Awake()
        {
            display = GetComponent<TMP_Text>();
        }


        internal void ClearBox()
        {
            bufferAndLivePhrase.Reset();

            nameString = "";

            display.text = "";
            nameOutdisplay.text = "";
           

            beenQueuedThisConversation = false;

            IncrimentContext();
        }

        public void SetName(string name)
        {
            nameString = name;
        }

        public void SetContinueDisplayShow(bool show)
        {
            continuedisplay.gameObject.SetActive(show);
            continuedisplay.text = show ? $"Press {nextPhraseKey}" : "";
        }

        public void StartNewNormal()
        {
            StopCurrent();

            textCoroutine = StartCoroutine(UpdateBufferedPhrase());
            typingCoroutine = StartCoroutine(MoveBufferToLive());
        }

        public void ResumeNormal()
        {
            StopCoroutines();
            grayedOutText.Clear();
            if(dialogueQueue.Count > 0)
            {
                beenQueuedThisConversation = true;
            }

            textCoroutine = StartCoroutine(UpdateBufferedPhrase());
            typingCoroutine = StartCoroutine(MoveBufferToLive());
        }
        
        public void StartNewRythm()
        {
            StopCoroutines();

            typingCoroutine = StartCoroutine(MoveBufferToLive());
        }

        public void ProgressRythm()
        {
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);
            grayedOutText.Clear();
            textCoroutine = StartCoroutine(BufferNextPhrase());
            
        }


        // Update is called once per frame
        void Update()
        {
            UpdateDisplay();

        }

        public void StopCurrent()
        {
            StopCoroutines();

            ClearQueuedPhrases();

            //  bufferPhrase = new DialoguePhrase();
            //liveString.Clear();

            grayedOutText.Clear();

            ClearBox();
        }

        public void PauseTyping(int value)
        {
            Paused = true;
   
            RythmEngine.Instance.QueueActionAfterBeats(() => Paused = false, value, 1/16f);
        }

        private void StopCoroutines()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
                textCoroutine = null;
            }
        }

        private void ClearQueuedPhrases()
        {
            foreach (var phrase in dialogueQueue)
            {
                phrase.UnQueue();
            }
            dialogueQueue.Clear();
        }

        /// <summary>
        /// Queue a phrase to be shown in the box as the user progresses thorough, will enter the queue on a beat
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="onBeat">The beat this phrase will enter the queue</param>
        /// <param name="forceContext">Experimental, will only allow the word to be added within this instance of conversation</param>
        public void QueueNewPhrase(DialoguePhrase phrase, float? onBeat = null, bool forceContext = false)
        {
            beenQueuedThisConversation = true;
            phrase.SetQueued();

            if ((RythmEngine.TryInstance?.PlayingMusic ?? false) && onBeat.HasValue)
            {
                QueuePhraseOnBeat(phrase, onBeat.Value, forceContext);
                return;
            }

            QueuePhrase(phrase);
        }



        public void ProgressNewPhraseDirectly(string speaker, float? onBeat = null, bool forceContext = false)
        {
            if ((RythmEngine.TryInstance?.PlayingMusic ?? false) && onBeat.HasValue)
            {
                ProgressPhraseDirectlyOnbeat(speaker, onBeat.Value, forceContext);
                return;
            }

            ProgressPhraseDirectly(speaker);
        }

      
        private void ProgressPhraseDirectly(string speaker, long? _context = null)
        {
            if (_context.HasValue && _context != Context)
            {
                return;
            }
            ClearCurrentPhrase();
            SetSpeaker(speaker);
        }

        private void ProgressPhraseDirectlyOnbeat(string speaker, float beat, bool forceContext)
        {
            long? _context = forceContext ? (long?)Context : null;
            RythmEngine.Instance.QueueActionAtExplicitBeat(() => ProgressPhraseDirectly(speaker, _context), beat);

        }

        /// <summary>
        /// Adds a new word directly to the buffer, optionally with a beat specified. *WARNING* this may result in unwanted beheaviour if beat is too far in the future
        /// </summary>
        /// <param name="word">string to be added</param>
        /// <param name="onBeat">The beat this will happen on</param>
        /// <param name="forceContext">Experimental, will only allow the word to be added within this instance of conversation</param>
        [System.Obsolete("This has been replaced by " + nameof(UnGreyOutWord))]
        public void AddNewWordDirectlyOld(string word, HitQuality hitQuality, float? onBeat = null, bool forceContext = false)
        {
            if ((RythmEngine.TryInstance?.PlayingMusic ?? false) && onBeat.HasValue)
            {
                AddWordDirectlyOnbeatOld(word, hitQuality, onBeat.Value, forceContext);
                return;
            }

            AddWordDirectlyOld(word, hitQuality);
        }

        public void UnGreyOutWord(string word, HitQuality hitQuality, float? onBeat = null, bool forceContext = false)
        {
            if ((RythmEngine.TryInstance?.PlayingMusic ?? false) && onBeat.HasValue)
            {
                UnGreyOutWordOnbeat(word, hitQuality, onBeat.Value, forceContext);
                return;
            }

            UnGreyOutWordDirectly(word, hitQuality);
        }

        private void UnGreyOutWordOnbeat(string word, HitQuality hitQuality, float beat, bool forceContext)
        {
            long? _context = forceContext ? (long?)Context : null;
            RythmEngine.Instance.QueueActionAtExplicitBeat(() => {
                //    Debug.Log($"_{word} {beat}: {RythmEngine.Instance.CurrentBeat}"); 
                UnGreyOutWordDirectly(word, hitQuality, _context);
            }, beat);
        }
        private void UnGreyOutWordDirectly(string word, HitQuality hitQuality, float? _context = null)
        {
            if (_context.HasValue && _context != Context)
            {
                Debug.LogError($"Word {word} out of context ({_context} vs {Context})");
                return;
            }
            grayedOutText.WhitenWord(word, hitQuality);
        }

        internal void UpdateGreyedOut()
        {
            bufferAndLivePhrase.Reset();
            bufferAndLivePhrase.AddToLiveDirectly(grayedOutText.Text);
        }



        /// <summary>
        /// "WARNING: Will add a string directly to the output and will bypass typing mode*
        /// </summary>
        /// <param name="tag"></param>
        public void AddRichTextTag(string tag)
        {
            bufferAndLivePhrase.AddToLiveDirectly(tag);
        }

        internal void AddLinePreview(string line)
        {
            if (!bufferAndLivePhrase.Empty)
            {
                Debug.LogError("Box is not empty");
            }
            if (!grayedOutText.Empty)
            {
                grayedOutText.Clear();
            }
            grayedOutText.AddLine(line, this, whitenCurve, perfectWhitenCurve);
            bufferAndLivePhrase.AddToLiveDirectly(grayedOutText.Text.ToString());
            OnTypedPhrase?.Invoke();
        }

        private void QueuePhrase(DialoguePhrase phrase, long? _context = null)
        {
            if (_context.HasValue && _context != Context)
            {
                return;
            }
            dialogueQueue.Enqueue(phrase);
        }

        private void QueuePhraseOnBeat(DialoguePhrase phrase, float beat, bool forceContext)
        {
            long? _context = forceContext ? (long?)Context : null;
            RythmEngine.Instance.QueueActionAtExplicitBeat(() => QueuePhrase(phrase, _context), beat);
        }

        private void AddWordDirectlyOld(string word, HitQuality hitQuality, long? _context = null)
        {
            if(_context.HasValue && _context != Context)
            {
                Debug.LogWarning($"Word {word} out of context ({_context} vs {Context})");
                return;
            }
            bufferAndLivePhrase.bufferPhrase.AddDirectly(word);

        }

        private void AddWordDirectlyOnbeatOld(string word, HitQuality hitQuality, float beat, bool forceContext)
        {
            long? _context = forceContext ? (long?)Context : null;
            RythmEngine.Instance.QueueActionAtExplicitBeat(() => { 
                AddWordDirectlyOld(word, hitQuality, _context); 
            }, beat); 

        }

        private IEnumerator UpdateBufferedPhrase()
        {
            while (true)
            {
                yield return BufferNextPhrase();

                yield return null; // wait at least one frame
                yield return WaitForInput();

                if (bufferAndLivePhrase.bufferPhrase.StillMovingBufferToLive)
                {
                    SkipToInstantFill();

                    yield return null; // wait at least one frame
                    yield return WaitForInput();
                }
            }
        }

        private IEnumerator WaitForInput()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(nextPhraseKey));
        }

        private IEnumerator WaitForDialogueEnqueue()
        {
            yield return new WaitUntil(() => HasDialougeQueued);
        }

        private IEnumerator BufferNextPhrase()
        {
            ClearCurrentPhrase();

            if (!HasDialougeQueued)
            {
                if (beenQueuedThisConversation) // prevents this from invoking before any phrases queued
                {
                    OnReachedEndOfQueue?.Invoke();
                }
                yield return WaitForDialogueEnqueue();
            }

            DialoguePhrase phrase = DequeueNextPhrase();

            InvokePhraseInitialActions(phrase);

            bufferAndLivePhrase.bufferPhrase.SetPhrase(phrase);
            
            SetSpeaker(phrase.Speaker);
        }

        private void SetSpeaker(string speaker)
        {
            nameString = speaker;
        }

        private void ClearCurrentPhrase()
        {
            bufferAndLivePhrase.Reset();
        }

        private void InvokePhraseInitialActions(DialoguePhrase phrase) => phrase.TriggerActions();

        private void UpdateDisplay()
        {
            display.text = bufferAndLivePhrase.LiveText;
            nameOutdisplay.text = nameString;
        }



        DialoguePhrase DequeueNextPhrase()
        {
            if(!HasDialougeQueued)
            {
                return null;
            }
            
            return dialogueQueue.Dequeue();     
        }

    
        
       

        IEnumerator MoveBufferToLive()
        {
            while (true)
            {
                yield return StartCoroutine(FillDialogeBox());

                if(!bufferAndLivePhrase.bufferPhrase.StillMovingBufferToLive)
                {
                    OnTypedPhrase?.Invoke();
                    yield return new WaitUntil(() => bufferAndLivePhrase.bufferPhrase.StillMovingBufferToLive);
                }
            }
        }

        IEnumerator FillDialogeBox()
        {
            while (bufferAndLivePhrase.bufferPhrase.StillMovingBufferToLive)
            {
                if (Paused)
                {
                    yield return new WaitUntil(() => !Paused);
                }
                //while (Paused)
                //{
                //    Debug.Log("Paused");
                //    yield return null;
                //}

                if (OnBeat)
                {
                    yield return RythmEngine.Instance.WaitUntilNextBeat(resolution: 1f / DisplayActionsPerBeat);
                }
                else
                {
                    yield return new WaitForSeconds(StandardTypingDelay + UnityEngine.Random.Range(0, RandomTypingDelayDelta));
                }


                switch (TypingMode)
                {
                    case TypingMode.Instant:
                        FillInstant();
                        break;
                    case TypingMode.Word:
                        FlowWordWhole();
                        break;
                    case TypingMode.WordByCharacter:
                        yield return FlowWordByCharacter();
                        break;
                    case TypingMode.Character:
                        FlowCharacterBeatless();
                        break;
                }
            }
        }

        void SkipToInstantFill()
        {
            Paused = false;
            FillInstant();
        }

        void FillInstant()
        {
            bufferAndLivePhrase.MoveAllRemainingTextToLive();
            Paused = false;
        }

        void FlowWordWhole()
        {
            bufferAndLivePhrase.MoveNextWordToLive();
        }

        void FlowCharacterBeatless()
        {
            bufferAndLivePhrase.MoveNextCharacterToLive();
        }

        IEnumerator FlowWordByCharacter()
        {
            string word = bufferAndLivePhrase.bufferPhrase.GetNextWord();
            yield return FlowCharacterOnbeat(word, beatsToFill: 1/DisplayActionsPerBeat, spaceWordFillsInBeat: SpaceWordByCharacterFillsInBeat);
        }

        IEnumerator FlowCharacterOnbeat(string text, float beatsToFill, float spaceWordFillsInBeat = 1)
        {
            var dt = Time.deltaTime;

            float durationOfCharacter;
            if (OnBeat)
            {
                float durationOfWord = (beatsToFill * spaceWordFillsInBeat * (RythmEngine.Instance.DurationOfBeat/DisplayActionsPerBeat));
                durationOfCharacter = durationOfWord / text.Length; // each word takes the same amount of time
            }
            else
            {
                durationOfCharacter = StandardTypingDelay;
            }

            foreach (var character in text)
            {
                if (character == '.')
                {
                    durationOfCharacter *= 4; // longer delay after sentence end
                }


                bufferAndLivePhrase.AddToLiveDirectly(character); // already handled instructions (hopefully)

                if ((dt -= durationOfCharacter) > 0)
                {
                    continue;
                }

                dt = Time.deltaTime;
                if (!OnBeat) durationOfCharacter += UnityEngine.Random.Range(0, spaceWordFillsInBeat * RandomTypingDelayDelta);

              //  Debug.LogWarning("Fiules");

                yield return new WaitForSeconds(durationOfCharacter);
            }
        }

    


    }

    internal class GrayedOutText
    {
        string text = "";

        public bool Active { get; private set; }

        public string Text { get => text; }
        public bool Empty => text.Length == 0;

        AnimationCurve curve;
        private AnimationCurve perfectCurve;
        DialogueTyper parent;

        public void AddLine(string line, DialogueTyper parent, AnimationCurve curve, AnimationCurve perfectWhitenCurve) 
        {
            if (Active)
            {
                Debug.LogError("Grayed out text already active");
            }

            this.parent = parent;
            this.curve = curve;
            this.perfectCurve = perfectWhitenCurve;

            var words = line.Split(' ');
            StringBuilder sb = new StringBuilder();
            foreach (var word in words)
            {
                sb.Append($"{DialogueTyper.greyTag}{word}{DialogueTyper.whiteTag} ");
            }

            text = sb.ToString();
            Active = true;
        }

        public void WhitenWord(string word, HitQuality hitQuality)
        {
            parent.StartCoroutine(WhitenCoroutine(word, hitQuality, parent));
        }

        IEnumerator WhitenCoroutine(string word, HitQuality hitQuality, DialogueTyper parent)
        {
           // float st = Time.time;
            float ct = 0;
            word = word.Trim();
            string pattern = $@"{DialogueTyper.escapedGreyTag}{word}{DialogueTyper.escapedWhiteTag}";
            string hitQualityTag = GetHitQuality(hitQuality);

            // float animationFactor = hitQuality != HitQuality.Perfect ? 1 : 1.5f;
            // float animationRate= hitQuality != HitQuality.Perfect ? 1 : 2f/3f;

            bool isPerfect = hitQuality == HitQuality.Perfect;

            while (Active && ct < 1)
            {
                ct += Time.deltaTime;
                string replacement = GetReplacement(word, hitQualityTag, ct, isPerfect);

                Regex regex = new Regex(pattern);

                if (Active)
                    text = regex.Replace(text, replacement, 1);

                pattern = replacement;

                parent.UpdateGreyedOut();
                yield return null;
            }
        }

        private string GetReplacement(string word, string hitQualityTag, float ct, bool isPerfect)
        {
            string percent = GetSizePercent(ct, isPerfect);
            
            return $"<b>{hitQualityTag}<size={percent}%>{word}</size></color></b>";
        }

        private string GetHitQuality(HitQuality hitQuality)
        {
            switch (hitQuality)
            {
                case HitQuality.Miss: return DialogueTyper.redTag;

                case HitQuality.Early: return DialogueTyper.amberTag;

                case HitQuality.Late: return DialogueTyper.amberTag;

                case HitQuality.Good: return DialogueTyper.greeenTag;

                case HitQuality.Great: return DialogueTyper.greeenTag;

                case HitQuality.Perfect: return DialogueTyper.tealTag;

                default: throw new NotImplementedException();
            }
        }

        private string GetSizePercent(float ct, bool isPerfect)
        {
            float s = EvaluateSize(ct,isPerfect);
            int size = (int)(s * 100f);
            string percent = size.ToString("00");
            return percent;
        }

        private float EvaluateSize(float t, bool isPerfect)
        {
            return !isPerfect ? curve.Evaluate(t):perfectCurve.Evaluate(t);
        }

        public void Clear()
        {
            text = "";
            Active = false;
        }


    }

    internal class BufferAndLivePhrase
    {
        public readonly BufferPhrase bufferPhrase =  new BufferPhrase();
        public string LiveText => livePhrase.ToString();

        public bool Empty => bufferPhrase.Empty && livePhrase.Length == 0;

        readonly StringBuilder livePhrase = new StringBuilder();


        public void AddToLiveDirectly(string str) => livePhrase.Append(str);
        public void AddToLiveDirectly(char c) => livePhrase.Append(c);



        public void Reset()
        {
            bufferPhrase.Reset();
            livePhrase.Clear();
        }


        public void MoveNextCharacterToLive()
        {
            char? character = bufferPhrase.GetNextCharacter();
            if (character.HasValue)
            {
                livePhrase.Append(character);
            }
        }

        public void MoveNextWordToLive()
        {
            string word = bufferPhrase.GetNextWord();
            livePhrase.Append(word);
        }

        public void MoveAllRemainingTextToLive()
        {
            while (bufferPhrase.StillMovingBufferToLive)
            {
                MoveNextCharacterToLive();
            }
            
        }


        internal class BufferPhrase 
        {
            private DialoguePhrase dialoguePhrase = new DialoguePhrase();

            public void AddDirectly(string word) => dialoguePhrase.Phrase.Append(word);

            public void SetPhrase(DialoguePhrase phrase)
            {
                AddDirectly(phrase.Phrase.ToString());

                this.dialoguePhrase.CopyInstructions(phrase);
               
            }

            private char NextCharacter => dialoguePhrase.Phrase[nextCharacterIndex];
            public int nextCharacterIndex { get; private set; } = 0;

            public bool StillMovingBufferToLive => nextCharacterIndex < dialoguePhrase.Phrase.Length;

            public bool Empty => dialoguePhrase.Phrase.Length == 0;

            public char? GetNextCharacter()
            {
                if (!StillMovingBufferToLive)
                {
                    return null;
                }


                if (NextCharacter != '[')
                {
                    
                    char nextCharacter = NextCharacter;
                    nextCharacterIndex++;
                    return nextCharacter;
                }

                else
                {
                    HandleInlineInstruction();
                    // return GetNextCharacter();
                    return null;
                }


            }


            private void HandleInlineInstruction()
            {
                if (!StillMovingBufferToLive)
                    return;

                if(NextCharacter == '[')
                {
                    string numberString = "";
                    
                    while(StillMovingBufferToLive)
                    {
                        nextCharacterIndex++;
                        if(NextCharacter == ']')
                        {
                            break;
                        }
                        numberString += NextCharacter;
                    }
                    nextCharacterIndex++; // skip over trailing ']'

                    int instructionIndex = int.Parse(numberString);


                    dialoguePhrase.InvokeInstrunction(instructionIndex);   

                    

                }
            }

            public string GetNextWord()
            {
                if (!StillMovingBufferToLive)
                {
                    return null;
                }

                StringBuilder word = new StringBuilder();


                bool foundSpace = false;
                while (StillMovingBufferToLive && !foundSpace)
                {
                    var currentChar = GetNextCharacter();
                    word.Append(currentChar);
                    foundSpace = currentChar == ' ';
                }

                return word.ToString();
            }


            public void Reset()
            {
                dialoguePhrase = new DialoguePhrase();
                nextCharacterIndex = 0;
            }
        }


    }

}