using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;
using Rythm;
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
        TMP_Text display;
        [SerializeField] TMP_Text nameOutdisplay;

        StringBuilder liveString = new StringBuilder();

        // StringBuilder bufferString = new StringBuilder();

        DialoguePhrase bufferPhrase = new DialoguePhrase();

        Queue<DialoguePhrase> dialogueQueue = new Queue<DialoguePhrase>();

        public float StandardTypingDelay { get => standardDelay; set => standardDelay = value; }
        [SerializeField, Range(0, 0.75f)] float standardDelay = 0.05f; 
        public float RandomTypingDelayDelta { get => randomDelayDelta; set => randomDelayDelta = value; }
        [SerializeField, Range(0, 0.75f)] float randomDelayDelta = 0.04f;

        [SerializeField] KeyCode nextPhraseKey = KeyCode.RightArrow;
        private Coroutine textCoroutine;
        Coroutine typingCoroutine;

        public bool HasDialougeQueued => dialogueQueue.Count > 0;
        public bool StillFillingInBox => liveString.Length < bufferPhrase.Phrase.Length;

        public event Action OnReachedEndOfQueue;
      

        public bool OnBeat { get => onBeat && RythmEngine.InstanceExists; set => onBeat = value; }
        [SerializeField] bool onBeat = false; 

        public TypingMode TypingMode { get => typingMode; set => typingMode = value; }
        [SerializeField] TypingMode typingMode;

        public float DisplayActionsPerBeat { get => displayActionsPerBeat; set => displayActionsPerBeat = value; }
        [SerializeField,Range(0.03125f,32)] float displayActionsPerBeat = 2f;

        public float SpaceWordByCharacterFillsInBeat { get => spaceWordByCharacterFillsInBeat; set => spaceWordByCharacterFillsInBeat = value; }
        [SerializeField, Range(0,1)] float spaceWordByCharacterFillsInBeat = 0.3f;
      
        private string nameString;
        private Coroutine fillingCoroutine;

        bool beenQueuedThisConversation = false;

        public long Context { get; private set; } = 0;

        public void IncrimentContext() => Context++;

        private void Awake()
        {
            display = GetComponent<TMP_Text>();
        }


        // Start is called before the first frame update
        void Start()
        {
           // StartNew();

            //QueueNewPhrase("What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, " +
            //    "and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with " +
            //    "precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am " +
            //    "contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes " +
            //    "out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I " +
            //    "extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, " +
            //    "you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't," +
            //    " and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.");
        }

       

        public void StartNewNormal()
        {
            textCoroutine = StartCoroutine(UpdateBufferedPhrase());
            typingCoroutine = StartCoroutine(MoveBufferToLive());
        }        
        
        public void StartNewRythm()
        {
            typingCoroutine = StartCoroutine(MoveBufferToLive());
        }


        // Update is called once per frame
        void Update()
        {
            UpdateDisplay();

        }

        public void StopCurrent()
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            if (fillingCoroutine != null)
                StopCoroutine(fillingCoroutine);
            if (textCoroutine != null)
                StopCoroutine(textCoroutine);

            ClearQueuedPhrases();
            bufferPhrase = new DialoguePhrase();
            liveString.Clear();
            nameString = "";

            display.text = "";
            nameOutdisplay.text = "";

            beenQueuedThisConversation = false;

            IncrimentContext();
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

            if ((RythmEngine.TryInstance?.InRythmSection ?? false) && onBeat.HasValue)
            {
                QueuePhraseOnBeat(phrase, onBeat.Value, forceContext);
                return;
            }

            QueuePhrase(phrase);
        }



        public void ProgressNewPhraseDirectly(string speaker, float? onBeat = null, bool forceContext = false)
        {
            if ((RythmEngine.TryInstance?.InRythmSection ?? false) && onBeat.HasValue)
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
        public void AddNewWordDirectly(string word, float? onBeat = null, bool forceContext = false)
        {
            if ((RythmEngine.TryInstance?.InRythmSection ?? false) && onBeat.HasValue)
            {
                AddWordDirectlyOnbeat(word, onBeat.Value, forceContext);
                return;
            }

            AddWordDirectly(word);
        }

        /// <summary>
        /// "WARNING: Will add a string directly to the output and will bypass typing mode*
        /// </summary>
        /// <param name="tag"></param>
        public void AddRichTextTag(string tag)
        {
            liveString.Append(tag);
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

        private void AddWordDirectly(string word, long? _context = null)
        {
            if(_context.HasValue && _context != Context)
            {
                return;
            }
            bufferPhrase.Phrase.Append(word);
        }

        private void AddWordDirectlyOnbeat(string word, float beat, bool forceContext)
        {
            long? _context = forceContext ? (long?)Context : null;
            RythmEngine.Instance.QueueActionAtExplicitBeat(() => AddWordDirectly(word, _context), beat);

        }

        private IEnumerator UpdateBufferedPhrase()
        {
            while (true)
            {
                yield return BufferNextPhrase();

                yield return null; // wait at least one frame
                yield return WaitForInput();

                if (StillFillingInBox)
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
                if (beenQueuedThisConversation) // do not do this before begining
                    OnReachedEndOfQueue?.Invoke();

                yield return WaitForDialogueEnqueue();
            }

            DialoguePhrase phrase = DequeueNextPhrase();
            InvokePhraseActions(phrase);

            bufferPhrase.Phrase.Append(phrase.Phrase);
            foreach (var item in phrase.inlineInstructions)
            {
                bufferPhrase.inlineInstructions.Add(item.Key,item.Value);
            }
            SetSpeaker(phrase.Speaker);
        }

        private void SetSpeaker(string speaker)
        {
            nameString = speaker;
        }

        private void ClearCurrentPhrase()
        {
            liveString.Clear();
            bufferPhrase.Phrase.Clear();
            bufferPhrase.inlineInstructions.Clear();
        }

        private void InvokePhraseActions(DialoguePhrase phrase) => phrase.TriggerActions();

        private void UpdateDisplay()
        {
            display.text = liveString.ToString();
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
                yield return  StartCoroutine(FillDialogeBox());

                if(!StillFillingInBox)
                {
                    yield return new WaitUntil(() => !StillFillingInBox);
                }
            }
        }

        IEnumerator FillDialogeBox()
        {
            while (StillFillingInBox)
            {

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
                        FlowCharacterWhole();
                        break;
                }
            }
        }

        void SkipToInstantFill()
        {
            StopCoroutine(fillingCoroutine);
            // liveString.Clear();
            FillInstant();
           // FillRestInstant(bufferPhrase);
        }

        void FillInstant()
        {
            //// remove any inline instructions
            //var escapedPattern = new Regex(@"\[\d+\]");
            //phrase = escapedPattern.Replace(phrase, "");

            //// trigger all instructions
            ///

            bool inInstructionID = false;
            string str = bufferPhrase.Phrase.ToString();
            for (int i = 0; i < str.Length; i++)
            {
                char item = (char)str[i];
                if (inInstructionID)
                {

                }
                else
                {
                    if(item == '[')
                    {
                        inInstructionID = true;
                        continue;
                    }

                   // if

                }
            //liveString.Append(phrase);
            }


        }

        void FlowWordWhole()
        {
            string word = GetWord(bufferPhrase.Phrase.ToString(), currentIndex: liveString.Length);
            // check for cmd
            liveString.Append(word);
        }

        IEnumerator FlowWordByCharacter()
        {
            string word = GetWord(bufferPhrase.phraseContextID.ToString(), currentIndex: liveString.Length);
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

                liveString.Append(character);

                if ((dt -= durationOfCharacter) > 0)
                {
                    continue;
                }

                dt = Time.deltaTime;
                if (!OnBeat) durationOfCharacter += UnityEngine.Random.Range(0, spaceWordFillsInBeat * RandomTypingDelayDelta);
                yield return new WaitForSeconds(durationOfCharacter);
            }
        }

        void FlowCharacterWhole()
        {
            char character = GetCharacter(bufferPhrase.Phrase.ToString(), currentIndex: liveString.Length);
            liveString.Append(character);
        }

       

        private string GetWord(string text, int currentIndex)
        {
            StringBuilder word = new StringBuilder();
            var index = currentIndex;
            char character = default;

            while (character != ' ' && currentIndex + word.Length < text.Length)
            {
                index = currentIndex + word.Length;
                character = text[index];
                word.Append(character);
            }

            return word.ToString();
        }
        private char GetCharacter(string text, int currentIndex) => text[currentIndex];

        //IEnumerator AddWords()
        //{
        //    var index = liveString.Length;
        //    liveString.Append(bufferString[index]);
        //    var waitTime = StandardTypingDelay + UnityEngine.Random.Range(0, RandomTypingDelayDelta);
        //    yield return new WaitForSeconds(waitTime);
        //}

    }
}