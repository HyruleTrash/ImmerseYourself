using System.Net.Sockets;

namespace ImmerseYourselfServer;

public class Client
{
    public static int dataBufferSize = 4096;
    public int id;
    public TCP tcp;

    public Client(int _client_id)
    {
        id = _client_id;
        tcp = new TCP(id);
    }
    
    public class TCP
    {
        public TcpClient socket;
        
        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient socket)
        {
            this.socket = socket;
            this.socket.ReceiveBufferSize = dataBufferSize;
            this.socket.SendBufferSize = dataBufferSize;
            
            stream  = this.socket.GetStream();
            
            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];
            
            stream.BeginRead(receiveBuffer,0, dataBufferSize, ReceiveCallback, null);
            
            ServerSend.Welcome(id, "You have joined the server!");
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
                Console.WriteLine($"Error sending data to player {id}: {e.Message}");
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
                    Server.packetHandlers[packedId](id, packet);
                });
                
                packetLength = 0;
                
                if (receivedData.UnreadLength() < 4) continue;
                
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }
            
            return packetLength <= 1;
        }

        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int byteLength = stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
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
                Server.clients[id].Disconnect();
                throw;
            }
        }
    }

    private void Disconnect()
    {
        Console.WriteLine($"Client {tcp.socket.Client.RemoteEndPoint} has been disconnected");
        
        // TODO: set player data to null
        
        tcp.Disconnect();
    }
}