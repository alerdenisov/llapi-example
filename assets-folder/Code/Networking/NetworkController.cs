using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Zenject;

namespace LlapiExample
{
    public class NetworkController : MonoBehaviour
    {
        public bool IsConnecting
        {
            get { return (state & ConnectionState.Connecting) == ConnectionState.Connecting; }
        }

        public bool IsConnected { get { return (state & ConnectionState.Connected) == ConnectionState.Connected; } }
        public bool IsServer { get { return (state & ConnectionState.Server) == ConnectionState.Server; } }
        public bool IsClient { get { return (state & ConnectionState.Client) == ConnectionState.Client; } }
        public bool InGame   { get { return (state & ConnectionState.InGame) == ConnectionState.InGame; } }

        public bool IsReady { get { return IsConnected && (IsServer || IsClient); } }
        public bool IsDefinedRole { get { return IsServer || IsClient; } }

        #region Dependencies
        [Inject] SelectRoleView Role;
        [Inject] ServerConfigView ServerConfig;
        [Inject] ClientConfigView ClientConfig;
        [Inject] DiContainer Container;
        [Inject] ConnectionStatus ConnectionStatus;
        [Inject] ConnectionsRepository Connections;
        [Inject] OutgoingCommandsQueue OutgoingCommands;
        [Inject] IncomingCommandsQueue IncomingCommands;
        #endregion

        string status;

        int unreliableChannel = -1;
        int realiableChannel = -1;
        int hostId = -1;

        string serverIp = "127.0.0.1";
        int outboundPort = 8080;
        int inboundPort = 8081;

        int connectionId = -1;

        List<INetworkLogic> logics;
        ConnectionState state { get { return ConnectionStatus.Current; } }

        private void Start()
        {
            logics = new List<INetworkLogic>();

            // Setup role ui
            Role.Hide();
            Role.OnClientSelected += OnClientRoleSelected;
            Role.OnServerSelected += OnServerRoleSelected;

            // setup server ui
            ServerConfig.Hide();
            ServerConfig.OnStart += OnServerStart;
            ServerConfig.OnPortChange += OnServerPortChange;

            // setup client ui
            ClientConfig.Hide();
            ClientConfig.OnStart += OnClientStart;
            ClientConfig.OnPortChange += OnClientPortChange;
            ClientConfig.OnAddressChange += OnClientAddressChange;

            ConnectionStatus.Set(ConnectionState.Disconnected);

            NetworkTransport.Init();
        }

        private void OnClientAddressChange(string ip)
        {
            if (!IsClient) return;
            serverIp = ip;
        }

        private void OnClientPortChange(int port)
        {
            if (!IsClient) return;
            outboundPort = port;
            inboundPort  = port + 1;
        }

        private void OnClientStart()
        {
            if (!IsClient) return;
            CreateHost();
            Connect();

            ConnectionStatus.Remove(ConnectionState.Connecting);
            ConnectionStatus.Append(ConnectionState.Connected);

            status = string.Format("Waiting for game start", outboundPort, inboundPort);
        }

        private void OnServerStart()
        {
            if (!IsServer) return;

            inboundPort = outboundPort;
            CreateHost();

            ConnectionStatus.Remove(ConnectionState.Disconnected);
            ConnectionStatus.Remove(ConnectionState.Connecting);
            ConnectionStatus.Append(ConnectionState.Connected);
        }

        private void OnServerPortChange(int port)
        {
            if (!IsServer) return;
            inboundPort = port;
        }

        private void OnClientRoleSelected()
        {
            ConnectionStatus.Remove(ConnectionState.Server);
            ConnectionStatus.Append(ConnectionState.Client);

            logics.Clear();
            logics = Container.ResolveIdAll<INetworkLogic>("client").Concat(Container.ResolveIdAll<INetworkLogic>("both")).ToList();

            foreach (var logic in logics)
            {
                logic.Setup();
            }
        }

