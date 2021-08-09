using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum stressPolarity
{
    Stress,
    Destress
}
public class StressPointRing : MonoBehaviour
{
    int stressPoints;
    [SerializeField] int maxStressPoints = 6;
    [SerializeField] int minStressPoints = -6;
    [SerializeField] StressPoint[] stressPointNodules = new StressPoint[6];
    [SerializeField] StressPoint[] destressPointNodules = new StressPoint[6];
    [SerializeField] Sprite[] sprites = new Sprite[2];
    [SerializeField] SpriteRenderer sR;
    [SerializeField] bool debugKeyPresses = false;
    stressPolarity flipDirection;
    bool isFlipping=false;
    bool hasSwapped = false;
    float flipTimer=0f;
    [SerializeField] float flipSpeed=100f;
    [SerializeField] Stress stressSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        ClearAllNodules(stressPointNodules);
        ClearAllNodules(destressPointNodules);
    }

    // Update is called once per frame
    void Update()
    {
        Animate();

        if (Input.GetKeyDown(KeyCode.I) && debugKeyPresses)
        {
            AddStressPoints(1);
        }
        if (Input.GetKeyDown(KeyCode.K) && debugKeyPresses)
        {
            RemoveStressPoints(1);
        }
    }

    private void Animate()
    {
        if (isFlipping)
        {
            if (!hasSwapped)
            {
                flipTimer += Time.deltaTime;
                if (flipTimer * flipSpeed > 1)
                {
                    flipTimer = 1 / flipSpeed;
                    if (flipDirection == stressPolarity.Stress)
                    {
                        sR.sprite = sprites[1];
                    }
                    else
                    {
                        sR.sprite = sprites[0];
                    }
                    hasSwapped = true;
                }
                this.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Rad2Deg * Mathf.Asin(flipTimer * flipSpeed), 0));
            }
            else
            {
                flipTimer -= Time.deltaTime;
                if (flipTimer < 0)
                {
                    flipTimer = 0f;
                    isFlipping = false;
                }
                float radianRotation = Mathf.Asin(flipTimer) * flipSpeed;
                float degreeRotation = Mathf.Rad2Deg * radianRotation;
                Quaternion temprotation = Quaternion.Euler(new Vector3(0, degreeRotation, 0));
                this.transform.rotation = temprotation;

            }
        }
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

    public void AddStressPoint() => AddStressPoints(1);

    public void AddStressPoints(int numberOfPoints)
    {
        if(numberOfPoints <0)
        {
            throw new System.Exception("AddStressPoints called with a negative integer, please use RemoveStressPoints");
        }

        int tempSP = stressPoints + numberOfPoints;
        if (tempSP > maxStressPoints)
        {
            tempSP = maxStressPoints;
        }
        if (tempSP > 0)
        {
            if (stressPoints < 0)
            {
                ClearAllNodules(destressPointNodules);
                FlipRing(stressPolarity.Stress);
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
                if ((stressPoints < 1)&& sR.sprite == sprites[0])
                {
                    FlipRing(stressPolarity.Stress);
                }
            }
            
        }
        else
        {
            for (int i = tempSP; i > stressPoints; i--)
            {
                destressPointNodules[Mathf.Abs(i)].Disappear();
            }
        }
        stressPoints = tempSP;
        if (stressPoints == maxStressPoints)
        {
            AddStress();
            RemoveStressPoints(6);

        }
    }

           
    public void RemoveStressPoint() => RemoveStressPoints(1);

    public void RemoveStressPoints(int numberOfPoints)
    {
      
        if (numberOfPoints < 0)
        {
            throw new System.Exception("RemoveStressPoints called with a negative integer, please use AddStressPoints");
        }

        int tempSP = stressPoints - numberOfPoints;
        
        if (tempSP < minStressPoints)
        {
            tempSP = minStressPoints;
        }
        if (tempSP < 0)
        {
            if (stressPoints>0)
            {
                ClearAllNodules(stressPointNodules);
                FlipRing(stressPolarity.Destress);
                for (int i = 0; i > tempSP; i--)
                {
                    destressPointNodules[Mathf.Abs(i)].Appear();
                }
            }
            else
            {
                for (int i = stressPoints; i > tempSP; i--)
                {
                    destressPointNodules[Mathf.Abs(i)].Appear();
                }
                if ((stressPoints > -1)&&sR.sprite==sprites[1])
                {
                    FlipRing(stressPolarity.Destress);
                }
            }
            
        }
        else
        {
            for (int i = tempSP; i < stressPoints; i++)
            {
                stressPointNodules[Mathf.Abs(i)].Disappear();
            }
        }
        stressPoints = tempSP;
        if (stressPoints == minStressPoints)
        {
            RemoveStress();
            AddStressPoints(6);
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
        stressSystem.AddStress();
    }
    void RemoveStress()
    {
        stressSystem.RemoveStress();
    }
    void FlipRing(stressPolarity direction)
    {
        if (!(isFlipping && flipDirection == direction))
        {
            flipTimer = 0f;
            flipDirection = direction;
            isFlipping = true;
            hasSwapped = false;
        }
    }

}