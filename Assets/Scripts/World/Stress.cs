using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    [SerializeField] int maxStress = 6;
    [SerializeField] int stress;
    [SerializeField] private bool fullyStressed=false;

    public bool FullyStressed { get => fullyStressed; private set => fullyStressed = value; }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddStress()
    {
        if (stress < maxStress)
        {
            stress++;
            if (stress == maxStress)
            {
                fullyStressed = true;
                Game.GameContextController.Instance.FullyStressed = true;
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
                Game.GameContextController.Instance.FullyStressed = false;
            }
        }
    }
}
