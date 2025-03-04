using System.IO;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    private string authFilePath;

    private void Start()
    {
        string targetFolder = GetOpenAIFolderPath();
        authFilePath = Path.Combine(targetFolder, "auth.json");

        // auth.json ���� �Ǵ� ����� (���ǹ� ����)
        Debug.Log(File.Exists(authFilePath) ? "auth.json ������ �̹� �����մϴ�. �����ϴ�." : "auth.json ������ �������� �ʽ��ϴ�. ���� �����մϴ�.");
        CreateEncryptedAuthFile();
    }

    /// <summary>
    /// .openai ���� ��θ� ��������, �������� ������ ����
    /// </summary>
    private string GetOpenAIFolderPath()
    {
        string userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string targetFolder = Path.Combine(userFolder, ".openai");

        // ������ �������� ������ �ڵ� ���� (�̹� �����ϸ� �ƹ� �۾� �� ��)
        Directory.CreateDirectory(targetFolder);
        Debug.Log($"���� Ȯ�� �� ���� �Ϸ�: {targetFolder}");

        return targetFolder;
    }

    /// <summary>
    /// auth.json�� ��ȣȭ�Ͽ� ���� (�������� ������ ���� ����, ������ �����)
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
            string jsonContent = File.ReadAllText(sourceAuthFilePath);
            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("auth.json ������ ������ ��� �ֽ��ϴ�.");
                return;
            }

            AuthData tmpAuthData = JsonUtility.FromJson<AuthData>(jsonContent);
            if (tmpAuthData == null)
            {
                Debug.LogError("auth.json ������ �ùٸ��� �Ľ����� ���߽��ϴ�.");
                return;
            }

            string encryptedJsonContent = EncryptionHelper.Encrypt(tmpAuthData.GetEncryptedJson());
            File.WriteAllText(authFilePath, encryptedJsonContent);

            Debug.Log($"��ȣȭ�� auth.json ���� ���� �Ϸ�: {authFilePath}");
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
    [SerializeField]
    private string api_key;

    [SerializeField]
    private string organization;

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

    // JSON ���� ���� �� ���� ��ȣȭ�� ������ ��ȯ (Ű �̸��� ���� ����)
    public string GetEncryptedJson()
    {
        // �������� JSON ���ڿ��� ����
        string json = $"{{\"api_key\":\"{api_key}\",\"organization\":\"{organization}\"}}";
        Debug.Log($"��ȣȭ�� JSON ����: {json}");
        return json;
    }

    // ��ȣȭ�� JSON�� ��ü�� ��ȯ�ϴ� �޼��� �߰�
    public static AuthData FromDecryptedJson(string json)
    {
        // Unity�� JsonUtility�� ����Ͽ� ������ȭ (�ڵ� ����)
        return JsonUtility.FromJson<AuthData>(json);
    }
}
