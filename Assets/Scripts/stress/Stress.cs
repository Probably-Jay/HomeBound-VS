using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    [SerializeField] int maxStress = 6;
    [SerializeField] int stress;
    [SerializeField] private bool fullyStressed=false;

    public bool FullyStressed { get => fullyStressed; private set => fullyStressed = value; }

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
        }
    }
}
