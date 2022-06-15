using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoShare.Server.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Server.BusinessLogic.Tests
{
    [TestClass()]
    public class EncryptionHandlerTests
    {
        private readonly EncryptionHandler _encryptionHandler = new EncryptionHandler();

        [TestMethod()]
        public async Task En_DecryptStreamTest()
        {
            var text = "To Encrypt";
            var file = Environment.CurrentDirectory + "/encryptionTest.txt";
            using var ms = new MemoryStream();
            using var streamWriter = new StreamWriter(ms);

            streamWriter.Write(text);
            await streamWriter.FlushAsync();
            ms.Position = 0;


            var aes = Aes.Create();
            aes.GenerateIV();
            aes.GenerateKey();

            using (var cryptStream = _encryptionHandler.EncryptStream(ms, aes.Key, aes.IV))
            {
                using var fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
                cryptStream.CopyTo(fileStream);
                fileStream.Position = 0;
                await fileStream.FlushAsync();
            }

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var fileReadStream = new StreamReader(fileStream))
            {
                var stillEncrypted = await fileReadStream.ReadToEndAsync();
                Assert.AreNotEqual(text, stillEncrypted);
            }

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var decrypt = _encryptionHandler.DecryptStream(fileStream, aes.Key, aes.IV))
            using (var fileReadStream = new StreamReader(decrypt))
            {
                var decrypted = await fileReadStream.ReadToEndAsync();
                Assert.AreEqual(text, decrypted);
            }

        }


    }
}
