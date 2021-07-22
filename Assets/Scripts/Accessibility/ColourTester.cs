using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourTester : MonoBehaviour
{
    [SerializeField] bool on;
    SpriteRenderer sp;
    Color color = new Color(1, 0, 0);
    float hue;
    float s;
    float v;
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        Color.RGBToHSV(color, out hue, out s, out v);
    }

    // Update is called once per frame
    void Update()
    {
        hue = ( hue + 0.1f * Time.deltaTime) % 1 ;
        var col = Color.HSVToRGB(hue, s, v);
        if (on)
            sp.color = Accessibility.ColourBlindHelper.GetColour(col);
        else
            sp.color = col;
    }
}
