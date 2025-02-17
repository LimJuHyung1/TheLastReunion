using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    // 환경 변수에서 암호화 키 가져오기 (없으면 예외 발생)
    private static readonly string encryptionKey = Environment.GetEnvironmentVariable("Path")
        ?? throw new Exception(" 환경 변수 'ENCRYPTION_KEY'가 설정되지 않았습니다.");

    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetKeyBytes();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.GenerateIV(); // IV 자동 생성

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aesAlg.IV, 0, aesAlg.IV.Length); // IV를 암호문 앞에 저장
                using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(inputBytes, 0, inputBytes.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string encryptedText)
    {
        byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetKeyBytes();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[aesAlg.IV.Length];
            Array.Copy(cipherTextBytes, 0, iv, 0, iv.Length);
            aesAlg.IV = iv; // 복호화 시 IV 설정

            using (MemoryStream ms = new MemoryStream(cipherTextBytes, iv.Length, cipherTextBytes.Length - iv.Length))
            using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
            {
                byte[] decryptedBytes = new byte[cipherTextBytes.Length - iv.Length];
                int decryptedCount = cs.Read(decryptedBytes, 0, decryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedCount);
            }
        }
    }

    private static byte[] GetKeyBytes()
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        Array.Resize(ref keyBytes, 32); // AES-256 (32바이트)
        return keyBytes;
    }
}
