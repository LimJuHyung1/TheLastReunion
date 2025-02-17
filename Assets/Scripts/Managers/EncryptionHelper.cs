using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    // ȯ�� �������� ��ȣȭ Ű �������� (������ ���� �߻�)
    private static readonly string encryptionKey = Environment.GetEnvironmentVariable("Path")
        ?? throw new Exception(" ȯ�� ���� 'ENCRYPTION_KEY'�� �������� �ʾҽ��ϴ�.");

    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GetKeyBytes();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.GenerateIV(); // IV �ڵ� ����

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aesAlg.IV, 0, aesAlg.IV.Length); // IV�� ��ȣ�� �տ� ����
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
            aesAlg.IV = iv; // ��ȣȭ �� IV ����

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
        Array.Resize(ref keyBytes, 32); // AES-256 (32����Ʈ)
        return keyBytes;
    }
}
