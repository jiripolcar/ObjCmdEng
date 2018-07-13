using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions  {

    public static ObjectIdentifier GetObjectIdentifier(this GameObject g)
    {
        return g.GetComponent<ObjectIdentifier>();
    }

}
