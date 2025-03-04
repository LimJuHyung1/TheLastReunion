using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    // ������ 32����Ʈ (256��Ʈ) ��ȣȭ Ű ����
    private static readonly byte[] encryptionKey = CreateEncryptionKey("YourSecretKey1234");

    // AES ��ȣȭ
    public static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.GenerateIV(); // 16����Ʈ IV �ڵ� ����
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] iv = aes.IV;

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length);
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    // AES ��ȣȭ
    public static string Decrypt(string encryptedText)
    {
        byte[] fullCipher = Convert.FromBase64String(encryptedText);
        using (Aes aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[16];
            byte[] cipherText = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv))
            using (MemoryStream ms = new MemoryStream(cipherText))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    // ���ڿ��� 32����Ʈ AES Ű�� ��ȯ�ϴ� �޼���
    private static byte[] CreateEncryptionKey(string key)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }
}