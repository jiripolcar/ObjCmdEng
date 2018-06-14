using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIdentifier : MonoBehaviour
{
    public static Dictionary<string, ObjectIdentifier> objectIdentifiers;

    [Tooltip("If null or zero length, will use gameObject.name")]
    [SerializeField] private string identifyAs = "";

    // Use this for initialization
    void Start()
    {
        Register();
    }

    private void Register()
    {
        string regString = identifyAs.Length > 0 ? identifyAs : name;
        if (objectIdentifiers == null)
            objectIdentifiers = new Dictionary<string, ObjectIdentifier>();
        if (objectIdentifiers.ContainsKey(regString))
            Debug.LogError("Regstring already exists! " + regString);
        objectIdentifiers.Add(regString, this);
    }

    public static ObjectIdentifier Find(string key)
    {
        ObjectIdentifier oi;
        if (objectIdentifiers.TryGetValue(key, out oi))
            return oi;
        else
            return null;
    }

}
