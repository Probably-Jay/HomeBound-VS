using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class MonoExtentions
{
    public static bool NullCheck<T>(this MonoBehaviour go, T obj) where T :  UnityEngine.Object
    {
        if(obj == null)
        {
            Debug.LogError($"Object is null in {go.name}", go);
            return false;
        }
        return true;
    }       
    
    public static bool NullCheck(this MonoBehaviour go, string str)
    {
        if(string.IsNullOrEmpty(str))
        {
            Debug.LogError($"String is null or empty in {go.name}", go);
            return false;
        }
        return true;
    }        
            

}
