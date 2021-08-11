using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;
using System;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject wordNotePrefab;
    [SerializeField] Canvas canvas;
    private RhythmSectionManager rSM;
    private StressPointRing stressPointRing;

    public void SpawnNote(string word, float climaxBeat, NoteSystem.Lane lane, float ? spawnBeat = null)
    {
        WordNote tempWordNote = GameObject.Instantiate(wordNotePrefab).GetComponent<WordNote>();
        if (spawnBeat.HasValue)
        {
            tempWordNote.Initialise(word, climaxBeat, lane, canvas, rSM, spawnBeat);
        }
        else
        {
            tempWordNote.Initialise(word, climaxBeat, lane, canvas, rSM);
        }
    }

    internal void Init(RhythmSectionManager rSM)
    {
        this.rSM = rSM;
    }
}
