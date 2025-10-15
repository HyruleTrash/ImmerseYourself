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
    
	private bool isConnected = false;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    private DisplayChanger displayChanger;

    private void Start()
    {
        tcp = new TCP();
        displayChanger = gameObject.AddComponent<DisplayChanger>();
        ConnectToServer();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        
        isConnected = true;
        
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

        public void SendData(Packet packet)
        {
            try
            {
                if (socket == null)
                    return;
                stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending data to server via TCP: {e}");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int byteLength = stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    instance.Disconnect();
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
                Disconnect();
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

        private void Disconnect()
        {
            instance.Disconnect();
            
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.Welcome, ClientHandle.Welcome},
            {(int)ServerPackets.ServerFull, ClientHandle.ServerFull}
        };

        Debug.Log("Packet handlers have been registered.");
    }

    public void Disconnect()
    {
        if (!isConnected)
            return;
        isConnected = false;
        tcp.socket.Close();

        UIManager.instance.DisableGameUI();

        Debug.Log("Disconnected from server.");
    }
    
    /// <summary>
    /// Moves the window to a specific monitor and optionally maximizes it
    /// </summary>
    public void MoveWindowToMonitor(int monitorIndex)
    {
        displayChanger.MoveToMonitor(monitorIndex);
    }
}
