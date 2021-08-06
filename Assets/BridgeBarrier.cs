using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameItems
{
    public class BridgeBarrier : MonoBehaviour
    {
        public void UnBlock()
        {
            Destroy(this.gameObject);
        }
    }
}