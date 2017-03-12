using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NetworkController : MonoBehaviour
{
    int unreliableChannel;
    int realiableChannel;
    int hostId;
    int serverPort = 8080;
    int clientPort = 9090;
    int connectionId;

    private void Start()
    {
        NetworkTransport.Init();
        var config = new ConnectionConfig();
        realiableChannel = config.AddChannel(QosType.Reliable);
        unreliableChannel = config.AddChannel(QosType.UnreliableSequenced);
        var topology = new HostTopology(config, 1);

        hostId = NetworkTransport.AddHost(topology, serverPort, "localhost");

        Debug.Log("Socket is open");

        Connect();
        SendHello();
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

    public void Connect()
    {
        byte error;
        connectionId = NetworkTransport.Connect(hostId, "localhost", clientPort, 0, out error);
        
        Debug.Log("Connected to server. ConnectionId: " + connectionId);
    }
}