        private void OnServerRoleSelected()
        {
            ConnectionStatus.Remove(ConnectionState.Client);
            ConnectionStatus.Append(ConnectionState.Server);

            logics.Clear();
            logics = Container.ResolveIdAll<INetworkLogic>("server").Concat(Container.ResolveIdAll<INetworkLogic>("both")).ToList();

            foreach (var logic in logics)
            {
                logic.Setup();
            }
        }

        private void Update()
        {
            if (!IsDefinedRole)
            {
                // Selected connection role
                Role.Show();
            }
            else
            {
                Role.Hide();
            }

            if (!IsConnected && !IsConnecting)
            {
                if (IsClient)
                    ClientConfig.Show();

                if (IsServer)
                    ServerConfig.Show();
            } else
            {
                ClientConfig.Hide();
                ServerConfig.Hide();
            }

            if(IsReady)
            {
                // listen
                ListenHost();
                SendHost();

                foreach (var logic in logics)
                {
                    logic.Update();
                }
            }
        }

        private void SendHost()
        {
            byte error;
            List<object> logicData = new List<object>();

            foreach (var command in OutgoingCommands)
            {
                logicData.Add(command.CommandId);
                logicData.AddRange(command.Data());
            }

            OutgoingCommands.Flush();
            
            if (logicData.Count == 0) return;

            int size;
            var buffer = UnnyNetPacker.PackObject(out size, logicData.ToArray());

            foreach (var connectionId in Connections)
            {
                NetworkTransport.Send(hostId, connectionId, realiableChannel, buffer, size, out error);
            }            
        }

        private void ListenHost()
        {
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            byte error;

            NetworkEventType recData = NetworkTransport.ReceiveFromHost(hostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            switch (recData)
            {
                case NetworkEventType.Nothing:         //1
                    break;
                case NetworkEventType.ConnectEvent:    //2
                    ConnectionEvent(connectionId);
                    break;
                case NetworkEventType.DataEvent:       //3
                    DataEvent(connectionId, channelId, dataSize, recBuffer);
                    break;
                case NetworkEventType.DisconnectEvent: //4
                    DisconnectEvent(connectionId);
                    break;
            }
        }

        private void DisconnectEvent(int connectionId)
        {
            Connections.Remove(connectionId);
        }

        private void DataEvent(int connectionId, int channelId, int dataSize, byte[] recBuffer)
        {
            // read data to commands
            var unpacked = UnnyNetPacker.UnpackObject(recBuffer, dataSize);

            for (int position = 0; position < unpacked.Length; position++)
            {
                var commandId = (byte)unpacked[position];

                var cmd = Container.ResolveId<ICommand>(commandId);
                cmd.Read(unpacked, ref position);
                cmd.Connection = connectionId;
                IncomingCommands.Enqueue(cmd);
            }
        }

        private void ConnectionEvent(int connectionId)
        {
            Connections.Add(connectionId);
        }

        private void CreateHost()
        {
            if (hostId >= 0)
            {
                NetworkTransport.RemoveHost(hostId);
            }

            var config = new ConnectionConfig();
            realiableChannel = config.AddChannel(QosType.Reliable);
            unreliableChannel = config.AddChannel(QosType.UnreliableSequenced);
            var topology = new HostTopology(config, 1);

            hostId = NetworkTransport.AddHost(topology, inboundPort, serverIp);

            ConnectionStatus.Append(ConnectionState.Connecting);
        }

        public void Connect()
        {
            byte error;
            connectionId = NetworkTransport.Connect(hostId, serverIp, outboundPort, 0, out error);
            status = error.ToString();
        }
        private void SendHello()
        {
            byte error;
            byte[] buffer = new byte[1024];
            Stream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, "HelloServer");

            int bufferSize = 1024;

            NetworkTransport.Send(hostId, connectionId, realiableChannel, buffer, bufferSize, out error);
        }
    }
}