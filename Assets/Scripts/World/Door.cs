using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld
{
    public class Door : MonoBehaviour
    {
        [SerializeField] Door partnerDoor;
        [SerializeField] WalkingDirection entryDirection;
        [SerializeField] Grid grid;
        [SerializeField] bool stopsMovement;
        //[SerializeField] Floo
        // Start is called before the first frame update
        void Start()
        {
            grid = FindObjectOfType<Grid>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Something entered the door");
            if (other.tag == "Player")
            {
                PlayerCharacterController controller = other.gameObject.GetComponent<PlayerCharacterController>();
                if (controller.FacingDirection == DirectionTools.OppositeDirection(entryDirection))
                {
                    partnerDoor.BringPlayerThruDoor(controller);
                    if (stopsMovement)
                    {
                        controller.RequireNewButtonPressToMove();
                    }
                }
            }
        }
        public void BringPlayerThruDoor(PlayerCharacterController player)
        {
            Vector3 ejectionSpace = grid.GetCellCenterWorld(grid.WorldToCell(this.transform.position) + Vector3Int.FloorToInt(DirectionTools.DirectionToVector(entryDirection)));
            //Vector2 opsDirVec = DirectionTools.DirectionToVector(DirectionTools.OppositeDirection(entryDirection));
            //ejectionSpace += new Vector3(opsDirVec.x * 0.75f, opsDirVec.y * 0.75f, 0f);
            player.gameObject.transform.position = new Vector3(ejectionSpace.x, ejectionSpace.y, player.gameObject.transform.position.z);
        }
    }
}
