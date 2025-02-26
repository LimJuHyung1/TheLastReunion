using System.IO;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    private string authFilePath;

    private void Start()
    {
        // 현재 Windows 사용자 폴더 경로 가져오기
        string userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

        // .openai 폴더 경로 설정
        string targetFolder = Path.Combine(userFolder, ".openai");

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            Debug.Log($"폴더 생성 완료: {targetFolder}");
        }
        else
        {
            Debug.Log($"폴더가 이미 존재합니다: {targetFolder}");
        }

        // auth.json 파일 경로 설정
        authFilePath = Path.Combine(targetFolder, "auth.json");

        // auth.json 파일이 없으면 새로 생성
        if (!File.Exists(authFilePath))
        {
            CreateEncryptedAuthFile();
        }
        else
        {
            Debug.Log($"auth.json 파일이 이미 존재합니다: {authFilePath}");
        }
    }

    /// <summary>
    /// 암호화된 AuthData를 JSON 파일로 저장
    /// </summary>
    private void CreateEncryptedAuthFile()
    {
        string sourceAuthFilePath = Path.Combine(Application.dataPath, "auth.json");

        if (!File.Exists(sourceAuthFilePath))
        {
            Debug.LogError($"Assets 폴더 내 auth.json 파일을 찾을 수 없습니다: {sourceAuthFilePath}");
            return;
        }

        try
        {
            // 기존 auth.json 파일의 내용 읽기
            string jsonContent = File.ReadAllText(sourceAuthFilePath);
            Debug.Log($"Assets/auth.json 파일 내용: {jsonContent}");

            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("auth.json 파일의 내용이 비어 있습니다.");
                return;
            }

            // 기존 JSON 데이터를 AuthData 객체로 변환
            AuthData tmpAuthData = JsonUtility.FromJson<AuthData>(jsonContent);

            if (tmpAuthData == null)
            {
                Debug.LogError("auth.json 파일을 올바르게 파싱하지 못했습니다.");
                return;
            }

            // 암호화된 AuthData 객체 생성
            AuthData authData = new AuthData
            {
                api_key = tmpAuthData.api_key,
                organization = tmpAuthData.organization
            };

            string encryptedJsonContent = authData.GetEncryptedJson();

            string targetAuthFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".openai", "auth.json");

            File.WriteAllText(targetAuthFilePath, encryptedJsonContent);
            Debug.Log($"암호화된 auth.json 파일 생성 완료: {targetAuthFilePath}");
            Debug.Log($"파일 내용: {encryptedJsonContent}");

            // 저장된 파일의 실제 내용을 다시 확인
            string savedContent = File.ReadAllText(targetAuthFilePath);
            Debug.Log($"저장된 auth.json 파일 내용: {savedContent}");

            // 복호화된 데이터 확인
            Debug.Log($"복호화된 API Key: {authData.api_key}");
            Debug.Log($"복호화된 Organization: {authData.organization}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"auth.json 파일 처리 중 오류 발생: {ex.Message}");
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

    // JSON 파일 저장 시 사용될 암호화된 데이터 반환
    public string GetEncryptedJson()
    {
        string json = JsonUtility.ToJson(this);
        Debug.Log($"암호화된 JSON 내용: {json}");
        return json;
    }
}
