using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class StairCollection : MonoBehaviour
{
    [SerializeField] List<TileBase> UpStairs = new List<TileBase> { };
    [SerializeField] List<TileBase> RightStairs = new List<TileBase> { };
    [SerializeField] List<TileBase> DownStairs = new List<TileBase> { };
    [SerializeField] List<TileBase> LeftStairs = new List<TileBase> { };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public Overworld.WalkingDirection StairDirection (TileBase tile){
        if (UpStairs.Contains(tile))
        {
            return Overworld.WalkingDirection.Up;
        }
        else if (DownStairs.Contains(tile))
        {
            return Overworld.WalkingDirection.Down;
        }
        else if (RightStairs.Contains(tile))
        {
            return Overworld.WalkingDirection.Right;
        }
        else if (LeftStairs.Contains(tile))
        {
            return Overworld.WalkingDirection.Left;
        }
        else
        {
            Debug.LogError("Tile is not contained in stair list");
            return Overworld.WalkingDirection.Down;
        }
    }
}
