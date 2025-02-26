using System.IO;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    private string authFilePath;

    private void Start()
    {
        // ���� Windows ����� ���� ��� ��������
        string userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

        // .openai ���� ��� ����
        string targetFolder = Path.Combine(userFolder, ".openai");

        // ������ �������� ������ ����
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            Debug.Log($"���� ���� �Ϸ�: {targetFolder}");
        }
        else
        {
            Debug.Log($"������ �̹� �����մϴ�: {targetFolder}");
        }

        // auth.json ���� ��� ����
        authFilePath = Path.Combine(targetFolder, "auth.json");

        // auth.json ������ ������ ���� ����
        if (!File.Exists(authFilePath))
        {
            CreateEncryptedAuthFile();
        }
        else
        {
            Debug.Log($"auth.json ������ �̹� �����մϴ�: {authFilePath}");
        }
    }

    /// <summary>
    /// ��ȣȭ�� AuthData�� JSON ���Ϸ� ����
    /// </summary>
    private void CreateEncryptedAuthFile()
    {
        string sourceAuthFilePath = Path.Combine(Application.dataPath, "auth.json");

        if (!File.Exists(sourceAuthFilePath))
        {
            Debug.LogError($"Assets ���� �� auth.json ������ ã�� �� �����ϴ�: {sourceAuthFilePath}");
            return;
        }

        try
        {
            // ���� auth.json ������ ���� �б�
            string jsonContent = File.ReadAllText(sourceAuthFilePath);
            Debug.Log($"Assets/auth.json ���� ����: {jsonContent}");

            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("auth.json ������ ������ ��� �ֽ��ϴ�.");
                return;
            }

            // ���� JSON �����͸� AuthData ��ü�� ��ȯ
            AuthData tmpAuthData = JsonUtility.FromJson<AuthData>(jsonContent);

            if (tmpAuthData == null)
            {
                Debug.LogError("auth.json ������ �ùٸ��� �Ľ����� ���߽��ϴ�.");
                return;
            }

            // ��ȣȭ�� AuthData ��ü ����
            AuthData authData = new AuthData
            {
                api_key = tmpAuthData.api_key,
                organization = tmpAuthData.organization
            };

            string encryptedJsonContent = authData.GetEncryptedJson();

            string targetAuthFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".openai", "auth.json");

            File.WriteAllText(targetAuthFilePath, encryptedJsonContent);
            Debug.Log($"��ȣȭ�� auth.json ���� ���� �Ϸ�: {targetAuthFilePath}");
            Debug.Log($"���� ����: {encryptedJsonContent}");

            // ����� ������ ���� ������ �ٽ� Ȯ��
            string savedContent = File.ReadAllText(targetAuthFilePath);
            Debug.Log($"����� auth.json ���� ����: {savedContent}");

            // ��ȣȭ�� ������ Ȯ��
            Debug.Log($"��ȣȭ�� API Key: {authData.api_key}");
            Debug.Log($"��ȣȭ�� Organization: {authData.organization}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"auth.json ���� ó�� �� ���� �߻�: {ex.Message}");
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string api_key;
    public string organization;

    public string ApiKey
    {
        set { api_key = value; }
        get { return api_key; }
    }

    public string Organization
    {
        set { organization = value; }
        get { return organization; }
    }

    // JSON ���� ���� �� ���� ��ȣȭ�� ������ ��ȯ
    public string GetEncryptedJson()
    {
        string json = JsonUtility.ToJson(this);
        Debug.Log($"��ȣȭ�� JSON ����: {json}");
        return json;
    }
}
