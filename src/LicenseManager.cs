using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BepInEx.Configuration;

namespace SnowsCameraMod
{
    public sealed class LicenseManager
    {
        private readonly ConfigEntry<string> storedHash;
        private readonly HashSet<string> validHashes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "6be690972aeab1c5c510188e7182030beea2db1d0c6013c998702ce4729e5f97",
            "356b30218669117a9a8c7680c1cc391b20f8e0f7d04e283f435a149b5396cbbb",
            "e46750907ec132eeb9f4613378486c3bb1ede756e4fa720f54e6492329a70042",
            "fd33e55acec81d2ae575b95a781244691db004ce8ce9ad6c035828d7a9bf77b6",
            "b0daa80155e8db08c7a1d0473f644e4d196712bc3505d14919ef04a1b22e69bf"
        };

        public bool IsPlus { get; private set; }
        public bool IsInjected { get; private set; }

        public LicenseManager(ConfigFile config)
        {
            storedHash = config.Bind("Snow's Camera Plus", "Activated License", "", "Stores only a SHA-256 hash of the activated key.");
            IsPlus = !string.IsNullOrWhiteSpace(storedHash.Value) && validHashes.Contains(storedHash.Value);
        }

        public bool Activate(string rawKey)
        {
            string normalized = Normalize(rawKey);
            if (!IsValidFormat(normalized)) return false;

            string hash = Hash(normalized);
            if (!validHashes.Contains(hash)) return false;

            storedHash.Value = hash;
            IsPlus = true;
            return true;
        }

        public void Deactivate()
        {
            storedHash.Value = "";
            IsPlus = false;
        }

        public void Inject() => IsInjected = true;

        public static string Normalize(string key) => key == null ? "" : key.Trim().ToUpperInvariant();

        public static bool IsValidFormat(string key)
        {
            if (key.Length != 24 || !key.StartsWith("SCM-")) return false;
            string[] parts = key.Split('-');
            if (parts.Length != 6) return false;

            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].Length != 4) return false;
                for (int c = 0; c < parts[i].Length; c++)
                {
                    char ch = parts[i][c];
                    if (!((ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'F'))) return false;
                }
            }
            return true;
        }

        private static string Hash(string value)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder builder = new StringBuilder(bytes.Length * 2);
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}