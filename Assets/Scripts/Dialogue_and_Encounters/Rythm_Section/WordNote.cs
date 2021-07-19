using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

namespace NoteSystem {
    public class WordNote : MonoBehaviour
    {
        [SerializeField] Transform startPos;
        [SerializeField] Transform endPos;
        public float absoluteBeatsToHit;
        float beatsOfExistence = 0;
        float spawningBeat = 0;
        [SerializeField] private float climaxBeat;
        HitZone myHitZone;
        [SerializeField] Lane lane;
        [SerializeField] public string word { get; private set; }
        private Canvas canvas;
        [SerializeField] GameObject wordUIPrefab;
        WordUI wordUI;
        private RhythmSectionManager rSM;
        private bool hasMissed=false;

        public bool CanAddWord => rSM.HasControl;
        private void OnEnable()
        {
            Rythm.RythmEngine.Instance.OnSongChanged += Remove;   
        }
        private void OnDisable()
        {
            if (Rythm.RythmEngine.InstanceExists)
            {
                Rythm.RythmEngine.Instance.OnSongChanged -= Remove;
            }
        }


        // Start is called before the first frame update
        void Awake()
        {
            //code for testing, delete when initialising these from script
            spawningBeat = Rythm.RythmEngine.Instance.CurrentBeat;

            if (lane != null)
            {
                Debug.LogError("Note has no lane");
                //lane.hitZone.Join(this);
            }
            beatsOfExistence = 0f;
            if (startPos != null)
            {
                this.transform.position = startPos.position;
            }


        }

        // Update is called once per frame
        void Update()
        {
            if (!Rythm.RythmEngine.Instance.PlayingMusic || Rythm.RythmEngine.Instance.CurrentBeat - 5 > climaxBeat)
            {
                Remove();
                return;
            }

            beatsOfExistence += Time.deltaTime;

            var newPos = Vector3.LerpUnclamped(startPos.position, endPos.position, (float)((Rythm.RythmEngine.Instance.CurrentBeat - spawningBeat) / absoluteBeatsToHit));
            UodatePosition(newPos);

            if ((Rythm.RythmEngine.Instance.CurrentBeat > this.climaxBeat) & (HitDetection.CheckHit(climaxBeat, Rythm.RythmEngine.Instance.CurrentBeat) == HitQuality.Miss)&!hasMissed)
            {
                BroadcastHasMissed();
            }
        }

        private void UodatePosition(Vector3 newPos)
        {
            float distanceMoved = (newPos.x - transform.position.x);
            if (distanceMoved > +0.00001f) // if we have moved any small value to the right
            {
                Debug.LogError($"Note attempting to move note to the right! {(Mathf.Sign(distanceMoved) > 0 ? "+" : "")}{distanceMoved} Preventing.");
                return;
            }
            this.transform.position = newPos;
        }

        public void Initialise(string passedWord, float hitBeat, Lane passedLane, Canvas canvas, RhythmSectionManager rSM, float? spawnBeat = null)
        {
            SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            if (spawnBeat.HasValue)
            {
                spawningBeat = (float)spawnBeat.Value;
            }
            else
            {
                spawningBeat = Rythm.RythmEngine.Instance.CurrentBeat;
            }
            absoluteBeatsToHit = (float)hitBeat - spawningBeat;
            lane = passedLane;
            word = passedWord;
            climaxBeat = hitBeat;
            lane.Join(this);

            transform.SetParent(lane.transform); // Added to keep the scene hirarcy clean -J

            beatsOfExistence = 0f;
            startPos = lane.spawnPoint.transform;
            endPos = lane.hitZone.transform;
            this.transform.position = startPos.position;
            this.canvas = canvas;
            spriteRenderer.sprite = lane.noteSprite;
            this.rSM = rSM;
            spriteRenderer.sortingLayerName = "UI";
            InitialiseWordUI();
        }
        public float TargetBeat()
        {
            return (float)climaxBeat;
        }
        public float GetClimaxBeat()
        {
            return climaxBeat;
        }
        private void InitialiseWordUI()
        {
            GameObject tempWordUI = GameObject.Instantiate(wordUIPrefab, canvas.gameObject.transform);
            wordUI = tempWordUI.GetComponent<WordUI>();
            wordUI.Initialise(word, this);
        }
        public void Remove()
        {
            wordUI.Remove();
            Destroy(gameObject);
        }
        private void BroadcastHasMissed()
        {
            hasMissed = true;
            rSM.Strikethrough(this);
        }
    }

}
