using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    [SerializeField] int maxStress = 6;
    [SerializeField] int stress;
    [SerializeField] private bool fullyStressed=false;
    [SerializeField] StressIndicator indicator;
    [SerializeField] bool debugNoIndicator;

    public bool FullyStressed { get => fullyStressed; private set => fullyStressed = value; }

    private void Awake()
    {
        if ((indicator == null)&&(!debugNoIndicator))
        {
            Debug.LogWarning("Stress indicator not assigned, perfoming automatic search.");
            indicator = GameObject.FindObjectOfType<StressIndicator>();
        }
        if (!debugNoIndicator) {
            indicator.UpdateIndication(stress);
                }

    }
    /// <summary>
    /// This funciton is foe werha add one there u go
    /// </summary>
    public void AlterStress(int v)
    {
        if (v > 0)
        {
            for (int i = 0; i < v; i++)
            {
                AddStress();
            }
        }
        else if (v < 0)
        {
            for (int i = 0; i < Mathf.Abs(v); i++)
            {
                RemoveStress();
            }
        }
    }

    public void AddStress()
    {
        if (stress < maxStress)
        {
            stress++;
            if (stress == maxStress)
            {
                fullyStressed = true;
                Game.GameContextController.Instance.SetStressed();
            }
            if (debugNoIndicator) { return; }
            indicator.UpdateIndication(stress);
        }

    }
    public void RemoveStress()
    {
        if (stress > 0)
        {
            stress--;
            if (fullyStressed && stress < maxStress)
            {
                fullyStressed = false;
                Game.GameContextController.Instance.SetUnstressed();
            }
            if (debugNoIndicator) { return; }
            indicator.UpdateIndication(stress);
        }
    }
}
