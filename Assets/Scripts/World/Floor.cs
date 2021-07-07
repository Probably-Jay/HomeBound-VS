using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Floor : MonoBehaviour
{
    [SerializeField] Tilemap obstacles;
    [SerializeField] Tilemap ground;

    public Tilemap Obstacles { get => obstacles; }
    public Tilemap Ground { get => obstacles; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
