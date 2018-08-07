using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{

    public static ObjectIdentifier GetObjectIdentifier(this GameObject g)
    {
        return g.GetComponent<ObjectIdentifier>();
    }

    public static GameObject FindChildWithName(this GameObject g, string search)
    {
        Transform t = g.transform;

        foreach (Transform child in t)
        {
            if (child.name == search)
                return child.gameObject;
        }

        foreach (Transform child in t)
        {
            GameObject result = child.gameObject.FindChildWithName(search);
            if (result != null)
                return result;
        }

        return null;
        
    }


}
