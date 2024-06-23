

namespace Commands
{
    class CommandHelp
    {
        public static int Run()
        {
            Console.WriteLine(@"enetcat utility

Commands

help                            Show help menu.
key generate <host>             Generate RSA key pair identified by an IP address.
key list                        List all keys and show the current one.
key use <id>                    Set the current key to the one specified by <id>. IDs can be found by using `key list`.
serv <host> <port>              Instantiate a server.
connect <host> <port>           Connect to a server.");
            return 0;
        }

    }
}