using ICSharpCode.SharpZipLib.Zip;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PhotoShare.Server.BusinessLogic;
using PhotoShare.Server.Database.Context;
using PhotoShare.Server.Files;
using PhotoShare.ServerTests.Extensions;
using PhotoShare.Shared;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoShare.Server.BusinessLogic.Tests
{
    [TestClass()]
    public class PictureCrudExecutorTests
    {
        private PictureCrudExecutor executor;
        private Guid GroupId;
        private Guid GroupId2;
        private Guid UploaderKey;
        private readonly string text = "Test String";
        private readonly Aes aes = Aes.Create() ;
        [TestInitialize()]
        public async Task PictureCrudExecutorTest()
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(text);
            streamWriter.Flush();
            GroupId = Guid.NewGuid();
            GroupId2 = Guid.NewGuid();
            UploaderKey = Guid.NewGuid();
            aes.GenerateIV();
            aes.GenerateKey();
            var pictures = GetPictures();
            var picturesDbSetMock = new Mock<DbSet<Picture>>();
            picturesDbSetMock.SetupDbSet(pictures);
            var groupKeyDbSetMock = new Mock<DbSet<GroupKey>>();
            groupKeyDbSetMock.SetupDbSet(GetGroupKeys());

            var dbContext = new Mock<PhotoShareContext>() ;
            dbContext.Object.Pictures = picturesDbSetMock.Object;
            dbContext.Object.GroupKeys = groupKeyDbSetMock.Object;
            dbContext.Setup(x => x.Set<Picture>()).Returns(() => picturesDbSetMock.Object);
            dbContext.Setup(x => x.Set<GroupKey>()).Returns(() => groupKeyDbSetMock.Object); 

            var fileHandler = new FileHandler();
            var encryptor = new EncryptionHandler();
            foreach(var picture in pictures)
            {
                using (var ms = new MemoryStream())
                using (var sw = new StreamWriter(ms))
                {
                    sw.Write(text+picture.fileName);
                    sw.Flush();
                    ms.Position = 0;
                    using var encryptStream = encryptor.EncryptStream(ms, aes.Key, aes.IV);
                    await fileHandler.SaveToFile(picture.Path,encryptStream );
                }

            }

           
            var appSettings = @"{""AppSettings"":{
            ""FileSaveLocation"" : """"
            }}";

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            executor = new PictureCrudExecutor(dbContext.Object,encryptor,fileHandler, configuration);
        }

        [TestMethod()]
        public void DeletePictureTest()
        {
                        Assert.Fail();
        }

        [TestMethod()]
        public async Task GetGroupPicturesTest()
        {
            var pictures = executor.GetGroupPictures(GroupId);
            Assert.AreEqual(2, pictures.Count);
           
        }

        [TestMethod()]
        public async Task GetPicturesTest()
        {
            var pictureSaved = GetPictures().Where(p => p.GroupId == GroupId);
            var picture = executor.GetPictures(GroupId, pictureSaved.Select(p => p.Id).ToList());

            Assert.IsNotNull(picture);
            Assert.IsTrue(picture.Count == 2);
        }

        [TestMethod()]
        public async Task GetPictureTest()
        {
            var pictureSaved = GetPictures().First();
            var picture = executor.GetPicture<Picture>(pictureSaved.GroupId, pictureSaved.Id);

            Assert.IsNotNull(picture);
            Assert.IsTrue(picture.GroupId.Equals(pictureSaved.GroupId));           
        }

        [TestMethod()]
        public void UploadPictureTest()
        {
            Assert.Fail();
        }


        private List<Picture> GetPictures()
        {
            return new List<Picture>()
            {
                new Picture()
                {
                    Date = DateTime.MinValue,
                    fileName = "Test1",
                    GroupId = GroupId,
                    Uploader = "Me",
                    UploaderKey = UploaderKey,
                    Path = $"{Environment.CurrentDirectory}/Test1",
                    IV = aes.IV
                },
                new Picture()
                {
                    Date = DateTime.MinValue,
                    fileName = "Test2",
                    GroupId = GroupId,
                    Uploader = "Me",
                    UploaderKey = UploaderKey,
                    Path = $"{Environment.CurrentDirectory}/Test2",
                    IV = aes.IV
                },
                new Picture()
                {
                    Date = DateTime.MinValue,
                    fileName = "Test3",
                    GroupId = GroupId2,
                    Uploader = "Me",
                    UploaderKey = UploaderKey,
                    Path = $"{Environment.CurrentDirectory}/Test3",
                    IV = aes.IV
                }
            };
        }

        private List<GroupKey> GetGroupKeys()
        {
            return new List<GroupKey>() {
                new GroupKey()
                {
                    GroupId = GroupId,
                    AdminKey = GroupId,
                    EncryptionKey = aes.Key
                },
                new GroupKey()
                {
                    GroupId = GroupId2,
                    AdminKey = GroupId2,
                    EncryptionKey = new byte[3]
                }
            };
        }
      
    }
}