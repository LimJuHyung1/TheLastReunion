using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    private static readonly string encryptionKey = "My16CharKey12345"; // 16, 24, 32글자 길이의 키 사용 (AES-256)

    // 문자열을 AES로 암호화
    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = new byte[16]; // 초기화 벡터 (IV)를 0으로 초기화 (보안 강화 시에는 난수 생성 사용)

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }

    // 암호화된 문자열을 AES로 복호화
    public static string Decrypt(string encryptedText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = new byte[16];

            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[memoryStream.Length];
                    int bytesRead = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                    return Encoding.UTF8.GetString(decryptedBytes, 0, bytesRead);
                }
            }
        }
    }
}
