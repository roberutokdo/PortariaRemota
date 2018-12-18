using Microsoft.AspNetCore.Mvc;
using PortariaRemotaAPI.Models;
using PortariaRemotaAPI.Models.DataContext;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PortariaRemotaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly KiperContext _context;
        private readonly string PassPhrase = "Security@!Pass";
        private string PassKey { get; set; }

        public LoginsController(KiperContext context)
        {
            _context = context;
            PassKey = GenerateAPassKey(this.PassPhrase);
        }

        [HttpGet]
        public IActionResult GetLogin(string user, string pass)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string decryptedPass = Decrypt(pass, PassKey);
            Login loginFounded = _context.Logins.FirstOrDefault(l => l.User.Equals(user) && l.Pass.Equals(decryptedPass));

            if (loginFounded == null)
            {
                return NotFound("Usuário e/ou Senha incorretos. Tente novamente.");
            }

            return Ok();
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

        private static string Decrypt(string encryptedText, string KeyString)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.ISO10126
            };

            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedText.ToCharArray(), 0, encryptedText.Length);
            return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }
    }
}