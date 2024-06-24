using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace ENetcat
{
    class TCPEchoHandler
    {
        public StreamWriter writer { get; set; }
        public StreamReader reader { get; set; }
        public Conf conf { get; set; }
        public Aes aes { get; set; }

        public TCPEchoHandler(NetworkStream stream)
        {
            this.writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            this.reader = new StreamReader(stream, Encoding.ASCII);
            this.conf = new Conf();
            this.aes = Aes.Create();
        }

        // Handshake: Part 1

        // SendRSAPublicKeyToClient sends the servers public key CSP blob to the client. This is the first step during the
        // handshake.
        public void SendRSAPublicKeyToClient()
        {
            this.writer.WriteLine(this.conf.GetDefaultPublicKeyCSPBlob());
        }


        // Handshake: Part 2

        // ReceiveClientRSAPublicKey reads the clients RSA public key CSP blob to be transmitted.
        // It will either return the RSA key when it gets it, or throw an exception due to incorrect input
        // or a timeout.
        public RSACryptoServiceProvider? ReceiveClientRSAPublicKey()
        {
            Conf c = new Conf();

            string? line = "";
            line = reader.ReadLine();

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            if (line == null )
            {
                return null;
            }

            rsa.ImportCspBlob(Convert.FromBase64String(line));

            return rsa;
        }

        // Handshake: Part 3 (Encryption begins)

        // ReceiveClientIVChallenge waits for the clients AES Challenge / IV. We are letting the
        // client generate the 16 byte IV because it also serves as a challenge to build trust between the client and
        // server. The data received from the client should be a single base64-encode line and it should have been encrypted
        // with the servers RSA public key.
        public byte[] ReceiveClientIVChallenge()
        {
            // load the config
            Conf c = new Conf();

            // get the iv
            string? line = "";
            line = this.reader.ReadLine();

            if (line == null)
            {
                return new Byte[0];
            }

            byte[] data = Convert.FromBase64String(line);
            byte[] iv = c.DecryptWithDefaultKey(data);

            return iv;
        }

        // Handshake: Part 4 (Last step)

        // SendAESKeyAndIVBackToClient encrypts the IV/challenge and the AES key with the clients RSA public key then
        // sends it to the client. This lets the client know that it actually has the expected RSA private key and that
        // it will be using a IV for AES communication.
        public void SendAESKeyAndIVBackToClient(byte[] iv, RSACryptoServiceProvider clientKey)
        {
            // load IV and generate a aes key
            this.aes.IV = iv;
            this.aes.GenerateKey();

            byte[] encrypted = clientKey.Encrypt(this.aes.Key, false);

            this.writer.WriteLine(Convert.ToBase64String(encrypted));
        }

        // // StartSecureCommunication starts a secure communication loop between the server and client after all of the
        // // previous handshake steps are completed. It waits for 
        // public void StartSecureCommunication()
        // {

        // }

        public int Serve()
        {
            // Handshake: Part 1
            this.SendRSAPublicKeyToClient();
            Console.WriteLine("SENDING SERVER PUB KEY TO CLIENT");

            // Handshake: Part 2
            RSACryptoServiceProvider? rsa = this.ReceiveClientRSAPublicKey();

            if (rsa == null) {
                Console.WriteLine("Invalid client public key.");
                return 1;
            }
            Console.WriteLine("GOT CLIENT PUB KEY");

            // Handshake: Part 3
            byte[] iv = this.ReceiveClientIVChallenge();

            if (iv.Length != 16)
            {
                Console.WriteLine("Invalid IV. Not 16-byte.");
                return 1;
            }
            Console.WriteLine("RECEIVED IV CHALLENGE");

            // Handshake: Part 4
            this.SendAESKeyAndIVBackToClient(iv, rsa);

            Console.WriteLine("Sent IV!");

            Console.WriteLine("Starting loop");

            ICryptoTransform encryptor = aes.CreateEncryptor(this.aes.Key, this.aes.IV);
            ICryptoTransform decryptor = aes.CreateDecryptor(this.aes.Key, this.aes.IV);

            while (true)
            {
                string? line = "";
                while (line != null)
                {
                    line = this.reader.ReadLine();

                    byte[] data = Convert.FromBase64String(line ?? "");
                    byte[] decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);

                    Console.WriteLine(Encoding.ASCII.GetString(decrypted, 0, decrypted.Length));

                    // send it back
                    byte[] encrypted = encryptor.TransformFinalBlock(decrypted, 0, decrypted.Length);
                }
                
                break;
            }

            return 0;

            // // send over the aes key encrypted with our private key
            // string base64key = Convert.ToBase64String(this.aes.Key);
            // this.writer.WriteLine(base64key);


            // bool readingKey = true;
            // string rsaPubKey = "";
            // while (true) {
            //     string? inputLine = "";
            //     while (inputLine != null)
            //     {
            //         inputLine = reader.ReadLine();
            //         string line = inputLine == null ? "" : inputLine;

            //         if (readingKey)
            //         {
            //             rsaPubKey += line;
            //             if (line == "-----END RSA PUBLIC KEY-----")
            //             {
            //                 readingKey = false;
            //             }

            //         } else {
            //             this.SendEncryptedLine(line);
            //         }
                    
            //         Console.WriteLine(inputLine);
            //     }
            //     break;
            // }
        }
    }
}