namespace Core.Dtos.Settings
{
    public class EncryptorSettings
    {
        public byte[] Key { get; set; } = null!;
        public byte[] IV { get; set; } = null!;
    }
}
