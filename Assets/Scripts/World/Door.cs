using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour
    {
        [SerializeField] Door partnerDoor;
        [SerializeField] WalkingDirection entryDirection;
        [SerializeField] Grid grid;
        [SerializeField] bool stopsMovement;
        [SerializeField] bool locked;

        BoxCollider2D col;

        public bool Locked => locked;

        public void SetLocked(bool value)
        {
            locked = value;
            UpdateLock();
        }

        private void UpdateLock()
        {
            col.isTrigger = !locked;
        }

        //[SerializeField] Floo
        // Start is called before the first frame update
        void Start()
        {
            grid = FindObjectOfType<Grid>();
            if(partnerDoor == null || partnerDoor == this)
            {
                Debug.LogError($"Door {name} has no partner door");
            }
            col = GetComponent<BoxCollider2D>();
            UpdateLock();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
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


            var beforePosition = player.transform.position;

            player.transform.position = new Vector3(ejectionSpace.x, ejectionSpace.y, player.transform.position.z);

            var posDelta = player.transform.position - beforePosition;
            Cinemachine.CinemachineCore.Instance.GetVirtualCamera(0).OnTargetObjectWarped(player.transform, posDelta);
        }
    }
}
