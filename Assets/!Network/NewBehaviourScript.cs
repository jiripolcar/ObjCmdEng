using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    public int socketPort { get { return Configuration.Data.networkPort; } set { Configuration.Data.networkPort = value; } }
    public string address { get { return Configuration.Data.defaultIP; } set { Configuration.Data.defaultIP = value; } }
    public string localHost = "127.0.0.1";

    public Socket listenSocket;
    public IPEndPoint localEndpoint;
    public IPEndPoint remoteEndpoint;

    public string received;

    // Use this for initialization
    void Start () {
        listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        localEndpoint = new IPEndPoint(IPAddress.Parse(localHost), socketPort);
        listenSocket.Bind(localEndpoint);
        
	}

    void Listening(Socket socket)
    {
        listenSocket.Listen(1);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
