using Newtonsoft.Json;
using OpenAI_API.Files;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    private static string filePath;

    private void Awake()
    {
        // ".openai" ���� ��� ����
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string openaiFolder = Path.Combine(userFolder, ".openai");
        filePath = Path.Combine(openaiFolder, "auth.json");

        // ������ �������� ������ ����
        if (!Directory.Exists(openaiFolder))
        {
            Directory.CreateDirectory(openaiFolder);
            Debug.Log($".openai ���� ������: {openaiFolder}");
        }

        Debug.Log($"API Ű ���� ���� ���: {filePath}"); // ���� ���� ��� Ȯ�� �α� �߰�

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogWarning("API Ű�� ������� �ʾҽ��ϴ�! ���� Ű�� �����Ͽ� �����մϴ�.");
            SaveKey();
        }

        string apiKey = LoadDecryptedAPIKey();

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API Ű�� �ҷ��� �� �����ϴ�! ������ Ȯ���ϼ���.");
        }
        else
        {
            Debug.Log("API Ű�� ���������� �ε�Ǿ����ϴ�.");
        }
    }

    public static void SaveEncryptedAPIKey(string apiKey, string organization)
    {
        string encryptedKey = EncryptionHelper.Encrypt(apiKey);
        string encryptedOrg = EncryptionHelper.Encrypt(organization);

        var json = new Dictionary<string, string>
        {
            { "api_key", encryptedKey },
            { "organization", encryptedOrg }
        };

        string jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);

        System.IO.File.WriteAllText(filePath, jsonString);
        Debug.Log("API Ű�� ���� ������ ��ȣȭ�Ǿ� ����Ǿ����ϴ�.");
    }

    public static string LoadDecryptedAPIKey()
    {
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string openaiFolder = Path.Combine(userFolder, ".openai");
        filePath = Path.Combine(openaiFolder, "auth.json");

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("API Ű ������ �������� �ʽ��ϴ�.");
            return null;
        }

        string jsonString = System.IO.File.ReadAllText(filePath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

        if (jsonData != null && jsonData.ContainsKey("api_key"))
        {
            return EncryptionHelper.Decrypt(jsonData["api_key"]);
        }

        return null;
    }

    public static string LoadDecryptedOrganization()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.Combine(Application.persistentDataPath, "auth.json");
        }

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("API Ű ������ �������� �ʽ��ϴ�.");
            return null;
        }

        string jsonString = System.IO.File.ReadAllText(filePath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

        if (jsonData != null && jsonData.ContainsKey("organization"))
        {
            return EncryptionHelper.Decrypt(jsonData["organization"]);
        }

        return null;
    }


    public static void SaveKey()
    {
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string openaiFolder = Path.Combine(userFolder, ".openai");
        filePath = Path.Combine(openaiFolder, "auth.json");

        // ������ �������� ������ ����
        if (!Directory.Exists(openaiFolder))
        {
            Directory.CreateDirectory(openaiFolder);
            Debug.Log($".openai ���� ������: {openaiFolder}");
        }

        // string randomApiKey = EncryptionHelper.GenerateRandomKey();
        // SaveEncryptedAPIKey(randomApiKey, "default-organization");

        if (System.IO.File.Exists(filePath))
        {
            Debug.Log($"API Ű ������ ���������� �����: {filePath}");
        }
        else
        {
            Debug.LogError($"API Ű ���� ���� ����! ���: {filePath}");
        }
    }

    public static void LoadKey()
    {
        string apiKey = LoadDecryptedAPIKey();
        Debug.Log($"API Ű: {apiKey}");
    }
}
