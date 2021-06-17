using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

public class RhythmInitialise : MonoBehaviour
{
    [SerializeField] TextAsset notes;
    [SerializeField] Rythm.RythmSong song;
    NoteSpawner noteSpawner;
    public List<Lane> lanes = new List<Lane> { };
    // Start is called before the first frame update
    void Start()
    {
        noteSpawner = this.GetComponent<NoteSpawner>();
        /*
        Rythm.RythmEngine.Instance.QueueActionNextBeat(() => {
            noteSpawner.SpawnNote("bumsex", 42, lanes[1]);
        }
                    );
        Rythm.RythmEngine.Instance.QueueActionNextBeat(() => {
            noteSpawner.SpawnNote("bumsex", 42, lanes[1]);
        }
                    );
                    */
        float offset = -4f;
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("It's", 42+offset, lanes[0]); }, 32+offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("a", 42.25f+offset, lanes[1]); }, 32.25f+offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("beautiful", 42.5f+offset, lanes[2]); }, 32.5f+offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("day", 43f+offset, lanes[0]); }, 33f+offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("outside", 43.75f + offset, lanes[0]); }, 33.75f + offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("birds", 44.25f + offset, lanes[1]); }, 34.25f + offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("are", 44.5f + offset, lanes[2]); }, 34.5f + offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("singing", 45.0f + offset, lanes[0]); }, 35.0f + offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("flowers", 45.250f + offset, lanes[1]); }, 35.25f + offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("are", 45.5f + offset, lanes[2]); }, 35.5f + offset);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("blooming", 45.750f + offset, lanes[0]); }, 35.75f + offset);
        //Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("dad", 44, lanes[1]); }, 34);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadSection()
    {

    }

}
