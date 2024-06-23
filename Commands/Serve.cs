using ENetcat;

namespace Commands
{
    class CommandServe
    {
        public static int Serve(string host, string port)
        {
            // Validate host ip
            bool isHostValid = Util.IP.IsValid(host);
            if (!isHostValid)
            {
                Console.WriteLine("invalid ip");
                return 1;
            }

            // Validate port number
            bool isPortValid = Util.Port.IsValid(port);
            if (!isPortValid)
            {
                Console.WriteLine("invalid port");
                return 1;
            }

            // Initialize config
            Conf c = new Conf();

            // Check that user has key set
            if (!c.HasDefaultKey())
            {
                Console.WriteLine("No default key.");
                Console.WriteLine("If you haven't generated a key yet, please run `enetcat key generate <host>`.");
                Console.WriteLine("And/or run `enetcat key use <id>` to set a default key.");
                return 1;
            }

            return 0;
        }
    }
}