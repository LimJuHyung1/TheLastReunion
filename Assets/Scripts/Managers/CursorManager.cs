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
}
