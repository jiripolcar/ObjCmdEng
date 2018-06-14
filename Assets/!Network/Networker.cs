using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Networker : MonoBehaviour {

    public ConnectionConfig config;
    public int reliableChannelId;
    public int unreliableChannelId;
    public HostTopology topology;
    public int socketId;
    public int socketPort = 8888;
    public string address = "localhost";
    public int connectionId;


    public InputField input;
    public Text statusText;
    public Text console;
    public InputField ip;

    // Use this for initialization
    void Start () {
        NetworkTransport.Init();
        config = new ConnectionConfig();
        reliableChannelId = config.AddChannel(QosType.Reliable);
        unreliableChannelId = config.AddChannel(QosType.Unreliable);
        topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, socketPort);
        Log("Socket Open. SocketId is: " + socketId);
    }

    public void Connect(string address)
    {
        byte error;
        connectionId = NetworkTransport.Connect(socketId, address, socketPort, 0, out error);
        Log("Connected to server. ConnectionId: " + connectionId);
    }


    public void SendSocketMessage(string message)
    {
        byte error;
        byte[] buffer = new byte[1024];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);

        int bufferSize = 1024;

        NetworkTransport.Send(socketId, connectionId, reliableChannelId, buffer, bufferSize, out error);
        Log( "Message sent. " + error);
    }

    private void Update()
    {
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                statusText.text = "nothing";
                break;
            case NetworkEventType.ConnectEvent:
                statusText.text = "incoming connection event received";
                Log("incoming connection event received");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                Log("incoming message event received: " + message);
                input.text = message;
                break;
            case NetworkEventType.DisconnectEvent:
                statusText.text = "remote client event disconnected";
                Log("remote client event disconnected");
                break;
        }
    }

    public void UI_Send() { SendSocketMessage(input.text); }
    public void UI_Connect() { Connect(ip.text); }

    public void Log(string log) { console.text += "\n" + log; }

}
