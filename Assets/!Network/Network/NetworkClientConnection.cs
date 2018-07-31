using System;
using System.Text;
using Demo.Infrastructure;
using UnityEngine;
using UnityEngine.Networking;

namespace Demo
{
    /// <summary>
    /// Simple network client; 'Ping' server every 2 seconds
    /// </summary>
    public class NetworkClientConnection : INetworkProtocol
    {
        private string _id;

        private float _elapsed;

        private NetworkMessageReader _message;

        public NetworkChannel Channel { get; set; }

        public void OnConnected()
        {
            _id = Guid.NewGuid().ToString();
            Debug.Log($"Client {_id}: Connected");
        }

        public void OnDisconnected()
        {
            Debug.Log($"Client {_id}: Disconnected");
        }

        public void Update()
        {
        }
        
        public void OnDataReceived(byte[] buffer, int dataSize)
        {
            if (_message == null)
            {
                _message = new NetworkMessageReader();
            }

            if (_message.Read(buffer, dataSize))
            {
                var bytes = _message.Payload();
                var output = Encoding.UTF8.GetString(bytes);
                Debug.Log(output);
                NetworkCommander.ReceiveSyncedCommand(output);
                _message = null;
            }
        }
    }
}