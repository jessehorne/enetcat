using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ENetcat
{
    class ENetCat
    {
        static int Main(string[] args)
        {
            // Setup base folder if it doesn't exist
            if (!Util.Storage.Setup())
            {
                Console.WriteLine("Couldn't set up storage.");
                return 1;
            }

            // check if calling help or user supplied no arguments
            bool noArgs = args.Length == 0;
            bool callingHelp = args.Length == 1 && args[0] == "help";

            if (noArgs || callingHelp)
            {
                return Commands.CommandHelp.Run();
            }

            // check if user is running a key command
            bool isKeyCommand = args.Length > 0 && args[0] == "key";

            if (isKeyCommand)
            {
                if (args.Length > 1)
                {
                    if (args[1] == "list")
                    {
                        return Commands.CommandKey.List();
                    } else if (args[1] == "generate")
                    {
                        if (args.Length == 3)
                        {
                            return Commands.CommandKey.Generate(args[2]);
                        }
                    } else if (args[1] == "use")
                    {
                        if (args.Length == 3)
                        {
                            return Commands.CommandKey.Use(args[2]);
                        }
                    } else if (args[1] == "remove")
                    {
                        if (args.Length == 3)
                        {
                            return Commands.CommandKey.Remove(args[2]);
                        }
                    }

                    return Commands.CommandHelp.Run();
                }
            }

            // check if user is trying to run a server
            bool isServeCommand = args.Length > 0 && args[0] == "serve";

            if (isServeCommand)
            {
                if (args.Length == 3)
                {
                    return Commands.CommandServe.Serve(args[1], args[2]);
                }
            }

            bool isConnectCommand = args.Length > 0 && args[0] == "connect";

            if (isConnectCommand)
            {
                if (args.Length == 3)
                {
                    return Commands.CommandConnect.Connect(args[1], args[2]);
                }
            }

            return Commands.CommandHelp.Run();
        }
    }
}