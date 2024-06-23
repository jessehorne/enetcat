
using System.Security.Cryptography;
using ENetcat;

namespace Commands
{
    public class CommandKey
    {
        public static int List()
        {
            // load config
            Conf c = new Conf();

            if (c.data.Keys.Count == 0)
            {
                Console.WriteLine("You don't have any keys yet. Run `enetcat key generate <host>`.");
                return 0;
            }

            int index = 0;
            foreach (Key key in c.data.Keys)
            {
                string f = "({0}) {1}";
                if (c.data.DefaultKey == index)
                {
                    f += " [default]";
                }
                Console.WriteLine(String.Format(f, index, key.Host));
                index++;
            }

            return 0;
        }

        public static int Generate(string host)
        {
            // load config
            Conf c = new Conf();

            // validate host
            if (!Util.IP.IsValid(host))
            {
                Console.WriteLine("Invalid host. Must be IPv4 or IPv6");
                return 1;
            }

            // generate 4096 rsa key pair
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            Console.WriteLine(String.Format("Generated RSA key pair of {0} bits.", rsa.KeySize));

            // write to disk
            c.AddKey(host, rsa, false);
            Console.WriteLine("Saved key.");

            c.Save();

            return 0;
        }

        public static int Use(string id)
        {
            int intID;
            try
            {
                intID = Int32.Parse(id);
            } catch
            {
                Console.WriteLine("Invalid ID.");
                return 1;
            }

            // load config
            Conf c = new Conf();

            // validate id
            if (intID < 0 || intID > c.data.Keys.Count-1)
            {
                Console.WriteLine("There is no key with that ID. Try `enetcat key list`");
                return 1;
            }

            c.data.DefaultKey = intID;
            c.Save();

            return 0;
        }

        public static int Remove(string id)
        {
            int intID;
            try
            {
                intID = Int32.Parse(id);
            } catch
            {
                Console.WriteLine("Invalid ID.");
                return 1;
            }

            // load config
            Conf c = new Conf();

            // validate id
            if (intID < 0 || intID > c.data.Keys.Count-1)
            {
                Console.WriteLine("There is no key with that ID. Try `enetcat key list`");
                return 1;
            }

            if (c.data.DefaultKey > intID)
            {
                // if the default key is larger than the intID being deleted, it should be updated to current-1
                c.data.DefaultKey--;
            } else if (c.data.DefaultKey == intID)
            {
                // if the default key is being deleted, we should set it back to -1 which means no default key is set
                c.data.DefaultKey = -1;
            }

            c.data.Keys.RemoveAt(intID);
            c.Save();

            return 0;
        }
    }
}