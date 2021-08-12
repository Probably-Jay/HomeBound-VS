using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestLogic
{

    public class ReleighDistanceTeleporter : MonoBehaviour
    {
        [SerializeField] Raleigh raleigh;
        [SerializeField] Overworld.PlayerCharacterController player;
        [SerializeField,Range(10,100)] float teleportDistance;

        bool teleportEnabled = false;

        void Awake()
        {
            this.NullCheck(raleigh);
            this.NullCheck(player);
        }

        public void EnableTeleport() => teleportEnabled = true;

        private void Update()
        {
            TryTeleportRaleigh();
        }

        private void TryTeleportRaleigh()
        {
            if (!teleportEnabled)
                return;

            float dist = Vector2.Distance(raleigh.transform.position, player.transform.position);
            if (dist > teleportDistance)
            {
                TeleportRaleigh();
            }
        }

        private void TeleportRaleigh()
        {
            raleigh.gameObject.SetActive(false);
        }
    }
}
