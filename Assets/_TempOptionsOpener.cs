using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary
{
    public class _TempOptionsOpener : MonoBehaviour
    {
        [SerializeField] GameObject optionsParent;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                OpenOrClose();
            }
            if ((Input.GetKeyDown(KeyCode.Escape)))
            {
                Close();
            }
        }

        private void Close()
        {
            optionsParent.SetActive(false);
        }
        private void Open()
        {
            optionsParent.SetActive(true);
        }



        private void OpenOrClose()
        {
            if (optionsParent.activeInHierarchy)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

    }
}