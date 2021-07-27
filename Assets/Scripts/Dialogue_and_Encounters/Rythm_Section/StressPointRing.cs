using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressPointRing : MonoBehaviour
{
    int stressPoints;
    [SerializeField] int maxStressPoints = 6;
    [SerializeField] int minStressPoints = -6;
    [SerializeField] StressPoint[] stressPointNodules = new StressPoint[6];
    [SerializeField] StressPoint[] destressPointNodules = new StressPoint[6];
    
    // Start is called before the first frame update
    void Start()
    {
        ClearAllNodules(stressPointNodules);
        ClearAllNodules(destressPointNodules);
        AddStressPoints(1);
        Debug.Log(stressPoints);
        AddStressPoints(1);
        Debug.Log(stressPoints);
        AddStressPoints(3);
        Debug.Log(stressPoints);
        AddStressPoints(3);
        Debug.Log("SP:"+stressPoints);
        RemoveStressPoints(4);
        RemoveStressPoints(4);
        Debug.Log(stressPoints);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeStressPoints(int numberOfPoints)
    {
        if (numberOfPoints > 0)
        {
            AddStressPoints(numberOfPoints);
        }
        else if(numberOfPoints < 0)
        {
            RemoveStressPoints(-1 * numberOfPoints);
        }
        else
        {
            throw new System.Exception("Change stress points called with 0, please specify a negative or positive numberOfPoints to change by");
        }
    }
    public void AddStressPoints(int? numberOfPoints)
    {
        if (numberOfPoints == null)
        {
            AddStressPoints(1);
            return;
        }
        if(numberOfPoints <0)
        {
            throw new System.Exception("AddStressPoints called with a negative integer, please use RemoveStressPoints");
        }
        int tempSP = stressPoints + (int)numberOfPoints;
        if (tempSP > maxStressPoints)
        {
            tempSP = maxStressPoints;
        }
        if (tempSP > 0)
        {
            if (stressPoints < 0)
            {
                ClearAllNodules(destressPointNodules);
                FlipRing();
                for (int i = 0; i < tempSP; i++)
                {
                    stressPointNodules[i].Appear();
                }
            }
            else
            {
                for (int i = stressPoints; i < tempSP; i++)
                {
                    stressPointNodules[i].Appear();
                }
            }
            
        }
        else
        {
            for (int i = tempSP; i > stressPoints; i--)
            {
                destressPointNodules[i].Disappear();
            }
        }
        stressPoints = tempSP;
        if (stressPoints == maxStressPoints)
        {
            AddStress();
        }
    }
    public void RemoveStressPoints(int? numberOfPoints)
    {
        if (numberOfPoints == null)
        {
            RemoveStressPoints(1);
            return;
        }
        if (numberOfPoints < 0)
        {
            throw new System.Exception("RemoveStressPoints called with a negative integer, please use AddStressPoints");
        }
        int tempSP = stressPoints - (int)numberOfPoints;
        
        if (tempSP < minStressPoints)
        {
            tempSP = minStressPoints;
        }
        if (tempSP < 0)
        {
            if (stressPoints>0)
            {
                ClearAllNodules(stressPointNodules);
                FlipRing();
                for (int i = 0; i > tempSP; i--)
                {
                    destressPointNodules[Mathf.Abs(i)].Appear();
                }
            }
            else
            {
                for (int i = stressPoints; i > tempSP; i--)
                {
                    destressPointNodules[i].Appear();
                }
            }
            
        }
        else
        {
            for (int i = tempSP; i < stressPoints; i++)
            {
                destressPointNodules[i].Disappear();
            }
        }
        stressPoints = tempSP;
        if (stressPoints == minStressPoints)
        {
            RemoveStress();
        }
    }
    void ClearAllNodules(StressPoint[] ring)
    {
        
        foreach(StressPoint sPN in ring)
        {
            sPN.Disappear();
        }
    }
    void AddStress()
    {

    }
    void RemoveStress()
    {

    }
    void FlipRing()
    {

    }
}