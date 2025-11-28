namespace Core.Interfaces
{
    public interface IEncryptor
    {
        public string Encrypt(string raw);
        public string Decrypt(string encrypted);
    }
}
