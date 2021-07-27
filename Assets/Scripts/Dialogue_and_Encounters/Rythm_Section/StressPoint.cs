using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressPoint : MonoBehaviour
{
    [SerializeField] SpriteRenderer mySR;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Appear()
    {
        mySR.color = new Color(mySR.color.r, mySR.color.g, mySR.color.b, 1);
    }
    public void Disappear()
    {
        mySR.color = new Color(mySR.color.r, mySR.color.g, mySR.color.b, 0);
    }
}
