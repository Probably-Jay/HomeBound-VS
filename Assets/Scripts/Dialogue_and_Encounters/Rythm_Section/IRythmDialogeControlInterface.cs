using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRythmDialogeControlInterface 
{
    bool InRythmSection { get; }
    bool RythmHasControl { get; }
    void StartNewRythm(string id);
    void PassControlToDialogue(float? onBeat);
    void PassControlToRythm();
    void EndRythmSection();

}
