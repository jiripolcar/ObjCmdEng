using UnityEngine;
using UnityEngine.Networking;

public class Serializer : MonoBehaviour
{
    NetworkServerSimple m_Server;
    NetworkClient m_Client;
    const short k_MyMessage = 100;

    // When using a server instance like this it must be pumped manually

    public bool server = false;

    private void Start()
    {
        if (addr.Length < 1)
            addr = address;
        if (server)
            StartServer();

        StartClient();
    }

    void Update()
    {
        if (sendm)
            SendMessage();
        sendm = false;
        if (m_Server != null)
            m_Server.Update();
    }

    public int socketPort { get { return Configuration.Data.networkPort; } set { Configuration.Data.networkPort = value; } }
    public string address { get { return Configuration.Data.defaultIP; } set { Configuration.Data.defaultIP = value; } }
    public string addr;

    void StartServer()
    {
        m_Server = new NetworkServerSimple();
        m_Server.RegisterHandler(k_MyMessage, OnMyMessage);
        if (m_Server.Listen(socketPort))
            Debug.Log("Started listening on port");
    }

    void StartClient()
    {
        m_Client = new NetworkClient();
        m_Client.RegisterHandler(MsgType.Connect, OnClientConnected);
        m_Client.Connect("127.0.0.1", socketPort);
        Debug.Log("klient");
    }

    void OnClientConnected(NetworkMessage netmsg)
    {
        Debug.Log("Client connected to server");
        SendMessage();
    }

    void SendMessage()
    {
        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(k_MyMessage); writer.Write(42);
        writer.Write(send);
        writer.FinishMessage();
        m_Client.SendWriter(writer, 0);
    }

    public string send;
    public bool sendm;
    public string receive;

    void OnMyMessage(NetworkMessage netmsg)
    {
        Debug.Log("Got message, size =" + netmsg.reader.Length);
        var someValue = netmsg.reader.ReadInt32();
        var someString = netmsg.reader.ReadString();
        receive = "Message value =" + someValue + " Message string =" + someString;
    }
}