using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebPortariaRemota.Models.Utils;

namespace WebPortariaRemota.Security
{
    public class EncryptLogin
    {
        private Settings settings = Settings.Instance;

        private EncryptLogin() { }
        private static EncryptLogin _EncryptLogin;
        public static EncryptLogin Instance
        {
            get
            {
                if (_EncryptLogin is null)
                    _EncryptLogin = new EncryptLogin();

                return _EncryptLogin;
            }
        }

        public string GetEncryptLoginPass(string pass)
        {
            if (string.IsNullOrWhiteSpace(pass))
                return string.Empty;

            string key = settings.GetEncryptedKey();
            return Encrypt(pass, GenerateAPassKey(key));
        }

        private static string GenerateAPassKey(string passphrase)
        {
            string passPhrase = passphrase;
            string saltValue = passphrase;
            string hashAlgorithm = "SHA1";
            int passwordIterations = 2;
            int keySize = 256;
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] Key = pdb.GetBytes(keySize / 11);
            String KeyString = Convert.ToBase64String(Key);

            return KeyString;
        }

        private static string Encrypt(string plainStr, string KeyString)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.ECB;
            aesEncryption.Padding = PaddingMode.ISO10126;
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText);
        }
    }
}
