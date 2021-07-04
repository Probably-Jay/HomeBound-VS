using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Obsolete("Replaced with" + nameof(FloorHandler))]
public class Floors : MonoBehaviour
{
    Dictionary<int, Tilemap> floors = new Dictionary<int, Tilemap> { };
    [SerializeField] List<int> indexes = new List<int> { };
    [SerializeField] List<Tilemap> tilemaps = new List<Tilemap> { };
    

    // Start is called before the first frame update
    void Awake()
    {
        floors.Clear();
        for(int i=0; i < indexes.Count; i++)
        {
            floors.Add(indexes[i], tilemaps[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Tilemap GetFloor(int index)
    {
        return floors[index];
    }
}
