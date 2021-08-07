using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameItems
{
    public class BridgeBarrier : MonoBehaviour
    {
        [SerializeField] Tile barrier;
        [SerializeField] Tilemap obsticalsMap;
        [SerializeField] int extremityHeight;
        private List<Vector3Int> tileLocations;

        private void Start()
        {
            GetTileLocations();
            Block();
        }

        private void Block()
        {
            foreach (var tilePos in tileLocations)
            {
                var tile = Instantiate(barrier);
                obsticalsMap.SetTile(tilePos, tile);
            }
        }

        private void GetTileLocations()
        {
            var centerTileIndex = obsticalsMap.WorldToCell(this.transform.position);
            tileLocations = new List<Vector3Int>();
            for (int i = -extremityHeight; i < extremityHeight; i++)
            {
                tileLocations.Add(centerTileIndex + (i * Vector3Int.up));
            }
        }

        public void UnBlock()
        {
            foreach (var tilePos in tileLocations)
            {
                obsticalsMap.SetTile(tilePos, null);
            }
            Destroy(this.gameObject);
        }
    }
}