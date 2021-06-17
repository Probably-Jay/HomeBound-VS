using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitDetection;

public class WordNote : MonoBehaviour
{
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    public float absoluteBeatsToHit;
    float beatsOfExistence = 0;
    float spawningBeat = 0;
    [SerializeField] private float climaxBeat;
    HitZone myHitZone;
    [SerializeField] Lane lane;

    // Start is called before the first frame update
    void Start()
    {
        //code for testing, delete when initialising these from script
        spawningBeat = Rythm.RythmEngine.Instance.CurrentBeat;
        //end code for testing
        absoluteBeatsToHit = climaxBeat - Rythm.RythmEngine.Instance.CurrentBeat;
        lane.hitZone.Join(this);
        beatsOfExistence = 0f;
        this.transform.position = startPos.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        beatsOfExistence += Time.deltaTime;
       // Debug.Log(Rythm.RythmEngine.Instance.CurrentBeat);
        this.transform.position = Vector3.LerpUnclamped(startPos.position, endPos.position, (float) ((Rythm.RythmEngine.Instance.CurrentBeat-spawningBeat) / absoluteBeatsToHit));
    }
    public void initialise (float hitBeat, Lane passedLane, float? spawnBeat = null)
    {
        
        if (spawnBeat.HasValue)
        {
            spawningBeat = (float)spawnBeat.Value;
        }
        else
        {
            spawningBeat = Rythm.RythmEngine.Instance.CurrentBeat;
        }
        absoluteBeatsToHit = (float)hitBeat-spawningBeat;
        lane = passedLane;
    }
    public float TargetBeat()
    {
        return (float) climaxBeat;
    }
    public float GetClimaxBeat()
    {
        return climaxBeat;
    }
}
