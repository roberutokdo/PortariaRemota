using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PortariaRemotaAPI.Controllers;
using PortariaRemotaAPI.Models;
using PortariaRemotaAPI.Models.DataContext;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestPortariaRemotaApi.Controller.Logins
{
    public class LoginsControllerLogin
    {
        private LoginsController _LoginsController;

        private KiperContext GetContext()
        {
            DbContextOptions<KiperContext> dbOptions;
            var dbOptionsBuilder = new DbContextOptionsBuilder<KiperContext>();

            dbOptionsBuilder.UseInMemoryDatabase().ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            dbOptions = dbOptionsBuilder.Options;
            return new KiperContext(dbOptions);
        }

        private async Task ForceAddInContext(KiperContext Context)
        {
            Login login = new Login
            {
                User = "Teste",
                Pass = "Teste@Kiper"
            };
            
            await Context.Logins.AddAsync(login);
            await Context.SaveChangesAsync();
        }

        [Fact]
        public async Task Login_WithNoPass()
        {
            KiperContext Context = GetContext();
            _LoginsController = new LoginsController(Context);
            await ForceAddInContext(Context);

            string user = "Teste";
            string pass = GetEncryptLoginPass("");
            var result = _LoginsController.GetLogin(user, pass) as NotFoundObjectResult;
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Usuário e/ou Senha incorretos. Tente novamente.".ToUpper(), result.Value.ToString().ToUpper());
        }

        [Fact]
        public async Task Login_WithWrongPass()
        {
            KiperContext Context = GetContext();
            _LoginsController = new LoginsController(Context);
            await ForceAddInContext(Context);

            string user = "Teste";
            string pass = GetEncryptLoginPass("Teste");
            var result = _LoginsController.GetLogin(user, pass) as NotFoundObjectResult;
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Usuário e/ou Senha incorretos. Tente novamente.".ToUpper(), result.Value.ToString().ToUpper());
        }

        [Fact]
        public async Task Login_WithCorrectPass()
        {
            KiperContext Context = GetContext();
            _LoginsController = new LoginsController(Context);
            await ForceAddInContext(Context);

            string user = "Teste";
            string pass = GetEncryptLoginPass("Teste@Kiper");
            var result = _LoginsController.GetLogin(user, pass) as OkResult;
            Assert.Equal(200, result.StatusCode);
        }

        private string GetEncryptLoginPass(string pass)
        {
            if (string.IsNullOrWhiteSpace(pass))
                return string.Empty;

            string key = "Security@!Pass";
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
