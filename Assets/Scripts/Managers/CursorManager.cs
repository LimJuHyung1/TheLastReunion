using System.IO;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ��� ���� ���� ����
    public static CursorManager Instance { get; private set; }

    void Awake()
    {
        // �̹� �ٸ� �ν��Ͻ��� �����ϸ� �� ��ü�� �ı�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ��ü�� �ı����� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �� ��ü�� �ı�
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

        // ���� ���� �� ���� ����
        if (Directory.Exists(targetFolder))
        {
            try
            {
                Directory.Delete(targetFolder, true); // true = ���� ���� �� ���ϵ� �����Ͽ� ����
                Debug.Log($"���� ���� �� ���� ������: {targetFolder}");
            }
            catch (IOException e)
            {
                Debug.LogError($"���� ���� �� ���� �߻�: {e.Message}");
            }
        }
    }
}
