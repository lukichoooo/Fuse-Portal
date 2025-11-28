using Core.Dtos.Settings;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Tests.Infrastructure
{
    [TestFixture]
    public class EncryptorTests
    {
        private IEncryptor _encryptor;
        private readonly EncryptorSettings _settings = new()
        {
            Key = Convert.FromBase64String("MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY="),
            IV = Convert.FromBase64String("bXlJbml1VmVjdG9yMTIzNA==")
        };


        [OneTimeSetUp]
        public void BeforeAll()
        {
            var options = Options.Create(_settings);
            _encryptor = new Encryptor(options);
        }

        [TestCase("askdadka")]
        [TestCase("saidadf2O_aa")]
        [TestCase("OHAGAwa(2)@@")]
        [TestCase("diaksWjfwe9jhaskdadka")]
        public void Encrypt_Decrypt(string data)
        {
            var startTime = DateTime.Now;
            var encrypted = _encryptor.Encrypt(data);
            var decrypted = _encryptor.Decrypt(encrypted);
            var timeDifference = DateTime.Now.Subtract(startTime);

            const double seconds = 0.5 / 60;

            Assert.That(timeDifference.TotalMinutes, Is.LessThan(seconds));
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(decrypted, Is.Not.Null);
            Assert.That(encrypted, Is.Not.EqualTo(data));
            Assert.That(encrypted, Is.Not.EqualTo(decrypted));
            Assert.That(decrypted, Is.EqualTo(data));
        }
    }
}
