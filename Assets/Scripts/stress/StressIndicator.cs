using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressIndicator : MonoBehaviour
{
    [SerializeField] Image[] segments= new Image[6];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateIndication(int stress)
    {
        //this.GetComponent<Text>().text=(stress.ToString() + "/6");
        ClearMeter();
        for(int i=0; i < segments.Length; i++)
        {
            if (i < stress)
            {
                segments[i].color=AdjustedTransparency(segments[i].color, 1f);
            }
            else
            {
                segments[i].color = AdjustedTransparency(segments[i].color, 0f);
            }
        }

    }
    private void ClearMeter()
    {
        foreach(Image segment in segments)
        {
            segment.color = AdjustedTransparency(segment.color, 0f);
        }
    }
    private Color AdjustedTransparency(Color color,float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
