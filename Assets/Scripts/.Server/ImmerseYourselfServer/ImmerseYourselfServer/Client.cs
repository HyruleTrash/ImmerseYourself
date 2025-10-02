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

        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;
            
            stream  = socket.GetStream();
            
            recieveBuffer = new byte[dataBufferSize];
            
            stream.BeginRead(recieveBuffer,0, dataBufferSize, ReceiveCallback, null);
            
            // TODO: Send welcome packet
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