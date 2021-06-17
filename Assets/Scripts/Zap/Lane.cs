﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

namespace NoteSystem
{
    public class Lane : MonoBehaviour
    {
        [SerializeField] private Transform editorSP;
        [SerializeField] private HitDetection.HitZone editorHZ;
        [SerializeField] public Transform spawnPoint { get; private set; }
        [SerializeField] public HitDetection.HitZone hitZone { get; private set; }
        // Start is called before the first frame update
        void Awake()
        {
            spawnPoint = editorSP;
            hitZone = editorHZ;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
