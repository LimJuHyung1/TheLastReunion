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
        // ".openai" 폴더 경로 설정
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string openaiFolder = Path.Combine(userFolder, ".openai");
        filePath = Path.Combine(openaiFolder, "auth.json");

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(openaiFolder))
        {
            Directory.CreateDirectory(openaiFolder);
            Debug.Log($".openai 폴더 생성됨: {openaiFolder}");
        }

        Debug.Log($"API 키 파일 저장 경로: {filePath}"); // 파일 저장 경로 확인 로그 추가

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogWarning("API 키가 저장되지 않았습니다! 랜덤 키를 생성하여 저장합니다.");
            SaveKey();
        }

        string apiKey = LoadDecryptedAPIKey();

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API 키를 불러올 수 없습니다! 설정을 확인하세요.");
        }
        else
        {
            Debug.Log("API 키가 성공적으로 로드되었습니다.");
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
        Debug.Log("API 키와 조직 정보가 암호화되어 저장되었습니다.");
    }

    public static string LoadDecryptedAPIKey()
    {
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string openaiFolder = Path.Combine(userFolder, ".openai");
        filePath = Path.Combine(openaiFolder, "auth.json");

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("API 키 파일이 존재하지 않습니다.");
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
            Debug.LogError("API 키 파일이 존재하지 않습니다.");
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

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(openaiFolder))
        {
            Directory.CreateDirectory(openaiFolder);
            Debug.Log($".openai 폴더 생성됨: {openaiFolder}");
        }

        // string randomApiKey = EncryptionHelper.GenerateRandomKey();
        // SaveEncryptedAPIKey(randomApiKey, "default-organization");

        if (System.IO.File.Exists(filePath))
        {
            Debug.Log($"API 키 파일이 성공적으로 저장됨: {filePath}");
        }
        else
        {
            Debug.LogError($"API 키 파일 저장 실패! 경로: {filePath}");
        }
    }

    public static void LoadKey()
    {
        string apiKey = LoadDecryptedAPIKey();
        Debug.Log($"API 키: {apiKey}");
    }
}
