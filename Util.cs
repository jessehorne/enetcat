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

        public static IPAddress Parse(string host)
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
                        return IPAddress.Loopback;
                }
            }

            return ip == null ? IPAddress.Loopback : ip;
        }
    }

    class Port
    {
        public static bool IsValid(string port)
        {
            int intPort = Parse(port);

            if (intPort <= 0) {
                return false;
            }

            return true;
        }

        public static int Parse(string port)
        {
            int intPort;
            bool isPortANumber = int.TryParse(port, out intPort);

            if (!isPortANumber)
            {
                return 0;
            }

            return intPort;
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