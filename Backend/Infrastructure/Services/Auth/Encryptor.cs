using System.Text;
using Core.Dtos.Settings;
using Core.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Infrastructure.Services;

public class Encryptor : IEncryptor
{
    private readonly EncryptorSettings _settings;

    public Encryptor(IOptions<EncryptorSettings> options)
    {
        _settings = options.Value;
    }

    public string Encrypt(string raw)
    {
        byte[] rawBytes = Encoding.UTF8.GetBytes(raw);
        using Aes aes = Aes.Create();
        aes.Key = _settings.Key;
        aes.IV = _settings.IV;
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        {
            cs.Write(rawBytes, 0, rawBytes.Length);
            cs.FlushFinalBlock();
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string encrypted)
    {
        byte[] rawBytes = Convert.FromBase64String(encrypted);
        using Aes aes = Aes.Create();
        aes.Key = _settings.Key;
        aes.IV = _settings.IV;
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        {
            cs.Write(rawBytes, 0, rawBytes.Length);
            cs.FlushFinalBlock();
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }
}
