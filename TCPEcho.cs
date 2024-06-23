// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Security.Cryptography;

// namespace ENetcat
// {
//     class TCPEchoHandler
//     {
//         public BinaryWriter writer { get; set; }
//         public BinaryReader reader { get; set; }

//         public TCPEchoHandler(NetworkStream stream)
//         {
//             this.writer = new BinaryWriter(stream);
//             this.reader = new BinaryReader(stream);
//         }

//         // Handshake: Part 1

//         // SendRSAPublicKeyToClient sends the servers public key CSP blob to the client. This is the first step during the
//         // handshake.
//         public void SendRSAPublicKeyToClient()
//         {
//             this.writer.
//         }


//         // Handshake: Part 2

//         // ReceiveClientRSAPublicKey starts a read loop that waits for the clients RSA public key CSP blob to be transmitted.
//         // It will either return the RSA key when it gets it, or throw an exception due to incorrect input
//         // or a timeout.
//         public RSA ReceiveClientRSAPublicKey()
//         {
//             string rsaKeyText = "";
//             string? line = "";
//             while (line != null)
//             {
//                 line = reader.ReadLine();
                
//                 rsaKeyText += line;

//                 if (line == "-----END RSA PUBLIC KEY-----")
//                 {
//                     break;
//                 }
//             }

//             Console.WriteLine("Received rsa public key from client...");
//             Console.WriteLine(rsaKeyText);


//         }

//         // Handshake: Part 3 (Encryption begins)

//         // ReceiveClientIVChallenge starts a read loop that waits for the clients AES Challenge / IV. We are letting the
//         // client generate the 16 byte IV because it also serves as a challenge to build trust between the client and
//         // server. It will either return the IV as a byte array or throw an exception due to incorrect input or
//         // a timeout. The data received from the client should be a single line and it should have been encrypted
//         // with the servers RSA public key.
//         public byte[] ReceiveClientIVChallenge()
//         {
            
//         }

//         // Handshake: Part 4 (Last step)

//         // SendAESKeyAndIVBackToClient encrypts the IV/challenge and the AES key with the clients RSA public key then
//         // sends it to the client. This lets the client know that it actually has the expected RSA private key and that
//         // it will be using a IV for AES communication.
//         public void SendAESKeyAndIVBackToClient()
//         {

//         }

//         // StartSecureCommunication starts a secure communication loop between the server and client after all of the
//         // previous handshake steps are completed. It waits for 
//         public void StartSecureCommunication()
//         {

//         }

//         public void StartServer()
//         {
            
//             this.writer =  new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
//             this.reader = new StreamReader(stream, Encoding.ASCII);

//             // send over the rsa public key first
//             this.SendRSAPublicKeyToClient()

//             // send over the aes key encrypted with our private key
//             string base64key = Convert.ToBase64String(this.aes.Key);
//             this.writer.WriteLine(base64key);


//             bool readingKey = true;
//             string rsaPubKey = "";
//             while (true) {
//                 string? inputLine = "";
//                 while (inputLine != null)
//                 {
//                     inputLine = reader.ReadLine();
//                     string line = inputLine == null ? "" : inputLine;

//                     if (readingKey)
//                     {
//                         rsaPubKey += line;
//                         if (line == "-----END RSA PUBLIC KEY-----")
//                         {
//                             readingKey = false;
//                         }

//                     } else {
//                         this.SendEncryptedLine(line);
//                     }
                    
//                     Console.WriteLine(inputLine);
//                 }
//                 break;
//             }
//         }
//     }
// }