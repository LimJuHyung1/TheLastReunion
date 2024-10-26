using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUtility : MonoBehaviour
{
    public static FadeUtility Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 존재하면 파괴
        }
    }

    // 그래픽(이미지 또는 텍스트) 페이드 인
    public IEnumerator FadeIn(Graphic graphic, float fadeDuration)
    {
        graphic.gameObject.SetActive(true);

        Color tmpColor = graphic.color;
        tmpColor.a = 0f;
        graphic.color = tmpColor;

        float elapsedTime = 0f;
        Color color = graphic.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            graphic.color = color;
            yield return null;
        }
    }

    // 그래픽(이미지 또는 텍스트) 페이드 아웃
    public IEnumerator FadeOut(Graphic graphic, float fadeDuration)
    {
        Color tmpColor = graphic.color;
        tmpColor.a = 1f;
        graphic.color = tmpColor;

        float elapsedTime = 0f;
        Color color = graphic.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            graphic.color = color;
            yield return null;
        }

        graphic.gameObject.SetActive(false);
    }

    public IEnumerator FadeInOut(Graphic graphic, float fadeDuration, float delay = 0.5f)
    {
        while (true)
        {
            // 페이드 인
            yield return StartCoroutine(FadeIn(graphic, fadeDuration));

            // 일정 시간 대기
            yield return new WaitForSeconds(delay);

            // 페이드 아웃
            yield return StartCoroutine(FadeOut(graphic, fadeDuration));

            // 다시 일정 시간 대기
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator SwitchCameraWithFade(Image screen, CameraManager cameraManager, GameObject player, NPCRole npcRole)
    {
        yield return StartCoroutine(FadeIn(screen, 1.5f));

        yield return new WaitForSeconds(1f);
        npcRole.TurnTowardPlayer(player.transform);
        player.GetComponent<Player>().TurnTowardNPC(npcRole.transform);
        player.GetComponent<Player>().ReadyConversation();
        yield return StartCoroutine(FadeOut(screen, 1.5f));
    }

    // 텍스트 알파 값 설정
    private void SetTextAlpha(Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
