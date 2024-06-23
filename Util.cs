using System.Net;

namespace Util
{
    class IP
    {
        public static bool IsValid(string host)
        {
            IPAddress? ip;
            if (IPAddress.TryParse(host, out ip))
            {
                switch (ip.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        break;
                    default:
                        return false;
                }
            } else {
                return false;
            }

            return true;
        }
    }

    class Port
    {
        public static bool IsValid(string port)
        {
            int intPort;
            bool isPortANumber = int.TryParse(port, out intPort);

            if (!isPortANumber) {
                return false;
            }

            if (intPort <= 0) {
                return false;
            }

            return true;
        }
    }

    class Storage
    {
        public static bool Setup()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (folder == "") {
                Console.WriteLine("Couldn't find LocalApplicationData folder on your system. Please submit a ticket on GitHub with details on your OS.");
                return false;
            }

            string enetcatDir = Path.Combine(folder, "enetcat");
            Directory.CreateDirectory(enetcatDir);

            return true;
        }
    }
}