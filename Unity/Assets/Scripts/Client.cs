using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using ImmerseYourselfServer;
using NUnit.Framework;

public class Client : SingletonBehaviour<Client>
{
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 4377;
    public int id;
    public TCP tcp;
    
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }
    
    public class TCP
    {
        public TcpClient socket;
        
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            socket.EndConnect(ar);

            if (!socket.Connected)
            {
                return;
            }
            
            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int byteLength = stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    // TODO: disconnect
                    // stream.Close();
                    return;
                }
                
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving TCP data: {e}");
                // TODO: disconnect
                throw;
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;
            
            receivedData.SetBytes(data);
            
            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using var packet = new Packet(packetBytes);
                    var packedId = packet.ReadInt();
                    packetHandlers[packedId](packet);
                });
                
                packetLength = 0;
                
                if (receivedData.UnreadLength() < 4) continue;
                
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }
            
            return packetLength <= 1;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.Welcome, ClientHandle.Welcome}
        };

        Debug.Log("Packet handlers have been registered.");
    }
}
