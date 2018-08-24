using System;
using System.Text;
using NetworkUnetDemo.Infrastructure;
using UnityEngine;
using UnityEngine.Networking;

namespace NetworkUnetDemo
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

        public void UpdateNetworkProtocol()
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
                //NetworkCommander.ReceiveSyncedCommand(output);
                NetworkCommander.ReceiveSyncMessages(output);
                _message = null;
            }
        }

        public void SendToServer(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            Channel.Send(bytes, bytes.Length);
        }
    }
}