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
        private byte[] recieveBuffer;

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
            
            recieveBuffer = new byte[dataBufferSize];
            
            stream.BeginRead(recieveBuffer,0, dataBufferSize, ReceiveCallback, null);
            
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
                Array.Copy(recieveBuffer, data, byteLength);
                
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