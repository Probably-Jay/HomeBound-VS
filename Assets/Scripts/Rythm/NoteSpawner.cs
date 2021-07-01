using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject wordNotePrefab;
    [SerializeField] Canvas canvas;


    public void SpawnNote(string word, float climaxBeat, NoteSystem.Lane lane, float ? spawnBeat = null)
    {
        WordNote tempWordNote = GameObject.Instantiate(wordNotePrefab).GetComponent<WordNote>();
        if (spawnBeat.HasValue)
        {
            tempWordNote.Initialise(word, climaxBeat, lane, canvas, spawnBeat);
        }
        else
        {
            tempWordNote.Initialise(word, climaxBeat, lane, canvas);
        }
    }
}
