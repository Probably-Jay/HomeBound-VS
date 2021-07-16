using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorHandler : MonoBehaviour
{
    Dictionary<int, Floor> floors = new Dictionary<int, Floor> { };
    [SerializeField] List<int> indexes = new List<int> { };
    [SerializeField] List<Floor> tilemaps = new List<Floor> { };


    // Start is called before the first frame update
    void Awake()
    {
        floors.Clear();
        for (int i = 0; i < indexes.Count; i++)
        {
            floors.Add(indexes[i], tilemaps[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Floor GetFloor(int index)
    {
        return floors[index];
    }
    public TileBase GetGroundTileOnFloor(int floorIndex, Vector3Int tilePosition)
    {
        return (floors[floorIndex].Ground.GetTile(tilePosition));
    }
    public TileBase GetObsTileOnFloor(int floorIndex, Vector3Int tilePosition)
    {
        return (floors[floorIndex].Obstacles.GetTile(tilePosition));
    }
}
