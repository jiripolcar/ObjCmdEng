﻿using System;
using System.Collections.Generic;
using CommanderEngine.Network;
using NetworkUnetDemo.Infrastructure;
using UnityEngine;
using UnityEngine.Networking;

namespace NetworkUnetDemo
{
    public class NetworkServer : MonoBehaviour, INetworkEventHandler
    {
        [Tooltip("The connection manager to use")]
        public NetworkHostManager HostManager; // TODO do Configu

        //[Tooltip("The local server port to bind to")]
        public int Port { get { return Configuration.Data.networkPort; } }

        private readonly NetworkConnectionManager _connectionManager = new NetworkConnectionManager();

        private bool _running;

        private int _serverId = -1;

        private void OnEnable()
        {
            StartServer();
            NetworkCommander.NetworkServer = this;
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

        void StartServer()
        {
            try
            {
                // Notice that the NeworkEventHandler has already called NetworkTransport.Init()
                var maxIncomingConnections = 10;
                var hostTopology = new HostTopology(NetworkConfig.Config, maxIncomingConnections);
                _serverId = NetworkTransport.AddHost(hostTopology, Port);
                HostManager?.AddHost(_serverId, this);
                Debug.Log($"Server running localhost:{Port}");
            }
            catch (Exception error)
            {
                Debug.LogError(error);
                StopServer();
            }

            _running = true;
        }

        void StopServer()
        {
            try
            {
                NetworkTransport.RemoveHost(_serverId);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }

            HostManager?.RemoveHost(_serverId);
            _connectionManager.Clear();
            _serverId = -1;
            _running = false;
            Debug.Log("Server halted.");
        }



        public void OnConnectEvent(int connectionId, int channelId)
        {
            Debug.LogWarning("OnConnectEvent");
            //  networkServerConnections.Add(_connectionManager.Manage<NetworkServerConnection>(_serverId, connectionId, channelId));
            networkServerConnection = _connectionManager.Manage<NetworkServerConnection>(_serverId, connectionId, channelId);
        }

        public void OnDataEvent(int connectionId, int channelId, byte[] buffer, int dataSize)
        {
            _connectionManager.DataReceived(connectionId, channelId, buffer, dataSize);
        }

        public void OnDisconnectEvent(int connectionId, int channelId)
        {
            _connectionManager.Disconnect(connectionId, channelId);
        }

        /*public List<NetworkServerConnection> networkServerConnections=new List<NetworkServerConnection>();

        public bool SendData(string data)
        {
            if (networkServerConnections == null || networkServerConnections.Count == 0)
                return false;
            networkServerConnections.ForEach((nsc) => nsc.BroadcastToClients(data));
            return true;
        }*/


        NetworkServerConnection networkServerConnection;
        public bool SendData(string data)
        {
            if (networkServerConnection == null)
                return false;
            networkServerConnection.BroadcastToClients(data);
            return true;
        }


    }
}