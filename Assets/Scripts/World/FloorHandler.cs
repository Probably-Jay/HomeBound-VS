using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorHandler : MonoBehaviour
{
    Dictionary<int, Floor> indexedFloors = new Dictionary<int, Floor> { };
    [SerializeField] List<int> indexes = new List<int> { };
    [SerializeField] List<Floor> floors = new List<Floor> { };
    [SerializeField] StairCollection stairs;
    //[SerializeField] List<TileBase> UpStairs = new List<TileBase> { };
    //[SerializeField] List<TileBase> RightStairs = new List<TileBase> { };
    //[SerializeField] List<TileBase> DownStairs = new List<TileBase> { };
    //[SerializeField] List<TileBase> LeftStairs = new List<TileBase> { };
    //List<TileBase>[] stairs = new List<TileBase>[4];


    // Start is called before the first frame update
    void Awake()
    {
        indexedFloors.Clear();
        for (int i = 0; i < indexes.Count; i++)
        {
            indexedFloors.Add(indexes[i], floors[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Floor GetFloor(int index)
    {
        return indexedFloors[index];
    }
    public TileBase GetObsTileOnFloor(int floorIndex, Vector3Int tilePosition)
    {
        return (indexedFloors[floorIndex].Obstacles.GetTile(tilePosition));
    }
    public TileBase GetGroundTileOnFloor(int floorIndex, Vector3Int tilePosition)
    {
        return (indexedFloors[floorIndex].Ground.GetTile(tilePosition));
    }
    public Overworld.WalkingDirection StairDirection(TileBase stairTile)
    {
        return stairs.StairDirection(stairTile);
    }
}
