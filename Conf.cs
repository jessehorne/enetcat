using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ENetcat
{
    public struct Key {
        public string Host { get; set; }
        public string KeyCSP { get; set; } // Base64 encoded CSP of RSA
    }

    class ConfData
    {
        public List<Key> Keys { get; set; }
        public int DefaultKey { get; set; }

        public ConfData()
        {
            this.Keys = new List<Key>();
            this.DefaultKey = -1; // -1 means that we haven't generated a default key yet
        }
    }

    class Conf{
        public ConfData data { get; set; }

        public Conf()
        {
            this.data = new ConfData();

            this.Load(); // will attempt to load an existing conf.json or fill defaults and save if not
        }
        
        public string GetDefaultPublicKeyCSPBlob()
        {
            return Convert.ToBase64String(this.GetDefaultKey().ExportCspBlob(false));
        }

        public byte[] DecryptWithDefaultKey(byte[] data)
        {
            return this.GetDefaultKey().Decrypt(data, false);
        }

        public RSACryptoServiceProvider GetDefaultKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            if (this.data.DefaultKey == -1)
            {
                return rsa;
            }

            byte[] csp = Convert.FromBase64String(this.data.Keys[this.data.DefaultKey].KeyCSP);
            rsa.ImportCspBlob(csp);

            return rsa;
        }

        public void AddKey(string host, RSACryptoServiceProvider rsa, bool useAsDefault)
        {
            Key newKey = new Key();
            newKey.Host = host;
            newKey.KeyCSP = Convert.ToBase64String(rsa.ExportCspBlob(true));

            this.data.Keys.Add(newKey);

            if (useAsDefault)
            {
                this.data.DefaultKey = this.data.Keys.Count - 1;
            }
        }

        public Boolean HasDefaultKey()
        {
            return this.data.DefaultKey != -1;
        }

        public void Save()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string confPath = Path.Join(basePath, "enetcat", "conf.json");
            string jsonString = JsonSerializer.Serialize(this.data);
            FileStream fs = File.Create(confPath);
            fs.Write(Encoding.ASCII.GetBytes(jsonString));
            fs.Close();
        }

        public void Load()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string confPath = Path.Join(basePath, "enetcat", "conf.json");

            // check if conf.json exists. if so, load it. if not, create it.
            if (!File.Exists(confPath))
            {
                // file doesn't exist; go ahead and save default
                this.Save();
            } else {
                // we need to load what we saved
                string jsonData = File.ReadAllText(confPath);
                ConfData? data = JsonSerializer.Deserialize<ConfData>(jsonData);

                if (data == null) {
                    return;
                }

                this.data = data;
            }
        }
    }
}