using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{
    [SerializeField]private Tilemap obstacles;
    [SerializeField]private Tilemap ground;
    [SerializeField] public Tilemap Obstacles { get => obstacles; private set => obstacles = value; }
    [SerializeField] public Tilemap Ground { get => ground; private set => ground = value; }

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(obstacles);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
