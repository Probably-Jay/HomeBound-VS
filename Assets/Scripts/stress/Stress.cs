using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    [SerializeField] int maxStress = 6;
    [SerializeField] int stress;
    [SerializeField] private bool fullyStressed=false;

    public bool FullyStressed { get => fullyStressed; private set => fullyStressed = value; }


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
            if (stress < maxStress)
            {
                fullyStressed = false;
                Game.GameContextController.Instance.SetUnstressed();
            }
        }
    }
}
