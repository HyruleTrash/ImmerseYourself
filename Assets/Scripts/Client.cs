using System;
using UnityEngine;
using System.Net.Sockets;
using NUnit.Framework;

public class Client : SingletonBehaviour<Client>
{
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 4377;
    public int id;
    public TCP tcp;

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        tcp.Connect();
    }
    
    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private byte[] recieveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            
            recieveBuffer = new byte[dataBufferSize];
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
            stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int _byteLength = stream.EndRead(ar);
                if (_byteLength <= 0)
                {
                    // TODO: disconnect
                    // stream.Close();
                    return;
                }
                
                byte[] _data = new byte[_byteLength];
                Array.Copy(recieveBuffer, _data, _byteLength);
                
                // TODO: handle data
                stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving TCP data: {e}");
                // TODO: disconnect
                throw;
            }
        }
    }
}
