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

        [SerializeField] GameObject open;
        [SerializeField] GameObject closed;
        private Vector3Int centerTileIndex;
        private List<Vector3Int> tileLocations;

        private void Awake()
        {
            this.NotNullCheck(barrier);
            this.NotNullCheck(obsticalsMap);
            this.NotNullCheck(open);
            this.NotNullCheck(closed);
        }

        private void Start()
        {
            open.SetActive(false);
            closed.SetActive(false);
            GetTileLocations();
            Block();
        }

        private void Block()
        {
            open.SetActive(false);
            closed.SetActive(true);
            foreach (var tilePos in tileLocations)
            {
                var tile = Instantiate(barrier);
                obsticalsMap.SetTile(tilePos, tile);
            }
        }

        private void GetTileLocations()
        {
            centerTileIndex = obsticalsMap.WorldToCell(this.transform.position);
            tileLocations = new List<Vector3Int>();
            for (int i = -extremityHeight; i < extremityHeight + 1; i++)
            {
                tileLocations.Add(centerTileIndex + (i * Vector3Int.up));
            }
        }

        public void UnBlock()
        {
            open.SetActive(true);
            closed.SetActive(false);
            //foreach (var tilePos in tileLocations)
            //{
            //    obsticalsMap.SetTile(tilePos, null);
            //}
            obsticalsMap.SetTile(centerTileIndex, null);
            //Destroy(this.gameObject);
        }
    }
}