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
        float leadTime = 5f;
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("It's", 42+offset, lanes[0]); }, 42+offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("a", 42.25f+offset, lanes[1]); }, 42.25f+offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("beautiful", 42.5f+offset, lanes[2]); }, 42.5f+offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("day", 43f+offset, lanes[0]); }, 43f+offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("outside", 43.75f + offset, lanes[0]); }, 43.75f + offset -leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("birds", 44.25f + offset, lanes[1]); }, 44.25f + offset - leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("are", 44.5f + offset, lanes[2]); }, 44.5f + offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("singing", 45.0f + offset, lanes[0]); }, 45.0f + offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("flowers", 45.250f + offset, lanes[1]); }, 45.25f + offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("are", 45.5f + offset, lanes[2]); }, 45.5f + offset-leadTime);
        Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("blooming", 45.750f + offset, lanes[0]); }, 45.75f + offset-leadTime);
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
