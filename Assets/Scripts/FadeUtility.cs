using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUtility : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// - `Instance`를 통해 전역에서 FadeUtility에 접근 가능
    /// </summary>
    public static FadeUtility Instance { get; private set; }
    private bool isFadingLoopActive = false;    // 지속적인 페이드 인/아웃 반복 여부

    /// <summary>
    /// 싱글톤 패턴을 적용하여 인스턴스를 유지
    /// - 한 개의 FadeUtility만 존재하도록 보장
    /// </summary>
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

    /// <summary>
    /// 그래픽(이미지 또는 텍스트)을 페이드 인/아웃 시키는 공통 메서드
    /// </summary>
    /// <param 페이드 효과를 적용할 UI 요소="graphic"></param>
    /// <param 페이드가 완료되는 시간="fadeDuration"></param>
    /// <param 시작 알파 값="startAlpha"> (0: 완전 투명, 1: 완전 불투명)</param>
    /// <param 목표 알파 값="targetAlpha"></param>
    /// <param 완료 후 UI 비활성화 여부="deactivateOnComplete"></param>
    private IEnumerator Fade(Graphic graphic, float fadeDuration, float startAlpha, float targetAlpha, bool deactivateOnComplete = false)
    {
        graphic.gameObject.SetActive(true);
        Color color = graphic.color;
        color.a = startAlpha;
        graphic.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            graphic.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        graphic.color = color;

        if (deactivateOnComplete)
        {
            graphic.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 그래픽을 페이드 인 (서서히 나타나게 함)
    /// </summary>
    /// <param name="graphic">페이드 효과를 적용할 UI 요소</param>
    /// <param name="fadeDuration">페이드가 완료되는 시간</param>
    /// <param name="targetAlpha">목표 알파 값</param>
    public IEnumerator FadeIn(Graphic graphic, float fadeDuration, float targetAlpha = 1.0f)
    {
        yield return StartCoroutine(Fade(graphic, fadeDuration, 0f, targetAlpha));
    }


    /// <summary>
    /// 그래픽을 페이드 아웃 (서서히 사라지게 함)
    /// </summary>
    /// <param name="graphic">페이드 효과를 적용할 UI 요소</param>
    /// <param name="fadeDuration">페이드가 완료되는 시간</param>
    /// <param name="alpha">초기 알파 값 (기본값: 1.0f)</param>
    public IEnumerator FadeOut(Graphic graphic, float fadeDuration, float alpha = 1.0f)
    {
        yield return StartCoroutine(Fade(graphic, fadeDuration, alpha, 0f, true));
    }

    /// <summary>
    /// 그래픽을 지속적으로 페이드 인/아웃 반복
    /// </summary>
    /// <param name="graphic">페이드 효과를 적용할 UI 요소</param>
    /// <param name="fadeDuration">페이드가 완료되는 시간</param>
    /// <param name="delay">페이드 간 대기 시간</param>
    public IEnumerator FadeInOut(Graphic graphic, float fadeDuration, float delay = 0.5f)
    {
        isFadingLoopActive = true;
        while (isFadingLoopActive)
        {
            yield return StartCoroutine(FadeIn(graphic, fadeDuration));
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(FadeOut(graphic, fadeDuration));
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// `FadeInOut()`의 반복을 중지하는 메서드
    /// </summary>
    public void StopFadeInOut()
    {
        isFadingLoopActive = false;
    }

    /// <summary>
    /// 카메라 전환을 페이드 효과와 함께 실행
    /// </summary>
    /// <param name="screen">페이드 효과를 적용할 이미지</param>
    /// <param name="cameraManager">카메라 관리자</param>
    /// <param name="player">플레이어 오브젝트</param>
    /// <param name="npcRole">NPC 역할</param>
    /// <param name="spawnManager">NPC 스폰 관리자 (선택 사항)</param>
    public IEnumerator SwitchCameraWithFade(Image screen, CameraManager cameraManager, GameObject player, NPCRole npcRole, SpawnManager spawnManager = null)
    {
        yield return FadeIn(screen, 1f);

        yield return new WaitForSeconds(1f);
        npcRole.TurnTowardPlayer(player.transform);
        player.GetComponent<Player>().TurnTowardNPC(npcRole.transform);
        player.GetComponent<Player>().ReadyConversation();

        if (spawnManager != null)
            spawnManager.RelocationNPC(npcRole);

        yield return FadeOut(screen, 1f);
    }
}
