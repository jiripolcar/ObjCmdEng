using System;
using NetworkUnetDemo.Infrastructure;
using UnityEngine;
using UnityEngine.Networking;

namespace NetworkUnetDemo
{
    public class NetworkClient : MonoBehaviour, INetworkEventHandler
    {
        [Tooltip("The connection manager to use")]
        [SerializeField] public NetworkHostManager HostManager;

       // [Tooltip("The remote server address to connect to")]
        public string RemoteHost { get { return Configuration.Data.defaultIP; } }

        //[Tooltip("The remote server port to bind connect to")]
        public int RemotePort { get { return Configuration.Data.networkPort; } }

        private readonly NetworkConnectionManager _connectionManager = new NetworkConnectionManager();


        private int _clientId = -1;

        private void OnEnable()
        {
            StartServer();
        }

        private void Start()
        {
            ConnectToRemoteHost();
        }

        void FixedUpdate()
        {
            if (_running)
            {
                _connectionManager.UpdateNonMono();
            }
        }

        private void OnDisable()
        {
            StopServer();
        }

        private bool _running;
        /// <summary>
        /// This is probably the most poorly understood part of the transport API.
        /// As a client, we must explicitly open a local server port to receive messages from the server from.
        /// In this case we pick max connections as 1 (server we connect to) and port 0 (arbitrary local port).
        /// </summary>
        void StartServer()
        {
            try
            {
                // Notice that the NeworkEventHandler has already called NetworkTransport.Init()
                var maxIncomingConnections = 1; // Only one server can ever connect to us.
                var hostTopology = new HostTopology(NetworkConfig.Config, maxIncomingConnections);
                _clientId = NetworkTransport.AddHost(hostTopology, 0);
                HostManager?.AddHost(_clientId, this);
                Debug.Log($"Client running on localhost");
            }
            catch (Exception error)
            {
                Debug.LogError(error);
                StopServer();
            }

            _running = true;
        }
        /// <summary>
        /// Stop this receiving port for this client.
        /// Close all connections targeting this port.
        /// </summary>
        void StopServer()
        {
            try
            {
                NetworkTransport.RemoveHost(_clientId);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }

            HostManager?.RemoveHost(_clientId);
            _connectionManager.Clear();
            _clientId = -1;
            _running = false;
            Debug.Log("Client halted.");
        }

        private void ConnectToRemoteHost()
        {
            try
            {
                byte errorCode;
                NetworkTransport.Connect(_clientId, RemoteHost, RemotePort, 0, out errorCode);
                var error = (NetworkError)errorCode;
                if (error != NetworkError.Ok)
                {
                    Debug.Log($"Connection failed: {error}");
                    StopServer();
                }
            }
            catch (Exception error)
            {
                Debug.Log(error);
                StopServer();
            }
        }





        public void OnConnectEvent(int connectionId, int channelId)
        {
            _connectionManager.Manage<NetworkClientConnection>(_clientId, connectionId, channelId);
        }

        public void OnDataEvent(int connectionId, int channelId, byte[] buffer, int dataSize)
        {
            _connectionManager.DataReceived(connectionId, channelId, buffer, dataSize);
        }

        public void OnDisconnectEvent(int connectionId, int channelId)
        {
            _connectionManager.Disconnect(connectionId, channelId);
        }
    }
}