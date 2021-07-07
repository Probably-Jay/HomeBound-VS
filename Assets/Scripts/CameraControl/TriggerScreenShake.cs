using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CameraControl
{

    public class TriggerScreenShake : MonoBehaviour
    {

        [SerializeField ]CinemachineImpulseSource source;



        public void TriggerImpulse(float v = 1)
        {
            source.GenerateImpulse(v);
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TriggerImpulse();
            }
#endif
        }
    }
}