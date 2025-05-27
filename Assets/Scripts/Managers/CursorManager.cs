using System.IO;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // 싱글톤 인스턴스를 위한 정적 변수
    public static CursorManager Instance { get; private set; }

    void Awake()
    {
        // 이미 다른 인스턴스가 존재하면 이 객체를 파괴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 객체를 파괴하지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 이 객체를 파괴
        }
    }

    void Start()
    {
        OnVisualization();
    }

    public void OnVisualization()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OffVisualization()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnApplicationQuit()
    {
        string userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string targetFolder = Path.Combine(userFolder, ".openai");

        // 게임 종료 시 폴더 삭제
        if (Directory.Exists(targetFolder))
        {
            try
            {
                Directory.Delete(targetFolder, true); // true = 하위 폴더 및 파일도 포함하여 삭제
                Debug.Log($"게임 종료 시 폴더 삭제됨: {targetFolder}");
            }
            catch (IOException e)
            {
                Debug.LogError($"폴더 삭제 중 오류 발생: {e.Message}");
            }
        }
    }
}
