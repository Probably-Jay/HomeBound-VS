using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class MonoExtentions
{
    public static bool AssignComponent<T>(this MonoBehaviour go, out T cmp) where T : Component
    {     
        cmp = go.GetComponent<T>();
        if (go.NotNullCheck(cmp))
        {
            return true;
        }
        return false;
    }        
    
    public static bool AssignComponentInChildren<T>(this MonoBehaviour go, out T cmp) where T : Component
    {     
        cmp = go.GetComponentInChildren<T>();
        if (go.NotNullCheck(cmp))
        {
            return true;
        }
        return false;
    }    

    public static bool NotNullCheck<T>(this MonoBehaviour go, T obj) where T :  UnityEngine.Object
    {
        if(obj == null || obj.Equals(null))
        {
            Type type = typeof(T);
            Debug.LogError($"{type.BaseType} {type.Name} is null in {go.name}", go);
            return false;
        }
        return true;
    }    

    public static bool NotNullCheck(this MonoBehaviour go, string str)
    {
        if(string.IsNullOrEmpty(str))
        {
            Debug.LogError($"String is null or empty in {go.name}", go);
            return false;
        }
        return true;
    }        
}
