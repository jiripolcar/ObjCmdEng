using Demo;
using Demo.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    private bool shouldBeServer { get { return Configuration.Data.startAsServer; } }

    public GameObject client;
    public GameObject server;
    public NetworkHostManager hostManager;

    // Use this for initialization
    void Awake()
    {
        if (shouldBeServer)
        {
            server.SetActive(true);
        }
        else
            client.SetActive(true);
        
    }

}
