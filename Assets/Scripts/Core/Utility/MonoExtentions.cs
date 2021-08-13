using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class MonoExtentions
{
    public static bool NullCheck<T>(this MonoBehaviour go, T obj, string name = null, Type type = null) where T :  UnityEngine.Object
    {
        if(obj == null || obj.Equals(null))
        {
            Debug.LogError($"{(string.IsNullOrEmpty(name) ? "Object " : $"{name} ")}{(type == null ? typeof(T).Name : $"of type {type.Name} ")}is null in {go.name}", go);
            return false;
        }
        return true;
    }       
    
    public static bool NullCheck(this MonoBehaviour go, string str, string name = "")
    {
        if(string.IsNullOrEmpty(str))
        {
            Debug.LogError($"String {name} is null or empty in {go.name}", go);
            return false;
        }
        return true;
    }        
           
    public static bool AssignComponent<T>(this MonoBehaviour go, out T cmp) where T : Component
    {     
        cmp = go.GetComponent<T>();
        if (go.NullCheck(cmp, name: "Component", type: typeof(T)))
        {
            return true;
        }
        return false;
    }    
    


}
