using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public class CowControl : MonoBehaviour
    {
        Animator animator;

        private void Awake()
        {
            this.AssignComponent(out animator);
            animator.SetFloat("Offset", UnityEngine.Random.Range(0.0f, 1.0f));
        }


      
    }
}