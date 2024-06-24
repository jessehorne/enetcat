using ENetcat;

namespace Commands
{
    class CommandConnect
    {
        public static int Connect(string host, string port)
        {
            TCPClient client = new TCPClient(host, port);
            return client.Connect();
        }
    }
}