using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary
{
    public class _TempOpener : MonoBehaviour
    {
        [SerializeField] KeyCode openKey = KeyCode.O;
        [SerializeField] GameObject parent;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(openKey))
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
            parent.SetActive(false);
        }
        private void Open()
        {
            parent.SetActive(true);
        }



        private void OpenOrClose()
        {
            if (parent.activeInHierarchy)
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