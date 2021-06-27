using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject wordNotePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnNote(string word, float climaxBeat, NoteSystem.Lane lane, float ? spawnBeat = null)
    {
        WordNote tempWordNote = GameObject.Instantiate(wordNotePrefab).GetComponent<WordNote>();
        if (spawnBeat.HasValue)
        {
            tempWordNote.initialise(word, climaxBeat, lane, spawnBeat);
        }
        else
        {
            tempWordNote.initialise(word, climaxBeat, lane);
        }
    }
}
