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

        // Start is called before the first frame update
        void Awake()
        {
            //code for testing, delete when initialising these from script
            spawningBeat = Rythm.RythmEngine.Instance.CurrentBeat;
            //end code for testing
            //absoluteBeatsToHit = climaxBeat - Rythm.RythmEngine.Instance.CurrentBeat;
            //Debug.Log(lane.gameObject);
            //Debug.Log(lane.hitZone);
            if (lane != null)
            {
                //lane.hitZone.Join(this);
            }
            beatsOfExistence = 0f;
            if (startPos != null)
            {
                this.transform.position = startPos.position;
            }
            //canvas = GameObject.Find("Canvas");
            //InitialiseWordUI();

        }

        // Update is called once per frame
        void Update()
        {
            if (!Rythm.RythmEngine.Instance.PlayingMusic || Rythm.RythmEngine.Instance.CurrentBeat - 10 > climaxBeat)
            {
                Remove();
                return;
            }

            beatsOfExistence += Time.deltaTime;
            // Debug.Log(Rythm.RythmEngine.Instance.CurrentBeat);
            this.transform.position = Vector3.LerpUnclamped(startPos.position, endPos.position, (float)((Rythm.RythmEngine.Instance.CurrentBeat - spawningBeat) / absoluteBeatsToHit));
        }
        public void Initialise(string passedWord, float hitBeat, Lane passedLane, Canvas canvas, float? spawnBeat = null)
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
            beatsOfExistence = 0f;
            startPos = lane.spawnPoint.transform;
            endPos = lane.hitZone.transform;
            this.transform.position = startPos.position;
            this.canvas = canvas;
            spriteRenderer.sprite = lane.noteSprite;
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
    }

}
