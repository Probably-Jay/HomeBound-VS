using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PointOfIntrestManager : MonoBehaviour
{
    [SerializeField,Range(0,1)] float playerWeight = 0.98f;
   // [SerializeField, Range(0, 1)] float otherWeights = 0.01f;
    [SerializeField] Transform player;
    [SerializeField] List<Transform> pointsOfIntrest;
    [SerializeField, Range(0, 10)] float distance;

   

    CinemachineTargetGroup group;

    private void Awake()
    {
        group = GetComponent<CinemachineTargetGroup>();
    }
    private void Start()
    {
        group.AddMember(player, playerWeight, 1);
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var item in pointsOfIntrest)
        {
            float magnitude = (item.position - player.position).magnitude;
            if (magnitude < distance)
            {
                int i = group.FindMember(item);
                float weight = (1-(magnitude / distance));
                if (i == -1)
                {
                    AddMember(item, weight);
                }
                else
                {
                    group.m_Targets[i].weight = weight;

                }
                
                
                
            }
            else
            {
                if (group.FindMember(item) != -1)
                {
                    RemoveMember(item);
                }
            }
        }
    }

    private void RemoveMember(Transform item)
    {
        Debug.Log("Removed");
        group.RemoveMember(item);
    }

    private void AddMember(Transform item, float weight)
    {
        Debug.Log("Added");

        group.AddMember(item, weight, 1);
    }
}
