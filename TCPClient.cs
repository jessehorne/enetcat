using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

namespace ENetcat
{
    class TCPClient
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }
        public RSACryptoServiceProvider ServerKey { get; set; }

        public TCPClient(string host, string port)
        {
            int intPort = Util.Port.Parse(port);

            this.Host = host;
            this.Port = intPort;

            this.Writer = StreamWriter.Null;
            this.Reader = StreamReader.Null;

            this.ServerKey = new RSACryptoServiceProvider();
        }

        public int Connect()
        {
            TcpClient client = new TcpClient(this.Host, this.Port);
            NetworkStream stream = client.GetStream();
            this.Reader = new StreamReader(stream);
            this.Writer = new StreamWriter(stream) { AutoFlush = true };

            // Part 1: Wait for server public key (base64 encoded public key csp blob)
            this.ServerKey = this.ReceiveServerPublicKey();
            Console.WriteLine("GOT SERVER KEY");

            // Part 2: Send Public Key to server
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.KeySize = 8192;
            this.Writer.WriteLine(Convert.ToBase64String(rsa.ExportCspBlob(false)));
            Console.WriteLine("SENT OUR ENCRYPTED PUBLIC KEY TO SERVER");

            // Part 3: Generate IV and send it over (base 64 encoded, encrypted with servers public key)
            Aes aes = Aes.Create();
            aes.GenerateIV();
            byte[] iv = aes.IV;
            byte[] encryptedIV = this.ServerKey.Encrypt(iv, false);
            string encodedIV = Convert.ToBase64String(encryptedIV);
            this.Writer.WriteLine(encodedIV);
            Console.WriteLine("GENERATED IV AND SENT IT OVER TO SERVER");

            // Part 4: Wait for the AES key to get sent over and verify IV is correct
            string? line = this.Reader.ReadLine();
            byte[] data = Convert.FromBase64String(line ?? "");
            byte[] decrypted = rsa.Decrypt(data, false);
            aes.Key = decrypted;

            Console.WriteLine("Got AES key!");

            // Part 5: Start read loop. Encrypt messages with AES key and IV. When a message is received, unencrypt it and print it plain text.
            while (true)
            {
                string? lineToSend = Console.ReadLine();
                Console.WriteLine("SENDING: " + lineToSend ?? "");

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // encrypt line
                byte[] plain = Encoding.ASCII.GetBytes(lineToSend ?? "");
                byte[] encrypted = encryptor.TransformFinalBlock(plain, 0, plain.Length);
                string based = Convert.ToBase64String(encrypted);
                this.Writer.WriteLine(based);
            }
        }

        public RSACryptoServiceProvider ReceiveServerPublicKey()
        {
            string? line = this.Reader.ReadLine();

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(Convert.FromBase64String(line ?? ""));
            return rsa;
        }
    }
}