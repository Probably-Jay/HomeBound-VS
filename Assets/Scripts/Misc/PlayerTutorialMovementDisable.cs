﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System;

public class PlayerTutorialMovementDisable : MonoBehaviour
{
    [SerializeField] bool On = true;
    private bool mutationComplete = false;

    private void OnEnable()
    {
        GameContextController.Instance.OnContextChange += Instance_OnContextChange;
    }
    private void OnDisable()
    {
        GameContextController.Instance.OnContextChange -= Instance_OnContextChange;
    }

    private void Instance_OnContextChange(Context currentContext, Context _)
    {
        switch (currentContext)
        {
            case Context.PreTutorial:
                if (mutationComplete)
                {
                    RevertContextMutation();
                    Destroy(this);
                }
                break;
        }
    }

  

    void Start()
    {
        if (!On) return;
        MutateContextToPreTutorial();
    }

    private void MutateContextToPreTutorial()
    {
        GameContextController.Instance.MutateContext(Context.PreTutorial);
        mutationComplete = true;
    }
    private void RevertContextMutation()
    {
        GameContextController.Instance.MutateContext(Context.Explore);
    }

}
