using System;
using System.Text;
using NetworkUnetDemo.Infrastructure;
using UnityEngine;

namespace NetworkUnetDemo
{
    /// <summary>
    /// Simple network server; respond to 'Ping' with 'Pong'.
    /// </summary>
    public class NetworkServerConnection : INetworkProtocol
    {
        private NetworkMessageReader _message;
        private string _id;

        public void OnConnected()
        {
            _id = Guid.NewGuid().ToString();
            Debug.Log($"Server: Client {_id} connected");
        }

        public void OnDisconnected()
        {
            Debug.Log($"Server: Client {_id} disconnected");
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
                Debug.Log($"Server: Received: {output}");
                if (output.Contains("PING"))
                {
                    BroadcastToClients("PING");
                }

                _message = null;
            }
        }



        public void BroadcastToClients(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            Channel.Send(bytes, bytes.Length);
        }

        public NetworkChannel Channel { get; set; }
    }
}