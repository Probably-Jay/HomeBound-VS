﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;
using System;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject wordNotePrefab;
    [SerializeField] Canvas canvas;
    private RhythmSectionManager rSM;

    public void SpawnNote(string word, float climaxBeat, NoteSystem.Lane lane, float ? spawnBeat = null)
    {
        if (!rSM.HasControl)
        {
            Debug.LogError("Attempting to spawn note when does not have control!");
            return;
        }
        Debug.Log("Spawned");
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

    internal void Init(RhythmSectionManager rSM)
    {
        this.rSM = rSM;
    }
}
